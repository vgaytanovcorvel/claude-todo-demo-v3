using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Constants;
using Corvel.ToDo.Common.Enums;
using Corvel.ToDo.Implementation.Validators;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace Corvel.ToDo.Implementation.Tests.Validators;

[TestClass]
public class UpdateToDoItemRequestValidatorTests
{
    private FakeTimeProvider timeProvider = null!;
    private Mock<UpdateToDoItemRequestValidator> validatorMock = null!;

    [TestInitialize]
    public void Setup()
    {
        timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero));
        validatorMock = new Mock<UpdateToDoItemRequestValidator>(timeProvider)
        { CallBase = true };
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateToDoItemRequest(
            "Valid Title",
            "Valid Description",
            Priority.Medium,
            ToDoItemStatus.InProgress,
            timeProvider.GetUtcNow().UtcDateTime.AddDays(7));

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldPass_WhenDescriptionIsNull()
    {
        // Arrange
        var request = new UpdateToDoItemRequest("Valid Title", null, Priority.Low, ToDoItemStatus.Pending, null);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldPass_WhenDueDateIsNull()
    {
        // Arrange
        var request = new UpdateToDoItemRequest("Valid Title", "Description", Priority.High, ToDoItemStatus.Completed, null);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenTitleIsEmpty()
    {
        // Arrange
        var request = new UpdateToDoItemRequest("", null, Priority.Low, ToDoItemStatus.Pending, null);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage.Contains("must not be empty"));
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenTitleExceedsMaxLength()
    {
        // Arrange
        var longTitle = new string('A', ValidationConstants.TitleMaxLength + 1);
        var request = new UpdateToDoItemRequest(longTitle, null, Priority.Low, ToDoItemStatus.Pending, null);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Title" &&
            e.ErrorMessage.Contains($"{ValidationConstants.TitleMaxLength}"));
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var longDescription = new string('A', ValidationConstants.DescriptionMaxLength + 1);
        var request = new UpdateToDoItemRequest("Valid Title", longDescription, Priority.Low, ToDoItemStatus.Pending, null);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Description" &&
            e.ErrorMessage.Contains($"{ValidationConstants.DescriptionMaxLength}"));
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenPriorityIsInvalid()
    {
        // Arrange
        var request = new UpdateToDoItemRequest("Valid Title", null, (Priority)99, ToDoItemStatus.Pending, null);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Priority");
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenStatusIsInvalid()
    {
        // Arrange
        var request = new UpdateToDoItemRequest("Valid Title", null, Priority.Low, (ToDoItemStatus)99, null);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Status");
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenDueDateIsInThePast()
    {
        // Arrange
        var pastDate = timeProvider.GetUtcNow().UtcDateTime.AddDays(-1);
        var request = new UpdateToDoItemRequest("Valid Title", null, Priority.Low, ToDoItemStatus.Pending, pastDate);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "DueDate" &&
            e.ErrorMessage.Contains("must be in the future"));
    }
}
