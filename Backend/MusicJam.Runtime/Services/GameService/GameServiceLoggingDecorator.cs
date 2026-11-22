using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Runtime.Models;
using MusicJam.Runtime.Telemetry;
using Shared;

namespace MusicJam.Runtime.Services.GameService;

public class GameServiceLoggingDecorator(
    IGameService gameService,
    ILogger<GameServiceLoggingDecorator> logger) : IGameService
{
    public async Task<Result<Game, IGameService.GameError>> Create(Guid hostId, Guid packId)
    {
        using var activity = RuntimeActivitySource.Source.StartActivity("GameService.Create");
        activity?.SetTag("host.id", hostId);
        logger.LogInformation("Create game with packId = {packId} and host = {hostId}",
            packId,
            hostId);

        var gameResult = await gameService.Create(hostId, packId);
        if (gameResult.TryGetFault(out var error, out var game))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            logger.LogInformation("Create game with packId = {packId} and host = {hostId} failed with statusCode = {StatusCode}",
                packId,
                hostId,
                error);
            return error;
        }

        logger.LogInformation("Create game with packId = {packId} and host = {hostId} was successful",
            packId,
            hostId);
        return game;
    }

    public async Task<ActionResult<Game>> Get(Guid gameId)
    {
        using var activity = RuntimeActivitySource.Source.StartActivity("GameService.Get");
        activity?.SetTag("game.id", gameId);
        
        logger.LogInformation("Get game with gameId = {gameId}",
            gameId);
        
        var gameResult = await gameService.Get(gameId);
        if (gameResult.Value is null)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "GameNotFound");
            logger.LogInformation("Get game with gameId = {gameId} failed: game not found",
                gameId);
            return gameResult;
        }

        logger.LogInformation("Get game with gameId = {gameId} was successful",
            gameId);
        return gameResult;
    }
}