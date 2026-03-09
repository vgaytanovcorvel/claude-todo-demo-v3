using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Enums;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Services;

public class ToDoItemService(
    IToDoItemRepository toDoItemRepository,
    TimeProvider timeProvider,
    IValidator<CreateToDoItemRequest> createValidator,
    IValidator<UpdateToDoItemRequest> updateValidator) : IToDoItemService
{
    public virtual async Task<ToDoItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await toDoItemRepository.ToDoItemSingleOrDefaultByIdAsync(id, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<ToDoItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await toDoItemRepository.ToDoItemGetAllAsync(cancellationToken);
    }

    public virtual async Task<ToDoItem> CreateAsync(CreateToDoItemRequest request, CancellationToken cancellationToken)
    {
        await createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var toDoItem = new ToDoItem
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = ToDoItemStatus.Pending,
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime,
            DueDate = request.DueDate
        };

        return await toDoItemRepository.ToDoItemAddAsync(toDoItem, cancellationToken);
    }

    public virtual async Task<ToDoItem> UpdateAsync(int id, UpdateToDoItemRequest request, CancellationToken cancellationToken)
    {
        await updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var existingItem = await toDoItemRepository.ToDoItemSingleByIdAsync(id, cancellationToken);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var completedAtUtc = existingItem.Status != ToDoItemStatus.Completed && request.Status == ToDoItemStatus.Completed
            ? now
            : existingItem.CompletedAtUtc;

        var updatedItem = new ToDoItem
        {
            Id = existingItem.Id,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = request.Status,
            CreatedAtUtc = existingItem.CreatedAtUtc,
            UpdatedAtUtc = now,
            DueDate = request.DueDate,
            CompletedAtUtc = completedAtUtc
        };

        return await toDoItemRepository.ToDoItemUpdateAsync(updatedItem, cancellationToken);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await toDoItemRepository.ToDoItemDeleteAsync(id, cancellationToken);
    }
}
