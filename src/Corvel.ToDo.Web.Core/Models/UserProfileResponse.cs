namespace Corvel.ToDo.Web.Core.Models;

public record UserProfileResponse(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
