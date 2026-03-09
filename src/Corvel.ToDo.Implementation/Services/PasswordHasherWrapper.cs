using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Microsoft.AspNetCore.Identity;

namespace Corvel.ToDo.Implementation.Services;

public class PasswordHasherWrapper() : IPasswordHasher
{
    private static readonly User SentinelUser = new();
    private readonly PasswordHasher<User> innerHasher = new();

    public virtual string HashPassword(string password)
    {
        return innerHasher.HashPassword(SentinelUser, password);
    }

    public virtual bool VerifyPassword(string password, string hash)
    {
        var result = innerHasher.VerifyHashedPassword(SentinelUser, hash, password);
        return result != PasswordVerificationResult.Failed;
    }
}
