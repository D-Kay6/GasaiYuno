using System.Linq.Expressions;

namespace GasaiYuno.Persistence.Data;

public interface IRepository
{
}

public interface IRepository<TEntity> : IRepository where TEntity : IEntity
{
    /// <summary>
    /// Adds an object to the database asynchronously. 
    /// </summary>
    /// <param name="entity">Entity that will be added to the database.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/></returns>
    Task AddAsync<T>(T entity, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Adds a <see cref="List{T}"/> of objects to the database asynchronously. 
    /// </summary>
    /// <param name="entities">Entities that will be added to the database.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/></returns>
    Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken token = default) where T : TEntity;

    // /// <summary>
    // /// Updates an object from the database.
    // /// </summary>
    // /// <param name="entity">The entity that will be updated in the database.</param>
    // /// <param name="token">The cancellation token.</param>
    // void Update<T>(T entity, CancellationToken token = default) where T : TEntity;
    //     
    // /// <summary>
    // /// Update a <see cref="List{T}"/> of objects from the database.
    // /// </summary>
    // /// <param name="entities">The entities that will be updated in the database.</param>
    // /// <param name="token">The cancellation token.</param>
    // void UpdateRange<T>(IEnumerable<T> entities, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Removes an object from the database.
    /// </summary>
    /// <param name="entity">The entity that will be removed from the database.</param>
    void Remove<T>(T entity) where T : TEntity;

    /// <summary>
    /// Remove a <see cref="List{T}"/> of objects from the database.
    /// </summary>
    /// <param name="entities">The entities that will be removed from the database.</param>
    void RemoveRange<T>(IEnumerable<T> entities) where T : TEntity;

    /// <summary>
    /// Gets all the objects from the table asynchronously.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{TEntity}"/></returns>
    Task<List<TEntity>> GetAllAsync(CancellationToken token = default);

    /// <summary>
    /// Gets all the objects from the table asynchronously.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
    Task<List<T>> GetAllAsync<T>(CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Gets the first object that matches the predicate.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <typeparamref name="TEntity"/> if a match was found; Otherwise default(<typeparamref name="TEntity"/>).</returns>
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);

    /// <summary>
    /// Gets the first object that matches the predicate.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <typeparamref name="TEntity"/> if a match was found; Otherwise default(<typeparamref name="TEntity"/>).</returns>
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool exact, CancellationToken token = default);

    /// <summary>
    /// Gets the first objects where the expression is true asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <typeparamref name="T"/> if a match was found; Otherwise default(<typeparamref name="T"/>).</returns>
    Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Gets the first objects where the expression is true asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <typeparamref name="T"/> if a match was found; Otherwise default(<typeparamref name="T"/>).</returns>
    Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, bool exact, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Gets the first object that matches the predicate.
    /// Throws an exception more than one match was found.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <typeparamref name="TEntity"/> if a match was found; Otherwise default(<typeparamref name="TEntity"/>).</returns>
    Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);

    /// <summary>
    /// Gets the first object that matches the predicate.
    /// Throws an exception more than one match was found.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <typeparamref name="TEntity"/> if a match was found; Otherwise default(<typeparamref name="TEntity"/>).</returns>
    Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool exact, CancellationToken token = default);

    /// <summary>
    /// Gets the first objects where the expression is true asynchronously.
    /// Throws an exception more than one match was found.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <typeparamref name="T"/> if a match was found; Otherwise default(<typeparamref name="T"/>).</returns>
    Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Gets the first objects where the expression is true asynchronously.
    /// Throws an exception more than one match was found.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <typeparamref name="T"/> if a match was found; Otherwise default(<typeparamref name="T"/>).</returns>
    Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, bool exact, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Gets all the objects where the expression is true asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
    Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);

    /// <summary>
    /// Gets all the objects where the expression is true asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
    Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, bool exact, CancellationToken token = default);

    /// <summary>
    /// Gets all the objects where the expression is true asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
    Task<List<T>> WhereAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Gets all the objects where the expression is true asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
    Task<List<T>> WhereAsync<T>(Expression<Func<T, bool>> predicate, bool exact, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Count the amount of objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="int"/></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);

    /// <summary>
    /// Count the amount of objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="int"/></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, bool exact, CancellationToken token = default);

    /// <summary>
    /// Count the amount of objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="int"/></returns>
    Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Count the amount of objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="int"/></returns>
    Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate, bool exact, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Check if there are any objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="bool"/></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);

    /// <summary>
    /// Check if there are any objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="bool"/></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, bool exact, CancellationToken token = default);

    /// <summary>
    /// Check if there are any objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="bool"/></returns>
    Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Check if there are any objects matching the expression asynchronously.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="bool"/></returns>
    Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate, bool exact, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Perform a search for documents which fields that match the searchTerms.
    /// If there is more than a single term, each of them will be checked independently.
    /// </summary>
    /// <param name="fieldSelector">Function returning the field to search on</param>
    /// <param name="searchTerms">Field terms to search for, separated with whitespaces</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{TEntity}"/></returns>
    Task<List<TEntity>> SearchAsync(Expression<Func<TEntity, object>> fieldSelector, string searchTerms, CancellationToken token = default);

    /// <summary>
    /// Perform a search for documents which fields that match the searchTerms.
    /// If there is more than a single term, each of them will be checked independently.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="fieldSelector">Function returning the field to search on</param>
    /// <param name="searchTerms">Field terms to search for, separated with whitespaces</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{TEntity}"/></returns>
    Task<List<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> fieldSelector, string searchTerms, CancellationToken token = default);

    /// <summary>
    /// Perform a search for documents which fields that match the searchTerms.
    /// If there is more than a single term, each of them will be checked independently.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="fieldSelector">Function returning the field to search on</param>
    /// <param name="searchTerms">Field terms to search for, separated with whitespaces</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{TEntity}"/></returns>
    Task<List<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate, bool exact, Expression<Func<TEntity, object>> fieldSelector, string searchTerms, CancellationToken token = default);

    /// <summary>
    /// Perform a search for documents which fields that match the searchTerms.
    /// If there is more than a single term, each of them will be checked independently.
    /// </summary>
    /// <param name="fieldSelector">Function returning the field to search on</param>
    /// <param name="searchTerms">Field terms to search for, separated with whitespaces</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
    Task<List<T>> SearchAsync<T>(Expression<Func<T, object>> fieldSelector, string searchTerms, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Perform a search for documents which fields that match the searchTerms.
    /// If there is more than a single term, each of them will be checked independently.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="fieldSelector">Function returning the field to search on</param>
    /// <param name="searchTerms">Field terms to search for, separated with whitespaces</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
    Task<List<T>> SearchAsync<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> fieldSelector, string searchTerms, CancellationToken token = default) where T : TEntity;

    /// <summary>
    /// Perform a search for documents which fields that match the searchTerms.
    /// If there is more than a single term, each of them will be checked independently.
    /// </summary>
    /// <param name="predicate">The expression that will be used to find the objects.</param>
    /// <param name="exact">Whether to check for a match case-sensitive (<c>True</c>) or case-insensitive (<c>False</c>)</param>
    /// <param name="fieldSelector">Function returning the field to search on</param>
    /// <param name="searchTerms">Field terms to search for, separated with whitespaces</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="List{T}"/></returns>
    Task<List<T>> SearchAsync<T>(Expression<Func<T, bool>> predicate, bool exact, Expression<Func<T, object>> fieldSelector, string searchTerms, CancellationToken token = default) where T : TEntity;
}