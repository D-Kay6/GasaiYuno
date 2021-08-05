using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure.Repositories
{
    public class DynamicChannelRepository : Repository<DynamicChannel>, IDynamicChannelRepository
    {
        /// <summary>
        /// Creates a new <see cref="DynamicChannelRepository"/>.
        /// </summary>
        /// <param name="context">The context that will be used.</param>
        public DynamicChannelRepository(DataContext context) : base(context) { }

        /// <inheritdoc/>
        public new void Add(DynamicChannel entity)
        {
            var dbo = new DynamicChannel
            {
                Name = entity.Name,
                Type = entity.Type
            };
            var trackedEntity = Context.DynamicChannels.Add(dbo);
            trackedEntity.Property("ServerId").CurrentValue = entity.Server.Id;
        }

        /// <inheritdoc/>
        public new Task<bool> AnyAsync(Expression<Func<DynamicChannel, bool>> predicate)
        {
            return Context.Set<DynamicChannel>().Include(x => x.Server).AnyAsync(predicate);
        }

        /// <inheritdoc/>
        public async Task<DynamicChannel> GetAsync(ulong serverId, string name)
        {
            return await Context.DynamicChannels
                .Include(x => x.Server)
                .FirstOrDefaultAsync(x => x.Server.Id == serverId && x.Name == name)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<DynamicChannel>> ListAsync(ulong serverId)
        {
            return await Context.DynamicChannels
                .Where(x => x.Server.Id == serverId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<DynamicChannel>> ListAsync(ulong serverId, AutomationType type)
        {
            return await Context.DynamicChannels
                .Where(x => x.Server.Id == serverId && x.Type == type)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}