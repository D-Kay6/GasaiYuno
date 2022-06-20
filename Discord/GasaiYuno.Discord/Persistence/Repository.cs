using GasaiYuno.Discord.Core.Interfaces;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Persistence;

internal class Repository<TEntity> : IRepository<TEntity> where TEntity : IEntity
{
    protected readonly IAsyncDocumentSession Session;

    public Repository(IAsyncDocumentSession session)
    {
        Session = session;
    }

    /// <inheritdoc />
    public Task AddAsync<T>(T entity, CancellationToken token = default) where T : TEntity
    {
        return Session.StoreAsync(entity, token);
    }

    /// <inheritdoc />
    public async Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken token = default) where T : TEntity
    {
        foreach (var entity in entities)
        {
            if (token.IsCancellationRequested)
                break;
            
            await AddAsync(entity, token).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void Remove<T>(T entity) where T : TEntity
    {
        Session.Delete(entity);
    }

    /// <inheritdoc />
    public void RemoveRange<T>(IEnumerable<T> entities) where T : TEntity
    {
        foreach (var entity in entities)
        {
            Session.Delete(entity);
        }
    }

    /// <inheritdoc />
    public Task<List<TEntity>> GetAllAsync(CancellationToken token = default)
    {
        return Session.Query<TEntity>().ToListAsync(token);
    }

    /// <inheritdoc />
    public Task<List<T>> GetAllAsync<T>(CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().ToListAsync(token);
    }
    
    /// <inheritdoc />
    public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
    {
        return Session.Query<TEntity>().FirstOrDefaultAsync(predicate, token);
    }
    
    /// <inheritdoc />
    public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool exact, CancellationToken token = default)
    {
        return Session.Query<TEntity>().Where(predicate, exact).FirstOrDefaultAsync(token);
    }

    /// <inheritdoc />
    public Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().FirstOrDefaultAsync(predicate, token);
    }

    /// <inheritdoc />
    public Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, bool exact, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().Where(predicate, exact).FirstOrDefaultAsync(token);
    }
    
    /// <inheritdoc />
    public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
    {
        return Session.Query<TEntity>().SingleOrDefaultAsync(predicate, token);
    }
    
    /// <inheritdoc />
    public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool exact, CancellationToken token = default)
    {
        return Session.Query<TEntity>().Where(predicate, exact).SingleOrDefaultAsync(token);
    }

    /// <inheritdoc />
    public Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().SingleOrDefaultAsync(predicate, token);
    }

    /// <inheritdoc />
    public Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, bool exact, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().Where(predicate, exact).SingleOrDefaultAsync(token);
    }

    /// <inheritdoc />
    public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
    {
        return Session.Query<TEntity>().Where(predicate).ToListAsync(token);
    }

    /// <inheritdoc />
    public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, bool exact, CancellationToken token = default)
    {
        return Session.Query<TEntity>().Where(predicate, exact).ToListAsync(token);
    }

    /// <inheritdoc />
    public Task<List<T>> WhereAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().Where(predicate).ToListAsync(token);
    }

    /// <inheritdoc />
    public Task<List<T>> WhereAsync<T>(Expression<Func<T, bool>> predicate, bool exact, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().Where(predicate, exact).ToListAsync(token);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
    {
        return Session.Query<TEntity>().Where(predicate).CountAsync(token);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, bool exact, CancellationToken token = default)
    {
        return Session.Query<TEntity>().Where(predicate, exact).CountAsync(token);
    }

    /// <inheritdoc />
    public Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().Where(predicate).CountAsync(token);
    }

    /// <inheritdoc />
    public Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate, bool exact, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().Where(predicate, exact).CountAsync(token);
    }

    /// <inheritdoc />
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
    {
        return Session.Query<TEntity>().Where(predicate).AnyAsync(token);
    }

    /// <inheritdoc />
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, bool exact, CancellationToken token = default)
    {
        return Session.Query<TEntity>().Where(predicate, exact).AnyAsync(token);
    }

    /// <inheritdoc />
    public Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().Where(predicate).AnyAsync(token);
    }

    /// <inheritdoc />
    public Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate, bool exact, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().Where(predicate, exact).AnyAsync(token);
    }

    public Task<List<TEntity>> SearchAsync(Expression<Func<TEntity, object>> fieldSelector, string searchTerms, CancellationToken token = default)
    {
        return Session.Query<TEntity>().Search(fieldSelector, searchTerms).ToListAsync(token);
    }

    public Task<List<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> fieldSelector, string searchTerms, CancellationToken token = default)
    {
        return Session.Query<TEntity>().Where(predicate).Search(fieldSelector, searchTerms).ToListAsync(token);
    }

    public Task<List<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate, bool exact, Expression<Func<TEntity, object>> fieldSelector, string searchTerms, CancellationToken token = default)
    {
        return Session.Query<TEntity>().Where(predicate, exact).Search(fieldSelector, searchTerms).ToListAsync(token);
    }

    public Task<List<T>> SearchAsync<T>(Expression<Func<T, object>> fieldSelector, string searchTerms, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().Search(fieldSelector, searchTerms).ToListAsync(token);
    }

    public Task<List<T>> SearchAsync<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> fieldSelector, string searchTerms, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().Where(predicate).Search(fieldSelector, searchTerms).ToListAsync(token);
    }

    public Task<List<T>> SearchAsync<T>(Expression<Func<T, bool>> predicate, bool exact, Expression<Func<T, object>> fieldSelector, string searchTerms, CancellationToken token = default) where T : TEntity
    {
        return Session.Query<T>().Where(predicate, exact).Search(fieldSelector, searchTerms).ToListAsync(token);
    }
}