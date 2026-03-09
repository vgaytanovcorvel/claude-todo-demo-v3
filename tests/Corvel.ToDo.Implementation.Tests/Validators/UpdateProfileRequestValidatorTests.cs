using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Constants;
using Corvel.ToDo.Implementation.Validators;
using FluentAssertions;
using Moq;

namespace Corvel.ToDo.Implementation.Tests.Validators;

[TestClass]
public class UpdateProfileRequestValidatorTests
{
    private Mock<UpdateProfileRequestValidator> validatorMock = null!;

    [TestInitialize]
    public void Setup()
    {
        validatorMock = new Mock<UpdateProfileRequestValidator>(
            () => new UpdateProfileRequestValidator())
        { CallBase = true };
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateProfileRequest("John", "Doe", "test@example.com");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenFirstNameIsEmpty()
    {
        // Arrange
        var request = new UpdateProfileRequest("", "Doe", "test@example.com");

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
        var request = new UpdateProfileRequest(longName, "Doe", "test@example.com");

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
        var request = new UpdateProfileRequest("John", "", "test@example.com");

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
        var request = new UpdateProfileRequest("John", longName, "test@example.com");

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "LastName" &&
            e.ErrorMessage.Contains($"{ValidationConstants.NameMaxLength}"));
        validatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFail_WhenEmailIsEmpty()
    {
        // Arrange
        var request = new UpdateProfileRequest("John", "Doe", "");

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
        var request = new UpdateProfileRequest("John", "Doe", "not-an-email");

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
        var request = new UpdateProfileRequest("John", "Doe", longEmail);

        // Act
        var result = await validatorMock.Object.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Email" &&
            e.ErrorMessage.Contains($"{ValidationConstants.EmailMaxLength}"));
        validatorMock.VerifyAll();
    }
}
