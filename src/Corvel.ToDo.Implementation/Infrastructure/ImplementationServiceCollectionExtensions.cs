using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Implementation.Services;
using Corvel.ToDo.Implementation.Validators;
using FluentValidation;

namespace Microsoft.Extensions.DependencyInjection;

public static class ImplementationServiceCollectionExtensions
{
    public static IServiceCollection AddImplementationServices(this IServiceCollection services)
    {
        services.AddScoped<IToDoItemService, ToDoItemService>();
        services.AddSingleton(TimeProvider.System);
        services.AddValidatorsFromAssemblyContaining<CreateToDoItemRequestValidator>();

        return services;
    }
}
