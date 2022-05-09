using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Domain.Persistence.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Adds an object to the database asynchronously. 
    /// </summary>
    /// <param name="entity">Entity that will be added to the database.</param>
    /// <returns>An awaitable <see cref="Task"/></returns>
    Task AddAsync<T>(T entity) where T : class, TEntity;
        
    /// <summary>
    /// Adds a <see cref="List{T}"/> of objects to the database asynchronously. 
    /// </summary>
    /// <param name="entities">Entities that will be added to the database.</param>
    /// <returns>An awaitable <see cref="Task"/></returns>
    Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, TEntity;
        
    /// <summary>
    /// Updates an object from the database.
    /// </summary>
    /// <param name="entity">The entity that will be updated in the database.</param>
    void Update<T>(T entity) where T : class, TEntity;
        
    /// <summary>
    /// Update a <see cref="List{T}"/> of objects from the database.
    /// </summary>
    /// <param name="entities">The entities that will be updated in the database.</param>
    void UpdateRange<T>(IEnumerable<T> entities) where T : class, TEntity;
        
    /// <summary>
    /// Removes an object from the database.
    /// </summary>
    /// <param name="entity">The entity that will be removed from the database.</param>
    void Remove<T>(T entity) where T : class, TEntity;
        
    /// <summary>
    /// Remove a <see cref="List{T}"/> of objects from the database.
    /// </summary>
    /// <param name="entities">The entities that will be removed from the database.</param>
    void RemoveRange<T>(IEnumerable<T> entities) where T : class, TEntity;

    /// <summary>
    /// Gets all the objects from the table asynchronously.
    /// </summary>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{TEntity}"/></returns>
    Task<List<TEntity>> GetAllAsync();

    /// <summary>
    /// Gets all the objects from the table asynchronously.
    /// </summary>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
    Task<List<T>> GetAllAsync<T>() where T : class, TEntity;
        
    /// <summary>
    /// Gets all the objects where the expression is true asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
    Task<List<T>> WhereAsync<T>(Expression<Func<T, bool>> predicate) where T : class, TEntity;
        
    /// <summary>
    /// Count the amount of objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="int"/></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        
    /// <summary>
    /// Count the amount of objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="int"/></returns>
    Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : class, TEntity;
        
    /// <summary>
    /// Check if there are any objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="bool"/></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        
    /// <summary>
    /// Check if there are any objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="bool"/></returns>
    Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class, TEntity;
}