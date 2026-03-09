using Corvel.ToDo.Abstractions.Models;

namespace Corvel.ToDo.Abstractions.Interfaces;

public interface ITokenService
{
    AuthToken GenerateToken(User user);
}
