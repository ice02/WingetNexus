using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;

namespace WingetNexus.Shared.Mappers.AutoMapperProfiles
{
    internal class PublisherProfile : Profile
    {
        public PublisherProfile()
        {
            CreateMap<Publisher, PublisherDto>().ReverseMap();
        }
    }
}
