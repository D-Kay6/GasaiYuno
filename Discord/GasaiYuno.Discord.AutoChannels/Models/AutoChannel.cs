using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.AutoChannels.Models;

public class AutoChannel : IEntity
{
    public ulong Server { get; init; }
    public ulong Channel { get; init; }
    public AutomationType Type { get; init; }
    public string GenerationName { get; set; }
    public List<ulong> GeneratedChannels { get; set; }

    public AutoChannel()
    {
        GenerationName = "-- [user] channel";
        GeneratedChannels = new List<ulong>();
    }
}