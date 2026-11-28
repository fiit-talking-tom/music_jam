using Microsoft.AspNetCore.SignalR;
using MusicJam.Runtime.Extensions;
using MusicJam.Runtime.Models;
using MusicJam.Runtime.Responses;
using Shared.Auth;

namespace MusicJam.Runtime.Hubs;

public sealed partial class GameHub
{
    public async Task<JoinResponse> JoinGame(Guid gameId, Role role, string? teamName)
    {
        var sessionId = GetSessionId();
        Guid? userId = Context.User?.Identity?.IsAuthenticated == true ? Context.User.GetUserId() : null;

        var binding = new Binding
        {
            SessionId = sessionId,
            GameId = gameId,
            Role = role,
            UserId = userId,
            TeamName = teamName,
            Connections = [Context.ConnectionId]
        };

        var result = await waitingRoomService.Join(binding);
        if (result.TryGetFault(out var error, out var joinResponse))
        {
            throw hubErrorFactory.Create(error);
        }

        var group = GroupName(gameId);

        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        await Clients.Caller.RoomState(joinResponse.Teams);

        if (joinResponse.Team is not null)
        {
            await Clients.OthersInGroup(group).TeamJoined(joinResponse.Team);
        }

        return joinResponse;
    }

    public async Task LeaveGame(Guid gameId, Guid teamId)
    {
        var sessionId = GetSessionId();
        var result = await waitingRoomService.Leave(sessionId, gameId, teamId);
        if (result.TryGetFault(out var error))
        {
            throw hubErrorFactory.Create(error);
        }

        var group = GroupName(gameId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        await Clients.Group(group).TeamLeft(teamId);
    }

    private Guid GetSessionId() =>
        Context.GetHttpContext()?.GetSessionId()
        ?? throw new HubException("Нет сессии, обновите страницу");
}
