using GasaiYuno.Discord.Notifications.Models;
using MediatR;

namespace GasaiYuno.Discord.Notifications.Mediator.Commands;

public record AddNotificationCommand : INotification
{
    public ulong ServerId { get; }
    public NotificationType Type { get; }
    public ulong? ChannelId { get; }
    public string Message { get; }
    public string Image { get; }

    public AddNotificationCommand(Notification notification)
    {
        ServerId = notification.Server;
        Type = notification.Type;
        ChannelId = notification.Channel;
        Message = notification.Message;
        Image = notification.Image;
    }

    public AddNotificationCommand(ulong serverId, NotificationType type, ulong? channelId = null, string message = null, string image = null)
    {
        ServerId = serverId;
        Type = type;
        ChannelId = channelId;
        Message = message;
        Image = image;
    }
}