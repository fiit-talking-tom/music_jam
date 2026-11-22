using System.Diagnostics;

namespace MusicJam.Runtime.Telemetry;

public class RuntimeActivitySource
{
    public const string Name = "MusicJam.Runtime";
    public static readonly ActivitySource Source = new(Name);
}