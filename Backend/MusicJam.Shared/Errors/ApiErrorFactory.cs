using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Errors;

public class ApiErrorFactory<TError>(IHttpContextAccessor httpContextAccessor)
    : IApiErrorFactory<TError> where TError : Enum
{
    public ApiErrorResponse Create(int statusCode, TError message)
    {
        return new ApiErrorResponse
        {
            StatusCode = statusCode,
            Message = message.ToString(),
            TraceId = GetTraceId()
        };
    }

    public ActionResult Result(int statusCode, TError message)
    {
        var error = Create(statusCode, message);
        return new ObjectResult(error)
        {
            StatusCode = statusCode
        };
    }

    public ActionResult BadRequest(TError message) => Result(StatusCodes.Status400BadRequest, message);

    public ActionResult Unauthorized(TError message) => Result(StatusCodes.Status401Unauthorized, message);

    public ActionResult Forbidden(TError message) => Result(StatusCodes.Status403Forbidden, message);

    public ActionResult NotFound(TError message) => Result(StatusCodes.Status404NotFound, message);

    public ActionResult Conflict(TError message) => Result(StatusCodes.Status409Conflict, message);

    public ActionResult ServerError(TError message) => Result(StatusCodes.Status500InternalServerError, message);

    private string? GetTraceId()
    {
        return Activity.Current?.TraceId.ToString() ?? httpContextAccessor.HttpContext?.TraceIdentifier;
    }
}
