﻿using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.Polls.Models;

public class Poll : IEntity
{
    public Guid Identity { get; init; }
    public ulong Server { get; init; }
    public ulong Channel { get; init; }
    public ulong Message { get; init; }
    public DateTime EndDate { get; init; }
    public string Text { get; init; }
    public List<PollOption> Options { get; init; }

    public Poll()
    {
        Options = new List<PollOption>();
    }
}