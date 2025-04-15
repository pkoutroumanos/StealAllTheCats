using StealAllTheCats.Dtos;

namespace StealAllTheCats.Services
{
    public interface ICatQueryService
    {
        Task<CatDto?> GetCatByIdAsync(int id);
        Task<PagedResult<CatDto>> GetCatsAsync(int page, int pageSize, string? tag = null);
    }
}
