using Corvel.ToDo.Common.Constants;
using FluentAssertions;

namespace Corvel.ToDo.Common.Tests.Constants;

[TestClass]
public class RouteConstantsTests
{
    [TestInitialize]
    public void Setup()
    {
        // No shared state to initialize for constant value tests
    }

    [TestMethod]
    public void ApiPrefix_ShouldReturnApi_WhenAccessed()
    {
        // Arrange
        var expected = "api";

        // Act
        var result = RouteConstants.ApiPrefix;

        // Assert
        result.Should().Be(expected);
    }

    [TestMethod]
    public void ToDoItemsRoute_ShouldReturnToDoItemsRoute_WhenAccessed()
    {
        // Arrange
        var expected = "todo-items";

        // Act
        var result = RouteConstants.ToDoItemsRoute;

        // Assert
        result.Should().Be(expected);
    }
}
