using Corvel.ToDo.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace Corvel.ToDo.Repository.Contexts;

public class ToDoDbContext(DbContextOptions<ToDoDbContext> options)
    : DbContext(options)
{
    public DbSet<ToDoItemEntity> ToDoItems => Set<ToDoItemEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ToDoDbContext).Assembly);
    }
}
