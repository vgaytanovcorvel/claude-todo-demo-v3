using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Common.Enums;
using FluentAssertions;

namespace Corvel.ToDo.Abstractions.Tests.Models;

[TestClass]
public class ToDoItemTests
{
    private ToDoItem toDoItem = null!;

    [TestInitialize]
    public void Setup()
    {
        toDoItem = new ToDoItem();
    }

    [TestMethod]
    public void Constructor_ShouldSetDefaultValues_WhenCreatedWithDefaults()
    {
        // Arrange & Act (toDoItem created in Setup)

        // Assert
        toDoItem.Id.Should().Be(0);
        toDoItem.Title.Should().BeEmpty();
        toDoItem.Description.Should().BeNull();
        toDoItem.Priority.Should().Be(Priority.Low);
        toDoItem.Status.Should().Be(ToDoItemStatus.Pending);
        toDoItem.CreatedAtUtc.Should().Be(default);
        toDoItem.UpdatedAtUtc.Should().BeNull();
        toDoItem.DueDate.Should().BeNull();
        toDoItem.CompletedAtUtc.Should().BeNull();
    }

    [TestMethod]
    public void Properties_ShouldReturnAssignedValues_WhenSetViaObjectInitializer()
    {
        // Arrange
        var createdAt = new DateTime(2026, 3, 9, 10, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2026, 3, 9, 11, 0, 0, DateTimeKind.Utc);
        var dueDate = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc);
        var completedAt = new DateTime(2026, 3, 10, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var item = new ToDoItem
        {
            Id = 1,
            Title = "Test Task",
            Description = "Test Description",
            Priority = Priority.High,
            Status = ToDoItemStatus.Completed,
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = updatedAt,
            DueDate = dueDate,
            CompletedAtUtc = completedAt
        };

        // Assert
        item.Id.Should().Be(1);
        item.Title.Should().Be("Test Task");
        item.Description.Should().Be("Test Description");
        item.Priority.Should().Be(Priority.High);
        item.Status.Should().Be(ToDoItemStatus.Completed);
        item.CreatedAtUtc.Should().Be(createdAt);
        item.UpdatedAtUtc.Should().Be(updatedAt);
        item.DueDate.Should().Be(dueDate);
        item.CompletedAtUtc.Should().Be(completedAt);
    }

    [TestMethod]
    public void WithExpression_ShouldCreateNewInstance_WhenPropertiesAreChanged()
    {
        // Arrange
        var original = new ToDoItem
        {
            Id = 1,
            Title = "Original Title",
            Priority = Priority.Low
        };

        // Act
        var updated = original with
        {
            Id = 2,
            Title = "Updated Title",
            Priority = Priority.High
        };

        // Assert
        updated.Id.Should().Be(2);
        updated.Title.Should().Be("Updated Title");
        updated.Priority.Should().Be(Priority.High);

        // Original should remain unchanged
        original.Id.Should().Be(1);
        original.Title.Should().Be("Original Title");
        original.Priority.Should().Be(Priority.Low);
    }

    [TestMethod]
    public void Title_ShouldDefaultToEmptyString_WhenNotExplicitlySet()
    {
        // Arrange & Act (toDoItem created in Setup)

        // Assert
        toDoItem.Title.Should().NotBeNull();
        toDoItem.Title.Should().BeEmpty();
    }

    [TestMethod]
    public void NullableProperties_ShouldAcceptNullValues_WhenSetViaWithExpression()
    {
        // Arrange
        var item = new ToDoItem
        {
            Description = "Some description",
            UpdatedAtUtc = DateTime.UtcNow,
            DueDate = DateTime.UtcNow,
            CompletedAtUtc = DateTime.UtcNow
        };

        // Act
        var cleared = item with
        {
            Description = null,
            UpdatedAtUtc = null,
            DueDate = null,
            CompletedAtUtc = null
        };

        // Assert
        cleared.Description.Should().BeNull();
        cleared.UpdatedAtUtc.Should().BeNull();
        cleared.DueDate.Should().BeNull();
        cleared.CompletedAtUtc.Should().BeNull();
    }
}
