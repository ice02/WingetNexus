using AutoMapper;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;
using Version = WingetNexus.Shared.Models.Entities.Version;

namespace WingetNexus.Shared.Mappers.AutoMapperProfiles
{
    public class VersionProfile : Profile
    {
        public VersionProfile()
        {
            CreateMap<Version, VersionDto>().ReverseMap();
        }
    }
}
