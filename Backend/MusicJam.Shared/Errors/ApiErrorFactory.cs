using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

    public ActionResult Result(int statusCode, string message)
    {
        var error = Create(statusCode, message);
        return new ObjectResult(error)
        {
            StatusCode = statusCode
        };
    }

    public ActionResult BadRequest(string message) => Result(StatusCodes.Status400BadRequest, message);

    public ActionResult Unauthorized(string message) => Result(StatusCodes.Status401Unauthorized, message);

    public ActionResult Forbidden(string message) => Result(StatusCodes.Status403Forbidden, message);

    public ActionResult NotFound(string message) => Result(StatusCodes.Status404NotFound, message);

    public ActionResult Conflict(string message) => Result(StatusCodes.Status409Conflict, message);

    public ActionResult ServerError(string message) => Result(StatusCodes.Status500InternalServerError, message);

    private string? GetTraceId()
    {
        return Activity.Current?.TraceId.ToString() ?? httpContextAccessor.HttpContext?.TraceIdentifier;
    }
}
