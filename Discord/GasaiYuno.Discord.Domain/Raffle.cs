using System;
using System.Collections.Generic;

namespace GasaiYuno.Discord.Domain
{
    public class Raffle
    {
        public Server Server { get; init; }
        public ulong Channel { get; init; }
        public ulong Message { get; init; }
        public string Title { get; init; }
        public DateTime EndDate { get; init; }
        public List<ulong> Users { get; init; }

        public Raffle()
        {
            Users = new List<ulong>();
        }
    }
}