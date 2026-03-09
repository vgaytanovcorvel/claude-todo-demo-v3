using Corvel.ToDo.Common.Enums;
using FluentAssertions;

namespace Corvel.ToDo.Common.Tests.Enums;

[TestClass]
public class ToDoItemStatusTests
{
    [TestInitialize]
    public void Setup()
    {
        // No shared state to initialize for enum value tests
    }

    [TestMethod]
    public void Pending_ShouldHaveValue0_WhenCastToInt()
    {
        // Arrange
        var expected = 0;

        // Act
        var result = (int)ToDoItemStatus.Pending;

        // Assert
        result.Should().Be(expected);
    }

    [TestMethod]
    public void InProgress_ShouldHaveValue1_WhenCastToInt()
    {
        // Arrange
        var expected = 1;

        // Act
        var result = (int)ToDoItemStatus.InProgress;

        // Assert
        result.Should().Be(expected);
    }

    [TestMethod]
    public void Completed_ShouldHaveValue2_WhenCastToInt()
    {
        // Arrange
        var expected = 2;

        // Act
        var result = (int)ToDoItemStatus.Completed;

        // Assert
        result.Should().Be(expected);
    }

    [TestMethod]
    public void Cancelled_ShouldHaveValue3_WhenCastToInt()
    {
        // Arrange
        var expected = 3;

        // Act
        var result = (int)ToDoItemStatus.Cancelled;

        // Assert
        result.Should().Be(expected);
    }
}
