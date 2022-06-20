using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.Notifications.Models;

public class Notification : IEntity
{
    public ulong Server { get; init; }
    public NotificationType Type { get; init; }
    public ulong? Channel { get; set; }
    public string Message { get; set; }
    public string Image { get; set; }

    public Notification()
    {
        Type = NotificationType.Welcome;
        Message = "Welcome to the party [user]. Hope you will have a good time with us.";
    }
}