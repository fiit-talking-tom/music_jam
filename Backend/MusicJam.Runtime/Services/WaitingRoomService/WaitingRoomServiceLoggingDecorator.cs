using MusicJam.Runtime.Models;
using MusicJam.Runtime.Responses;
using Shared;

namespace MusicJam.Runtime.Services.WaitingRoomService;

public class WaitingRoomServiceLoggingDecorator(
    IWaitingRoomService waitingRoomService,
    ILogger<WaitingRoomServiceLoggingDecorator> logger) : IWaitingRoomService
{
    public async Task<Result<JoinResponse, IWaitingRoomService.WaitingRoomError>> Join(Binding binding)
    {
        logger.LogInformation("Join game with gameId = {connectionId} and role = {role}",
            binding.GameId,
            binding.Role);

        var roleRsult = await waitingRoomService.Join(binding);
        if (roleRsult.TryGetFault(out var error, out var game))
        {
            logger.LogInformation("Join game with gameId = {connectionId} and role = {role} failed with statusCode = {StatusCode}",
                binding.GameId,
                binding.Role,
                error);
            return error;
        }

        logger.LogInformation("Join game with gameId = {connectionId} and role = {role} was successful",
            binding.GameId,
            binding.Role);
        return game;
    }

    public async Task<Result<IWaitingRoomService.WaitingRoomError>> Leave(Guid sessionId, Guid gameId, Guid teamId)
    {
        logger.LogInformation("Leave game with game = {gameId} and person = {teamId}",
            gameId,
            teamId);

        var leaveResult = await waitingRoomService.Leave(sessionId, gameId, teamId);
        if (leaveResult.TryGetFault(out var error))
        {
            logger.LogInformation("Leave game with game = {gameId} and person = {teamId} failed with statusCode = {StatusCode}",
                gameId,
                teamId,
                error);
            return error;
        }

        logger.LogInformation(
            "Leave game with game = {gameId} and person = {teamId} was successful",
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