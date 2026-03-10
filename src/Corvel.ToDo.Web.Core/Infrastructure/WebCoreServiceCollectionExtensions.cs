using Corvel.ToDo.Abstractions.Interfaces;
using Corvel.ToDo.Web.Core.Services;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebCoreServiceCollectionExtensions
{
    public static IServiceCollection AddWebCore(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddControllers();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();

        return services;
    }
}
