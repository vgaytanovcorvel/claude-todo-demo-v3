namespace Corvel.ToDo.Repository.Entities;

public class UserEntity
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ICollection<ToDoItemEntity> ToDoItems { get; set; } = [];
}
