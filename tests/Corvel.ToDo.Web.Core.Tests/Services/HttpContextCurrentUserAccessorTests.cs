using System.Security.Claims;
using Corvel.ToDo.Web.Core.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Corvel.ToDo.Web.Core.Tests.Services;

[TestClass]
public class HttpContextCurrentUserAccessorTests
{
    private Mock<IHttpContextAccessor> httpContextAccessorMock = new(MockBehavior.Strict);
    private Mock<HttpContextCurrentUserAccessor> accessorMock = null!;

    [TestInitialize]
    public void Setup()
    {
        accessorMock = new Mock<HttpContextCurrentUserAccessor>(
            () => new HttpContextCurrentUserAccessor(httpContextAccessorMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public void UserId_ShouldReturnUserId_WhenValidClaimExists()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "42")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        httpContextAccessorMock
            .Setup(accessor => accessor.HttpContext)
            .Returns(httpContext)
            .Verifiable(Times.Once());

        accessorMock
            .Setup(accessor => accessor.UserId)
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = accessorMock.Object.UserId;

        // Assert
        result.Should().Be(42);

        accessorMock.VerifyAll();
        httpContextAccessorMock.VerifyAll();
    }

    [TestMethod]
    public void UserId_ShouldThrowInvalidOperationException_WhenNoClaimExists()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        httpContextAccessorMock
            .Setup(accessor => accessor.HttpContext)
            .Returns(httpContext)
            .Verifiable(Times.Once());

        accessorMock
            .Setup(accessor => accessor.UserId)
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var action = () => accessorMock.Object.UserId;

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("User is not authenticated.");

        accessorMock.VerifyAll();
        httpContextAccessorMock.VerifyAll();
    }

    [TestMethod]
    public void UserId_ShouldThrowInvalidOperationException_WhenClaimIsNotInteger()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "not-a-number")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        httpContextAccessorMock
            .Setup(accessor => accessor.HttpContext)
            .Returns(httpContext)
            .Verifiable(Times.Once());

        accessorMock
            .Setup(accessor => accessor.UserId)
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var action = () => accessorMock.Object.UserId;

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("User is not authenticated.");

        accessorMock.VerifyAll();
        httpContextAccessorMock.VerifyAll();
    }
}
