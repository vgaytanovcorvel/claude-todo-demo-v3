namespace Corvel.ToDo.Abstractions.Models;

public record AuthToken(string Token, DateTime ExpiresAtUtc);
