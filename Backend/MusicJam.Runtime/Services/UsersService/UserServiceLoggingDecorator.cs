using System.Diagnostics;
using MusicJam.Runtime.Responses;
using MusicJam.Runtime.Telemetry;
using Shared;

namespace MusicJam.Runtime.Services.UsersService;

public class UserServiceLoggingDecorator(
    IUsersService userService,
    ILogger<UserServiceLoggingDecorator> logger) : IUsersService
{
    public async Task<Result<CheckRoleResponse?, IUsersService.UserError>> GetRole(Guid gameId, Guid sessionId)
    {
        using var activity = RuntimeActivitySource.Source.StartActivity("UserService.GetRole");
        activity?.SetTag("game.id", gameId);
        
        logger.LogInformation("GetRole game with gameId = {gameId} and person = {sessionId}",
            gameId,
            sessionId);

        var roleRsult = await userService.GetRole(gameId, sessionId);
        if (roleRsult.TryGetFault(out var error, out var game))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            logger.LogInformation("GetRole game with gameId = {gameId} and person = {sessionId} failed with statusCode = {StatusCode}",
                gameId,
                sessionId,
                error);
            return error;
        }

        logger.LogInformation("GetRole game with gameId = {gameId} and person = {sessionId} was successful",
            gameId,
            sessionId);
        return game;
    }

    public async Task<Result<bool, IUsersService.UserError>> IsHost(Guid gameId, Guid? userId)
    {
        using var activity = RuntimeActivitySource.Source.StartActivity("UserService.GetRole");
        activity?.SetTag("game.id", gameId);
        
        logger.LogInformation("IsHost game with gameId = {gameId} and person = {userId}",
            gameId,
            userId);

        var roleRsult = await userService.IsHost(gameId, userId);
        if (roleRsult.TryGetFault(out var error, out var game))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            logger.LogInformation("IsHost game with gameId = {gameId} and person = {userId} failed with statusCode = {StatusCode}",
                gameId,
                userId,
                error);
            return error;
        }

        logger.LogInformation("IsHost game with gameId = {gameId} and person = {userId} was successful",
            gameId,
            userId);
        return game;
    }
}