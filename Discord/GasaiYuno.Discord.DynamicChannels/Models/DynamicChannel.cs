using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.DynamicChannels.Models;

public class DynamicChannel : IEntity
{
    public ulong Server { get; init; }
    public string Name { get; init; }
    public AutomationType Type { get; init; }
    public string GenerationName { get; set; }
    public List<ulong> Channels { get; set; }
    public List<ulong> GeneratedChannels { get; set; }

    public DynamicChannel()
    {
        GenerationName = "-- [user] channel";
        Channels = new List<ulong>();
        GeneratedChannels = new List<ulong>();
    }
}