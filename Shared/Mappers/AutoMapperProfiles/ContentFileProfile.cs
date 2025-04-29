using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;

namespace WingetNexus.Shared.Mappers.AutoMapperProfiles
{
    public class ContentFileProfile : Profile
    {
        public ContentFileProfile()
        {
            CreateMap<ContentFiles, ContentFileDto>().ReverseMap();
        }
    }
}
