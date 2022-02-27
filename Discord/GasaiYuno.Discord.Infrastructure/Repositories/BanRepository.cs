using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure.Repositories
{
    public class BanRepository : Repository<Ban>, IBanRepository
    {
        /// <summary>
        /// Creates a new <see cref="BanRepository"/>.
        /// </summary>
        /// <param name="context">The context that will be used.</param>
        public BanRepository(DataContext context) : base(context) { }

        /// <inheritdoc/>
        public new void Add(Ban entity)
        {
            var dto = new Ban
            {
                User = entity.User,
                EndDate = entity.EndDate,
                Reason = entity.Reason
            };
            var trackedEntity = Context.Bans.Add(dto);
            trackedEntity.Property("ServerId").CurrentValue = entity.Server.Id;
        }

        /// <inheritdoc/>
        public async Task<Ban> GetAsync(ulong serverId, ulong userId)
        {
            var ban = await Context.Bans
                .Include(x => x.Server)
                .FirstOrDefaultAsync(x => x.Server.Id == serverId && x.User == userId)
                .ConfigureAwait(false);

            return ban;
        }

        /// <inheritdoc/>
        public async Task<List<Ban>> ListAsync(ulong? serverId = null, bool? expired = null)
        {
            var query = Context.Bans.AsQueryable();
            if (serverId.HasValue) query = query.Where(x => x.Server.Id == serverId);
            if (expired.HasValue) query = expired.Value ? query.Where(x => x.EndDate < DateTime.Now) : query.Where(x => x.EndDate >= DateTime.Now);

            var bans = await query.ToListAsync().ConfigureAwait(false);
            return bans;
        }

        /// <inheritdoc/>
        public async Task<List<Ban>> ListAsync(Expression<Func<Ban, bool>> predicate)
        {
            var bans = await Context.Bans
                .Where(predicate)
                .ToListAsync()
                .ConfigureAwait(false);

            return bans;
        }
    }
}