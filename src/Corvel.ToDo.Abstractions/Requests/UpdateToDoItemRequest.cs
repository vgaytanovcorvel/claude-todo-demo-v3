using Corvel.ToDo.Common.Enums;

namespace Corvel.ToDo.Abstractions.Requests;

public record UpdateToDoItemRequest(
    string Title,
    string? Description,
    Priority Priority,
    ToDoItemStatus Status,
    DateTime? DueDate);
