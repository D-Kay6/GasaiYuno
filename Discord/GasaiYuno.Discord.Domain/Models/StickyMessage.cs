namespace GasaiYuno.Discord.Domain.Models;

public class StickyMessage
{
    public ulong Identity { get; init; }
    public ulong Server { get; init; }
    public ulong Channel { get; init; }
    public ulong Message { get; set; }
    public bool IsEmbed { get; init; }
    public string Text { get; init; }
    public string Image { get; init; }
}