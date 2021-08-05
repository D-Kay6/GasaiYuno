﻿using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure.Repositories
{
    public class PollRepository : Repository<Poll>, IPollRepository
    {
        /// <summary>
        /// Creates a new <see cref="PollRepository"/>.
        /// </summary>
        /// <param name="context">The context that will be used.</param>
        public PollRepository(DataContext context) : base(context) { }

        /// <inheritdoc/>
        public new void Add(Poll entity)
        {
            var dto = new Poll
            {
                Channel = entity.Channel,
                Message = entity.Message,
                MultiSelect = entity.MultiSelect,
                EndDate = entity.EndDate,
                Text = entity.Text,
                Options = entity.Options
            };
            var trackedEntity = Context.Polls.Add(dto);
            trackedEntity.Property("ServerId").CurrentValue = entity.Server.Id;
        }

        /// <inheritdoc/>
        public async Task<Poll> GetAsync(ulong id, ulong channelId, ulong messageId)
        {
            return await Context.Polls
                .Include(x => x.Server)
                .ThenInclude(x => x.Language)
                .FirstOrDefaultAsync(x => x.Server.Id == id && x.Channel == channelId && x.Message == messageId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<Poll>> ListAsync(ulong serverId)
        {
            return await Context.Polls
                .Where(x => x.Server.Id == serverId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<Poll>> ListAsync(ulong serverId, ulong channelId)
        {
            return await Context.Polls
                .Where(x => x.Server.Id == serverId && x.Channel == channelId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<Poll>> ListAsync(ulong? serverId = null, bool? expired = null)
        {
            var query = Context.Polls
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