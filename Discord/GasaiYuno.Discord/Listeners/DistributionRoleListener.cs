using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners;

internal class DistributionRoleListener : IListener
{
    public int Priority => 1;
    
    private readonly DiscordShardedClient _client;
    private readonly Func<IUnitOfWork> _unitOfWorkFactory;
    private readonly IMediator _mediator;
    private readonly ILogger<DistributionRoleListener> _logger;

    public DistributionRoleListener(DiscordShardedClient client, Func<IUnitOfWork> unitOfWorkFactory, IMediator mediator, ILogger<DistributionRoleListener> logger)
    {
        _client = client;
        _unitOfWorkFactory = unitOfWorkFactory;
        _mediator = mediator;
        _logger = logger;
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

        var unitOfWork = _unitOfWorkFactory();
        var distributionRoles = await unitOfWork.DistributionRoles.ListAsync(guildChannel.Guild.Id, guildChannel.Id).ConfigureAwait(false);
        if (!distributionRoles.Any()) return;

        unitOfWork.DistributionRoles.RemoveRange(distributionRoles);
        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task OnMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
    {
        var channel = await cachedChannel.GetOrDownloadAsync().ConfigureAwait(false);
        if (channel is not SocketGuildChannel guildChannel) return;

        var unitOfWork = _unitOfWorkFactory();
        var distributionRole = await unitOfWork.DistributionRoles.GetAsync(guildChannel.Guild.Id, guildChannel.Id, cachedMessage.Id).ConfigureAwait(false);
        if (distributionRole == null) return;

        unitOfWork.DistributionRoles.Remove(distributionRole);
        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}