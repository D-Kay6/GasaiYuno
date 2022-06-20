using System;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Core.Interfaces;

public interface IUnitOfWork<TEntity> : IRepository<TEntity>, IDisposable where TEntity : IEntity
{
    /// <summary>
    /// Save all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}