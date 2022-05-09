namespace GasaiYuno.Discord.Domain.Models;

public class CustomCommand
{
    public ulong Server { get; init; }
    public string Command { get; set; }
    public string Response { get; set; }
}