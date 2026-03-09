using Corvel.ToDo.Repository.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Corvel.ToDo.Repository.Tests.Helpers;

internal sealed class TestDbContextFactory(
    DbContextOptions<ToDoDbContext> options) : IDbContextFactory<ToDoDbContext>
{
    public ToDoDbContext CreateDbContext()
    {
        return new ToDoDbContext(options);
    }

    public Task<ToDoDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(CreateDbContext());
    }
}
