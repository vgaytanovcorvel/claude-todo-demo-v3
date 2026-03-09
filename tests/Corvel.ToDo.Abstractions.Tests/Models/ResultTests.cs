using Corvel.ToDo.Abstractions.Models;
using FluentAssertions;

namespace Corvel.ToDo.Abstractions.Tests.Models;

[TestClass]
public class ResultTests
{
    [TestInitialize]
    public void Setup()
    {
    }

    [TestMethod]
    public void Success_ShouldSetIsSuccessToTrue_WhenCalledWithValue()
    {
        // Arrange
        var value = "test value";

        // Act
        var result = Result<string>.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Error.Should().BeNull();
    }

    [TestMethod]
    public void Failure_ShouldSetIsSuccessToFalse_WhenCalledWithError()
    {
        // Arrange
        var error = "something went wrong";

        // Act
        var result = Result<string>.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
        result.Value.Should().BeNull();
    }

    [TestMethod]
    public void Success_ShouldReturnCorrectValue_WhenCalledWithIntegerType()
    {
        // Arrange
        var value = 42;

        // Act
        var result = Result<int>.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        result.Error.Should().BeNull();
    }

    [TestMethod]
    public void Failure_ShouldReturnDefaultValue_WhenCalledWithValueType()
    {
        // Arrange
        var error = "not found";

        // Act
        var result = Result<int>.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().Be(default(int));
        result.Error.Should().Be(error);
    }

    [TestMethod]
    public void Success_ShouldSupportComplexTypes_WhenCalledWithObjectValue()
    {
        // Arrange
        var list = new List<string> { "a", "b", "c" };

        // Act
        var result = Result<List<string>>.Success(list);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(list);
        result.Error.Should().BeNull();
    }

    [TestMethod]
    public void Equality_ShouldReturnTrue_WhenTwoSuccessResultsHaveSameValue()
    {
        // Arrange
        var result1 = Result<string>.Success("hello");
        var result2 = Result<string>.Success("hello");

        // Act & Assert
        result1.Should().Be(result2);
    }

    [TestMethod]
    public void Equality_ShouldReturnTrue_WhenTwoFailureResultsHaveSameError()
    {
        // Arrange
        var result1 = Result<string>.Failure("error");
        var result2 = Result<string>.Failure("error");

        // Act & Assert
        result1.Should().Be(result2);
    }

    [TestMethod]
    public void Equality_ShouldReturnFalse_WhenSuccessAndFailureCompared()
    {
        // Arrange
        var success = Result<string>.Success("value");
        var failure = Result<string>.Failure("error");

        // Act & Assert
        success.Should().NotBe(failure);
    }
}
