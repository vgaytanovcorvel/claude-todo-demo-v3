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
    private Mock<ILogger<GlobalExceptionHandlerMiddleware>> loggerMock = new(MockBehavior.Strict);
    private Mock<GlobalExceptionHandlerMiddleware> middlewareMock = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [TestInitialize]
    public void Setup()
    {
        RequestDelegate noOpNext = _ => Task.CompletedTask;

        middlewareMock = new Mock<GlobalExceptionHandlerMiddleware>(
            () => new GlobalExceptionHandlerMiddleware(noOpNext),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldReturn404_WhenNotFoundExceptionIsThrown()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate throwingNext = _ => throw new NotFoundException("ToDo item not found (Id: 1).");
        var throwingMiddlewareMock = new Mock<GlobalExceptionHandlerMiddleware>(
            () => new GlobalExceptionHandlerMiddleware(throwingNext),
            MockBehavior.Strict);

        throwingMiddlewareMock
            .Setup(middleware => middleware.InvokeAsync(context, loggerMock.Object))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        await throwingMiddlewareMock.Object.InvokeAsync(context, loggerMock.Object);

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

        throwingMiddlewareMock.VerifyAll();
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

        RequestDelegate throwingNext = _ => throw new ValidationException(validationFailures);
        var throwingMiddlewareMock = new Mock<GlobalExceptionHandlerMiddleware>(
            () => new GlobalExceptionHandlerMiddleware(throwingNext),
            MockBehavior.Strict);

        throwingMiddlewareMock
            .Setup(middleware => middleware.InvokeAsync(context, loggerMock.Object))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        await throwingMiddlewareMock.Object.InvokeAsync(context, loggerMock.Object);

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

        throwingMiddlewareMock.VerifyAll();
        loggerMock.VerifyAll();
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldReturn500_WhenUnhandledExceptionIsThrown()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new InvalidOperationException("Something went wrong.");
        RequestDelegate throwingNext = _ => throw exception;
        var throwingMiddlewareMock = new Mock<GlobalExceptionHandlerMiddleware>(
            () => new GlobalExceptionHandlerMiddleware(throwingNext),
            MockBehavior.Strict);

        loggerMock
            .Setup(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Verifiable(Times.Once());

        throwingMiddlewareMock
            .Setup(middleware => middleware.InvokeAsync(context, loggerMock.Object))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        await throwingMiddlewareMock.Object.InvokeAsync(context, loggerMock.Object);

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

        throwingMiddlewareMock.VerifyAll();
        loggerMock.VerifyAll();
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldReturn409_WhenDuplicateEmailExceptionIsThrown()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate throwingNext = _ => throw new DuplicateEmailException("Email already exists.");
        var throwingMiddlewareMock = new Mock<GlobalExceptionHandlerMiddleware>(
            () => new GlobalExceptionHandlerMiddleware(throwingNext),
            MockBehavior.Strict);

        throwingMiddlewareMock
            .Setup(middleware => middleware.InvokeAsync(context, loggerMock.Object))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        await throwingMiddlewareMock.Object.InvokeAsync(context, loggerMock.Object);

        // Assert
        context.Response.StatusCode.Should().Be(409);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseBody, JsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Error.Should().Be("A conflict occurred with the provided data.");
        apiResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

        throwingMiddlewareMock.VerifyAll();
        loggerMock.VerifyAll();
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldReturn401_WhenAuthenticationFailedExceptionIsThrown()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate throwingNext = _ => throw new AuthenticationFailedException("Invalid credentials.");
        var throwingMiddlewareMock = new Mock<GlobalExceptionHandlerMiddleware>(
            () => new GlobalExceptionHandlerMiddleware(throwingNext),
            MockBehavior.Strict);

        throwingMiddlewareMock
            .Setup(middleware => middleware.InvokeAsync(context, loggerMock.Object))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        await throwingMiddlewareMock.Object.InvokeAsync(context, loggerMock.Object);

        // Assert
        context.Response.StatusCode.Should().Be(401);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseBody, JsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Error.Should().Be("Authentication failed.");
        apiResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        throwingMiddlewareMock.VerifyAll();
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

        var noThrowMiddlewareMock = new Mock<GlobalExceptionHandlerMiddleware>(
            () => new GlobalExceptionHandlerMiddleware(next),
            MockBehavior.Strict);

        noThrowMiddlewareMock
            .Setup(middleware => middleware.InvokeAsync(context, loggerMock.Object))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        await noThrowMiddlewareMock.Object.InvokeAsync(context, loggerMock.Object);

        // Assert
        nextCalled.Should().BeTrue();

        noThrowMiddlewareMock.VerifyAll();
        loggerMock.VerifyAll();
    }
}
