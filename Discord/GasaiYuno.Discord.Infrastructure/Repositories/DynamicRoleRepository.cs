using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure.Repositories
{
    public class DynamicRoleRepository : Repository<DynamicRole>, IDynamicRoleRepository
    {
        /// <summary>
        /// Creates a new <see cref="DynamicRoleRepository"/>.
        /// </summary>
        /// <param name="context">The context that will be used.</param>
        public DynamicRoleRepository(DataContext context) : base(context) { }

        /// <inheritdoc/>
        public Task<DynamicRole> GetAsync(ulong serverId, AutomationType type, string status)
        {
            throw new NotImplementedException();
            //return await Context.DynamicRoles
            //    .Include(x => x.Server)
            //    .FirstOrDefaultAsync(x => x.Server.Id == serverId && x.Type == type && x.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
            //    .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<List<DynamicRole>> ListAsync(ulong serverId)
        {
            throw new NotImplementedException();
            //return await Context.DynamicRoles
            //    .Where(x => x.Server.Id == serverId)
            //    .ToListAsync()
            //    .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<List<DynamicRole>> ListAsync(ulong serverId, AutomationType type)
        {
            throw new NotImplementedException();
            //return await Context.DynamicRoles
            //    .Where(x => x.Server.Id == serverId && x.Type == type)
            //    .ToListAsync()
            //    .ConfigureAwait(false);
        }
    }
}