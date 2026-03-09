using Corvel.ToDo.Common.Enums;

namespace Corvel.ToDo.Abstractions.Requests;

public record CreateToDoItemRequest(
    string Title,
    string? Description,
    Priority Priority,
    DateTime? DueDate);
