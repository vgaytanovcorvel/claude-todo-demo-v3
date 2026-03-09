using System.Net;
using System.Text.Json;
using Corvel.ToDo.Abstractions.Exceptions;
using Corvel.ToDo.Common.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Corvel.ToDo.Web.Core.Middleware;

public class GlobalExceptionHandlerMiddleware(
    ILogger<GlobalExceptionHandlerMiddleware> logger) : IMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public virtual async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteResponseAsync(
                context,
                HttpStatusCode.NotFound,
                ex.Message);
        }
        catch (DuplicateEmailException ex)
        {
            await WriteResponseAsync(
                context,
                HttpStatusCode.Conflict,
                ex.Message);
        }
        catch (AuthenticationFailedException ex)
        {
            await WriteResponseAsync(
                context,
                HttpStatusCode.Unauthorized,
                ex.Message);
        }
        catch (ValidationException ex)
        {
            var errorMessage = string.Join(" ", ex.Errors.Select(e => e.ErrorMessage));

            await WriteResponseAsync(
                context,
                HttpStatusCode.BadRequest,
                errorMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");

            await WriteResponseAsync(
                context,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponseAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string error)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.FailureResponse(error, statusCode);

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions));
    }
}
