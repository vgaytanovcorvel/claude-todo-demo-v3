using System.Net;
using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Dtos;
using Corvel.ToDo.Web.Core.Controllers;
using Corvel.ToDo.Web.Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Corvel.ToDo.Web.Core.Tests.Controllers;

[TestClass]
public class UserProfileControllerTests
{
    private Mock<IUserService> userServiceMock = new(MockBehavior.Strict);
    private Mock<UserProfileController> controllerMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        controllerMock = new Mock<UserProfileController>(
            () => new UserProfileController(userServiceMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task GetProfile_ShouldReturnUserProfile_WhenUserIsAuthenticated()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hashed-password-should-not-appear",
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = null
        };

        userServiceMock
            .Setup(service => service.GetProfileAsync(cancellationToken))
            .ReturnsAsync(user)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.GetProfile(cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Ok(It.IsAny<object>()))
            .CallBase();

        // Act
        var result = await controllerMock.Object.GetProfile(cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponse<UserProfileResponse>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(1);
        apiResponse.Data.Email.Should().Be("test@example.com");
        apiResponse.Data.FirstName.Should().Be("John");
        apiResponse.Data.LastName.Should().Be("Doe");
        apiResponse.Data.CreatedAtUtc.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        apiResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        controllerMock.VerifyAll();
        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateProfile_ShouldReturnUpdatedProfile_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateProfileRequest("Jane", "Smith", "jane@example.com");
        var updatedUser = new User
        {
            Id = 1,
            Email = "jane@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            PasswordHash = "hashed-password-should-not-appear",
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2026, 3, 9, 0, 0, 0, DateTimeKind.Utc)
        };

        userServiceMock
            .Setup(service => service.UpdateProfileAsync(request, cancellationToken))
            .ReturnsAsync(updatedUser)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.UpdateProfile(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.Ok(It.IsAny<object>()))
            .CallBase();

        // Act
        var result = await controllerMock.Object.UpdateProfile(request, cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponse<UserProfileResponse>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.FirstName.Should().Be("Jane");
        apiResponse.Data.LastName.Should().Be("Smith");
        apiResponse.Data.Email.Should().Be("jane@example.com");
        apiResponse.Data.UpdatedAtUtc.Should().Be(new DateTime(2026, 3, 9, 0, 0, 0, DateTimeKind.Utc));
        apiResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        controllerMock.VerifyAll();
        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task ChangePassword_ShouldReturnNoContent_WhenPasswordChanged()
    {
        // Arrange
        var request = new ChangePasswordRequest("OldPassword123!", "NewPassword456!");

        userServiceMock
            .Setup(service => service.ChangePasswordAsync(request, cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.ChangePassword(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        controllerMock
            .Setup(controller => controller.NoContent())
            .CallBase();

        // Act
        var result = await controllerMock.Object.ChangePassword(request, cancellationToken);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);

        controllerMock.VerifyAll();
        userServiceMock.VerifyAll();
    }
}
