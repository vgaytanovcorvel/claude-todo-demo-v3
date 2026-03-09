using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Implementation.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Corvel.ToDo.Implementation.Services;

public class TokenService(
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider) : ITokenService
{
    public virtual AuthToken GenerateToken(User user)
    {
        var options = jwtOptions.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName)
        };

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expiration = now.AddMinutes(options.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: now,
            expires: expiration,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthToken(tokenString, expiration);
    }
}
