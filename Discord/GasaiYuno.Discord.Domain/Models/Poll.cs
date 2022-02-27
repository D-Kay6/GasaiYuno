using System;
using System.Collections.Generic;

namespace GasaiYuno.Discord.Domain.Models
{
    public class Poll
    {
        public ulong Id { get; init; }
        public Server Server { get; init; }
        public ulong Channel { get; init; }
        public ulong Message { get; init; }
        public DateTime EndDate { get; init; }
        public string Text { get; init; }
        public List<PollOption> Options { get; init; }
        public List<PollSelection> Selections { get; init; }

        public Poll()
        {
            Options = new List<PollOption>();
            Selections = new List<PollSelection>();
        }
    }
}