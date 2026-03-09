using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Web.Core.Middleware;
using Corvel.ToDo.Web.Core.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebCoreServiceCollectionExtensions
{
    public static IServiceCollection AddWebCoreServices(this IServiceCollection services)
    {
        services.AddScoped<GlobalExceptionHandlerMiddleware>();
        services.AddControllers();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();

        return services;
    }
}
