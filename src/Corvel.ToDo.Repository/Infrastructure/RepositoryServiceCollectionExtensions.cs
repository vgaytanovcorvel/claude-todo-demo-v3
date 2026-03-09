using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Repository.Contexts;
using Corvel.ToDo.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextFactory<ToDoDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IToDoItemRepository, ToDoItemRepository>();

        return services;
    }
}
