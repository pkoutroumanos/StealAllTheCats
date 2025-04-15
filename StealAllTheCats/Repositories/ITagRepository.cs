using StealAllTheCats.Models;

namespace StealAllTheCats.Repositories
{
    public interface ITagRepository
    {
        Task<TagEntity?> GetByNameAsync(string name);
        Task AddAsync(TagEntity tag);
    }
}
