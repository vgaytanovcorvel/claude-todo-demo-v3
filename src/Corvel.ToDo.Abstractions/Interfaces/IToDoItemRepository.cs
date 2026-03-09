using Corvel.ToDo.Abstractions.Models;

namespace Corvel.ToDo.Abstractions.Interfaces;

public interface IToDoItemRepository
{
    Task<ToDoItem> ToDoItemSingleByIdAsync(int id, CancellationToken cancellationToken);
    Task<ToDoItem?> ToDoItemSingleOrDefaultByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ToDoItem>> ToDoItemGetAllAsync(CancellationToken cancellationToken);
    Task<ToDoItem> ToDoItemAddAsync(ToDoItem toDoItem, CancellationToken cancellationToken);
    Task<ToDoItem> ToDoItemUpdateAsync(ToDoItem toDoItem, CancellationToken cancellationToken);
    Task ToDoItemDeleteAsync(int id, CancellationToken cancellationToken);
}
