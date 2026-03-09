using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Enums;
using FluentAssertions;

namespace Corvel.ToDo.Abstractions.Tests.Requests;

[TestClass]
public class UpdateToDoItemRequestTests
{
    [TestInitialize]
    public void Setup()
    {
    }

    [TestMethod]
    public void Constructor_ShouldSetAllProperties_WhenCreatedWithValues()
    {
        // Arrange
        var dueDate = new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var request = new UpdateToDoItemRequest(
            Title: "Updated Task",
            Description: "Updated description",
            Priority: Priority.Critical,
            Status: ToDoItemStatus.InProgress,
            DueDate: dueDate);

        // Assert
        request.Title.Should().Be("Updated Task");
        request.Description.Should().Be("Updated description");
        request.Priority.Should().Be(Priority.Critical);
        request.Status.Should().Be(ToDoItemStatus.InProgress);
        request.DueDate.Should().Be(dueDate);
    }

    [TestMethod]
    public void Constructor_ShouldAllowNullOptionalProperties_WhenNotProvided()
    {
        // Arrange & Act
        var request = new UpdateToDoItemRequest(
            Title: "Task",
            Description: null,
            Priority: Priority.Low,
            Status: ToDoItemStatus.Pending,
            DueDate: null);

        // Assert
        request.Description.Should().BeNull();
        request.DueDate.Should().BeNull();
    }

    [TestMethod]
    public void Equality_ShouldReturnTrue_WhenAllPropertiesAreEqual()
    {
        // Arrange
        var request1 = new UpdateToDoItemRequest("Task", "Desc", Priority.High, ToDoItemStatus.Completed, null);
        var request2 = new UpdateToDoItemRequest("Task", "Desc", Priority.High, ToDoItemStatus.Completed, null);

        // Act & Assert
        request1.Should().Be(request2);
    }
}
