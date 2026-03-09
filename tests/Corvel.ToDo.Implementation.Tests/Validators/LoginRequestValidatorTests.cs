using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Implementation.Validators;
using FluentAssertions;
using Moq;

namespace Corvel.ToDo.Implementation.Tests.Validators;

[TestClass]
public class LoginRequestValidatorTests
{
    private Mock<LoginRequestValidator> validatorMock = null!;

    [TestInitialize]
    public void Setup()
    {
        validatorMock = new Mock<LoginRequestValidator>(
            () => new LoginRequestValidator())
        { CallBase = true };
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "Password123!");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenEmailIsEmpty()
    {
        // Arrange
        var request = new LoginRequest("", "Password123!");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage.Contains("must not be empty"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenPasswordIsEmpty()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password" && e.ErrorMessage.Contains("must not be empty"));
        validatorMock.VerifyAll();
    }
}
