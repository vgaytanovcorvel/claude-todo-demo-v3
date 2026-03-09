using Corvel.ToDo.Abstractions.Models;

namespace Corvel.ToDo.Abstractions.Interfaces;

public interface IUserRepository
{
    Task<User> UserSingleByIdAsync(int id, CancellationToken cancellationToken);
    Task<User?> UserSingleOrDefaultByIdAsync(int id, CancellationToken cancellationToken);
    Task<User?> UserSingleOrDefaultByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User> UserAddAsync(User user, CancellationToken cancellationToken);
    Task<User> UserUpdateAsync(User user, CancellationToken cancellationToken);
}
