using Corvel.ToDo.Abstractions.Models;
using Corvel.ToDo.Abstractions.Requests;

namespace Corvel.ToDo.Abstractions.Interfaces;

public interface IToDoItemService
{
    Task<ToDoItem?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ToDoItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<ToDoItem> CreateAsync(CreateToDoItemRequest request, CancellationToken cancellationToken);
    Task<ToDoItem> UpdateAsync(int id, UpdateToDoItemRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
