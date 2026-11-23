using MusicJam.Runtime.Responses;
using Shared;

namespace MusicJam.Runtime.Services.UsersService;

public interface IUsersService
{
    public enum UserError
    {
        GameNotFound,
    }

    Task<Result<CheckRoleResponse?, UserError>> GetRole(Guid gameId, Guid sessionId);

    Task<Result<bool, UserError>> IsHost(Guid gameId, Guid? userId);
}
