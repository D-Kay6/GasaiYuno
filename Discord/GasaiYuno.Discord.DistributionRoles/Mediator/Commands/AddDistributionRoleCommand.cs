using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Commands;

public record AddDistributionRoleCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public ulong MessageId { get; }
    public string Description { get; }
    public string ButtonText { get; }
    public int MinSelected { get; }
    public int MaxSelected { get; }

    public AddDistributionRoleCommand(ulong serverId, ulong channelId, ulong messageId, string description, string buttonText)
    {
        ServerId = serverId;
        ChannelId = channelId;
        MessageId = messageId;
        Description = description;
        ButtonText = buttonText;
    }

    public AddDistributionRoleCommand(ulong serverId, ulong channelId, ulong messageId, string description, int minSelected, int maxSelected)
    {
        ServerId = serverId;
        ChannelId = channelId;
        MessageId = messageId;
        Description = description;
        MinSelected = minSelected;
        MaxSelected = maxSelected;
    }
}