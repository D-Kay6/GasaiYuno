using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.Bans.Models;

public class Ban : IEntity
{
    public ulong Server { get; init; }
    public ulong User { get; init; }
    public DateTime EndDate { get; init; }
    public string Reason { get; init; }
}