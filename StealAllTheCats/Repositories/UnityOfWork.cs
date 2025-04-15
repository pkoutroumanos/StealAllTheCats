using StealAllTheCats.Data;

namespace StealAllTheCats.Repositories
{
    /// <summary>
    /// Implements the Unit of Work pattern, allowing atomic operations across multiple repositories.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        /// <summary>
        /// Gets the repository for accessing cat entities.
        /// </summary>
        public ICatRepository Cats { get; }
        /// <summary>
        /// Gets the repository for accessing tag entities.
        /// </summary>
        public ITagRepository Tags { get; }

        public UnitOfWork(AppDbContext context, ICatRepository cats, ITagRepository tags)
        {
            _context = context;
            Cats = cats;
            Tags = tags;
        }

        /// <summary>
        /// Commits all changes made in the context to the underlying database.
        /// </summary>
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
