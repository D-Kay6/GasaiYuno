using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure.Repositories
{
    public class ServerRepository : Repository<Server>, IServerRepository
    {
        /// <summary>
        /// Creates a new <see cref="ServerRepository"/>.
        /// </summary>
        /// <param name="context">The context that will be used.</param>
        public ServerRepository(DataContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task AddOrUpdateAsync(ulong serverId, string name)
        {
            var transaction = await Context.BeginTransactionAsync().ConfigureAwait(false);

            var server = await GetAsync(serverId).ConfigureAwait(false);
            if (server == null)
            {
                Context.Servers.Add(new Server
                {
                    Id = serverId,
                    Name = name
                });
            }
            else if (!server.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                server.Name = name;
                Context.Update(server);
            }

            await Context.CommitTransactionAsync(transaction).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Server> GetAsync(ulong serverId)
        {
            return await Context.Servers
                .FirstOrDefaultAsync(x => x.Id == serverId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Server> GetOrAddAsync(ulong serverId, string name)
        {
            var entity = await GetAsync(serverId).ConfigureAwait(false);
            if (entity == null)
            {
                var transaction = await Context.BeginTransactionAsync().ConfigureAwait(false);
                var server = Context.Servers.Add(new Server
                {
                    Id = serverId,
                    Name = name
                });
                await Context.CommitTransactionAsync(transaction).ConfigureAwait(false);
                return server.Entity;
            }

            if (!entity.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                entity.Name = name;
                var transaction = await Context.BeginTransactionAsync().ConfigureAwait(false);
                Context.Update(entity);
                await Context.CommitTransactionAsync(transaction).ConfigureAwait(false);
            }

            return entity;
        }

        /// <inheritdoc/>
        public async Task<List<Server>> ListAsync()
        {
            return await Context.Servers
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}