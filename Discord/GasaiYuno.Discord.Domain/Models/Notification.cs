namespace GasaiYuno.Discord.Domain.Models;

public class Notification
{
    public ulong Server { get; init; }
    public NotificationType Type { get; init; }
    public string Message { get; set; }
    public string Image { get; set; }
    public ulong? Channel { get; set; }
}