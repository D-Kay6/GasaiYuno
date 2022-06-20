using GasaiYuno.Discord.Notifications.Models;
using MediatR;

namespace GasaiYuno.Discord.Notifications.Mediator.Commands;

public record RemoveNotificationCommand : INotification
{
    public ulong ServerId { get; }
    public NotificationType Type { get; }

    public RemoveNotificationCommand(ulong serverId, NotificationType type)
    {
        ServerId = serverId;
        Type = type;
    }
}