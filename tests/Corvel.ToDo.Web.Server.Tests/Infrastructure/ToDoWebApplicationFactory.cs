using Corvel.ToDo.Repository.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Corvel.ToDo.Web.Server.Tests.Infrastructure;

public class ToDoWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = "ToDoTestDb_" + Guid.NewGuid().ToString("N");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(IDbContextFactory<ToDoDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions<ToDoDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(ToDoDbContext) ||
                    (d.ServiceType.IsGenericType &&
                     d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            var databaseName = _databaseName;

            services.AddSingleton<DbContextOptions<ToDoDbContext>>(_ =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ToDoDbContext>();
                optionsBuilder.UseInMemoryDatabase(databaseName);
                return optionsBuilder.Options;
            });

            services.AddSingleton<IDbContextFactory<ToDoDbContext>>(sp =>
            {
                var options = sp.GetRequiredService<DbContextOptions<ToDoDbContext>>();
                return new InMemoryDbContextFactory(options);
            });
        });
    }

    private sealed class InMemoryDbContextFactory(DbContextOptions<ToDoDbContext> options)
        : IDbContextFactory<ToDoDbContext>
    {
        public ToDoDbContext CreateDbContext()
        {
            return new ToDoDbContext(options);
        }
    }
}
