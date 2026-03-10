using Microsoft.Extensions.Options;

namespace Corvel.ToDo.Implementation.Options;

public class JwtOptionsValidator : IValidateOptions<JwtOptions>
{
    public ValidateOptionsResult Validate(string? name, JwtOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Key))
            failures.Add("Jwt:Key is required.");
        else if (options.Key.Length < 32)
            failures.Add("Jwt:Key must be at least 32 characters (256 bits) for HMAC-SHA256.");

        if (string.IsNullOrWhiteSpace(options.Issuer))
            failures.Add("Jwt:Issuer is required.");

        if (string.IsNullOrWhiteSpace(options.Audience))
            failures.Add("Jwt:Audience is required.");

        if (options.ExpirationMinutes <= 0)
            failures.Add("Jwt:ExpirationMinutes must be a positive number.");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
