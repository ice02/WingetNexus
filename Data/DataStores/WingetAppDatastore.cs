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
    public class WingetAppDatastore : IWingetAppDatastore
    {
        private readonly WingetNexusCtx _context;
        private readonly ILogger<WingetAppDatastore> _logger;

        public WingetAppDatastore(WingetNexusCtx context, ILogger<WingetAppDatastore> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Application> CreateApplicationAsync(Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<Application> GetApplicationByIdAsync(int id)
        {
            var application = await _context.Applications
                .Include(a => a.Publisher)
                .Include(a => a.Versions) // Add versions as navigation property
                .FirstOrDefaultAsync(a => a.Id == id);
            if (application == null)
            {
                throw new KeyNotFoundException($"Application with id {id} not found.");
            }
            return application;
        }

        public async Task<Application> GetApplicationByUidAsync(string uid)
        {
            var application = await _context.Applications
                .Include(a => a.Publisher)
                .Include(a => a.Versions) // Add versions as navigation property
                .FirstOrDefaultAsync(a => a.PackageIdentifier == uid);
            if (application == null)
            {
                throw new KeyNotFoundException($"Application with PackageIdentifier {uid} not found.");
            }
            return application;
        }

        public async Task<IEnumerable<Application>> GetAllApplicationsAsync(
            string? filter = null, int? pageNumber = 1, int? pageSize = 10, string? orderBy = null, string? orderway="DESC")
        {
            var query = _context.Applications
                .Include(a => a.Publisher)
                .Include(a => a.Versions) // Add versions as navigation property
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(a => a.Name.Contains(filter) || a.Publisher.Name.Contains(filter));
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                query = orderBy switch
                {
                    "name" => query.OrderBy(a => a.Name),
                    "publisher" => query.OrderBy(a => a.Publisher.Name),
                    _ => query
                };
            }

            if (!string.IsNullOrEmpty(orderway))
            {
                query = orderway switch
                {
                    "ASC" => query.OrderBy(a => a.Name),
                    "DESC" => query.OrderByDescending(a => a.Name),
                    _ => query
                };
            }

            return await query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value).ToListAsync();
        }

        public async Task<Application> UpdateApplicationAsync(Application application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task DeleteApplicationAsync(string packageIdentifier)
        {
            var application = await _context.Applications.FirstOrDefaultAsync(p=>p.PackageIdentifier == packageIdentifier);
            if (application != null)
            {
                _context.Applications.Remove(application);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Application> GetApplicationByPublisherAndNameAsync(int publisherId, string name)
        {
            return await _context.Applications
                .FirstOrDefaultAsync(a => a.PublisherId == publisherId && a.Name.Equals(name, StringComparison.OrdinalIgnoreCase))!;
        }

        public async Task<Application> GetApplicationByPublisherAndNameAsync(string publisherName, string name)
        {
            return await _context.Applications
                .FirstOrDefaultAsync(a => a.Name == publisherName && a.Name.Equals(name, StringComparison.OrdinalIgnoreCase))!;
        }

        public Task<int> GetApplicationCountAsync(string? filter = null)
        {
            return _context.Applications
                .Where(a => string.IsNullOrEmpty(filter) || a.Name.Contains(filter) || a.Publisher.Name.Contains(filter))
                .CountAsync();
        }

        public Task<Application> GetApplicationByPackageIdentifierAsync(string packageIdentifier)
        {
            return _context.Applications
                .Include(a => a.Publisher)
                .Include(a => a.Versions) // Add versions as navigation property
                .FirstOrDefaultAsync(a => a.PackageIdentifier == packageIdentifier);
        }
    }
}
