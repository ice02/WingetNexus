using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Entities;

namespace WingetNexus.Data.DataStores
{
    public class PublisherDataStore : IPublisherDataStore
    {
        private readonly WingetNexusCtx _context;
        private readonly ILogger<PublisherDataStore> _logger;

        public PublisherDataStore(WingetNexusCtx context, ILogger<PublisherDataStore> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Publisher>> SearchPublishersAsync(string query)
        {
            return await _context.Publishers
                .Where(p => p.Name.Contains(query))
                .ToListAsync();
        }

        public async Task<Publisher> CreatePublisherAsync(Publisher publisher)
        {
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();
            return publisher;
        }

        public async Task<Publisher> GetPublisherByIdAsync(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                throw new KeyNotFoundException($"Publisher with id {id} not found.");
            }
            return publisher;
        }

        public async Task<IEnumerable<Publisher>> GetAllPublishersAsync(string? nameFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Publishers.AsQueryable();

            if (!string.IsNullOrEmpty(nameFilter))
            {
                query = query.Where(p => p.Name.Contains(nameFilter));
            }

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Publisher> UpdatePublisherAsync(Publisher publisher)
        {
            _context.Publishers.Update(publisher);
            await _context.SaveChangesAsync();
            return publisher;
        }

        public async Task DeletePublisherAsync(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher != null)
            {
                _context.Publishers.Remove(publisher);
                await _context.SaveChangesAsync();
            }
        }
    }
}
