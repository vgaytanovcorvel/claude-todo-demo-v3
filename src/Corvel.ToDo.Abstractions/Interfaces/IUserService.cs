using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;

namespace Corvel.ToDo.Abstractions.Interfaces;

public interface IUserService
{
    Task<AuthToken> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<AuthToken> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<User> GetProfileAsync(CancellationToken cancellationToken);
    Task<User> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken);
    Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken);
}
