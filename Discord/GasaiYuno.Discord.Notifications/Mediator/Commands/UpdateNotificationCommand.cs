using GasaiYuno.Discord.Notifications.Models;
using MediatR;

namespace GasaiYuno.Discord.Notifications.Mediator.Commands;

public record UpdateNotificationCommand : INotification
{
    public ulong ServerId { get; }
    public NotificationType Type { get; }
    public ulong? ChannelId { get; }
    public string Message { get; }
    public string Image { get; }

    public UpdateNotificationCommand(Notification notification)
    {
        ServerId = notification.Server;
        Type = notification.Type;
        ChannelId = notification.Channel;
        Message = notification.Message;
        Image = notification.Image;
    }
}