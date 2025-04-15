using AutoMapper;
using StealAllTheCats.Dtos;
using StealAllTheCats.Repositories;

namespace StealAllTheCats.Services
{
    /// <summary>
    /// Service responsible for querying cat data, converting database entities to DTOs.
    /// </summary>
    public class CatQueryService : ICatQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CatQueryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a cat by its unique identifier and maps the result to DTO.
        /// </summary>
        public async Task<CatDto?> GetCatByIdAsync(int id)
        {
            var cat = await _unitOfWork.Cats.GetByIdAsync(id);
            if (cat == null)
                return null;

            return _mapper.Map<CatDto>(cat);
        }

        /// <summary>
        /// Retrieves a paged list of cats, optionally filtered by a tag, and maps the results to DTOs.
        /// </summary>
        public async Task<PagedResult<CatDto>> GetCatsAsync(int page, int pageSize, string? tag = null)
        {
            var (cats, totalCount) = await _unitOfWork.Cats.GetCatsPagedAsync(page, pageSize, tag);

            var catDtos = _mapper.Map<List<CatDto>>(cats);

            return new PagedResult<CatDto>
            {
                Items = catDtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
