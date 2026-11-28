using Microsoft.AspNetCore.Mvc;
using MusicJam.Runtime.Extensions;
using MusicJam.Runtime.Services.UsersService;
using Shared.Auth;
using Shared.Errors;

namespace MusicJam.Runtime.Controllers;

[ApiController]
[Route("/api/runtime/games/{gameId:guid}/users")]
public class UsersController(
    IUsersService usersService,
    IApiErrorFactory<IUsersService.UserError> apiErrorFactory)
    : ControllerBase
{
    [HttpGet("check-role")]
    public async Task<IActionResult> CheckRole(Guid gameId)
    {
        var session = HttpContext.GetSessionId();
        var result = await usersService.GetRole(gameId, session);
        if (result.TryGetFault(out var error, out var role))
            return ErrorResponse(error);
        return Ok(role);
    }

    [HttpGet("check-host")]
    public async Task<IActionResult> IsHost(Guid gameId)
    {
        Guid? userId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        var result = await usersService.IsHost(gameId, userId);
        if (result.TryGetFault(out var error, out var isHost))
            return ErrorResponse(error);
        return Ok(new { isHost });
    }

    private ActionResult ErrorResponse(IUsersService.UserError error)
    {
        return error switch
        {
            IUsersService.UserError.GameNotFound => apiErrorFactory.NotFound(error),
            _ => apiErrorFactory.ServerError(error),
        };
    }
}
