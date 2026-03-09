using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Constants;
using Corvel.ToDo.Implementation.Validators;
using FluentAssertions;
using Moq;

namespace Corvel.ToDo.Implementation.Tests.Validators;

[TestClass]
public class RegisterRequestValidatorTests
{
    private Mock<RegisterRequestValidator> validatorMock = null!;

    [TestInitialize]
    public void Setup()
    {
        validatorMock = new Mock<RegisterRequestValidator>(
            () => new RegisterRequestValidator())
        { CallBase = true };
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterRequest("test@example.com", "Password123!", "John", "Doe");

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
        var request = new RegisterRequest("", "Password123!", "John", "Doe");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage.Contains("must not be empty"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenEmailIsInvalidFormat()
    {
        // Arrange
        var request = new RegisterRequest("not-an-email", "Password123!", "John", "Doe");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenEmailExceedsMaxLength()
    {
        // Arrange
        var longEmail = new string('a', ValidationConstants.EmailMaxLength) + "@example.com";
        var request = new RegisterRequest(longEmail, "Password123!", "John", "Doe");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Email" &&
            e.ErrorMessage.Contains($"{ValidationConstants.EmailMaxLength}"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenPasswordIsEmpty()
    {
        // Arrange
        var request = new RegisterRequest("test@example.com", "", "John", "Doe");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password" && e.ErrorMessage.Contains("must not be empty"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenPasswordIsTooShort()
    {
        // Arrange
        var shortPassword = new string('a', ValidationConstants.PasswordMinLength - 1);
        var request = new RegisterRequest("test@example.com", shortPassword, "John", "Doe");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Password" &&
            e.ErrorMessage.Contains($"{ValidationConstants.PasswordMinLength}"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenPasswordExceedsMaxLength()
    {
        // Arrange
        var longPassword = new string('a', ValidationConstants.PasswordMaxLength + 1);
        var request = new RegisterRequest("test@example.com", longPassword, "John", "Doe");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Password" &&
            e.ErrorMessage.Contains($"{ValidationConstants.PasswordMaxLength}"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenFirstNameIsEmpty()
    {
        // Arrange
        var request = new RegisterRequest("test@example.com", "Password123!", "", "Doe");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName" && e.ErrorMessage.Contains("must not be empty"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenFirstNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('A', ValidationConstants.NameMaxLength + 1);
        var request = new RegisterRequest("test@example.com", "Password123!", longName, "Doe");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "FirstName" &&
            e.ErrorMessage.Contains($"{ValidationConstants.NameMaxLength}"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenLastNameIsEmpty()
    {
        // Arrange
        var request = new RegisterRequest("test@example.com", "Password123!", "John", "");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName" && e.ErrorMessage.Contains("must not be empty"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenLastNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('A', ValidationConstants.NameMaxLength + 1);
        var request = new RegisterRequest("test@example.com", "Password123!", "John", longName);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "LastName" &&
            e.ErrorMessage.Contains($"{ValidationConstants.NameMaxLength}"));
        validatorMock.VerifyAll();
    }
}
