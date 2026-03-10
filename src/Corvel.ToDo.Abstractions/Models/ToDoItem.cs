using Corvel.ToDo.Common.Enums;

namespace Corvel.ToDo.Abstractions.Models;

public record ToDoItem
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Priority Priority { get; init; }
    public ToDoItemStatus Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
}
