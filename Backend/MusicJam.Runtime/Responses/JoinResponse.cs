using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Responses;

public sealed class JoinResponse
{
    public Guid SessionId { get; set; }
    public Role Role { get; set; }
    public List<Team> Teams { get; set; }
    public Team? Team { get; set; }
    public Guid? TeamId { get; set; }
    public string? TeamName { get; set; }
}
