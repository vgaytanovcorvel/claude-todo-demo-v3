using Corvel.ToDo.Common.Enums;

namespace Corvel.ToDo.Web.Core.Models;

public record ToDoItemResponse(
    int Id,
    int UserId,
    string Title,
    string? Description,
    Priority Priority,
    ToDoItemStatus Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DueDate,
    DateTime? CompletedAtUtc);
