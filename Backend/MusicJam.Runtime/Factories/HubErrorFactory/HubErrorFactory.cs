using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using MusicJam.Runtime.Responses;

namespace MusicJam.Runtime.Factories.HubErrorFactory;

public class HubErrorFactory<TError>(IHttpContextAccessor httpContextAccessor)
    : IHubErrorFactory<TError> where TError : Enum
{
    public HubException Create(TError message)
    {
        var payload = new HubErrorResponse
        {
            Message = message.ToString(),
            TraceId = GetTraceId()
        };

        return new HubException(JsonSerializer.Serialize(payload));
    }

    private string? GetTraceId()
    {
        return Activity.Current?.TraceId.ToString() ?? httpContextAccessor.HttpContext?.TraceIdentifier;
    }
}
