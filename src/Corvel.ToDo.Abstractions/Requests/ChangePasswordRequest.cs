namespace Corvel.ToDo.Abstractions.Requests;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
