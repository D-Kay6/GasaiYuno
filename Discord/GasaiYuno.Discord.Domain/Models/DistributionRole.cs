using System.Collections.Generic;

namespace GasaiYuno.Discord.Domain.Models;

public class DistributionRole
{
    public ulong Server { get; init; }
    public ulong Channel { get; init; }
    public ulong Message { get; set; }

    public string Description { get; set; }
    public Dictionary<ulong, string> Roles { get; set; }

    public string ButtonText { get; set; }
    public int MinSelected { get; set; }
    public int MaxSelected { get; set; }

    public DistributionRole()
    {
        Roles = new Dictionary<ulong, string>();
        MinSelected = 0;
        MaxSelected = 1;
    }
}