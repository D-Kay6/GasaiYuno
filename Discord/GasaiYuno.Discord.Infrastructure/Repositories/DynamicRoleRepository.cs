using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<DynamicRole> GetAsync(ulong serverId, AutomationType type, string status)
        {
            throw new NotImplementedException();
            //return await Context.DynamicRoles
            //    .Include(x => x.Server)
            //    .FirstOrDefaultAsync(x => x.Server.Id == serverId && x.Type == type && x.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
            //    .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<DynamicRole>> ListAsync(ulong serverId)
        {
            throw new NotImplementedException();
            //return await Context.DynamicRoles
            //    .Where(x => x.Server.Id == serverId)
            //    .ToListAsync()
            //    .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<DynamicRole>> ListAsync(ulong serverId, AutomationType type)
        {
            throw new NotImplementedException();
            //return await Context.DynamicRoles
            //    .Where(x => x.Server.Id == serverId && x.Type == type)
            //    .ToListAsync()
            //    .ConfigureAwait(false);
        }
    }
}