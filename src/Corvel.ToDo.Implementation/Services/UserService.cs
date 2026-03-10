using Corvel.ToDo.Abstractions.Exceptions;
using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Services;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ICurrentUserAccessor currentUserAccessor,
    TimeProvider timeProvider,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator,
    IValidator<UpdateProfileRequest> updateProfileValidator,
    IValidator<ChangePasswordRequest> changePasswordValidator) : IUserService
{
    public virtual async Task<AuthToken> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        await registerValidator.ValidateAndThrowAsync(request, cancellationToken);

        var existingUser = await userRepository.UserSingleOrDefaultByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new DuplicateEmailException("An account with this email already exists.");
        }

        var hashedPassword = passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = hashedPassword,
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        };

        var createdUser = await userRepository.UserAddAsync(user, cancellationToken);

        return tokenService.GenerateToken(createdUser);
    }

    public virtual async Task<AuthToken> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        await loginValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await userRepository.UserSingleOrDefaultByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            throw new AuthenticationFailedException("Invalid email or password.");
        }

        var isPasswordValid = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new AuthenticationFailedException("Invalid email or password.");
        }

        return tokenService.GenerateToken(user);
    }

    public virtual async Task<User> GetProfileAsync(CancellationToken cancellationToken)
    {
        var userId = currentUserAccessor.UserId;
        return await userRepository.UserSingleByIdAsync(userId, cancellationToken);
    }

    public virtual async Task<User> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        await updateProfileValidator.ValidateAndThrowAsync(request, cancellationToken);

        var userId = currentUserAccessor.UserId;
        var existingUser = await userRepository.UserSingleByIdAsync(userId, cancellationToken);

        if (!string.Equals(existingUser.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailTaken = await userRepository.UserSingleOrDefaultByEmailAsync(request.Email, cancellationToken);
            if (emailTaken is not null)
            {
                throw new DuplicateEmailException("An account with this email already exists.");
            }
        }

        var updatedUser = new User
        {
            Id = existingUser.Id,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = existingUser.PasswordHash,
            CreatedAtUtc = existingUser.CreatedAtUtc,
            UpdatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        };

        return await userRepository.UserUpdateAsync(updatedUser, cancellationToken);
    }

    public virtual async Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await changePasswordValidator.ValidateAndThrowAsync(request, cancellationToken);

        var userId = currentUserAccessor.UserId;
        var existingUser = await userRepository.UserSingleByIdAsync(userId, cancellationToken);

        var isCurrentPasswordValid = passwordHasher.VerifyPassword(request.CurrentPassword, existingUser.PasswordHash);
        if (!isCurrentPasswordValid)
        {
            throw new AuthenticationFailedException("Current password is incorrect.");
        }

        var newHash = passwordHasher.HashPassword(request.NewPassword);

        var updatedUser = new User
        {
            Id = existingUser.Id,
            Email = existingUser.Email,
            FirstName = existingUser.FirstName,
            LastName = existingUser.LastName,
            PasswordHash = newHash,
            CreatedAtUtc = existingUser.CreatedAtUtc,
            UpdatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        };

        await userRepository.UserUpdateAsync(updatedUser, cancellationToken);
    }
}
