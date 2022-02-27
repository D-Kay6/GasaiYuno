using GasaiYuno.Discord.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Domain.Persistence.Repositories
{
    public interface IBanRepository : IRepository<Ban>
    {
        /// <summary>
        /// Gets the ban object asynchronously.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <param name="userId">The id of the user.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="Ban"/>.</returns>
        Task<Ban> GetAsync(ulong serverId, ulong userId);

        /// <summary>
        /// Gets all the bans from the database.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <param name="expired">The expired state of the ban.</param>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="Ban"/>.</returns>
        Task<List<Ban>> ListAsync(ulong? serverId = null, bool? expired = null);

        /// <summary>
        /// Gets all the bans where the expression is true from the database.
        /// </summary>
        /// <param name="predicate">The expression that will be used to find the objects.</param>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="Ban"/>.</returns>
        Task<List<Ban>> ListAsync(Expression<Func<Ban, bool>> predicate);
    }
}