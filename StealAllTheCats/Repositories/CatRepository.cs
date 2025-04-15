using Microsoft.EntityFrameworkCore;
using StealAllTheCats.Data;
using StealAllTheCats.Models;

namespace StealAllTheCats.Repositories
{
    /// <summary>
    /// Provides data access methods for <see cref="CatEntity"/> objects.
    /// </summary>
    public class CatRepository : ICatRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CatRepository> _logger;

        public CatRepository(AppDbContext context, ILogger<CatRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a paged list of cat entities along with the total count, optionally filtered by a tag.
        /// </summary>
        public async Task<(List<CatEntity> Cats, int TotalCount)> GetCatsPagedAsync(int page, int pageSize, string? tagFilter = null)
        {
            try
            {
                _logger.LogInformation("Fetching paged cats. Page: {Page}, PageSize: {PageSize}, TagFilter: {TagFilter}", page, pageSize, tagFilter);
                var query = _context.Cats
                    .Include(c => c.CatTags)
                    .ThenInclude(ct => ct.TagEntity)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(tagFilter))
                {
                    query = query.Where(c => c.CatTags.Any(ct => ct.TagEntity.Name == tagFilter));
                }

                var totalCount = await query.CountAsync();

                var cats = await query
                    .OrderByDescending(c => c.Created)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                _logger.LogInformation("Returning {Count} cats (TotalCount: {TotalCount}).", cats.Count, totalCount);
                return (cats, totalCount);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error retrieving paged cats.");
                throw;
            }

        }

        /// <summary>
        /// Retrieves a cat entity by its unique identifier.
        /// </summary>
        public async Task<CatEntity?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching cat with Id {Id}.", id);
                return await _context.Cats
                    .Include(c => c.CatTags)
                        .ThenInclude(ct => ct.TagEntity)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cat by Id {Id}.", id);
                throw;
            }

        }

        /// <summary>
        /// Checks whether a cat entity exists based on its API-provided identifier.
        /// </summary>
        public async Task<bool> ExistsByCatApiIdAsync(string catApiId)
        {
            try
            {
                _logger.LogDebug("Checking existence of cat with API Id {CatApiId}.", catApiId);
                return await _context.Cats.AnyAsync(c => c.CatId == catApiId);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error checking existence for cat with API Id {CatApiId}.", catApiId);
                throw;
            }            
        }

        /// <summary>
        /// Adds a new cat entity to the database context.
        /// </summary>
        public async Task AddAsync(CatEntity cat)
        {
            try
            {
                _logger.LogInformation("Adding new cat with API Id {CatApiId}.", cat.CatId);
                await _context.Cats.AddAsync(cat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new cat with API Id {CatApiId}.", cat.CatId);
                throw;
            }
        }
    }
}
