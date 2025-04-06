using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;

namespace WingetNexus.Shared.Mappers
{
    public class PublisherMapper
    {
        public static PublisherDto ToDto(Publisher publisher)
        {
            return new PublisherDto
            {
                Id = publisher.Id,
                Name = publisher.Name,
                GitHubUrl = publisher.GitHubUrl,
                CreatedDate = publisher.CreatedDate,
                ModifiedDate = publisher.ModifiedDate
            };
        }

        public static Publisher ToEntity(PublisherDto publisherDto)
        {
            return new Publisher
            {
                Id = publisherDto.Id,
                Name = publisherDto.Name,
                GitHubUrl = publisherDto.GitHubUrl,
                CreatedDate = publisherDto.CreatedDate,
                ModifiedDate = publisherDto.ModifiedDate
            };
        }
    }
}
