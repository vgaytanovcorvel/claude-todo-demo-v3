using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Constants;
using Corvel.ToDo.Implementation.Validators;
using FluentAssertions;
using Moq;

namespace Corvel.ToDo.Implementation.Tests.Validators;

[TestClass]
public class ChangePasswordRequestValidatorTests
{
    private Mock<ChangePasswordRequestValidator> validatorMock = null!;

    [TestInitialize]
    public void Setup()
    {
        validatorMock = new Mock<ChangePasswordRequestValidator>(
            () => new ChangePasswordRequestValidator())
        { CallBase = true };
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var request = new ChangePasswordRequest("OldPassword123!", "NewPassword456!");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenCurrentPasswordIsEmpty()
    {
        // Arrange
        var request = new ChangePasswordRequest("", "NewPassword456!");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CurrentPassword" && e.ErrorMessage.Contains("must not be empty"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenNewPasswordIsEmpty()
    {
        // Arrange
        var request = new ChangePasswordRequest("OldPassword123!", "");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NewPassword" && e.ErrorMessage.Contains("must not be empty"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenNewPasswordIsTooShort()
    {
        // Arrange
        var shortPassword = new string('a', ValidationConstants.PasswordMinLength - 1);
        var request = new ChangePasswordRequest("OldPassword123!", shortPassword);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "NewPassword" &&
            e.ErrorMessage.Contains($"{ValidationConstants.PasswordMinLength}"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenNewPasswordExceedsMaxLength()
    {
        // Arrange
        var longPassword = new string('a', ValidationConstants.PasswordMaxLength + 1);
        var request = new ChangePasswordRequest("OldPassword123!", longPassword);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "NewPassword" &&
            e.ErrorMessage.Contains($"{ValidationConstants.PasswordMaxLength}"));
        validatorMock.VerifyAll();
    }
}
