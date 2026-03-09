using Corvel.ToDo.Abstractions.Exceptions;
using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Repository.Contexts;
using Corvel.ToDo.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace Corvel.ToDo.Repository.Repositories;

public class ToDoItemRepository(
    IDbContextFactory<ToDoDbContext> contextFactory)
    : RepositoryBase<ToDoDbContext>(contextFactory), IToDoItemRepository
{
    public virtual async Task<ToDoItem> ToDoItemSingleByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await ToDoItemSingleOrDefaultByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"ToDoItem not found (Id: {id}).");
    }

    public virtual async Task<ToDoItem?> ToDoItemSingleOrDefaultByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.ToDoItems
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    public virtual async Task<IReadOnlyList<ToDoItem>> ToDoItemGetAllAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entities = await dbContext.ToDoItems
            .AsNoTracking()
            .OrderByDescending(e => e.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomain).ToList();
    }

    public virtual async Task<ToDoItem> ToDoItemAddAsync(ToDoItem toDoItem, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = MapToEntity(toDoItem);
        var entry = await dbContext.ToDoItems.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDomain(entry.Entity);
    }

    public virtual async Task<ToDoItem> ToDoItemUpdateAsync(ToDoItem toDoItem, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.ToDoItems
            .FirstOrDefaultAsync(e => e.Id == toDoItem.Id, cancellationToken)
            ?? throw new NotFoundException($"ToDoItem not found (Id: {toDoItem.Id}).");

        entity.Title = toDoItem.Title;
        entity.Description = toDoItem.Description;
        entity.Priority = toDoItem.Priority;
        entity.Status = toDoItem.Status;
        entity.UpdatedAtUtc = toDoItem.UpdatedAtUtc;
        entity.DueDate = toDoItem.DueDate;
        entity.CompletedAtUtc = toDoItem.CompletedAtUtc;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDomain(entity);
    }

    public virtual async Task ToDoItemDeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var entity = await dbContext.ToDoItems
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new NotFoundException($"ToDoItem not found (Id: {id}).");

        dbContext.ToDoItems.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static ToDoItem MapToDomain(ToDoItemEntity entity)
    {
        return new ToDoItem
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Priority = entity.Priority,
            Status = entity.Status,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc,
            DueDate = entity.DueDate,
            CompletedAtUtc = entity.CompletedAtUtc
        };
    }

    private static ToDoItemEntity MapToEntity(ToDoItem toDoItem)
    {
        return new ToDoItemEntity
        {
            Id = toDoItem.Id,
            Title = toDoItem.Title,
            Description = toDoItem.Description,
            Priority = toDoItem.Priority,
            Status = toDoItem.Status,
            CreatedAtUtc = toDoItem.CreatedAtUtc,
            UpdatedAtUtc = toDoItem.UpdatedAtUtc,
            DueDate = toDoItem.DueDate,
            CompletedAtUtc = toDoItem.CompletedAtUtc
        };
    }
}
