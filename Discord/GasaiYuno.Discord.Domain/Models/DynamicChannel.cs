using System.Collections.Generic;

namespace GasaiYuno.Discord.Domain.Models
{
    public class DynamicChannel
    {
        public Server Server { get; init; }
        public string Name { get; init; }
        public AutomationType Type { get; init; }
        public string GenerationName { get; set; }
        public List<ulong> Channels { get; init; }
        public List<ulong> GeneratedChannels { get; init; }

        public DynamicChannel()
        {
            Channels = new List<ulong>();
            GeneratedChannels = new List<ulong>();
        }
    }
}