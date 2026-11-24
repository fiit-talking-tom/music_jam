using System.Diagnostics;
using Amazon.S3.Model;
using MusicJam.Studio.Models.Tracks;
using MusicJam.Studio.Telemetry;
using Shared;

namespace MusicJam.Studio.Services.TracksService;

public class TracksServiceLoggingDecorator(
    ITracksService tracksService,
    ILogger<TracksServiceLoggingDecorator> logger) : ITracksService
{
    public async Task<Result<Track, ITracksService.TrackError>> UploadTrack(IFormFile file, Guid? trackId = null, CancellationToken cancellationToken = default)
    {
        using var activity = StudioActivitySource.Source.StartActivity("TracksService.UploadTrack");
        activity?.SetTag("track.id", trackId);
        
        logger.LogInformation(
            "Uploading track trackId = {trackId}",
            trackId);

        var trackResult = await tracksService.UploadTrack(file, trackId, cancellationToken);
        if (trackResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "Uploading track trackId = {trackId} finished with fault = {StatusCode}",
                trackId,
                error);
            return error;
        }

        logger.LogInformation(
            "Uploading track trackId = {trackId} finished successfully. Access token = {Token}",
            trackId,
            token.Id);

        return trackResult;
    }

    public async Task<Result<ITracksService.TrackError>> DeleteTrack(Guid trackId, CancellationToken cancellationToken = default)
    {
        using var activity = StudioActivitySource.Source.StartActivity("TracksService.DeleteTrack");
        activity?.SetTag("track.id", trackId);
        
        logger.LogInformation(
            "Delete track trackId = {trackId}",
            trackId);

        var trackResult = await tracksService.DeleteTrack(trackId, cancellationToken);
        if (trackResult.TryGetFault(out var error))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());

            logger.LogInformation(
                "Delete track trackId = {trackId} finished with fault = {StatusCode}",
                trackId,
                error);
            return error;
        }

        logger.LogInformation(
            "Delete track trackId = {trackId} finished successfully",
            trackId);

        return trackResult;
    }

    public async Task<Result<long, ITracksService.TrackError>> GetTrackSize(Guid trackId, CancellationToken cancellationToken = default)
    {
        using var activity = StudioActivitySource.Source.StartActivity("TracksService.GetTrackSize");
        activity?.SetTag("track.id", trackId);
        
        logger.LogInformation(
            "GetTrackSize track trackId = {trackId}",
            trackId);

        var trackResult = await tracksService.GetTrackSize(trackId, cancellationToken);
        if (trackResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "GetTrackSize track trackId = {trackId} finished with fault = {StatusCode}",
                trackId,
                error);
            return error;
        }

        logger.LogInformation(
            "GetTrackSize track trackId = {trackId} finished successfully. Size = {Size}",
            trackId,
            token);

        return trackResult;
    }

    public async Task<Result<GetObjectResponse, ITracksService.TrackError>> GetTrack(Guid trackId, CancellationToken cancellationToken = default)
    {
        using var activity = StudioActivitySource.Source.StartActivity("TracksService.GetTrack");
        activity?.SetTag("track.id", trackId);
        
        logger.LogInformation(
            "GetTrack track trackId = {trackId}",
            trackId);

        var trackResult = await tracksService.GetTrack(trackId, cancellationToken);
        if (trackResult.TryGetFault(out var error, out var _))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "GetTrack track trackId = {trackId} finished with fault = {StatusCode}",
                trackId,
                error);
            return error;
        }

        logger.LogInformation(
            "GetTrack track trackId = {trackId} finished successfully",
            trackId);

        return trackResult;
    }

    public async Task<Result<GetObjectResponse, ITracksService.TrackError>> GetTrackRange(Guid trackId, long start, long end, CancellationToken cancellationToken = default)
    {
        using var activity = StudioActivitySource.Source.StartActivity("TracksService.GetTrackRange");
        activity?.SetTag("track.id", trackId);
        
        logger.LogInformation(
            "GetTrackRange track trackId = {trackId}",
            trackId);

        var trackResult = await tracksService.GetTrackRange(trackId, start, end, cancellationToken);
        if (trackResult.TryGetFault(out var error, out var _))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());

            logger.LogInformation(
                "GetTrackRange track trackId = {trackId} finished with fault = {StatusCode}",
                trackId,
                error);
            return error;
        }

        logger.LogInformation(
            "GetTrackRange track trackId = {trackId} finished successfully",
            trackId);

        return trackResult;
    }
}