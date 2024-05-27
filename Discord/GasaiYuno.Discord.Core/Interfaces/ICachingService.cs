namespace GasaiYuno.Discord.Core.Interfaces;

public interface ICachingService
{
    /// <summary>
    /// Get an item from the cache.
    /// </summary>
    /// <param name="key">The key of the item.</param>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <returns>An awaitable <see cref="Task{T}"/>, containing the cached instance of <see cref="T"/>, or its default value.</returns>
    Task<T> GetAsync<T>(string key);

    /// <summary>
    /// Add an item to the cache.
    /// </summary>
    /// <param name="item">The item to cache.</param>
    /// <param name="retention">The optional retention time of the item.</param>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <returns>An awaitable <see cref="Task{T}"/>, containing the cache key for the item.</returns>
    Task<string> AddAsync<T>(T item, TimeSpan? retention = null);

    /// <summary>
    /// Remove an item from the cache.
    /// </summary>
    /// <param name="key">The key of the item.</param>
    /// <returns>An awaitable <see cref="Task"/></returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// Get an item from the cache, or add it if it doesn't exist.
    /// </summary>
    /// <param name="key">The key of the item.</param>
    /// <param name="factory">The factory method to create the item.</param>
    /// <param name="retention">The optional retention time of the item.</param>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <returns>An awaitable <see cref="Task{T}"/>, containing the cached instance of <see cref="T"/>, or its default value.</returns>
    Task<T> GetOrAddAsync<T>(string key, Func<T> factory, TimeSpan? retention = null);

    /// <summary>
    /// Get an item from the cache, or add it if it doesn't exist.
    /// </summary>
    /// <param name="key">The key of the item.</param>
    /// <param name="factory">The factory method to create the item.</param>
    /// <param name="retention">The optional retention time of the item.</param>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <returns>An awaitable <see cref="Task{T}"/>, containing the cached instance of <see cref="T"/>, or its default value.</returns>
    Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? retention = null);
}