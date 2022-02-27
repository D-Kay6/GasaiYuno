using System;
using System.Collections.Generic;

namespace GasaiYuno.Discord.Domain.Models
{
    public class Raffle
    {
        public ulong Id { get; init; }
        public Server Server { get; init; }
        public ulong Channel { get; init; }
        public ulong Message { get; init; }
        public string Title { get; init; }
        public DateTime EndDate { get; init; }
        public List<RaffleEntry> Entries { get; init; }

        public Raffle()
        {
            Entries = new List<RaffleEntry>();
        }
    }
}