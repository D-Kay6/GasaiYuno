using GasaiYuno.Discord.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GasaiYuno.Discord.Services;

internal class CachingService : ICachingService
{
    private readonly IMemoryCache _cache;
    private readonly SemaphoreSlim _lock;

    public CachingService(IMemoryCache cache)
    {
        _cache = cache;
        _lock = new SemaphoreSlim(1, 1);
    }

    /// <inheritdoc />
    public Task<T> GetAsync<T>(string key)
    {
        return Task.FromResult(_cache.TryGetValue(key, out T value) ? value : default);
    }

    /// <inheritdoc />
    public async Task<string> AddAsync<T>(T item, TimeSpan? retention = null)
    {
        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            var key = Guid.NewGuid().ToString();
            _cache.Set(key, item, retention ?? TimeSpan.FromMinutes(5));
            return key;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key)
    {
        if (!_cache.TryGetValue(key, out var value))
            return;

        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            _cache.Remove(key);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<T> GetOrAddAsync<T>(string key, Func<T> factory, TimeSpan? retention = null)
    {
        if (_cache.TryGetValue(key, out T value))
            return value;

        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            value = factory();
            _cache.Set(key, value, retention ?? TimeSpan.FromMinutes(5));
            return value;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? retention = null)
    {
        if (_cache.TryGetValue(key, out T value))
            return value;

        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            value = await factory().ConfigureAwait(false);
            _cache.Set(key, value, retention ?? TimeSpan.FromMinutes(5));
            return value;
        }
        finally
        {
            _lock.Release();
        }
    }
}