using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Runtime.Requests;
using MusicJam.Runtime.Responses.Games;
using MusicJam.Runtime.Services.GameService;
using MusicJam.Studio.Constants;
using Shared.Auth;

namespace MusicJam.Runtime.Controllers;

[ApiController]
[Route("/api/runtime/games")]
[Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
public class GamesController(IGameService gameService) : ControllerBase
{
    [HttpPost]
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
        var result = await gameService.Get(gameId);
        if (result.TryGetFault(out var error, out var game))
        {
            return ErrorResponse(error);
        }
        
        // не можем этот блок вытащить в сервис, потому что метод там используется еще и для подключения к игре
        var hostId = User.GetUserId();
        if (hostId != game.HostId)
        {
            return ErrorResponse(IGameService.GameError.GameNotFound);
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
            IGameService.GameError.GameNotFound => NotFound(),
            _ => throw new ArgumentOutOfRangeException(error.ToString()),
        };
    }
}
