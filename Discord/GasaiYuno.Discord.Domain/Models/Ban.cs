using System;

namespace GasaiYuno.Discord.Domain.Models;

public class Ban
{
    public ulong Server { get; init; }
    public ulong User { get; init; }
    public DateTime EndDate { get; init; }
    public string Reason { get; init; }
}