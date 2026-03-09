using Corvel.ToDo.Abstractions.Exceptions;
using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Repository.Contexts;
using Corvel.ToDo.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace Corvel.ToDo.Repository.Repositories;

public class UserRepository(
    IDbContextFactory<ToDoDbContext> contextFactory)
    : RepositoryBase<ToDoDbContext>(contextFactory), IUserRepository
{
    public virtual async Task<User> UserSingleByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await UserSingleOrDefaultByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"User not found (UserId: {id}).");
    }

    public virtual async Task<User?> UserSingleOrDefaultByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    public virtual async Task<User?> UserSingleOrDefaultByEmailAsync(string email, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Email == email, cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    public virtual async Task<User> UserAddAsync(User user, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = MapToEntity(user);
        var entry = await dbContext.Users.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDomain(entry.Entity);
    }

    public virtual async Task<User> UserUpdateAsync(User user, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.Users
            .FirstOrDefaultAsync(e => e.Id == user.Id, cancellationToken)
            ?? throw new NotFoundException($"User not found (UserId: {user.Id}).");

        entity.Email = user.Email;
        entity.FirstName = user.FirstName;
        entity.LastName = user.LastName;
        entity.PasswordHash = user.PasswordHash;
        entity.UpdatedAtUtc = user.UpdatedAtUtc;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDomain(entity);
    }

    private static User MapToDomain(UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            Email = entity.Email,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            PasswordHash = entity.PasswordHash,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }

    private static UserEntity MapToEntity(User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PasswordHash = user.PasswordHash,
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc
        };
    }
}
