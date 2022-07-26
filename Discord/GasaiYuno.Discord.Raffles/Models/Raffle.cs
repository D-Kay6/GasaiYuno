using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.Raffles.Models;

public class Raffle : IEntity
{
    public Guid Identity { get; init; }
    public ulong Server { get; init; }
    public ulong Channel { get; init; }
    public ulong Message { get; init; }
    public string Title { get; init; }
    public DateTime EndDate { get; init; }
    public List<ulong> Entries { get; init; }

    public Raffle()
    {
        Entries = new List<ulong>();
    }
}