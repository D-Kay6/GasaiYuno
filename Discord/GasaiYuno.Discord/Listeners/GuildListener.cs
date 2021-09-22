using Discord.WebSocket;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Models;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners
{
    internal class GuildListener
    {
        private readonly DiscordShardedClient _client;
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public GuildListener(Connection connection, Func<IUnitOfWork> unitOfWorkFactory)
        {
            _client = connection.Client;
            _unitOfWorkFactory = unitOfWorkFactory;

            connection.Ready += OnReady;
        }

        private Task OnReady()
        {
            _client.GuildUpdated += GuildUpdated;
            _client.JoinedGuild += GuildJoinedAsync;
            _client.LeftGuild += GuildLeftAsync;

#if DEBUG
            return Task.CompletedTask;
#else
            return CheckGuilds();
#endif
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
                    Id = newGuild.Id,
                    Name = newGuild.Name
                };
                unitOfWork.Servers.Add(server);
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
                Id = guild.Id,
                Name = guild.Name
            };
            unitOfWork.Servers.Add(server);
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
            
            foreach (var server in servers.Where(server => _client.GetGuild(server.Id) == null))
                unitOfWork.Servers.Remove(server);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            foreach (var guild in _client.Guilds)
                await unitOfWork.Servers.AddOrUpdateAsync(guild.Id, guild.Name).ConfigureAwait(false);
        }
    }
}