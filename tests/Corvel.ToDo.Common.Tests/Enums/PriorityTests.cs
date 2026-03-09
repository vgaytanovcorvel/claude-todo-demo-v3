using Corvel.ToDo.Common.Enums;
using FluentAssertions;

namespace Corvel.ToDo.Common.Tests.Enums;

[TestClass]
public class PriorityTests
{
    [TestInitialize]
    public void Setup()
    {
        // No shared state to initialize for enum value tests
    }

    [TestMethod]
    public void Low_ShouldHaveValue0_WhenCastToInt()
    {
        // Arrange
        var expected = 0;

        // Act
        var result = (int)Priority.Low;

        // Assert
        result.Should().Be(expected);
    }

    [TestMethod]
    public void Medium_ShouldHaveValue1_WhenCastToInt()
    {
        // Arrange
        var expected = 1;

        // Act
        var result = (int)Priority.Medium;

        // Assert
        result.Should().Be(expected);
    }

    [TestMethod]
    public void High_ShouldHaveValue2_WhenCastToInt()
    {
        // Arrange
        var expected = 2;

        // Act
        var result = (int)Priority.High;

        // Assert
        result.Should().Be(expected);
    }

    [TestMethod]
    public void Critical_ShouldHaveValue3_WhenCastToInt()
    {
        // Arrange
        var expected = 3;

        // Act
        var result = (int)Priority.Critical;

        // Assert
        result.Should().Be(expected);
    }
}
