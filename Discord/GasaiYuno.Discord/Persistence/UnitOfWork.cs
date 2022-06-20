using GasaiYuno.Discord.Core.Interfaces;
using Raven.Client.Documents.Session;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Persistence;

internal class UnitOfWork<TEntity> : Repository<TEntity>, IUnitOfWork<TEntity> where TEntity : IEntity
{
    public UnitOfWork(IAsyncDocumentSession session) : base(session)
    {
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Session.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        Session?.Dispose();
    }
}