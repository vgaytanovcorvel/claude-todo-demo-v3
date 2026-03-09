using Microsoft.EntityFrameworkCore;

namespace Corvel.ToDo.Repository.Repositories;

public abstract class RepositoryBase<TContext>(
    IDbContextFactory<TContext> contextFactory) where TContext : DbContext
{
    protected virtual async Task<TContext> CreateContextAsync(CancellationToken cancellationToken)
    {
        return await contextFactory.CreateDbContextAsync(cancellationToken);
    }
}
