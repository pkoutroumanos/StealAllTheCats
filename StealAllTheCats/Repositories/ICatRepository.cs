using StealAllTheCats.Models;

namespace StealAllTheCats.Repositories
{
    public interface ICatRepository
    {
        Task<(List<CatEntity> Cats, int TotalCount)> GetCatsPagedAsync(int page, int pageSize, string? tagFilter = null);
        Task<CatEntity?> GetByIdAsync(int id);
        Task<bool> ExistsByCatApiIdAsync(string catApiId);
        Task AddAsync(CatEntity cat);
    }
}
