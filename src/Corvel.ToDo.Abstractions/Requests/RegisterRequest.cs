namespace Corvel.ToDo.Abstractions.Requests;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
