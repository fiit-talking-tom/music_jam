using Microsoft.AspNetCore.SignalR;

namespace MusicJam.Runtime.Factories.HubErrorFactory;

public interface IHubErrorFactory<in TError> where TError : Enum
{
    HubException Create(TError message);
}
