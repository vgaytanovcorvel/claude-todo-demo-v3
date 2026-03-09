using System.Net;
using System.Text.Json;
using Corvel.ToDo.Abstractions.Exceptions;
using Corvel.ToDo.Common.Dtos;
using Corvel.ToDo.Web.Core.Middleware;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Corvel.ToDo.Web.Core.Tests.Middleware;

[TestClass]
public class GlobalExceptionHandlerMiddlewareTests
{
    private Mock<ILogger<GlobalExceptionHandlerMiddleware>> loggerMock = null!;
    private Mock<GlobalExceptionHandlerMiddleware> middlewareMock = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [TestInitialize]
    public void Setup()
    {
        loggerMock = new Mock<ILogger<GlobalExceptionHandlerMiddleware>>(MockBehavior.Strict);
        middlewareMock = new Mock<GlobalExceptionHandlerMiddleware>(
            () => new GlobalExceptionHandlerMiddleware(loggerMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldReturn404_WhenNotFoundExceptionIsThrown()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw new NotFoundException("ToDo item not found (Id: 1).");

        middlewareMock
            .Setup(middleware => middleware.InvokeAsync(context, next))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        await middlewareMock.Object.InvokeAsync(context, next);

        // Assert
        context.Response.StatusCode.Should().Be(404);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseBody, JsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Error.Should().Be("ToDo item not found (Id: 1).");
        apiResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        middlewareMock.VerifyAll();
        loggerMock.VerifyAll();
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldReturn400_WhenValidationExceptionIsThrown()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var validationFailures = new List<ValidationFailure>
        {
            new("Title", "Title is required."),
            new("Priority", "Priority is invalid.")
        };

        RequestDelegate next = _ => throw new ValidationException(validationFailures);

        middlewareMock
            .Setup(middleware => middleware.InvokeAsync(context, next))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        await middlewareMock.Object.InvokeAsync(context, next);

        // Assert
        context.Response.StatusCode.Should().Be(400);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseBody, JsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Error.Should().Contain("Title is required.");
        apiResponse.Error.Should().Contain("Priority is invalid.");
        apiResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        middlewareMock.VerifyAll();
        loggerMock.VerifyAll();
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldReturn500_WhenUnhandledExceptionIsThrown()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new InvalidOperationException("Something went wrong.");
        RequestDelegate next = _ => throw exception;

        loggerMock
            .Setup(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Verifiable(Times.Once());

        middlewareMock
            .Setup(middleware => middleware.InvokeAsync(context, next))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        await middlewareMock.Object.InvokeAsync(context, next);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseBody, JsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Error.Should().Be("An unexpected error occurred.");
        apiResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        middlewareMock.VerifyAll();
        loggerMock.VerifyAll();
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldCallNext_WhenNoExceptionIsThrown()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextCalled = false;

        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        middlewareMock
            .Setup(middleware => middleware.InvokeAsync(context, next))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        await middlewareMock.Object.InvokeAsync(context, next);

        // Assert
        nextCalled.Should().BeTrue();

        middlewareMock.VerifyAll();
        loggerMock.VerifyAll();
    }
}
