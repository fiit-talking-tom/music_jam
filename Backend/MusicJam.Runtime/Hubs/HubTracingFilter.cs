using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using MusicJam.Runtime.Telemetry;
using MusicJam.Studio.Extensions;

namespace MusicJam.Runtime.Hubs;

public sealed class HubTracingFilter : IHubFilter
{
    
 public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var hubName = invocationContext.Hub.GetType().Name;
        var spanName = $"{hubName}/{invocationContext.HubMethodName}";
        var previous = Activity.Current;
        Activity.Current = null;
        try
        {
            using var activity = RuntimeActivitySource.Source.StartActivity(spanName, ActivityKind.Server);

            activity?.SetTag("signalr.method", invocationContext.HubMethodName);
            activity?.SetTag("signalr.connection.id", invocationContext.Context.ConnectionId);
            SetUserTag(activity, invocationContext.Context);
            SetGameIdTag(activity, invocationContext);

            try
            {
                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }
        finally
        {
            Activity.Current = previous;
        }
    }

    public async Task OnDisconnectedAsync(
        HubLifetimeContext context,
        Exception? exception,
        Func<HubLifetimeContext, Exception?, Task> next)
    {
        var previous = Activity.Current;
        Activity.Current = null;
        try
        {
            using var activity = RuntimeActivitySource.Source.StartActivity(
                $"{context.Hub.GetType().Name}/OnDisconnected", ActivityKind.Server);

            activity?.SetTag("signalr.connection.id", context.Context.ConnectionId);
            SetUserTag(activity, context.Context);

            if (exception is not null)
            {
                activity?.SetTag("signalr.disconnect.error", exception.Message);
            }

            try
            {
                await next(context, exception);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }
        finally
        {
            Activity.Current = previous;
        }
    }

    private static void SetUserTag(Activity? activity, HubCallerContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            activity?.SetTag("user.id", context.User.GetUserId());
        }
    }
    
    private static void SetGameIdTag(Activity? activity, HubInvocationContext context)
    {
        if (activity is null)
        {
            return;
        }

        var parameters = context.HubMethod.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].Name != "gameId") continue;
            activity.SetTag("game.id", context.HubMethodArguments[i]);
            return;
        }
    }
}