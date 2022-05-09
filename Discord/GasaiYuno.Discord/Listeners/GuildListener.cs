using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners;

internal class GuildListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly Func<IUnitOfWork> _unitOfWorkFactory;

    public GuildListener(DiscordShardedClient client, Func<IUnitOfWork> unitOfWorkFactory)
    {
        _client = client;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public Task Start()
    {
        _client.GuildUpdated += GuildUpdated;
        _client.JoinedGuild += GuildJoinedAsync;
        _client.LeftGuild += GuildLeftAsync;

#if DEBUG
        return Task.CompletedTask;
#endif
        return CheckGuilds();
    }

    private async Task GuildUpdated(SocketGuild oldGuild, SocketGuild newGuild)
    {
        if (string.IsNullOrEmpty(oldGuild.Name)) return;
        if (oldGuild.Name.Equals(newGuild.Name)) return;

        var unitOfWork = _unitOfWorkFactory();
        var server = await unitOfWork.Servers.GetAsync(newGuild.Id).ConfigureAwait(false);
        if (server == null)
        {
            server = new Server
            {
                Identity = newGuild.Id,
                Name = newGuild.Name
            };
            await unitOfWork.Servers.AddAsync(server).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return;
        }

        if (server.Name.Equals(newGuild.Name, StringComparison.OrdinalIgnoreCase))
            return;

        server.Name = newGuild.Name;
        unitOfWork.Servers.Update(server);
        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task GuildJoinedAsync(SocketGuild guild)
    {
        var unitOfWork = _unitOfWorkFactory();

        var server = await unitOfWork.Servers.GetAsync(guild.Id).ConfigureAwait(false);
        if (server != null) return;

        server = new Server
        {
            Identity = guild.Id,
            Name = guild.Name
        };
        await unitOfWork.Servers.AddAsync(server).ConfigureAwait(false);
        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task GuildLeftAsync(SocketGuild guild)
    {
        var unitOfWork = _unitOfWorkFactory();

        var server = await unitOfWork.Servers.GetAsync(guild.Id).ConfigureAwait(false);
        if (server == null) return;

        unitOfWork.Servers.Remove(server);
        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task CheckGuilds()
    {
        var unitOfWork = _unitOfWorkFactory();
        var servers = await unitOfWork.Servers.ListAsync().ConfigureAwait(false);
        foreach (var server in servers.Where(server => _client.GetGuild(server.Identity) == null))
            unitOfWork.Servers.Remove(server);
        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        foreach (var guild in _client.Guilds)
        {
            unitOfWork = _unitOfWorkFactory();
            await unitOfWork.Servers.AddOrUpdateAsync(guild.Id, guild.Name).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }
    
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}