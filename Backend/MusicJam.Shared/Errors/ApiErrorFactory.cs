using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Shared.Errors;

public class ApiErrorFactory(IHttpContextAccessor httpContextAccessor) : IApiErrorFactory
{
    public ApiErrorResponse Create(int statusCode, string message)
    {
        return new ApiErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            TraceId = GetTraceId()
        };
    }

    public ApiErrorResponse BadRequest(string message) => Create(StatusCodes.Status400BadRequest, message);

    public ApiErrorResponse Unauthorized(string message) => Create(StatusCodes.Status401Unauthorized, message);

    public ApiErrorResponse Forbidden(string message) => Create(StatusCodes.Status403Forbidden, message);

    public ApiErrorResponse NotFound(string message) => Create(StatusCodes.Status404NotFound, message);

    public ApiErrorResponse Conflict(string message) => Create(StatusCodes.Status409Conflict, message);

    public ApiErrorResponse ServerError(string message) => Create(StatusCodes.Status500InternalServerError, message);

    private string? GetTraceId()
    {
        return Activity.Current?.TraceId.ToString() ?? httpContextAccessor.HttpContext?.TraceIdentifier;
    }
}
