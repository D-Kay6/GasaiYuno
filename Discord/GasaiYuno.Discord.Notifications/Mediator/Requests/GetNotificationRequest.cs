using GasaiYuno.Discord.Notifications.Models;
using MediatR;

namespace GasaiYuno.Discord.Notifications.Mediator.Requests;

public record GetNotificationRequest : IRequest<Notification>
{
    public ulong ServerId { get; }
    public NotificationType Type { get; }

    public GetNotificationRequest(ulong serverId, NotificationType type)
    {
        ServerId = serverId;
        Type = type;
    }
}