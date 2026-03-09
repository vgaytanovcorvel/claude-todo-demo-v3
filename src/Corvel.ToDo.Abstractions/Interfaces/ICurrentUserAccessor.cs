namespace Corvel.ToDo.Abstractions.Interfaces;

/// <summary>
/// Provides access to the current authenticated user's identity.
/// Must only be used in contexts where authentication is guaranteed (e.g., behind [Authorize]).
/// Throws if accessed when no authenticated user is present.
/// </summary>
public interface ICurrentUserAccessor
{
    int UserId { get; }
}
