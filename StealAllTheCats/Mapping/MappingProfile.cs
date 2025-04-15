using AutoMapper;
using StealAllTheCats.Dtos;
using StealAllTheCats.Models;
using System.Linq;

namespace StealAllTheCats.Mapping
{
    /// <summary>
    /// Defines AutoMapper mapping configurations for converting between domain entities and DTOs.
    /// </summary>
    /// <remarks>
    /// This mapping profile sets up the conversion from a <see cref="CatEntity"/> to a <see cref="CatDto"/>.
    /// It specifically maps the <see cref="CatDto.Tags"/> property by extracting distinct tag names from
    /// the <see cref="CatEntity.CatTags"/> collection.
    /// </remarks>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Configures the mapping between <see cref="CatEntity"/> and <see cref="CatDto"/>.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<CatEntity, CatDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(
                    src => src.CatTags.Select(ct => ct.TagEntity.Name).Distinct().ToList()
                ));
        }
    }
}
