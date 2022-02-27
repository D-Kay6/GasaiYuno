using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Domain.Persistence.Repositories
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Adds an object to the database.
        /// </summary>
        /// <param name="entity">Entity that will be added to the database.</param>
        void Add(T entity);

        /// <summary>
        /// Adds a <see cref="List{T}"/> of objects to the database. 
        /// </summary>
        /// <param name="entities">Entities that will be added to the database.</param>
        void AddRange(IEnumerable<T> entities);

        /// <summary>
        /// Adds an object to the database asynchronously. 
        /// </summary>
        /// <param name="entity">Entity that will be added to the database.</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Adds a <see cref="List{T}"/> of objects to the database asynchronously. 
        /// </summary>
        /// <param name="entities">Entities that will be added to the database.</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an object from the database.
        /// </summary>
        /// <param name="entity">The entity that will be updated in the database.</param>
        void Update(T entity);

        /// <summary>
        /// Update a <see cref="List{T}"/> of objects from the database.
        /// </summary>
        /// <param name="entities">The entities that will be updated in the database.</param>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// Removes an object from the database.
        /// </summary>
        /// <param name="entity">The entity that will be removed from the database.</param>
        void Remove(T entity);

        /// <summary>
        /// Remove a <see cref="List{T}"/> of objects from the database.
        /// </summary>
        /// <param name="entities">The entities that will be removed from the database.</param>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Gets all the objects from the table.
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Gets all the objects from the table asynchronously.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Gets all the objects where the expression is true.
        /// </summary>
        /// <param name="predicate">The expression that will be used to find the objects.</param>
        /// <returns><see cref="IEnumerable{T}"/></returns>
        IEnumerable<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets all the objects where the expression is true asynchronously.
        /// </summary>
        /// <param name="predicate">The expression that will be used to find the objects.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
        Task<List<T>> WhereAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Count the amount of objects matching the expression.
        /// </summary>
        /// <param name="predicate">The expression that will be used to find the objects.</param>
        /// <returns><see cref="int"/></returns>
        int Count(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Count the amount of objects matching the expression asynchronously.
        /// </summary>
        /// <param name="predicate">The expression that will be used to find the objects.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="int"/></returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Check if there are any objects matching the expression.
        /// </summary>
        /// <param name="predicate">The expression that will be used to find the objects.</param>
        /// <returns><see cref="bool"/></returns>
        bool Any(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Check if there are any objects matching the expression asynchronously.
        /// </summary>
        /// <param name="predicate">The expression that will be used to find the objects.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="bool"/></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}