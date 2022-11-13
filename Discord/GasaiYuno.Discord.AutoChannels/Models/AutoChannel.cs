using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.AutoChannels.Models;

public class AutoChannel : IEntity
{
    public ulong Server { get; init; }
    public ulong Channel { get; init; }
    public AutomationType Type { get; init; }
    public List<ulong> RelatedChannels { get; set; }
    public string GenerationName { get; set; }
    public List<ulong> GeneratedChannels { get; set; }

    public AutoChannel()
    {
        GenerationName = "-- [user] channel";
        RelatedChannels = new List<ulong>();
        GeneratedChannels = new List<ulong>();
    }
}