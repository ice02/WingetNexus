using AutoMapper;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;

namespace WingetNexus.Shared.Mappers.AutoMapperProfiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Application, ApplicationDto>()
                .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher))
                .ForMember(dest => dest.Versions, opt => opt.MapFrom(src => src.Versions))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.GitHubUrl, opt => opt.MapFrom(src => src.GitHubUrl))
                .ForMember(dest => dest.PackageIdentifier, opt => opt.MapFrom(src => src.PackageIdentifier))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<ApplicationDto, Application>()
                .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher))
                .ForMember(dest => dest.Versions, opt => opt.MapFrom(src => src.Versions))
                .ForMember(dest => dest.GitHubUrl, opt => opt.MapFrom(src => src.GitHubUrl))
                .ForMember(dest => dest.PackageIdentifier, opt => opt.MapFrom(src => src.PackageIdentifier))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) // Ignore as it's set automatically
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore()); // Ignore as it's set automatically
        }
    }
}
