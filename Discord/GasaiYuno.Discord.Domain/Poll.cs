using System;
using System.Collections.Generic;

namespace GasaiYuno.Discord.Domain
{
    public class Poll
    {
        public Server Server { get; init; }
        public ulong Channel { get; init; }
        public ulong Message { get; init; }
        public bool MultiSelect { get; init; }
        public DateTime EndDate { get; init; }
        public string Text { get; init; }
        public List<PollOption> Options { get; init; }

        public Poll()
        {
            Options = new List<PollOption>();
        }
    }
}