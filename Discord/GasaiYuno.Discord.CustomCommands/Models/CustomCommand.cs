using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.CustomCommands.Models;

public class CustomCommand : IEntity
{
    public ulong Server { get; init; }
    public string Command { get; set; }
    public string Response { get; set; }
}