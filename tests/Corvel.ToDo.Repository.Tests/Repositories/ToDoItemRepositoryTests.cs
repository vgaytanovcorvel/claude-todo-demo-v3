using Corvel.ToDo.Abstractions.Exceptions;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Common.Enums;
using Corvel.ToDo.Repository.Contexts;
using Corvel.ToDo.Repository.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Corvel.ToDo.Repository.Tests.Repositories;

[TestClass]
public class ToDoItemRepositoryTests
{
    private DbContextOptions<ToDoDbContext> dbContextOptions = null!;
    private IDbContextFactory<ToDoDbContext> contextFactory = null!;
    private ToDoItemRepository repository = null!;

    [TestInitialize]
    public void Setup()
    {
        dbContextOptions = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        contextFactory = new TestDbContextFactory(dbContextOptions);
        repository = new ToDoItemRepository(contextFactory);
    }

    [TestMethod]
    public async Task ToDoItemSingleByIdAsync_ShouldReturnToDoItem_WhenItemExists()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var toDoItem = new ToDoItem
        {
            Title = "Test Item",
            Description = "Test Description",
            Priority = Priority.High,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var added = await repository.ToDoItemAddAsync(toDoItem, cancellationToken);

        // Act
        var result = await repository.ToDoItemSingleByIdAsync(added.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(added.Id);
        result.Title.Should().Be("Test Item");
        result.Description.Should().Be("Test Description");
        result.Priority.Should().Be(Priority.High);
        result.Status.Should().Be(ToDoItemStatus.Pending);
    }

    [TestMethod]
    public async Task ToDoItemSingleByIdAsync_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var nonExistentId = 999;

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => repository.ToDoItemSingleByIdAsync(nonExistentId, cancellationToken));

        exception.Message.Should().Contain("ToDoItem not found");
        exception.Message.Should().Contain("999");
    }

    [TestMethod]
    public async Task ToDoItemSingleOrDefaultByIdAsync_ShouldReturnToDoItem_WhenItemExists()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var toDoItem = new ToDoItem
        {
            Title = "Existing Item",
            Priority = Priority.Medium,
            Status = ToDoItemStatus.InProgress,
            CreatedAtUtc = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var added = await repository.ToDoItemAddAsync(toDoItem, cancellationToken);

        // Act
        var result = await repository.ToDoItemSingleOrDefaultByIdAsync(added.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(added.Id);
        result.Title.Should().Be("Existing Item");
        result.Priority.Should().Be(Priority.Medium);
        result.Status.Should().Be(ToDoItemStatus.InProgress);
    }

    [TestMethod]
    public async Task ToDoItemSingleOrDefaultByIdAsync_ShouldReturnNull_WhenItemDoesNotExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var nonExistentId = 999;

        // Act
        var result = await repository.ToDoItemSingleOrDefaultByIdAsync(nonExistentId, cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task ToDoItemGetAllAsync_ShouldReturnAllItems_WhenItemsExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var item1 = new ToDoItem
        {
            Title = "First Item",
            Priority = Priority.Low,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        var item2 = new ToDoItem
        {
            Title = "Second Item",
            Priority = Priority.High,
            Status = ToDoItemStatus.InProgress,
            CreatedAtUtc = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        await repository.ToDoItemAddAsync(item1, cancellationToken);
        await repository.ToDoItemAddAsync(item2, cancellationToken);

        // Act
        var result = await repository.ToDoItemGetAllAsync(cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Second Item");
        result[1].Title.Should().Be("First Item");
    }

    [TestMethod]
    public async Task ToDoItemGetAllAsync_ShouldReturnEmptyList_WhenNoItemsExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await repository.ToDoItemGetAllAsync(cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public async Task ToDoItemAddAsync_ShouldCreateAndReturnItem_WhenItemIsValid()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var toDoItem = new ToDoItem
        {
            Title = "New Item",
            Description = "New Description",
            Priority = Priority.Critical,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
            DueDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var result = await repository.ToDoItemAddAsync(toDoItem, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("New Item");
        result.Description.Should().Be("New Description");
        result.Priority.Should().Be(Priority.Critical);
        result.Status.Should().Be(ToDoItemStatus.Pending);
        result.CreatedAtUtc.Should().Be(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc));
        result.DueDate.Should().Be(new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [TestMethod]
    public async Task ToDoItemUpdateAsync_ShouldModifyAndReturnItem_WhenItemExists()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var toDoItem = new ToDoItem
        {
            Title = "Original Title",
            Description = "Original Description",
            Priority = Priority.Low,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var added = await repository.ToDoItemAddAsync(toDoItem, cancellationToken);

        var updatedItem = new ToDoItem
        {
            Id = added.Id,
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = Priority.Critical,
            Status = ToDoItemStatus.Completed,
            CreatedAtUtc = added.CreatedAtUtc,
            UpdatedAtUtc = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            CompletedAtUtc = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var result = await repository.ToDoItemUpdateAsync(updatedItem, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(added.Id);
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
        result.Priority.Should().Be(Priority.Critical);
        result.Status.Should().Be(ToDoItemStatus.Completed);
        result.UpdatedAtUtc.Should().Be(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        result.CompletedAtUtc.Should().Be(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [TestMethod]
    public async Task ToDoItemUpdateAsync_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var nonExistentItem = new ToDoItem
        {
            Id = 999,
            Title = "Non-existent",
            Priority = Priority.Low,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => repository.ToDoItemUpdateAsync(nonExistentItem, cancellationToken));

        exception.Message.Should().Contain("ToDoItem not found");
        exception.Message.Should().Contain("999");
    }

    [TestMethod]
    public async Task ToDoItemDeleteAsync_ShouldRemoveItem_WhenItemExists()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var toDoItem = new ToDoItem
        {
            Title = "Item to Delete",
            Priority = Priority.Low,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var added = await repository.ToDoItemAddAsync(toDoItem, cancellationToken);

        // Act
        await repository.ToDoItemDeleteAsync(added.Id, cancellationToken);

        // Assert
        var result = await repository.ToDoItemSingleOrDefaultByIdAsync(added.Id, cancellationToken);
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task ToDoItemDeleteAsync_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var nonExistentId = 999;

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => repository.ToDoItemDeleteAsync(nonExistentId, cancellationToken));

        exception.Message.Should().Contain("ToDoItem not found");
        exception.Message.Should().Contain("999");
    }

    [TestCleanup]
    public void Cleanup()
    {
        using var context = new ToDoDbContext(dbContextOptions);
        context.Database.EnsureDeleted();
    }

    private sealed class TestDbContextFactory(
        DbContextOptions<ToDoDbContext> options) : IDbContextFactory<ToDoDbContext>
    {
        public ToDoDbContext CreateDbContext()
        {
            return new ToDoDbContext(options);
        }

        public Task<ToDoDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(CreateDbContext());
        }
    }
}
