using System.Diagnostics;
using MusicJam.Runtime.Models;
using MusicJam.Runtime.Telemetry;
using Shared;

namespace MusicJam.Runtime.Services.InvitesService;

internal class InvitesServiceLoggerDecorator(
    ILogger<InvitesService> logger,
    InvitesService invitesService)
    : IInvitesService
{
    public async Task<GameInvite> CreateInvite(Guid gameId)
    {
        using var activity = RuntimeActivitySource.Source.StartActivity("InviteService.CreateInvite");
        activity?.SetTag("game.id", gameId);
        logger.LogInformation(
            "Starting new invite creation. GameId: {GameId}",
            gameId);

        var invite = await invitesService.CreateInvite(gameId);

        logger.LogInformation(
            "Invite created successfully. GameId: {GameId}, InviteId: {InviteId}, Code: {Code}",
            gameId,
            invite.InviteId,
            invite.Code);

        return invite;
    }

    public async Task<Result<string, IInvitesService.InviteError>> JoinGameByCode(string code)
    {
        using var activity = RuntimeActivitySource.Source.StartActivity("InviteService.JoinGameByCode");
        activity?.SetTag("code", code);
        
        logger.LogInformation(
            "Attempting to join game with code. Code: {Code}",
            code);

        var result = await invitesService.JoinGameByCode(code);
        if (result.TryGetFault(out var error, out var backUrl))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            logger.LogWarning(
                "Failed to join game with code. Code: {Code}, Error: {Error}",
                code,
                error);

            return error;
        }

        logger.LogInformation(
            "Successfully joined game with code. Code: {Code}, BackUrl: {BackUrl}",
            code,
            backUrl);

        return backUrl;
    }

    public async Task<Result<string, IInvitesService.InviteError>> JoinGameByLink(Guid inviteId)
    {
        using var activity = RuntimeActivitySource.Source.StartActivity("InviteService.JoinGameByLink");
        activity?.SetTag("invite.id", inviteId);
        
        logger.LogInformation(
            "Attempting to join game with link. InviteId: {InviteId}",
            inviteId);

        var result = await invitesService.JoinGameByLink(inviteId);
        if (result.TryGetFault(out var error, out var backUrl))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            logger.LogWarning(
                "Failed to join game with link. InviteId: {InviteId}, Error: {Error}",
                inviteId,
                error);

            return error;
        }

        logger.LogInformation(
            "Successfully joined game with link. InviteId: {InviteId}, BackUrl: {BackUrl}",
            inviteId,
            backUrl);

        return backUrl;
    }
}
