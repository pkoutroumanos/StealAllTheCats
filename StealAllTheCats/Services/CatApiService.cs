using Microsoft.Extensions.Options;
using StealAllTheCats.Infastracture;
using StealAllTheCats.Models;
using StealAllTheCats.Options;
using StealAllTheCats.Repositories;
using System.Net.Http.Json;

namespace StealAllTheCats.Services
{
    /// <summary>
    /// Service responsible for fetching cat data from an external API and saving it to the database.
    /// </summary>
    public class CatApiService : ICatApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheTokenProvider _tokenProvider;
        private readonly ILogger<CatApiService> _logger;
        private readonly CatApiOptions _catApiOptions;

        public CatApiService(HttpClient httpClient, IUnitOfWork unitOfWork, ICacheTokenProvider tokenProvider, ILogger<CatApiService> logger, IOptions<CatApiOptions> options)
        {
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
            _tokenProvider = tokenProvider;
            _logger = logger;
            _catApiOptions = options.Value;
        }

        /// <summary>
        /// Fetches a set number of cat records from the external API and saves them to the database.
        /// </summary>
        /// <remarks>
        /// For each cat fetched, the service checks if the cat already exists in the database (using the external API id). 
        /// If the cat exists or if the cat has no breed information (which is used to generate tags), it is skipped.
        /// Otherwise, the cat image is downloaded and the breeds' temperaments are split into individual tags that are saved along with the cat.
        /// Once all cats are processed, the unit of work commits the changes and the cache token is expired 
        /// to invalidate any cached query results that share the token(The results with key page-pagesize-tag since it is expected to be affected by the insert
        /// Records with key the id are not invalidated since no update is done).
        /// </remarks>
        public async Task FetchCatsAsync(int count)
        {
            try
            {
                var cats = await _httpClient.GetFromJsonAsync<List<CatApiResponse>>($"images/search?limit={count}&has_breeds=1&api_key={_catApiOptions.ApiKey}");
                if (cats == null)
                {
                    _logger.LogWarning("No cats were returned from the external API.");
                    return;
                }
                var tagsAlreadyProccessed = new Dictionary<string, TagEntity>();

                foreach (var cat in cats!)
                {
                    if (string.IsNullOrEmpty(cat.Id))
                    {
                        _logger.LogWarning("Cat API response contained a null or empty Id; skipping this entry.");
                        continue;
                    }

                    var exists = await _unitOfWork.Cats.ExistsByCatApiIdAsync(cat.Id);
                    if (exists) 
                    {
                        _logger.LogDebug("Cat with API id {CatId} already exists, skipping.", cat.Id);
                        continue;
                    }

                    if (cat.Breeds == null || !cat.Breeds.Any())
                    {
                        _logger.LogDebug("Cat with API id {CatId} has no breeds, skipping.", cat.Id);
                        continue;
                    }

                    var imageBytes = await _httpClient.GetByteArrayAsync(cat.Url);

                    var catEntity = new CatEntity
                    {
                        CatId = cat.Id,
                        Width = cat.Width,
                        Height = cat.Height,
                        Image = imageBytes,
                        Created = DateTime.UtcNow,
                        CatTags = new List<CatTag>()
                    };

                    foreach (var breed in cat.Breeds)
                    {
                        var tags = GetTagsOfBreed(breed);

                        foreach (var tag in tags)
                        {
                            var tagEntity = await GetOrCreateTagEntityAsync(tag, tagsAlreadyProccessed);                            
                            catEntity.CatTags.Add(new CatTag { TagEntity = tagEntity });
                        }
                    }
                    await _unitOfWork.Cats.AddAsync(catEntity);
                }
                await _unitOfWork.CompleteAsync();
                _tokenProvider.ExpireToken();
                _logger.LogInformation("Successfully fetched and saved {Count} cats.", count);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occurred while fetching cats from the external API.");
                throw;
            }
        }

        private IEnumerable<string> GetTagsOfBreed(Breed breed)
        {
            return (breed.Temperament ?? string.Empty)
                        .Split(',')
                        .Select(t => t.Trim())
                        .Where(t => !string.IsNullOrEmpty(t))
                        .Distinct();
        }

        private async Task<TagEntity> GetOrCreateTagEntityAsync(string tag, Dictionary<string, TagEntity> processedTags)
        {
            string normalizedTag = tag.Trim().ToLowerInvariant().Replace(" ", "");
            if (processedTags.Keys.Contains(normalizedTag))
                return processedTags[normalizedTag];

            var tagEntity = await _unitOfWork.Tags.GetByNameAsync(tag)
                    ?? new TagEntity { Name = tag, Created = DateTime.UtcNow };

            if (tagEntity.Id == 0)
                await _unitOfWork.Tags.AddAsync(tagEntity);

            processedTags.Add(normalizedTag, tagEntity);
            return tagEntity; 
        }

        // Private classes to model the API response.
        private class CatApiResponse
        {
            public string? Id { get; set; }
            public string? Url { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public List<Breed>? Breeds { get; set; }
        }

        private class Breed
        {
            public string? Temperament { get; set; }
        }
    }
}