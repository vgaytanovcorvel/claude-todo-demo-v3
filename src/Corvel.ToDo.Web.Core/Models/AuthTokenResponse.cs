namespace Corvel.ToDo.Web.Core.Models;

public record AuthTokenResponse(string Token, DateTime ExpiresAtUtc);
