using System.Diagnostics;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests.Packs;
using MusicJam.Studio.Responses.Packs;
using MusicJam.Studio.Telemetry;
using Shared;

namespace MusicJam.Studio.Services.PacksService;

internal class PacksServiceLoggingDecorator(
    IPacksService packsService,
    ILogger<PacksServiceLoggingDecorator> logger) : IPacksService
{
    public async Task<Result<ShortPackResponse[], IPacksService.PackError>> GetShortPacks(Guid userId, GetPackRequest query)
    {
        using var activity = StudioActivitySource.Source.StartActivity("PacksService.GetShortPacks");
        activity?.SetTag("user.id", userId);
        
        logger.LogInformation(
            "Get short packs with userId = {userId}",
            userId);
        var packResult = await packsService.GetShortPacks(userId, query);
        if (packResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            logger.LogInformation(
                "GetShortPacks with user = {userId} finished with fault = {StatusCode}",
                userId,
                error);

            return error;
        }
        
        logger.LogInformation(
            "GetShortPacks with user = {userId} finished successfully. AllShortPacks length = {Length}",
            userId,
            token.Length);
        
        return packResult;
    }

    public async Task<Result<PackCardResponse[], IPacksService.PackError>> GetPackCards(Guid userId, GetPackRequest request)
    {
        using var activity = StudioActivitySource.Source.StartActivity("PacksService.GetPackCards");
        activity?.SetTag("user.id", userId);
        
        logger.LogInformation(
            "Get pack cards with userId = {userId}",
            userId);
        
        var packResult = await packsService.GetPackCards(userId, request);
        if (packResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            logger.LogInformation(
                "GetPackCards with user = {userId} finished with fault = {StatusCode}",
                userId,
                error);

            return error;
        }
        
        logger.LogInformation(
            "GetPackCards with user = {userId} finished successfully. ALLGetPackCards length = {Length}",
            userId,
            token.Length);
        
        return packResult;
    }

    public async Task<Result<EditPackResponse, IPacksService.PackError>> GetEditPack(Guid userId, Guid packId)
    {
        using var activity = StudioActivitySource.Source.StartActivity("PacksService.GetEditPack");
        activity?.SetTag("user.id", userId);
        activity?.SetTag("pack.id", packId);

        logger.LogInformation(
            "GetEditPack with userId = {userId} and packId = {packId}",
            userId,
            packId);
        
        var packResult = await packsService.GetEditPack(userId, packId);
        if (packResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());

            logger.LogInformation(
                "GetEditPack with user = {userId} and packId = {packId} finished with fault = {StatusCode}",
                userId,
                packId,
                error);

            return error;
        }

        logger.LogInformation(
            "GetEditPack with user = {userId} and packId = {packId} finished successfully",
            userId,
            packId);
        
        return packResult;
    }

    public async Task<Result<Pack, IPacksService.PackError>> GetPack(Guid userId, Guid packId)
    {
        using var activity = StudioActivitySource.Source.StartActivity("PacksService.GetPack");
        activity?.SetTag("user.id", userId);
        activity?.SetTag("pack.id", packId);
        
        logger.LogInformation(
            "GetPack with userId = {userId} and packId = {packId}",
            userId,
            packId);
        
        var packResult = await packsService.GetPack(userId, packId);
        if (packResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "GetPack with user = {userId} and packId = {packId} finished with fault = {StatusCode}",
                userId,
                packId,
                error);

            return error;
        }

        logger.LogInformation(
            "GetPack with user = {userId} and packId = {packId} finished successfully",
            userId,
            packId);
        
        return packResult;
    }

    public async Task<Result<Pack, IPacksService.PackError>> Create(Guid userId, CreatePackRequest request)
    {
        using var activity = StudioActivitySource.Source.StartActivity("PacksService.Create");
        activity?.SetTag("user.id", userId);
        activity?.SetTag("pack.id", request.PackId);
        
        logger.LogInformation(
            "Create with userId = {userId} and packId = {packId}",
            userId,
            request.PackId);
        
        var packResult = await packsService.Create(userId, request);
        if (packResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "Create with user = {userId} and packId = {packId} finished with fault = {StatusCode}",
                userId,
                request.PackId,
                error);

            return error;
        }

        logger.LogInformation(
            "Create with user = {userId} and packId = {packId} finished successfully",
            userId,
            request.PackId);
        
        return packResult;
    }

    public async Task<Result<Pack, IPacksService.PackError>> Update(Guid userId, Guid packId, UpdatePackRequest request)
    {
        using var activity = StudioActivitySource.Source.StartActivity("PacksService.Update");
        activity?.SetTag("user.id", userId);
        activity?.SetTag("pack.id", packId);

        logger.LogInformation(
            "Update with userId = {userId} and packId = {packId}",
            userId,
            packId);
        
        var packResult = await packsService.Update(userId, packId, request);
        if (packResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "Update with user = {userId} and packId = {packId} finished with fault = {StatusCode}",
                userId,
                packId,
                error);

            return error;
        }

        logger.LogInformation(
            "Update with user = {userId} and packId = {packId} finished successfully",
            userId,
            packId);
        
        return packResult;
    }

    public async Task<Result<bool, IPacksService.PackError>> Delete(Guid userId, Guid packId)
    {
        using var activity = StudioActivitySource.Source.StartActivity("PacksService.Delete");
        activity?.SetTag("user.id", userId);
        activity?.SetTag("pack.id", packId);
        
        logger.LogInformation(
            "Delete with userId = {userId} and packId = {packId}",
            userId,
            packId);
        
        var packResult = await packsService.Delete(userId, packId);
        if (packResult.TryGetFault(out var error, out var _))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "Delete with user = {userId} and packId = {packId} finished with fault = {StatusCode}",
                userId,
                packId,
                error);

            return error;
        }

        logger.LogInformation(
            "Delete with user = {userId} and packId = {packId} finished successfully",
            userId,
            packId);
        
        return packResult;
    }
}