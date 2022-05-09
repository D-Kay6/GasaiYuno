using System.Collections.Generic;

namespace GasaiYuno.Discord.Domain.Models;

public class GameRole
{
    public ulong Server { get; init; }
    public string Name { get; init; }
    public AutomationType Type { get; init; }
    public List<ulong> Roles { get; set; }
    public List<string> Games { get; set; }

    public GameRole()
    {
        Roles = new List<ulong>();
        Games = new List<string>();
    }
}