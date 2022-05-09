namespace GasaiYuno.Discord.Domain.Models;

public class Server 
{
    public ulong Identity { get; init; }
    public string Name { get; set; }
    public bool WarningDisabled { get; set; }
    public string Prefix { get; set; }
    public Languages Language { get; set; }
}