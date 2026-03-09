using Corvel.ToDo.Common.Constants;
using FluentAssertions;

namespace Corvel.ToDo.Common.Tests.Constants;

[TestClass]
public class ValidationConstantsTests
{
    [TestInitialize]
    public void Setup()
    {
        // No shared state to initialize for constant value tests
    }

    [TestMethod]
    public void TitleMaxLength_ShouldReturn200_WhenAccessed()
    {
        // Arrange
        var expected = 200;

        // Act
        var result = ValidationConstants.TitleMaxLength;

        // Assert
        result.Should().Be(expected);
    }

    [TestMethod]
    public void DescriptionMaxLength_ShouldReturn2000_WhenAccessed()
    {
        // Arrange
        var expected = 2000;

        // Act
        var result = ValidationConstants.DescriptionMaxLength;

        // Assert
        result.Should().Be(expected);
    }
}
