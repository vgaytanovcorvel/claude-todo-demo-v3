using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Enums;
using Corvel.ToDo.Implementation.Services;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace Corvel.ToDo.Implementation.Tests.Services;

[TestClass]
public class ToDoItemServiceTests
{
    private Mock<IToDoItemRepository> toDoItemRepositoryMock = new(MockBehavior.Strict);
    private Mock<IValidator<CreateToDoItemRequest>> createValidatorMock = new(MockBehavior.Strict);
    private Mock<IValidator<UpdateToDoItemRequest>> updateValidatorMock = new(MockBehavior.Strict);
    private FakeTimeProvider timeProvider = null!;
    private Mock<ToDoItemService> toDoItemServiceMock = null!;

    private readonly CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero));

        toDoItemServiceMock = new Mock<ToDoItemService>(
            () => new ToDoItemService(
                toDoItemRepositoryMock.Object,
                timeProvider,
                createValidatorMock.Object,
                updateValidatorMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnToDoItem_WhenItemExists()
    {
        // Arrange
        var itemId = 1;
        var expectedItem = new ToDoItem { Id = itemId, Title = "Test Item" };

        toDoItemServiceMock
            .Setup(service => service.GetByIdAsync(itemId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemSingleOrDefaultByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(expectedItem)
            .Verifiable(Times.Once());

        // Act
        var result = await toDoItemServiceMock.Object.GetByIdAsync(itemId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(itemId);
        result.Title.Should().Be("Test Item");

        toDoItemServiceMock.VerifyAll();
        toDoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnNull_WhenItemDoesNotExist()
    {
        // Arrange
        var itemId = 999;

        toDoItemServiceMock
            .Setup(service => service.GetByIdAsync(itemId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemSingleOrDefaultByIdAsync(itemId, cancellationToken))
            .ReturnsAsync((ToDoItem?)null)
            .Verifiable(Times.Once());

        // Act
        var result = await toDoItemServiceMock.Object.GetByIdAsync(itemId, cancellationToken);

        // Assert
        result.Should().BeNull();

        toDoItemServiceMock.VerifyAll();
        toDoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetAllAsync_ShouldReturnAllItems_WhenItemsExist()
    {
        // Arrange
        var expectedItems = new List<ToDoItem>
        {
            new() { Id = 1, Title = "Item 1" },
            new() { Id = 2, Title = "Item 2" }
        };

        toDoItemServiceMock
            .Setup(service => service.GetAllAsync(cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemGetAllAsync(cancellationToken))
            .ReturnsAsync(expectedItems)
            .Verifiable(Times.Once());

        // Act
        var result = await toDoItemServiceMock.Object.GetAllAsync(cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Item 1");
        result[1].Title.Should().Be("Item 2");

        toDoItemServiceMock.VerifyAll();
        toDoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoItemsExist()
    {
        // Arrange
        var expectedItems = new List<ToDoItem>();

        toDoItemServiceMock
            .Setup(service => service.GetAllAsync(cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemGetAllAsync(cancellationToken))
            .ReturnsAsync(expectedItems)
            .Verifiable(Times.Once());

        // Act
        var result = await toDoItemServiceMock.Object.GetAllAsync(cancellationToken);

        // Assert
        result.Should().BeEmpty();

        toDoItemServiceMock.VerifyAll();
        toDoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task CreateAsync_ShouldReturnCreatedItem_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateToDoItemRequest("New Item", "Description", Priority.High, null);
        var expectedUtc = timeProvider.GetUtcNow().UtcDateTime;
        var savedItem = new ToDoItem
        {
            Id = 1,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = expectedUtc
        };

        toDoItemServiceMock
            .Setup(service => service.CreateAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        createValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<CreateToDoItemRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemAddAsync(
                It.Is<ToDoItem>(item =>
                    item.Title == request.Title &&
                    item.Description == request.Description &&
                    item.Priority == request.Priority &&
                    item.Status == ToDoItemStatus.Pending &&
                    item.CreatedAtUtc == expectedUtc),
                cancellationToken))
            .ReturnsAsync(savedItem)
            .Verifiable(Times.Once());

        // Act
        var result = await toDoItemServiceMock.Object.CreateAsync(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("New Item");
        result.Status.Should().Be(ToDoItemStatus.Pending);
        result.CreatedAtUtc.Should().Be(expectedUtc);

        toDoItemServiceMock.VerifyAll();
        createValidatorMock.VerifyAll();
        toDoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task CreateAsync_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new CreateToDoItemRequest("", null, Priority.Low, null);
        var validationFailures = new List<ValidationFailure>
        {
            new("Title", "'Title' must not be empty.")
        };

        toDoItemServiceMock
            .Setup(service => service.CreateAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        createValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<CreateToDoItemRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ThrowsAsync(new ValidationException(validationFailures))
            .Verifiable(Times.Once());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ValidationException>(
            () => toDoItemServiceMock.Object.CreateAsync(request, cancellationToken));

        toDoItemServiceMock.VerifyAll();
        createValidatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldReturnUpdatedItem_WhenRequestIsValid()
    {
        // Arrange
        var itemId = 1;
        var request = new UpdateToDoItemRequest("Updated Title", "Updated Desc", Priority.Medium, ToDoItemStatus.InProgress, null);
        var expectedUtc = timeProvider.GetUtcNow().UtcDateTime;
        var existingItem = new ToDoItem
        {
            Id = itemId,
            Title = "Original Title",
            Description = "Original Desc",
            Priority = Priority.Low,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = expectedUtc.AddDays(-1)
        };
        var updatedItem = new ToDoItem
        {
            Id = itemId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = request.Status,
            CreatedAtUtc = existingItem.CreatedAtUtc,
            UpdatedAtUtc = expectedUtc
        };

        toDoItemServiceMock
            .Setup(service => service.UpdateAsync(itemId, request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        updateValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<UpdateToDoItemRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemSingleByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(existingItem)
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemUpdateAsync(
                It.Is<ToDoItem>(item =>
                    item.Id == itemId &&
                    item.Title == request.Title &&
                    item.Description == request.Description &&
                    item.Priority == request.Priority &&
                    item.Status == request.Status &&
                    item.UpdatedAtUtc == expectedUtc &&
                    item.CompletedAtUtc == null),
                cancellationToken))
            .ReturnsAsync(updatedItem)
            .Verifiable(Times.Once());

        // Act
        var result = await toDoItemServiceMock.Object.UpdateAsync(itemId, request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Title");
        result.Status.Should().Be(ToDoItemStatus.InProgress);
        result.UpdatedAtUtc.Should().Be(expectedUtc);

        toDoItemServiceMock.VerifyAll();
        updateValidatorMock.VerifyAll();
        toDoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldSetCompletedAtUtc_WhenStatusChangesToCompleted()
    {
        // Arrange
        var itemId = 1;
        var request = new UpdateToDoItemRequest("Title", null, Priority.Low, ToDoItemStatus.Completed, null);
        var expectedUtc = timeProvider.GetUtcNow().UtcDateTime;
        var existingItem = new ToDoItem
        {
            Id = itemId,
            Title = "Title",
            Priority = Priority.Low,
            Status = ToDoItemStatus.InProgress,
            CreatedAtUtc = expectedUtc.AddDays(-1)
        };
        var updatedItem = new ToDoItem
        {
            Id = itemId,
            Title = request.Title,
            Priority = request.Priority,
            Status = ToDoItemStatus.Completed,
            CreatedAtUtc = existingItem.CreatedAtUtc,
            UpdatedAtUtc = expectedUtc,
            CompletedAtUtc = expectedUtc
        };

        toDoItemServiceMock
            .Setup(service => service.UpdateAsync(itemId, request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        updateValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<UpdateToDoItemRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemSingleByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(existingItem)
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemUpdateAsync(
                It.Is<ToDoItem>(item =>
                    item.Id == itemId &&
                    item.Status == ToDoItemStatus.Completed &&
                    item.CompletedAtUtc == expectedUtc &&
                    item.UpdatedAtUtc == expectedUtc),
                cancellationToken))
            .ReturnsAsync(updatedItem)
            .Verifiable(Times.Once());

        // Act
        var result = await toDoItemServiceMock.Object.UpdateAsync(itemId, request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.CompletedAtUtc.Should().Be(expectedUtc);
        result.Status.Should().Be(ToDoItemStatus.Completed);

        toDoItemServiceMock.VerifyAll();
        updateValidatorMock.VerifyAll();
        toDoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldPreserveCompletedAtUtc_WhenStatusAlreadyCompleted()
    {
        // Arrange
        var itemId = 1;
        var originalCompletedAt = new DateTime(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc);
        var request = new UpdateToDoItemRequest("Title", null, Priority.Low, ToDoItemStatus.Completed, null);
        var expectedUtc = timeProvider.GetUtcNow().UtcDateTime;
        var existingItem = new ToDoItem
        {
            Id = itemId,
            Title = "Title",
            Priority = Priority.Low,
            Status = ToDoItemStatus.Completed,
            CreatedAtUtc = expectedUtc.AddDays(-5),
            CompletedAtUtc = originalCompletedAt
        };
        var updatedItem = new ToDoItem
        {
            Id = itemId,
            Title = request.Title,
            Priority = request.Priority,
            Status = ToDoItemStatus.Completed,
            CreatedAtUtc = existingItem.CreatedAtUtc,
            UpdatedAtUtc = expectedUtc,
            CompletedAtUtc = originalCompletedAt
        };

        toDoItemServiceMock
            .Setup(service => service.UpdateAsync(itemId, request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        updateValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<UpdateToDoItemRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemSingleByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(existingItem)
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemUpdateAsync(
                It.Is<ToDoItem>(item =>
                    item.Id == itemId &&
                    item.CompletedAtUtc == originalCompletedAt),
                cancellationToken))
            .ReturnsAsync(updatedItem)
            .Verifiable(Times.Once());

        // Act
        var result = await toDoItemServiceMock.Object.UpdateAsync(itemId, request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.CompletedAtUtc.Should().Be(originalCompletedAt);

        toDoItemServiceMock.VerifyAll();
        updateValidatorMock.VerifyAll();
        toDoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var itemId = 1;
        var request = new UpdateToDoItemRequest("", null, Priority.Low, ToDoItemStatus.Pending, null);
        var validationFailures = new List<ValidationFailure>
        {
            new("Title", "'Title' must not be empty.")
        };

        toDoItemServiceMock
            .Setup(service => service.UpdateAsync(itemId, request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        updateValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<UpdateToDoItemRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ThrowsAsync(new ValidationException(validationFailures))
            .Verifiable(Times.Once());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ValidationException>(
            () => toDoItemServiceMock.Object.UpdateAsync(itemId, request, cancellationToken));

        toDoItemServiceMock.VerifyAll();
        updateValidatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldCallRepositoryDelete_WhenCalled()
    {
        // Arrange
        var itemId = 1;

        toDoItemServiceMock
            .Setup(service => service.DeleteAsync(itemId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemDeleteAsync(itemId, cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once());

        // Act
        await toDoItemServiceMock.Object.DeleteAsync(itemId, cancellationToken);

        // Assert
        toDoItemServiceMock.VerifyAll();
        toDoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task CreateAsync_ShouldSetDueDate_WhenDueDateIsProvided()
    {
        // Arrange
        var futureDate = timeProvider.GetUtcNow().UtcDateTime.AddDays(7);
        var request = new CreateToDoItemRequest("Item with due date", null, Priority.Medium, futureDate);
        var expectedUtc = timeProvider.GetUtcNow().UtcDateTime;
        var savedItem = new ToDoItem
        {
            Id = 1,
            Title = request.Title,
            Priority = request.Priority,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = expectedUtc,
            DueDate = futureDate
        };

        toDoItemServiceMock
            .Setup(service => service.CreateAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        createValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<CreateToDoItemRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        toDoItemRepositoryMock
            .Setup(repo => repo.ToDoItemAddAsync(
                It.Is<ToDoItem>(item =>
                    item.Title == request.Title &&
                    item.DueDate == futureDate),
                cancellationToken))
            .ReturnsAsync(savedItem)
            .Verifiable(Times.Once());

        // Act
        var result = await toDoItemServiceMock.Object.CreateAsync(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.DueDate.Should().Be(futureDate);

        toDoItemServiceMock.VerifyAll();
        createValidatorMock.VerifyAll();
        toDoItemRepositoryMock.VerifyAll();
    }
}
