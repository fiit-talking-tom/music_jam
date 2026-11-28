using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Runtime.Extensions;
using MusicJam.Runtime.Requests;
using MusicJam.Runtime.Responses.Games;
using MusicJam.Runtime.Services.GameService;
using MusicJam.Studio.Constants;
using Shared.Auth;
using Shared.Errors;

namespace MusicJam.Runtime.Controllers;

[ApiController]
[Route("/api/runtime/games")]
public class GamesController(
    IGameService gameService,
    IApiErrorFactory<IGameService.GameError> apiErrorFactory)
    : ControllerBase
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateGameResponse>> CreateGame([FromBody] CreateGameRequest request)
    {
        var hostId = User.GetUserId();
        var result = await gameService.Create(hostId, request.PackId);
        if (result.TryGetFault(out var error, out var game))
        {
            return ErrorResponse(error);
        }

        return new CreateGameResponse { GameId = game.GameId };
    }

    [HttpGet("{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetGameResponse>> GetGame([FromRoute] Guid gameId)
    {
        var binding = HttpContext.GetBinding();
        if (binding is null || binding.GameId != gameId)
        {
            return ErrorResponse(IGameService.GameError.GameNotFound);
        }

        var result = await gameService.Get(gameId);
        if (result.TryGetFault(out var error, out var game))
        {
            return ErrorResponse(error);
        }

        var response = new GetGameResponse
        {
            GameId = game.GameId,
            InviteCode = game.Invite.Code,
            InviteLink = game.Invite.Link
        };

        return response;
    }

    private ActionResult ErrorResponse(IGameService.GameError error)
    {
        return error switch
        {
            IGameService.GameError.GameNotFound => apiErrorFactory.NotFound(error),
            _ => apiErrorFactory.ServerError(error),
        };
    }
}
