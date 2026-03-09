using Corvel.ToDo.Abstractions.Exceptions;
using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Implementation.Services;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace Corvel.ToDo.Implementation.Tests.Services;

[TestClass]
public class UserServiceTests
{
    private Mock<IUserRepository> userRepositoryMock = new(MockBehavior.Strict);
    private Mock<IPasswordHasher> passwordHasherMock = new(MockBehavior.Strict);
    private Mock<ITokenService> tokenServiceMock = new(MockBehavior.Strict);
    private Mock<ICurrentUserAccessor> currentUserAccessorMock = new(MockBehavior.Strict);
    private Mock<IValidator<RegisterRequest>> registerValidatorMock = new(MockBehavior.Strict);
    private Mock<IValidator<LoginRequest>> loginValidatorMock = new(MockBehavior.Strict);
    private Mock<IValidator<UpdateProfileRequest>> updateProfileValidatorMock = new(MockBehavior.Strict);
    private Mock<IValidator<ChangePasswordRequest>> changePasswordValidatorMock = new(MockBehavior.Strict);
    private FakeTimeProvider timeProvider = null!;
    private Mock<UserService> userServiceMock = null!;

    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly int userId = 42;

    [TestInitialize]
    public void Setup()
    {
        timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero));

        userServiceMock = new Mock<UserService>(
            () => new UserService(
                userRepositoryMock.Object,
                passwordHasherMock.Object,
                tokenServiceMock.Object,
                currentUserAccessorMock.Object,
                timeProvider,
                registerValidatorMock.Object,
                loginValidatorMock.Object,
                updateProfileValidatorMock.Object,
                changePasswordValidatorMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task RegisterAsync_ShouldReturnAuthToken_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterRequest("test@example.com", "Password123!", "John", "Doe");
        var expectedUtc = timeProvider.GetUtcNow().UtcDateTime;
        var hashedPassword = "hashed_password_value";
        var createdUser = new User
        {
            Id = 1,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = hashedPassword,
            CreatedAtUtc = expectedUtc
        };
        var expectedToken = new AuthToken("jwt_token_value", expectedUtc.AddMinutes(60));

        userServiceMock
            .Setup(service => service.RegisterAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        registerValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<RegisterRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleOrDefaultByEmailAsync(request.Email, cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        passwordHasherMock
            .Setup(h => h.HashPassword(request.Password))
            .Returns(hashedPassword)
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserAddAsync(
                It.Is<User>(u =>
                    u.Email == request.Email &&
                    u.FirstName == request.FirstName &&
                    u.LastName == request.LastName &&
                    u.PasswordHash == hashedPassword &&
                    u.CreatedAtUtc == expectedUtc),
                cancellationToken))
            .ReturnsAsync(createdUser)
            .Verifiable(Times.Once());

        tokenServiceMock
            .Setup(t => t.GenerateToken(createdUser))
            .Returns(expectedToken)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.RegisterAsync(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("jwt_token_value");
        result.ExpiresAtUtc.Should().Be(expectedToken.ExpiresAtUtc);

        userServiceMock.VerifyAll();
        registerValidatorMock.VerifyAll();
        userRepositoryMock.VerifyAll();
        passwordHasherMock.VerifyAll();
        tokenServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task RegisterAsync_ShouldThrowDuplicateEmailException_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest("existing@example.com", "Password123!", "John", "Doe");
        var existingUser = new User { Id = 1, Email = request.Email };

        userServiceMock
            .Setup(service => service.RegisterAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        registerValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<RegisterRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleOrDefaultByEmailAsync(request.Email, cancellationToken))
            .ReturnsAsync(existingUser)
            .Verifiable(Times.Once());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<DuplicateEmailException>(
            () => userServiceMock.Object.RegisterAsync(request, cancellationToken));

        userServiceMock.VerifyAll();
        registerValidatorMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task RegisterAsync_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new RegisterRequest("", "", "", "");
        var validationFailures = new List<ValidationFailure>
        {
            new("Email", "'Email' must not be empty.")
        };

        userServiceMock
            .Setup(service => service.RegisterAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        registerValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<RegisterRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ThrowsAsync(new ValidationException(validationFailures))
            .Verifiable(Times.Once());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ValidationException>(
            () => userServiceMock.Object.RegisterAsync(request, cancellationToken));

        userServiceMock.VerifyAll();
        registerValidatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task LoginAsync_ShouldReturnAuthToken_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "Password123!");
        var expectedUtc = timeProvider.GetUtcNow().UtcDateTime;
        var user = new User
        {
            Id = 1,
            Email = request.Email,
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hashed_password"
        };
        var expectedToken = new AuthToken("jwt_token_value", expectedUtc.AddMinutes(60));

        userServiceMock
            .Setup(service => service.LoginAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        loginValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<LoginRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleOrDefaultByEmailAsync(request.Email, cancellationToken))
            .ReturnsAsync(user)
            .Verifiable(Times.Once());

        passwordHasherMock
            .Setup(h => h.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(true)
            .Verifiable(Times.Once());

        tokenServiceMock
            .Setup(t => t.GenerateToken(user))
            .Returns(expectedToken)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.LoginAsync(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("jwt_token_value");

        userServiceMock.VerifyAll();
        loginValidatorMock.VerifyAll();
        userRepositoryMock.VerifyAll();
        passwordHasherMock.VerifyAll();
        tokenServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task LoginAsync_ShouldThrowAuthenticationFailedException_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequest("unknown@example.com", "Password123!");

        userServiceMock
            .Setup(service => service.LoginAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        loginValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<LoginRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleOrDefaultByEmailAsync(request.Email, cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<AuthenticationFailedException>(
            () => userServiceMock.Object.LoginAsync(request, cancellationToken));

        userServiceMock.VerifyAll();
        loginValidatorMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task LoginAsync_ShouldThrowAuthenticationFailedException_WhenPasswordIsWrong()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "WrongPassword!");
        var user = new User
        {
            Id = 1,
            Email = request.Email,
            PasswordHash = "hashed_password"
        };

        userServiceMock
            .Setup(service => service.LoginAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        loginValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<LoginRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleOrDefaultByEmailAsync(request.Email, cancellationToken))
            .ReturnsAsync(user)
            .Verifiable(Times.Once());

        passwordHasherMock
            .Setup(h => h.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(false)
            .Verifiable(Times.Once());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<AuthenticationFailedException>(
            () => userServiceMock.Object.LoginAsync(request, cancellationToken));

        userServiceMock.VerifyAll();
        loginValidatorMock.VerifyAll();
        userRepositoryMock.VerifyAll();
        passwordHasherMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetProfileAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var expectedUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        userServiceMock
            .Setup(service => service.GetProfileAsync(cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        currentUserAccessorMock
            .Setup(a => a.UserId)
            .Returns(userId)
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleByIdAsync(userId, cancellationToken))
            .ReturnsAsync(expectedUser)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.GetProfileAsync(cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Email.Should().Be("test@example.com");

        userServiceMock.VerifyAll();
        currentUserAccessorMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateProfileAsync_ShouldReturnUpdatedUser_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateProfileRequest("Jane", "Smith", "jane@example.com");
        var expectedUtc = timeProvider.GetUtcNow().UtcDateTime;
        var existingUser = new User
        {
            Id = userId,
            Email = "old@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hashed_password",
            CreatedAtUtc = expectedUtc.AddDays(-30)
        };
        var updatedUser = new User
        {
            Id = userId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = existingUser.PasswordHash,
            CreatedAtUtc = existingUser.CreatedAtUtc,
            UpdatedAtUtc = expectedUtc
        };

        userServiceMock
            .Setup(service => service.UpdateProfileAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        updateProfileValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<UpdateProfileRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        currentUserAccessorMock
            .Setup(a => a.UserId)
            .Returns(userId)
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleByIdAsync(userId, cancellationToken))
            .ReturnsAsync(existingUser)
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleOrDefaultByEmailAsync(request.Email, cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserUpdateAsync(
                It.Is<User>(u =>
                    u.Id == userId &&
                    u.Email == request.Email &&
                    u.FirstName == request.FirstName &&
                    u.LastName == request.LastName &&
                    u.PasswordHash == existingUser.PasswordHash &&
                    u.UpdatedAtUtc == expectedUtc),
                cancellationToken))
            .ReturnsAsync(updatedUser)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.UpdateProfileAsync(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        result.Email.Should().Be("jane@example.com");

        userServiceMock.VerifyAll();
        updateProfileValidatorMock.VerifyAll();
        currentUserAccessorMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateProfileAsync_ShouldThrowDuplicateEmailException_WhenNewEmailAlreadyTaken()
    {
        // Arrange
        var request = new UpdateProfileRequest("Jane", "Smith", "taken@example.com");
        var existingUser = new User
        {
            Id = userId,
            Email = "old@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hashed_password",
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime.AddDays(-30)
        };
        var otherUser = new User { Id = 99, Email = "taken@example.com" };

        userServiceMock
            .Setup(service => service.UpdateProfileAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        updateProfileValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<UpdateProfileRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        currentUserAccessorMock
            .Setup(a => a.UserId)
            .Returns(userId)
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleByIdAsync(userId, cancellationToken))
            .ReturnsAsync(existingUser)
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleOrDefaultByEmailAsync(request.Email, cancellationToken))
            .ReturnsAsync(otherUser)
            .Verifiable(Times.Once());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<DuplicateEmailException>(
            () => userServiceMock.Object.UpdateProfileAsync(request, cancellationToken));

        userServiceMock.VerifyAll();
        updateProfileValidatorMock.VerifyAll();
        currentUserAccessorMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task ChangePasswordAsync_ShouldSucceed_WhenCurrentPasswordIsCorrect()
    {
        // Arrange
        var request = new ChangePasswordRequest("OldPassword123!", "NewPassword456!");
        var expectedUtc = timeProvider.GetUtcNow().UtcDateTime;
        var existingUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "old_hashed_password",
            CreatedAtUtc = expectedUtc.AddDays(-30)
        };
        var newHash = "new_hashed_password";

        userServiceMock
            .Setup(service => service.ChangePasswordAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        changePasswordValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<ChangePasswordRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        currentUserAccessorMock
            .Setup(a => a.UserId)
            .Returns(userId)
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleByIdAsync(userId, cancellationToken))
            .ReturnsAsync(existingUser)
            .Verifiable(Times.Once());

        passwordHasherMock
            .Setup(h => h.VerifyPassword(request.CurrentPassword, existingUser.PasswordHash))
            .Returns(true)
            .Verifiable(Times.Once());

        passwordHasherMock
            .Setup(h => h.HashPassword(request.NewPassword))
            .Returns(newHash)
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserUpdateAsync(
                It.Is<User>(u =>
                    u.Id == userId &&
                    u.PasswordHash == newHash &&
                    u.UpdatedAtUtc == expectedUtc),
                cancellationToken))
            .ReturnsAsync(existingUser)
            .Verifiable(Times.Once());

        // Act
        await userServiceMock.Object.ChangePasswordAsync(request, cancellationToken);

        // Assert
        userServiceMock.VerifyAll();
        changePasswordValidatorMock.VerifyAll();
        currentUserAccessorMock.VerifyAll();
        userRepositoryMock.VerifyAll();
        passwordHasherMock.VerifyAll();
    }

    [TestMethod]
    public async Task ChangePasswordAsync_ShouldThrowAuthenticationFailedException_WhenCurrentPasswordIsWrong()
    {
        // Arrange
        var request = new ChangePasswordRequest("WrongPassword!", "NewPassword456!");
        var existingUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime.AddDays(-30)
        };

        userServiceMock
            .Setup(service => service.ChangePasswordAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        changePasswordValidatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<ChangePasswordRequest>>(ctx => ctx.InstanceToValidate == request),
                cancellationToken))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(Times.Once());

        currentUserAccessorMock
            .Setup(a => a.UserId)
            .Returns(userId)
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(repo => repo.UserSingleByIdAsync(userId, cancellationToken))
            .ReturnsAsync(existingUser)
            .Verifiable(Times.Once());

        passwordHasherMock
            .Setup(h => h.VerifyPassword(request.CurrentPassword, existingUser.PasswordHash))
            .Returns(false)
            .Verifiable(Times.Once());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<AuthenticationFailedException>(
            () => userServiceMock.Object.ChangePasswordAsync(request, cancellationToken));

        userServiceMock.VerifyAll();
        changePasswordValidatorMock.VerifyAll();
        currentUserAccessorMock.VerifyAll();
        userRepositoryMock.VerifyAll();
        passwordHasherMock.VerifyAll();
    }
}
