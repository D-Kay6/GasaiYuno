using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.Core.Models;

public class Server : IEntity
{
    public ulong Identity { get; init; }
    public string Name { get; set; }
    public bool WarningDisabled { get; set; }
    public string Prefix { get; set; }
    public Languages Language { get; set; }
}