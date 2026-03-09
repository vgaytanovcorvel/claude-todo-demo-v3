using Corvel.ToDo.Implementation.Services;
using FluentAssertions;
using Moq;

namespace Corvel.ToDo.Implementation.Tests.Services;

[TestClass]
public class PasswordHasherWrapperTests
{
    private Mock<PasswordHasherWrapper> passwordHasherWrapperMock = null!;

    [TestInitialize]
    public void Setup()
    {
        passwordHasherWrapperMock = new Mock<PasswordHasherWrapper>(
            () => new PasswordHasherWrapper(),
            MockBehavior.Strict);
    }

    [TestMethod]
    public void HashPassword_ShouldReturnNonEmptyHash_WhenPasswordIsValid()
    {
        // Arrange
        var password = "SecurePassword123!";

        passwordHasherWrapperMock
            .Setup(h => h.HashPassword(password))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = passwordHasherWrapperMock.Object.HashPassword(password);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotBe(password);

        passwordHasherWrapperMock.VerifyAll();
    }

    [TestMethod]
    public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatchesHash()
    {
        // Arrange
        var password = "SecurePassword123!";

        passwordHasherWrapperMock
            .Setup(h => h.HashPassword(password))
            .CallBase()
            .Verifiable(Times.Once());

        passwordHasherWrapperMock
            .Setup(h => h.VerifyPassword(password, It.IsAny<string>()))
            .CallBase()
            .Verifiable(Times.Once());

        var hash = passwordHasherWrapperMock.Object.HashPassword(password);

        // Act
        var result = passwordHasherWrapperMock.Object.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();

        passwordHasherWrapperMock.VerifyAll();
    }

    [TestMethod]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatchHash()
    {
        // Arrange
        var password = "SecurePassword123!";
        var wrongPassword = "WrongPassword456!";

        passwordHasherWrapperMock
            .Setup(h => h.HashPassword(password))
            .CallBase()
            .Verifiable(Times.Once());

        passwordHasherWrapperMock
            .Setup(h => h.VerifyPassword(wrongPassword, It.IsAny<string>()))
            .CallBase()
            .Verifiable(Times.Once());

        var hash = passwordHasherWrapperMock.Object.HashPassword(password);

        // Act
        var result = passwordHasherWrapperMock.Object.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();

        passwordHasherWrapperMock.VerifyAll();
    }
}
