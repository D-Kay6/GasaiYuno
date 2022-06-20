using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Commands;

public record RemovePollsCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    
    public RemovePollsCommand(ulong serverId, ulong channelId)
    {
        ServerId = serverId;
        ChannelId = channelId;
    }
}