namespace MusicJam.Runtime.Models;

public class Binding
{
    public Guid SessionId { get; set; }

    public Guid GameId { get; set; }

    public Role Role { get; set; }

    // не null - когда Player
    public string? TeamName { get; set; }

    public HashSet<string> Connections { get; set; } = [];

    // не null - когда Host
    public Guid? UserId { get; set; }
}
