using Raven.Client.Documents.Session;

namespace GasaiYuno.Persistence.Data;

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