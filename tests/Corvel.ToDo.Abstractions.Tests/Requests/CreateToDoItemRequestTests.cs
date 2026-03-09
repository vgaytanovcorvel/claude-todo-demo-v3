using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Enums;
using FluentAssertions;

namespace Corvel.ToDo.Abstractions.Tests.Requests;

[TestClass]
public class CreateToDoItemRequestTests
{
    [TestInitialize]
    public void Setup()
    {
    }

    [TestMethod]
    public void Constructor_ShouldSetAllProperties_WhenCreatedWithValues()
    {
        // Arrange
        var dueDate = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var request = new CreateToDoItemRequest(
            Title: "New Task",
            Description: "Task description",
            Priority: Priority.High,
            DueDate: dueDate);

        // Assert
        request.Title.Should().Be("New Task");
        request.Description.Should().Be("Task description");
        request.Priority.Should().Be(Priority.High);
        request.DueDate.Should().Be(dueDate);
    }

    [TestMethod]
    public void Constructor_ShouldAllowNullOptionalProperties_WhenNotProvided()
    {
        // Arrange & Act
        var request = new CreateToDoItemRequest(
            Title: "Minimal Task",
            Description: null,
            Priority: Priority.Low,
            DueDate: null);

        // Assert
        request.Description.Should().BeNull();
        request.DueDate.Should().BeNull();
    }

    [TestMethod]
    public void Equality_ShouldReturnTrue_WhenAllPropertiesAreEqual()
    {
        // Arrange
        var request1 = new CreateToDoItemRequest("Task", "Desc", Priority.Medium, null);
        var request2 = new CreateToDoItemRequest("Task", "Desc", Priority.Medium, null);

        // Act & Assert
        request1.Should().Be(request2);
    }
}
