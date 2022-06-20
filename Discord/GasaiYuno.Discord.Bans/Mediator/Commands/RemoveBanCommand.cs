using MediatR;

namespace GasaiYuno.Discord.Bans.Mediator.Commands;

public record RemoveBanCommand : INotification
{
    public ulong ServerId { get; }
    public ulong UserId { get; }

    public RemoveBanCommand(ulong serverId, ulong userId)
    {
        ServerId = serverId;
        UserId = userId;
    }
}