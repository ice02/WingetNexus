using Microsoft.AspNetCore.Mvc;
using WingetNexus.Data.DataStores;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;
using System.Threading.Tasks;
using AutoMapper;

namespace WingetNexus.Server.Controllers.v2
{
    [ApiController]
    [Route("api/v2/publishers")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherDataStore _publisherDataStore;
        private readonly IMapper _mapper;

        public PublishersController(IPublisherDataStore publisherDataStore, IMapper mapper)
        {
            _publisherDataStore = publisherDataStore;
            _mapper = mapper;
        }

        [HttpGet("checkExists")]
        public async Task<ActionResult<PublisherDto>> CheckPublisherExists(string name)
        {
            var publisherDto = await _publisherDataStore.GetPublisherByNameAsync(name);
            if (publisherDto != null)
            {
                return Ok(publisherDto);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PublisherDto>> CreatePublisher([FromBody] PublisherDto publisherDto)
        {
            if (publisherDto == null || string.IsNullOrEmpty(publisherDto.Name))
            {
                return BadRequest("Invalid publisher data.");
            }

            var createdPublisherDto = await _publisherDataStore.CreatePublisherAsync(publisherDto);

            return CreatedAtAction(nameof(CheckPublisherExists), new { name = createdPublisherDto.Name }, createdPublisherDto);
        }
    }
}