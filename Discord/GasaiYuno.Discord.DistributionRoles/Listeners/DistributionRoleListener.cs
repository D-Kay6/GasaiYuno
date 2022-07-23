using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DistributionRoles.Mediator.Commands;
using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Listeners;

internal class DistributionRoleListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly IMediator _mediator;

    public DistributionRoleListener(DiscordShardedClient client, IMediator mediator)
    {
        _client = client;
        _mediator = mediator;
    }

    public Task Start()
    {
        _client.ChannelDestroyed += OnChannelDestroyed;
        _client.MessageDeleted += OnMessageDeleted;

        return Task.CompletedTask;
    }

    private async Task OnChannelDestroyed(SocketChannel channel)
    {
        if (channel is not SocketGuildChannel guildChannel) return;

        await _mediator.Publish(new RemoveDistributionRolesCommand(guildChannel.Guild.Id, guildChannel.Id)).ConfigureAwait(false);
    }

    private async Task OnMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
    {
        var channel = await cachedChannel.GetOrDownloadAsync().ConfigureAwait(false);
        if (channel is not SocketGuildChannel guildChannel) return;

        await _mediator.Publish(new RemoveDistributionRoleCommand(guildChannel.Guild.Id, guildChannel.Id, cachedMessage.Id)).ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}