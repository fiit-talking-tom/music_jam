using System.Diagnostics;

namespace MusicJam.Studio.Telemetry;

public static class StudioActivitySource
{
    public const string Name = "MusicJam.Studio";
    public static readonly ActivitySource Source = new(Name);
}