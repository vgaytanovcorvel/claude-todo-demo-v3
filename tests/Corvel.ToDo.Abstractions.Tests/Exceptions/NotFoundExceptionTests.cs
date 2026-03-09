using Corvel.ToDo.Abstractions.Exceptions;
using FluentAssertions;

namespace Corvel.ToDo.Abstractions.Tests.Exceptions;

[TestClass]
public class NotFoundExceptionTests
{
    [TestInitialize]
    public void Setup()
    {
    }

    [TestMethod]
    public void Constructor_ShouldSetMessage_WhenMessageIsProvided()
    {
        // Arrange
        var message = "ToDoItem not found (Id: 42).";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [TestMethod]
    public void Constructor_ShouldInheritFromException_WhenCreated()
    {
        // Arrange & Act
        var exception = new NotFoundException("Not found.");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
