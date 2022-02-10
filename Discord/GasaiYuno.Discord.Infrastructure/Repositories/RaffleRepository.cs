using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure.Repositories
{
    public class RaffleRepository : Repository<Raffle>, IRaffleRepository
    {
        /// <summary>
        /// Creates a new <see cref="RaffleRepository"/>.
        /// </summary>
        /// <param name="context">The context that will be used.</param>
        public RaffleRepository(DataContext context) : base(context) { }

        /// <inheritdoc/>
        public new void Add(Raffle entity)
        {
            var dto = new Raffle
            {
                Channel = entity.Channel,
                Message = entity.Message,
                Title = entity.Title,
                Users = entity.Users,
                EndDate = entity.EndDate
            };
            var trackedEntity = Context.Raffles.Add(dto);
            trackedEntity.Property("ServerId").CurrentValue = entity.Server.Id;
        }

        /// <inheritdoc/>
        public async Task<Raffle> GetAsync(ulong id, ulong channelId, ulong messageId)
        {
            return await Context.Raffles
                .Include(x => x.Server)
                .ThenInclude(x => x.Language)
                .FirstOrDefaultAsync(x => x.Server.Id == id && x.Channel == channelId && x.Message == messageId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<Raffle>> ListAsync(ulong serverId)
        {
            return await Context.Raffles
                .Where(x => x.Server.Id == serverId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<Raffle>> ListAsync(ulong serverId, ulong channelId)
        {
            return await Context.Raffles
                .Where(x => x.Server.Id == serverId && x.Channel == channelId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<Raffle>> ListAsync(ulong? serverId = null, bool? expired = null)
        {
            var query = Context.Raffles
                .Include(x => x.Server)
                .ThenInclude(x => x.Language)
                .AsQueryable();
            if (serverId.HasValue) query = query.Where(x => x.Server.Id == serverId);
            if (expired.HasValue) query = expired.Value ? query.Where(x => x.EndDate < DateTime.Now) : query.Where(x => x.EndDate >= DateTime.Now);

            var bans = await query.ToListAsync().ConfigureAwait(false);
            return bans;
        }
    }
}