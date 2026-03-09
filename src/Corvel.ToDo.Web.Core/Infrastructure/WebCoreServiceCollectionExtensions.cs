using Corvel.ToDo.Web.Core.Middleware;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebCoreServiceCollectionExtensions
{
    public static IServiceCollection AddWebCoreServices(this IServiceCollection services)
    {
        services.AddScoped<GlobalExceptionHandlerMiddleware>();
        services.AddControllers();

        return services;
    }
}
