using MusicJam.Runtime.Models;
using MusicJam.Runtime.Responses;
using Shared;

namespace MusicJam.Runtime.Services.WaitingRoomService;

public interface IWaitingRoomService
{
    public const int MaxTeamNameLength = 50;

    enum WaitingRoomError
    {
        EmptyName,
        LongName,
        AlreadyInRoom,
        NameTaken,
        NotYourTeam,
        GameNotFound,
        GameAlreadyStarted,
        InvalidRole,
        NotHost
    }

    Task<Result<JoinResponse, WaitingRoomError>> Join(Binding binding);

    Task<Result<WaitingRoomError>> Leave(Guid sessionId, Guid gameId, Guid teamId);

    Task<DisconnectedTeam?> Disconnect(string connectionId);
}
