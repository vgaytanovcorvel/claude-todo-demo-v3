using System.Net;

namespace Corvel.ToDo.Common.Dtos;

public record ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public HttpStatusCode StatusCode { get; init; }

    public static ApiResponse<T> SuccessResponse(T data, HttpStatusCode statusCode = HttpStatusCode.OK) =>
        new()
        {
            Success = true,
            Data = data,
            Error = null,
            StatusCode = statusCode
        };

    public static ApiResponse<T> FailureResponse(string error, HttpStatusCode statusCode) =>
        new()
        {
            Success = false,
            Data = default,
            Error = error,
            StatusCode = statusCode
        };
}
