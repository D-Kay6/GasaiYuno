using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public readonly DataContext Context;

        /// <summary>
        /// Creates a new <see cref="Repository{T}"/>.
        /// </summary>
        /// <param name="context">The context that will be used.</param>
        public Repository(DataContext context)
        {
            Context = context;
        }

        /// <inheritdoc/>
        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        /// <inheritdoc/>
        public void AddRange(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }

        /// <inheritdoc/>
        public async Task AddAsync(T entity)
        {
            await Context.Set<T>().AddAsync(entity).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await Context.Set<T>().AddRangeAsync(entities).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public void Update(T entity)
        {
            Context.Set<T>().Update(entity);
        }

        /// <inheritdoc/>
        public void UpdateRange(IEnumerable<T> entities)
        {
            Context.Set<T>().UpdateRange(entities);
        }

        /// <inheritdoc/>
        public void Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        /// <inheritdoc/>
        public void RemoveRange(IEnumerable<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }

        /// <inheritdoc/>
        public IEnumerable<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        /// <inheritdoc/>
        public Task<List<T>> GetAllAsync()
        {
            return Context.Set<T>().ToListAsync();
        }

        /// <inheritdoc/>
        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Where(predicate);
        }

        /// <inheritdoc/>
        public Task<List<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Where(predicate).ToListAsync();
        }

        /// <inheritdoc/>
        public int Count(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Count(predicate);
        }

        /// <inheritdoc/>
        public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().CountAsync(predicate);
        }

        /// <inheritdoc/>
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Any(predicate);
        }

        /// <inheritdoc/>
        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().AnyAsync(predicate);
        }
    }
}