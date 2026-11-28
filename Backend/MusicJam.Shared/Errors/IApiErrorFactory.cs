using Microsoft.AspNetCore.Mvc;

namespace Shared.Errors;

public interface IApiErrorFactory<in TError> where TError : Enum
{
    ApiErrorResponse Create(int statusCode, TError message);
    ActionResult Result(int statusCode, TError message);

    ActionResult BadRequest(TError message);
    ActionResult Unauthorized(TError message);
    ActionResult Forbidden(TError message);
    ActionResult NotFound(TError message);
    ActionResult Conflict(TError message);
    ActionResult ServerError(TError message);
}
