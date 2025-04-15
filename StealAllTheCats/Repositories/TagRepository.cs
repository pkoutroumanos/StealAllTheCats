using Microsoft.EntityFrameworkCore;
using StealAllTheCats.Data;
using StealAllTheCats.Models;

namespace StealAllTheCats.Repositories
{
    /// <summary>
    /// Provides methods for accessing and modifying tag entities in the database.
    /// </summary>
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TagRepository> _logger;

        public TagRepository(AppDbContext context, ILogger<TagRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a tag entity by its name.
        /// </summary>
        public async Task<TagEntity?> GetByNameAsync(string name)
        {
            try
            {
                _logger.LogInformation("Fetching tag by name: {TagName}.", name);
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
                if (tag == null)
                    _logger.LogDebug("Tag not found for name: {TagName}.", name);
                else
                    _logger.LogInformation("Tag found for name: {TagName}.", name);

                return tag;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tag by name {TagName}.", name);
                throw;
            }
        }

        /// <summary>
        /// Adds a new tag entity to the database context.
        /// </summary>
        public async Task AddAsync(TagEntity tag)
        {
            try
            {
                _logger.LogInformation("Adding new tag: {TagName}.", tag.Name);
                await _context.Tags.AddAsync(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tag: {TagName}.", tag.Name);
                throw;
            }
        }
    }
}
