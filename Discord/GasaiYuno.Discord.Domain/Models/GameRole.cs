using System.Collections.Generic;

namespace GasaiYuno.Discord.Domain.Models
{
    public class GameRole : DynamicRole
    {
        public AutomationType Type { get; init; }
        public List<string> Games { get; set; }

        public GameRole()
        {
            Games = new List<string>();
        }
    }
}