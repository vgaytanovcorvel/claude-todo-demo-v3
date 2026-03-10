using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Implementation.Options;
using Corvel.ToDo.Implementation.Services;
using Corvel.ToDo.Implementation.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ImplementationServiceCollectionExtensions
{
    public static IServiceCollection AddImplementationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IToDoItemService, ToDoItemService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasherWrapper>();
        services.AddSingleton(TimeProvider.System);
        services.AddValidatorsFromAssemblyContaining<CreateToDoItemRequestValidator>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();

        return services;
    }
}
