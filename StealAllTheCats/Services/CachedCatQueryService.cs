using Azure;
using Microsoft.Extensions.Caching.Memory;
using StealAllTheCats.Dtos;
using StealAllTheCats.Infastracture;

namespace StealAllTheCats.Services
{
    /// <summary>
    /// A caching decorator for ICatQueryService that uses an in-memory cache to store results.
    /// </summary>
    public class CachedCatQueryService : ICatQueryService
    {
        private readonly ICatQueryService _inner;
        private readonly IMemoryCache _cache;
        private readonly ICacheTokenProvider _tokenProvider;
        private readonly ILogger<CachedCatQueryService> _logger;

        public CachedCatQueryService(ICatQueryService inner, IMemoryCache cache, ICacheTokenProvider tokenProvider, ILogger<CachedCatQueryService> logger)
        {
            _inner = inner;
            _cache = cache;
            _tokenProvider = tokenProvider;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a paged list of cats from the inner service and caches the result.
        /// </summary>
        /// <remarks>
        /// This method uses a composite cache key based on page, pageSize, and tag. Cache entries expire absolutely after 5 minutes
        /// and via sliding expiration after 2 minutes. A cancellation token is attached to allow global cache invalidation
        /// (for example, when new cats are fetched).
        /// </remarks>
        public async Task<PagedResult<CatDto>> GetCatsAsync(int page, int pageSize, string? tag = null)
        {
            var key = $"cats:page={page}:size={pageSize}:tag={tag ?? "all"}";
            _logger.LogInformation("Checking cache for key: {CacheKey}", key);

            var result =  await _cache.GetOrCreateAsync(key, entry =>
            {
                _logger.LogInformation("Key: {CacheKey} not found in cache.", key);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                entry.SlidingExpiration = TimeSpan.FromMinutes(2);
                entry.SetSize(1);
                entry.AddExpirationToken(_tokenProvider.GetToken());
                return _inner.GetCatsAsync(page, pageSize, tag);
            });

            _logger.LogInformation("Returning cached data for key: {CacheKey}", key);
            return result;
        }

        /// <summary>
        /// Retrieves a single cat by its identifier from the inner service and caches the result.
        /// </summary>
        /// /// <remarks>
        /// This method caches individual cat lookups using a key based solely on the cat's id. Since cat data is immutable,
        /// a longer cache duration is used (absolute expiration of 30 minutes and sliding expiration of 10 minutes) and no cancellation token is added.
        /// </remarks>
        public async Task<CatDto?> GetCatByIdAsync(int id)
        {
            var key = $"cats:id={id}";
            _logger.LogInformation("Checking cache for single cat with key: {CacheKey}", key);
            var result =  await _cache.GetOrCreateAsync(key, entry =>
            {
                _logger.LogInformation("Key: {CacheKey} not found in cache.", key);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                entry.SetSize(1);
                return _inner.GetCatByIdAsync(id);
            });

            _logger.LogInformation("Returning cached data for key: {CacheKey}", key);
            return result;
        }
    }
}
