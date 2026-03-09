using System.Net;
using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Dtos;
using Corvel.ToDo.Web.Core.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Corvel.ToDo.Web.Core.Tests.Controllers;

[TestClass]
public class AuthControllerTests
{
    private Mock<IUserService> userServiceMock = new(MockBehavior.Strict);
    private Mock<AuthController> controllerMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        controllerMock = new Mock<AuthController>(
            () => new AuthController(userServiceMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task Register_ShouldReturnCreatedAuthToken_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterRequest("test@example.com", "Password123!", "John", "Doe");
        var authToken = new AuthToken("jwt-token-value", new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc));

        userServiceMock
            .Setup(service => service.RegisterAsync(request, cancellationToken))
            .ReturnsAsync(authToken)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Register(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.StatusCode(201, It.IsAny<object>()))
            .CallBase();

        // Act
        var result = await controllerMock.Object.Register(request, cancellationToken);

        // Assert
        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(201);

        var apiResponse = objectResult.Value as ApiResponse<AuthToken>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Token.Should().Be("jwt-token-value");
        apiResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        controllerMock.VerifyAll();
        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Login_ShouldReturnAuthToken_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "Password123!");
        var authToken = new AuthToken("jwt-token-value", new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc));

        userServiceMock
            .Setup(service => service.LoginAsync(request, cancellationToken))
            .ReturnsAsync(authToken)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Login(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Ok(It.IsAny<object>()))
            .CallBase();

        // Act
        var result = await controllerMock.Object.Login(request, cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponse<AuthToken>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Token.Should().Be("jwt-token-value");
        apiResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        controllerMock.VerifyAll();
        userServiceMock.VerifyAll();
    }
}
