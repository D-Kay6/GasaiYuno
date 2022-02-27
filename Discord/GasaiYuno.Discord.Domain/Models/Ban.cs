using System;

namespace GasaiYuno.Discord.Domain.Models
{
    public class Ban
    {
        public Server Server { get; init; }
        public ulong User { get; init; }
        public DateTime EndDate { get; init; }
        public string Reason { get; init; }
    }
}