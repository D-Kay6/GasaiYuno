using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.StickyMessages.Models;

public class StickyMessage : IEntity
{
    public ulong Identity { get; init; }
    public ulong Server { get; init; }
    public ulong Channel { get; init; }
    public ulong Message { get; set; }
    public bool IsEmbed { get; init; }
    public string Text { get; init; }
    public string Image { get; init; }
}