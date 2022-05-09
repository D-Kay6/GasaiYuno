using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.RavenDB.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected IAsyncDocumentSession Session { get; }

    public Repository(IAsyncDocumentSession session)
    {
        Session = session;
    }
        
    /// <inheritdoc />
    public Task AddAsync<T>(T entity) where T : class, TEntity
    {
        return Session.StoreAsync(entity);
    }
        
    /// <inheritdoc />
    public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, TEntity
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity).ConfigureAwait(false);
        }
    }
        
    /// <inheritdoc />
    public void Update<T>(T entity) where T : class, TEntity
    { }
        
    /// <inheritdoc />
    public void UpdateRange<T>(IEnumerable<T> entities) where T : class, TEntity
    { }
        
    /// <inheritdoc />
    public void Remove<T>(T entity) where T : class, TEntity
    {
        Session.Delete(entity);
    }
        
    /// <inheritdoc />
    public void RemoveRange<T>(IEnumerable<T> entities) where T : class, TEntity
    {
        foreach (var entity in entities)
        {
            Session.Delete(entity);
        }
    }

    /// <inheritdoc />
    public Task<List<TEntity>> GetAllAsync()
    {
        return Session.Query<TEntity>().ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<T>> GetAllAsync<T>() where T : class, TEntity
    {
        return Session.Query<T>().ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<T>> WhereAsync<T>(Expression<Func<T, bool>> predicate) where T : class, TEntity
    {
        return Session.Query<T>().Where(predicate, true).ToListAsync();
    }
        
    /// <inheritdoc />
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Session.Query<TEntity>().Where(predicate, true).CountAsync();
    }
        
    /// <inheritdoc />
    public Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : class, TEntity
    {
        return Session.Query<T>().Where(predicate, true).CountAsync();
    }
        
    /// <inheritdoc />
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Session.Query<TEntity>().Where(predicate, true).AnyAsync();
    }
        
    /// <inheritdoc />
    public Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class, TEntity
    {
        return Session.Query<T>().Where(predicate, true).AnyAsync();
    }
}