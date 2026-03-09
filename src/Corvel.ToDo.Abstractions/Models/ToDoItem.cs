using Corvel.ToDo.Common.Enums;

namespace Corvel.ToDo.Abstractions.Models;

public class ToDoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Priority Priority { get; set; }
    public ToDoItemStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
