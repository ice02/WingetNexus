using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;
using Version = WingetNexus.Shared.Models.Entities.Version;

namespace WingetNexus.Data.DataStores
{
    public class WingetAppDatastore : IWingetAppDatastore
    {
        private readonly WingetNexusContext _context;
        private readonly ILogger<WingetAppDatastore> _logger;
        private readonly IMapper _mapper;

        private readonly IVersionDatastore _versionDatastore;
        private readonly IPublisherDataStore _publisherDataStore;

        public WingetAppDatastore(WingetNexusContext context, ILogger<WingetAppDatastore> logger, IMapper mapper, 
            IVersionDatastore versionDatastore, 
            IPublisherDataStore publisherDataStore)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _versionDatastore = versionDatastore;
            _publisherDataStore = publisherDataStore;
        }

        public async Task<ApplicationDto> CreateApplicationAsync(ApplicationDto application)
        {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            try
            {
                //TODO: replace by calling publisher datastore
                var publisher = await _context.Publishers.FirstOrDefaultAsync(
                    p => p.Name.ToUpper() == application.Publisher.Name.ToUpper());

                if (publisher == null)
                {
                    publisher = new Publisher
                    {
                        Name = application.Publisher.Name
                    };
                    var resultPublisher = _context.Publishers.Add(publisher);
                }

                var newApp = _mapper.Map<Application>(application);
                newApp.Publisher = publisher;

                var resultApp = _context.Applications.Add(newApp);
                await _context.SaveChangesAsync();

                var res =  _mapper.Map<ApplicationDto>(resultApp.Entity);

                return res;
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "Failed to create application {0} / {1}", application.Publisher, application.Name);
                throw;
            }
        }

        private int AddContentToDB(ContentFiles content)
        {
            try
            {
                var result = _context.ContentFiles.Add(content);
                _context.SaveChanges();
                return result.Entity.Id;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Failed to add content to DB");
                return -1;
            }
            
        }

        public async Task<ApplicationDto> GetApplicationByIdAsync(int id)
        {
            try
            {
                var application = await _context.Applications
                .Include(a => a.Publisher)
                .Include(a => a.Versions) // Add versions as navigation property
                .FirstOrDefaultAsync(a => a.Id == id);
                if (application == null)
                {
                    throw new KeyNotFoundException($"Application with id {id} not found.");
                }
                return _mapper.Map<ApplicationDto>(application);
            }
            catch (Exception)
            {
                _logger.LogError("Failed to get application by id {0}", id);
                throw;
            }
        }

        public async Task<ApplicationDto> GetApplicationByUidAsync(string uid)
        {
            try
            {
                var application = await _context.Applications
                .Include(a => a.Publisher)
                .Include(a => a.Versions) // Add versions as navigation property
                .FirstOrDefaultAsync(a => a.PackageIdentifier == uid);
                if (application == null)
                {
                    throw new KeyNotFoundException($"Application with PackageIdentifier {uid} not found.");
                }
                return _mapper.Map<ApplicationDto>(application);
            }
            catch (Exception)
            {
                _logger.LogError("Failed to get application by uid {0}", uid);
                throw;
            }
            
        }

        public async Task<IEnumerable<ApplicationDto>> GetAllApplicationsAsync(FilterDto filterDto)
        {
            try
            {
                var query = _context.Applications
                .Include(a => a.Publisher)
                .Include(a => a.Versions) // Add versions as navigation property
                .AsQueryable();

                if (filterDto != null && !string.IsNullOrEmpty(filterDto.Filter))
                {
                    query = query.Where(a => a.Name.Contains(filterDto.Filter) || a.Publisher.Name.Contains(filterDto.Filter));
                }

                if (filterDto != null && !string.IsNullOrEmpty(filterDto.OrderBy))
                {
                    query = filterDto.OrderBy switch
                    {
                        "name" => query.OrderBy(a => a.Name),
                        "publisher" => query.OrderBy(a => a.Publisher.Name),
                        _ => query
                    };
                }

                if (filterDto != null && !string.IsNullOrEmpty(filterDto.OrderWay))
                {
                    query = filterDto.OrderWay switch
                    {
                        "ASC" => query.OrderBy(a => a.Name),
                        "DESC" => query.OrderByDescending(a => a.Name),
                        _ => query
                    };
                }

                if (filterDto == null && filterDto.PageNumber.HasValue && filterDto.PageSize.HasValue)
                {
                    query = query
                        .Skip((filterDto.PageNumber.Value ) * filterDto.PageSize.Value)
                        .Take(filterDto.PageSize.Value);
                }

                var result = await query.ToListAsync();



                return _mapper.Map<List<ApplicationDto>>(result);
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "Error during get all applications");
                throw;
            }
            
        }

        public async Task<ApplicationDto> UpdateApplicationAsync(ApplicationDto application)
        {
            try
            {
                var uptApp = _context.Applications.Update(_mapper.Map<Application>(application));
                
                await _context.SaveChangesAsync();

                return _mapper.Map<ApplicationDto>(uptApp);
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "Failed to update application {0} / {1}", application.Publisher, application.Name);
                throw;
            }
            
        }

        public async Task DeleteApplicationAsync(string packageIdentifier)
        {
            try
            {
                var application = await _context.Applications.FirstOrDefaultAsync(p => p.PackageIdentifier == packageIdentifier);
                if (application != null)
                {
                    _context.Applications.Remove(application);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "Failed to delete application by uid {0}", packageIdentifier);
                throw;
            }
        }

        public async Task<ApplicationDto> GetApplicationByPublisherAndNameAsync(int publisherId, string name)
        {
            try
            {
                return _mapper.Map<ApplicationDto>(await _context.Applications
                .FirstOrDefaultAsync(
                a => a.PublisherId == publisherId &&
                    a.Name.Equals(name, StringComparison.OrdinalIgnoreCase))!);
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "Failed to get application by publisherId {0} and name {1}", publisherId, name);
                throw;
            }
            
        }

        public async Task<ApplicationDto> GetApplicationByPublisherAndNameAsync(string publisherName, string name)
        {
            try
            {
                return _mapper.Map<ApplicationDto>(await _context.Applications
                .FirstOrDefaultAsync(
                a => a.Publisher.Name.Equals(publisherName, StringComparison.OrdinalIgnoreCase) &&
                    a.Name.Equals(name, StringComparison.OrdinalIgnoreCase))!);
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "Failed to get application by publisher name {0} and name {1}", publisherName, name);
                throw;
            }
        }

        public async Task<int> GetApplicationCountAsync(string? filter = null)
        {
            return await _context.Applications
                .Where(a => string.IsNullOrEmpty(filter) || a.Name.Contains(filter) || a.Publisher.Name.Contains(filter))
                .CountAsync();
        }

        public async Task<ApplicationDto> GetApplicationByPackageIdentifierAsync(string packageIdentifier)
        {
            try
            {
                return _mapper.Map<ApplicationDto>(await _context.Applications
                .Include(a => a.Publisher)
                .Include(a => a.Versions) // Add versions as navigation property
                .Include("Versions.DefaultLocaleContent")
                .FirstOrDefaultAsync(a => a.PackageIdentifier == packageIdentifier));
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "Failed to get application by package identifier {0}", packageIdentifier);
                throw;
            }
            
        }

        public async Task<int> GetAppCountByIdentyifierAsync(string packageIdentifier)
        {
            try
            {
                return await _context.Applications.CountAsync(p => p.PackageIdentifier == packageIdentifier);

            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "Failed to get application count by package identifier {0}", packageIdentifier);
                throw;
            }
        }

        public Task<List<string>> SearchPublishersAsync(string search)
        {
            return _context.Publishers
                .Where(p => p.Name.Contains(search))
                .Select(p => p.Name)
                .ToListAsync();
        }
    }
}
