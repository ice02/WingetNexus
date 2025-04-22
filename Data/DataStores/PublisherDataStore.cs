using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;

namespace WingetNexus.Data.DataStores
{
    public class PublisherDataStore : IPublisherDataStore
    {
        private readonly WingetNexusContext _context;
        private readonly ILogger<PublisherDataStore> _logger;
        private readonly IMapper _mapper;

        public PublisherDataStore(WingetNexusContext context, ILogger<PublisherDataStore> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PublisherDto>> SearchPublishersAsync(string query)
        {
            try
            {
                var publishers = await _context.Publishers
                    .Where(p => p.Name.Contains(query))
                    .ToListAsync();

                return _mapper.Map<IEnumerable<PublisherDto>>(publishers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching publishers with query: {Query}", query);
                throw;
            }
        }

        public async Task<PublisherDto> CreatePublisherAsync(PublisherDto publisherDto)
        {
            try
            {
                var publisher = _mapper.Map<Publisher>(publisherDto);
                _context.Publishers.Add(publisher);
                await _context.SaveChangesAsync();

                return _mapper.Map<PublisherDto>(publisher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a publisher");
                throw;
            }
        }

        public async Task<PublisherDto> GetPublisherByIdAsync(int id)
        {
            try
            {
                var publisher = await _context.Publishers.FindAsync(id);
                if (publisher == null)
                {
                    throw new KeyNotFoundException($"Publisher with id {id} not found.");
                }

                return _mapper.Map<PublisherDto>(publisher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving publisher with id: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<PublisherDto>> GetAllPublishersAsync(string? nameFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Publishers.AsQueryable();

                if (!string.IsNullOrEmpty(nameFilter))
                {
                    query = query.Where(p => p.Name.Contains(nameFilter));
                }

                var publishers = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<PublisherDto>>(publishers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all publishers");
                throw;
            }
        }

        public async Task<PublisherDto> UpdatePublisherAsync(PublisherDto publisherDto)
        {
            try
            {
                var publisher = _mapper.Map<Publisher>(publisherDto);
                _context.Publishers.Update(publisher);
                await _context.SaveChangesAsync();

                return _mapper.Map<PublisherDto>(publisher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating a publisher");
                throw;
            }
        }

        public async Task DeletePublisherAsync(int id)
        {
            try
            {
                var publisher = await _context.Publishers.FindAsync(id);
                if (publisher != null)
                {
                    _context.Publishers.Remove(publisher);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting publisher with id: {Id}", id);
                throw;
            }
        }

        public async Task<PublisherDto?> GetPublisherByNameAsync(string name)
        {
            try
            {
                var publisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Name == name);
                return publisher != null ? _mapper.Map<PublisherDto>(publisher) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving publisher with name: {Name}", name);
                throw;
            }
        }
    }
}
