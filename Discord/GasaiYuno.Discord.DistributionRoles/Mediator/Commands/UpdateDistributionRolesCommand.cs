using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Commands;

public record UpdateDistributionRolesCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public ulong MessageId { get; }
    public string SelectorId { get; }
    public ulong[] SelectedRoles { get; }
    
    public UpdateDistributionRolesCommand(ulong serverId, ulong channelId, ulong messageId, string selectorId, ulong[] selectedRoles)
    {
        ServerId = serverId;
        ChannelId = channelId;
        MessageId = messageId;
        SelectorId = selectorId;
        SelectedRoles = selectedRoles;
    }
}