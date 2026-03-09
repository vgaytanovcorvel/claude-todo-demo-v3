using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Implementation.Options;
using Corvel.ToDo.Implementation.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace Corvel.ToDo.Implementation.Tests.Services;

[TestClass]
public class TokenServiceTests
{
    private Mock<IOptions<JwtOptions>> jwtOptionsMock = new(MockBehavior.Strict);
    private FakeTimeProvider timeProvider = null!;
    private Mock<TokenService> tokenServiceMock = null!;

    private readonly JwtOptions jwtOptions = new()
    {
        Key = "ThisIsAVeryLongSecretKeyForTestingPurposesOnly123456!",
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        ExpirationMinutes = 30
    };

    [TestInitialize]
    public void Setup()
    {
        timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero));

        jwtOptionsMock
            .Setup(o => o.Value)
            .Returns(jwtOptions)
            .Verifiable(Times.AtLeastOnce());

        tokenServiceMock = new Mock<TokenService>(
            () => new TokenService(
                jwtOptionsMock.Object,
                timeProvider),
            MockBehavior.Strict);
    }

    [TestMethod]
    public void GenerateToken_ShouldReturnTokenWithCorrectClaims_WhenUserIsValid()
    {
        // Arrange
        var user = new User
        {
            Id = 42,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        tokenServiceMock
            .Setup(service => service.GenerateToken(user))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = tokenServiceMock.Object.GenerateToken(user);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.Token);

        jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value.Should().Be("42");
        jwt.Claims.First(c => c.Type == ClaimTypes.Email).Value.Should().Be("test@example.com");
        jwt.Claims.First(c => c.Type == ClaimTypes.GivenName).Value.Should().Be("John");
        jwt.Claims.First(c => c.Type == ClaimTypes.Surname).Value.Should().Be("Doe");
        jwt.Issuer.Should().Be("TestIssuer");
        jwt.Audiences.Should().Contain("TestAudience");

        tokenServiceMock.VerifyAll();
        jwtOptionsMock.VerifyAll();
    }

    [TestMethod]
    public void GenerateToken_ShouldSetCorrectExpiration_WhenCalled()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };
        var expectedExpiration = timeProvider.GetUtcNow().UtcDateTime.AddMinutes(jwtOptions.ExpirationMinutes);

        tokenServiceMock
            .Setup(service => service.GenerateToken(user))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = tokenServiceMock.Object.GenerateToken(user);

        // Assert
        result.Should().NotBeNull();
        result.ExpiresAtUtc.Should().Be(expectedExpiration);

        tokenServiceMock.VerifyAll();
        jwtOptionsMock.VerifyAll();
    }
}
