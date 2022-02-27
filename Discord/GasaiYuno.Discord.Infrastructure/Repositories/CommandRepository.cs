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
    public class CommandRepository : Repository<CustomCommand>, ICommandRepository
    {
        /// <summary>
        /// Creates a new <see cref="CommandRepository"/>.
        /// </summary>
        /// <param name="context">The context that will be used.</param>
        public CommandRepository(DataContext context) : base(context) { }

        /// <inheritdoc/>
        public new void Add(CustomCommand entity)
        {
            var dto = new CustomCommand
            {
                Command = entity.Command,
                Response = entity.Response
            };
            var trackedEntity = Context.CustomCommands.Add(dto);
            trackedEntity.Property("ServerId").CurrentValue = entity.Server.Id;
        }

        /// <inheritdoc/>
        public async Task<CustomCommand> GetAsync(ulong serverId, string command)
        {
            return await Context.CustomCommands
                .Include(x => x.Server)
                .FirstOrDefaultAsync(x => x.Server.Id == serverId && x.Command == command)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<List<CustomCommand>> SearchAsync(ulong serverId, string command)
        {
            return Context.CustomCommands
                .Include(x => x.Server)
                .Where(x => x.Server.Id == serverId && EF.Functions.Like(x.Command, $"%{command}%"))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<CustomCommand>> ListAsync(ulong serverId)
        {
            return await Context.CustomCommands
                .Where(x => x.Server.Id == serverId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public new Task<int> CountAsync(Expression<Func<CustomCommand, bool>> predicate)
        {
            return Context.Set<CustomCommand>().Include(x => x.Server).CountAsync(predicate);
        }
    }
}