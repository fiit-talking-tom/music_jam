namespace MusicJam.Runtime.Responses.Games;

public class GetGameResponse
{
    public Guid GameId { get; set; }

    public string InviteCode { get; set; }

    public string InviteLink { get; set; }
}
