using Discord.WebSocket;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Models;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners
{
    internal class GuildListener
    {
        private readonly DiscordShardedClient _client;
        private readonly Func<IUnitOfWork<IServerRepository>> _unitOfWork;

        public GuildListener(Connection connection, Func<IUnitOfWork<IServerRepository>> unitOfWork)
        {
            _client = connection.Client;
            _unitOfWork = unitOfWork;

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

            var repository = _unitOfWork();
            var server = await repository.DataSet.GetAsync(newGuild.Id).ConfigureAwait(false);
            if (server == null)
            {
                server = new Server
                {
                    Id = newGuild.Id,
                    Name = newGuild.Name
                };
                await repository.BeginAsync().ConfigureAwait(false);
                repository.DataSet.Add(server);
                await repository.SaveAsync().ConfigureAwait(false);
                return;
            }

            if (server.Name.Equals(newGuild.Name, StringComparison.OrdinalIgnoreCase))
                return;

            server.Name = newGuild.Name;
            await repository.BeginAsync().ConfigureAwait(false);
            repository.DataSet.Update(server);
            await repository.SaveAsync().ConfigureAwait(false);
        }

        private async Task GuildJoinedAsync(SocketGuild guild)
        {
            var repository = _unitOfWork();

            var server = await repository.DataSet.GetAsync(guild.Id).ConfigureAwait(false);
            if (server != null) return;

            server = new Server
            {
                Id = guild.Id,
                Name = guild.Name
            };
            await repository.BeginAsync().ConfigureAwait(false);
            repository.DataSet.Add(server);
            await repository.SaveAsync().ConfigureAwait(false);
        }

        private async Task GuildLeftAsync(SocketGuild guild)
        {
            var repository = _unitOfWork();

            var server = await repository.DataSet.GetAsync(guild.Id).ConfigureAwait(false);
            if (server == null) return;

            await repository.BeginAsync().ConfigureAwait(false);
            repository.DataSet.Remove(server);
            await repository.SaveAsync().ConfigureAwait(false);
        }

        private async Task CheckGuilds()
        {
            var repository = _unitOfWork();
            var servers = await repository.DataSet.ListAsync().ConfigureAwait(false);

            await repository.BeginAsync().ConfigureAwait(false);
            foreach (var server in servers.Where(server => _client.GetGuild(server.Id) == null))
                repository.DataSet.Remove(server);
            await repository.SaveAsync().ConfigureAwait(false);

            foreach (var guild in _client.Guilds)
                await repository.DataSet.AddOrUpdateAsync(guild.Id, guild.Name).ConfigureAwait(false);
        }
    }
}