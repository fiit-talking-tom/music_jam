using Microsoft.AspNetCore.Mvc;

namespace Shared.Errors;

public interface IApiErrorFactory
{
    ApiErrorResponse Create(int statusCode, string message);
    ActionResult Result(int statusCode, string message);

    ActionResult BadRequest(string message);
    ActionResult Unauthorized(string message);
    ActionResult Forbidden(string message);
    ActionResult NotFound(string message);
    ActionResult Conflict(string message);
    ActionResult ServerError(string message);
}
