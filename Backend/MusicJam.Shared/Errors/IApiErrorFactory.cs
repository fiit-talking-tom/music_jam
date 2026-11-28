namespace Shared.Errors;

public interface IApiErrorFactory
{
    ApiErrorResponse Create(int statusCode, string message);

    ApiErrorResponse BadRequest(string message);
    ApiErrorResponse Unauthorized(string message);
    ApiErrorResponse Forbidden(string message);
    ApiErrorResponse NotFound(string message);
    ApiErrorResponse Conflict(string message);
    ApiErrorResponse ServerError(string message);
}
