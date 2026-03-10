using System.Net;
using System.Text.Json;
using Corvel.ToDo.Abstractions.Exceptions;
using Corvel.ToDo.Common.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Corvel.ToDo.Web.Core.Middleware;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public virtual async Task InvokeAsync(HttpContext context, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteResponseAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (DuplicateEmailException)
        {
            await WriteResponseAsync(context, HttpStatusCode.Conflict, "A conflict occurred with the provided data.");
        }
        catch (AuthenticationFailedException)
        {
            await WriteResponseAsync(context, HttpStatusCode.Unauthorized, "Authentication failed.");
        }
        catch (ValidationException ex)
        {
            var errorMessage = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, errorMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string error)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        var response = ApiResponse<object>.FailureResponse(error, statusCode);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
