using System.Net;
using Corvel.ToDo.Common.Dtos;
using FluentAssertions;

namespace Corvel.ToDo.Common.Tests.Dtos;

[TestClass]
public class ApiResponseTests
{
    [TestInitialize]
    public void Setup()
    {
        // No shared state to initialize for static factory method tests
    }

    [TestMethod]
    public void SuccessResponse_ShouldReturnSuccessTrue_WhenCalledWithData()
    {
        // Arrange
        var data = "test data";

        // Act
        var result = ApiResponse<string>.SuccessResponse(data);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(data);
        result.Error.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public void SuccessResponse_ShouldUseProvidedStatusCode_WhenStatusCodeIsSpecified()
    {
        // Arrange
        var data = 42;

        // Act
        var result = ApiResponse<int>.SuccessResponse(data, HttpStatusCode.Created);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(42);
        result.Error.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [TestMethod]
    public void FailureResponse_ShouldReturnSuccessFalse_WhenCalledWithError()
    {
        // Arrange
        var error = "Something went wrong.";

        // Act
        var result = ApiResponse<string>.FailureResponse(error, HttpStatusCode.BadRequest);

        // Assert
        result.Success.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Error.Should().Be(error);
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public void FailureResponse_ShouldSetDefaultData_WhenCalledForValueType()
    {
        // Arrange
        var error = "Not found.";

        // Act
        var result = ApiResponse<int>.FailureResponse(error, HttpStatusCode.NotFound);

        // Assert
        result.Success.Should().BeFalse();
        result.Data.Should().Be(0);
        result.Error.Should().Be(error);
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public void SuccessResponse_ShouldReturnImmutableRecord_WhenCopiedWithModifiedValues()
    {
        // Arrange
        var data = "immutable";

        // Act
        var result = ApiResponse<string>.SuccessResponse(data);
        var copy = result with { Error = "modified" };

        // Assert
        result.Error.Should().BeNull();
        copy.Error.Should().Be("modified");
        copy.Success.Should().BeTrue();
        copy.Data.Should().Be(data);
    }

    [TestMethod]
    public void FailureResponse_ShouldReturnInternalServerError_WhenStatusCodeIsInternalServerError()
    {
        // Arrange
        var error = "Internal server error.";

        // Act
        var result = ApiResponse<object>.FailureResponse(error, HttpStatusCode.InternalServerError);

        // Assert
        result.Success.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Error.Should().Be(error);
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [TestMethod]
    public void SuccessResponse_ShouldReturnSuccessWithNullData_WhenCalledWithNull()
    {
        // Arrange
        string? data = null;

        // Act
        var result = ApiResponse<string?>.SuccessResponse(data);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Error.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public void FailureResponse_ShouldReturnFailureWithEmptyError_WhenErrorIsEmptyString()
    {
        // Arrange
        var error = string.Empty;

        // Act
        var result = ApiResponse<string>.FailureResponse(error, HttpStatusCode.BadRequest);

        // Assert
        result.Success.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Error.Should().BeEmpty();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public void SuccessResponse_ShouldBeEqual_WhenTwoResponsesHaveSameValues()
    {
        // Arrange
        var data = "test";

        // Act
        var result1 = ApiResponse<string>.SuccessResponse(data);
        var result2 = ApiResponse<string>.SuccessResponse(data);

        // Assert
        result1.Should().Be(result2);
    }
}
