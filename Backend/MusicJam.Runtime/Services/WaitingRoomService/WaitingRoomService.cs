using MusicJam.Runtime.Models;
using MusicJam.Runtime.Responses;
using MusicJam.Runtime.Storages.BindingsStorage;
using MusicJam.Runtime.Storages.GameStorage;
using Shared;
using WaitingRoomError = MusicJam.Runtime.Services.WaitingRoomService.IWaitingRoomService.WaitingRoomError;

namespace MusicJam.Runtime.Services.WaitingRoomService;

public sealed class WaitingRoomService(
    IGameStorage gameStorage,
    IBindingsStorage bindingsStorage,
    ILogger<WaitingRoomService> logger) : IWaitingRoomService
{
    public async Task<Result<JoinResponse, WaitingRoomError>> Join(Binding binding)
    {
        logger.LogInformation(
            "Start processing join game. SessionId: {SessionId}, GameId: {GameId}, Role: {Role}, ConnectionId: {ConnectionId}",
            binding.SessionId,
            binding.GameId,
            binding.Role,
            binding.Connections.First());

        var validateResult = ValidateBinding(binding);
        if (validateResult.TryGetFault(out var validateError))
        {
            return validateError;
        }

        var game = await gameStorage.Get(binding.GameId);
        if (game is null)
        {
            return WaitingRoomError.GameNotFound;
        }

        var existingBinding = await bindingsStorage.GetBinding(binding.SessionId);
        if (IsAlreadyInRoom(binding, existingBinding))
        {
            logger.LogWarning(
                "User already in room. SessionId: {SessionId}, GameId: {GameId}, Role: {Role}",
                binding.SessionId,
                binding.GameId,
                binding.Role);

            return WaitingRoomError.AlreadyInRoom;
        }

        binding.Connections =
        [
            .. binding.Connections,
            .. existingBinding?.Connections ?? []
        ];

        var result = binding.Role switch
        {
            Role.Host => JoinAsHost(game, binding),
            Role.Viewer => JoinAsViewer(game, binding),
            Role.Player => JoinAsPlayer(game, binding),
        };

        if (result.TryGetFault(out var joinError, out var joinResponse))
        {
            return joinError;
        }

        await bindingsStorage.AddOrUpdate(binding);

        lock (game.Lock)
        {
            joinResponse.Teams = game.Teams.Values.ToList();
            logger.LogInformation("Teams count = {Count}", joinResponse.Teams.Count);
        }

        logger.LogInformation(
            "Join finished successfully. SessionId: {SessionId}",
            binding.SessionId);

        return joinResponse;
    }

    private static Result<JoinResponse, WaitingRoomError> JoinAsHost(Game game, Binding binding)
    {
        if (binding.UserId != game.HostId)
        {
            return WaitingRoomError.NotHost;
        }

        lock (game.Lock)
        {
            game.HostSessionId = binding.SessionId;
        }

        return new JoinResponse
        {
            SessionId = binding.SessionId,
            Role = Role.Host,
        };
    }

    private static Result<JoinResponse, WaitingRoomError> JoinAsViewer(Game game, Binding binding)
    {
        lock (game.Lock)
        {
            game.Viewers.Add(binding.SessionId);
        }

        return new JoinResponse
        {
            SessionId = binding.SessionId,
            Role = Role.Viewer,
        };
    }

    private static Result<JoinResponse, WaitingRoomError> JoinAsPlayer(Game game, Binding binding)
    {
        lock (game.Lock)
        {
            var existing = game.Teams.Values.FirstOrDefault(t => t.SessionId == binding.SessionId);
            if (existing is not null)
            {
                existing.State = TeamState.Ready;

                return new JoinResponse
                {
                    SessionId = binding.SessionId,
                    Role = Role.Player,
                    Team = existing,
                    TeamName = existing.TeamName,
                    TeamId = existing.TeamId
                };
            }

            if (game.State != GameState.Pending)
            {
                return WaitingRoomError.GameAlreadyStarted;
            }

            var name = binding.TeamName?.Trim();

            var validateResult = ValidateTeamName(name, game);
            if (validateResult.TryGetFault(out var validateError))
            {
                return validateError;
            }

            var team = new Team
            {
                TeamId = Guid.NewGuid(),
                TeamName = name!,
                State = TeamState.Ready,
                SessionId = binding.SessionId
            };

            game.Teams.Add(team.TeamId, team);

            return new JoinResponse
            {
                SessionId = binding.SessionId,
                Role = Role.Player,
                Team = team,
                TeamName = team.TeamName,
                TeamId = team.TeamId
            };
        }
    }

    public async Task<Result<WaitingRoomError>> Leave(Guid sessionId, Guid gameId, Guid teamId)
    {
        var binding = await bindingsStorage.GetBinding(sessionId);
        if (binding is null || binding.GameId != gameId || binding.Role != Role.Player)
        {
            return WaitingRoomError.NotYourTeam;
        }

        var game = await gameStorage.Get(gameId);
        if (game is null)
        {
            return WaitingRoomError.GameNotFound;
        }

        lock (game.Lock)
        {
            if (!game.Teams.TryGetValue(teamId, out var team) || team.SessionId != sessionId)
            {
                return WaitingRoomError.NotYourTeam;
            }

            game.Teams.Remove(teamId);
            logger.LogInformation("Teams count = {Count}", game.Teams.Count);
        }

        await bindingsStorage.DeleteBinding(sessionId);

        return Result<WaitingRoomError>.Success();
    }

    public async Task<DisconnectedTeam?> Disconnect(string connectionId)
    {
        logger.LogInformation("Start disconnect. ConnectionId: {ConnectionId}", connectionId);

        var binding = await bindingsStorage.RemoveConnection(connectionId);
        if (binding is null)
        {
            logger.LogInformation("Binding is null");
            return null;
        }

        if (binding.Connections.Count != 0)
        {
            logger.LogInformation("Connections count != 0. Count: {Count}", binding.Connections.Count);
            return null;
        }

        await bindingsStorage.DeleteBinding(binding.SessionId);

        logger.LogInformation(
            "Delete binding. ConnectionId: {ConnectionId}, SessionId: {SessionId}",
            connectionId,
            binding.SessionId);

        var game = await gameStorage.Get(binding.GameId);
        if (game is null)
        {
            return null;
        }

        lock (game.Lock)
        {
            var team = game.Teams.Values.FirstOrDefault(t => t.SessionId == binding.SessionId);
            if (team is null)
            {
                return null;
            }

            team.State = TeamState.Disconnected;

            logger.LogInformation("Team disconnected. TeamId: {TeamId}", team.TeamId);

            return new DisconnectedTeam(game.GameId, team);
        }
    }

    private static Result<WaitingRoomError> ValidateTeamName(string? name, Game game)
    {
        if (string.IsNullOrEmpty(name))
        {
            return WaitingRoomError.EmptyName;
        }

        if (name.Length > IWaitingRoomService.MaxTeamNameLength)
        {
            return WaitingRoomError.LongName;
        }

        if (game.Teams.Values.Any(t => string.Equals(t.TeamName, name, StringComparison.OrdinalIgnoreCase)))
        {
            return WaitingRoomError.NameTaken;
        }

        return Result<WaitingRoomError>.Success();
    }

    // todo дописать прочие проверки на корректность биндинга
    private static Result<WaitingRoomError> ValidateBinding(Binding binding)
    {
        if (!Enum.IsDefined(binding.Role))
        {
            return WaitingRoomError.InvalidRole;
        }

        return Result<WaitingRoomError>.Success();
    }

    private static bool IsAlreadyInRoom(Binding binding, Binding? existingBinding)
    {
        if (existingBinding is null)
        {
            return false;
        }

        // если уже в другой игре или уже с другой ролью - надо лучше обдумать эти сценарии, пока так
        if (binding.GameId != existingBinding.GameId || binding.Role != existingBinding.Role)
        {
            return true;
        }

        // если почему-то совпали id соединений (хотя, по идее, не должны)
        return existingBinding.Connections.Contains(binding.Connections.First());
    }
}
