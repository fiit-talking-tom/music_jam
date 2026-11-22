using MusicJam.Runtime.Models;
using MusicJam.Runtime.Responses;
using Shared;

namespace MusicJam.Runtime.Services.WaitingRoomService;

public class WaitingRoomServiceLoggingDecorator(
    IWaitingRoomService waitingRoomService,
    ILogger<WaitingRoomServiceLoggingDecorator> logger) : IWaitingRoomService
{
    public async Task<Result<JoinResponse, IWaitingRoomService.WaitingRoomError>> Join(string connectionId, Guid sessionId, Guid? userId, Guid gameId, Role role, string? teamName)
    {
        logger.LogInformation("Join game with connection = {connectionId} and person = {sessionId}",
            gameId,
            sessionId);

        var roleRsult = await waitingRoomService.Join(connectionId, sessionId, userId, gameId, role, teamName);
        if (roleRsult.TryGetFault(out var error, out var game))
        {
            logger.LogInformation("Join game with connection = {connectionId} and person = {sessionId} failed with statusCode = {StatusCode}",
                gameId,
                sessionId,
                error);
            return error;
        }

        logger.LogInformation("Join game with connection = {connectionId} and person = {sessionId} was successful",
            gameId,
            sessionId);
        return game;
    }

    public async Task<Result<IWaitingRoomService.WaitingRoomError>> Leave(string connectionId, Guid gameId, Guid teamId)
    {
        logger.LogInformation("Leave game with connection = {connectionId} and game = {gameId} and person = {teamId}",
            connectionId,
            gameId,
            teamId);

        var leaveResult = await waitingRoomService.Leave(connectionId, gameId, teamId);
        if (leaveResult.TryGetFault(out var error))
        {
            logger.LogInformation("Leave game with connection = {connectionId} and game = {gameId} and person = {teamId} failed with statusCode = {StatusCode}",
                connectionId,
                gameId,
                teamId,
                error);
            return error;
        }

        logger.LogInformation(
            "Leave game with connection = {connectionId} and game = {gameId} and person = {teamId} was successful",
            connectionId,
            gameId,
            teamId);
        return leaveResult;
    }

    public async Task<DisconnectedTeam?> Disconnect(string connectionId)
    {
        logger.LogInformation("Disconnect game with connection = {connectionId}",
            connectionId);
        
        var disconnected = await waitingRoomService.Disconnect(connectionId);
        if (disconnected is null)
        {
            logger.LogInformation("Disconnect game with connection = {connectionId}: connection was not in a waiting room",
                connectionId);
            return null;
        }

        logger.LogInformation("Disconnect game with connection = {connectionId} was successful, game = {gameId}",
            connectionId,
            disconnected.GameId);
        return disconnected;
    }
}