using System.Net;
using System.Text.Json;
using System.Threading.RateLimiting;
using Corvel.ToDo.Common.Constants;
using Corvel.ToDo.Common.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace Microsoft.Extensions.DependencyInjection;

public static class RateLimitingServiceCollectionExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                var response = ApiResponse<object>.FailureResponse(
                    "Too many requests. Please try again later.",
                    HttpStatusCode.TooManyRequests);
                await context.HttpContext.Response.WriteAsync(
                    JsonSerializer.Serialize(response, JsonOptions),
                    cancellationToken);
            };

            options.AddFixedWindowLimiter(RouteConstants.AuthRateLimitPolicy, opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 10;
                opt.QueueLimit = 0;
            });
        });

        return services;
    }
}
