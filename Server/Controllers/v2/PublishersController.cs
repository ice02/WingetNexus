using Microsoft.AspNetCore.Mvc;
using WingetNexus.Data.DataStores;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> GetPublishers(string? filter, int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and pageSize must be greater than 0.");
            }

            var publishers = await _publisherDataStore.GetAllPublishersAsync(filter, page, pageSize);

            if (publishers == null || !publishers.Any())
            {
                return NoContent();
            }

            return Ok(publishers);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            var publisher = await _publisherDataStore.GetPublisherByIdAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }
            await _publisherDataStore.DeletePublisherAsync(id);
            return NoContent();
        }
    }
}