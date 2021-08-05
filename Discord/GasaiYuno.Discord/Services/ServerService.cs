using Discord;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Services
{
    public class ServerService
    {
        private readonly Func<IUnitOfWork<IServerRepository>> _repoFactory;

        public ServerService(Func<IUnitOfWork<IServerRepository>> repoFactory)
        {
            _repoFactory = repoFactory;
        }

        /// <summary>
        /// Load the details for the guild.
        /// </summary>
        /// <param name="guild">The guild to load.</param>
        /// <returns>The guild as a server object.</returns>
        public async Task<Server> Load(IGuild guild)
        {
            var repository = _repoFactory();

            var server = await repository.DataSet.GetOrAddAsync(guild.Id, guild.Name).ConfigureAwait(false);
            if (!server.Name.Equals(guild.Name, StringComparison.Ordinal))
            {
                server.Name = guild.Name;

                await repository.BeginAsync().ConfigureAwait(false);
                repository.DataSet.Update(server);
                await repository.SaveAsync().ConfigureAwait(false);
            }

            return server;
        }
    }
}