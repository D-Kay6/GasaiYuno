using System.Collections.Generic;

namespace GasaiYuno.Discord.Domain.Models;

public abstract class DynamicRole
{
    public ulong Server { get; init; }
    public string Name { get; init; }
    public List<ulong> Roles { get; set; }

    protected DynamicRole()
    {
        Roles = new List<ulong>();
    }
}