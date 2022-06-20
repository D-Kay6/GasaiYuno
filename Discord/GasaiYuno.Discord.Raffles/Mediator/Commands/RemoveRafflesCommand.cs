using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Commands;

public record RemoveRafflesCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    
    public RemoveRafflesCommand(ulong serverId, ulong channelId)
    {
        ServerId = serverId;
        ChannelId = channelId;
    }
}