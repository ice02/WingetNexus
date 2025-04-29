using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using WingetNexus.Shared.Mappers;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;
using Version = WingetNexus.Shared.Models.Entities.Version;

namespace WingetNexus.Data.DataStores
{
    public class VersionDatastore : IVersionDatastore
    {
        private readonly WingetNexusContext _context;
        private readonly ILogger<VersionDatastore> _logger;
        private readonly IMapper _mapper;
        //private readonly VersionMapper _versionMapper;

        public VersionDatastore(WingetNexusContext context, ILogger<VersionDatastore> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            //_versionMapper = versionMapper;
        }

        public async Task<VersionDto> CreateVersionAsync(VersionDto version, string packageIdentifier)
        {
            try
            {
                var application = await _context.Applications.FirstOrDefaultAsync(p=>p.PackageIdentifier == packageIdentifier);
                if (application == null)
                {
                    _logger.LogDebug($"Package not found for identifier {packageIdentifier}");
                    throw new KeyNotFoundException($"Application with identifier {packageIdentifier} not found.");
                }

                // Check if the version already exists
                var existingVersion = await _context.Versions
                    .FirstOrDefaultAsync(v => v.VersionNumber == version.VersionNumber && v.Application.Id == application.Id);

                if (existingVersion != null)
                {
                    _logger.LogWarning($"Version {version.VersionNumber} already exists for application {packageIdentifier}.");
                    return _mapper.Map<VersionDto>(existingVersion);
                }

                //var dbVersion = new Version
                //{
                //    Id = version.Id,
                //    VersionNumber = version.VersionNumber,
                //    DefaultLocale = version.PackageLocale,
                //    CreatedDate = DateTime.UtcNow,
                //    ModifiedDate = DateTime.UtcNow,
                //    ManifestVersion = version.ManifestVersion,
                //    DatasJson = JsonSerializer.Serialize(version.Installers),
                //    Application = application,
                //    Locales = new List<Locale>(),

                //};
                var dbVersion = _mapper.Map<Version>(version);

                //foreach (var installer in version.Installers)
                //{
                //    var locale = new Locale
                //    {
                //        PackageLocale = application.PackageIdentifier,
                //        JsonContent = JsonSerializer.Serialize(installer),
                //        JsonVersion = version.ManifestVersion,
                //        Version = dbVersion
                //    };
                //    dbVersion.Locales.Add(locale);
                //}

                _context.Versions.Add(dbVersion);
                await _context.SaveChangesAsync();
                _logger.LogDebug("Version {VersionCode} created successfully for application {ApplicationId}.", version.VersionNumber, application.PackageIdentifier);

                return _mapper.Map<VersionDto>(dbVersion);
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "Error creating version: {Message}", exc.Message);
                throw;
            }
            
        }

        public async Task<VersionDto> GetVersionByIdAsync(int id)
        {
            var version = await _context.Versions.Include(v => v.Application).FirstOrDefaultAsync(v => v.Id == id);
            if (version == null)
            {
                throw new KeyNotFoundException($"Version with id {id} not found.");
            }

            //if (!string.IsNullOrEmpty(version.InstallersDatasJson))
            //{
            //    var manifestVersion = new System.Version(version.ManifestVersion);
            //    var typeName = $"WingetNexus.Shared.Models.Yaml.v{manifestVersion.Major}._{manifestVersion.Minor}.InstallerClass, WingetNexus.Shared";
            //    var type = Type.GetType(typeName);
            //    if (type != null)
            //    {
            //        version.Datas = JsonSerializer.Deserialize(version.DatasJson, type);
            //    }
            //}

            //return VersionMapper.VersionToVersionDto(version);

            return _mapper.Map<VersionDto>(version);
        }

        public async Task<IEnumerable<VersionDto>> GetAllVersionsAsync(string? versionFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Versions.Include(v => v.Application).AsQueryable();

            if (!string.IsNullOrEmpty(versionFilter))
            {
                query = query.Where(v => v.VersionNumber.Contains(versionFilter));
            }

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(p=>_mapper.Map<VersionDto>(p)).ToListAsync();
        }

        public async Task<VersionDto> UpdateVersionAsync(VersionDto version)
        {
            var dbVersion = await _context.Versions.Include(v => v.Application).FirstOrDefaultAsync(v => v.Id == version.Id);
            if (dbVersion == null)
            {
                throw new KeyNotFoundException($"Version with id {version.Id} not found.");
            }

            dbVersion = _mapper.Map<Version>(version);

            _context.Versions.Update(dbVersion);
            await _context.SaveChangesAsync();

            return _mapper.Map<VersionDto>(dbVersion);
        }

        public async Task<bool> DeleteVersionAsync(int id)
        {
            try
            {
                var version = await _context.Versions.FindAsync(id);
                if (version != null)
                {
                    _context.Versions.Remove(version);
                    await _context.SaveChangesAsync();

                    _logger.LogDebug("Version {VersionId} deleted successfully.", id);

                    return true;
                }
                else
                {
                    _logger.LogDebug("Version {VersionId} not found.", id);
                    return false;
                }
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "Error deleting version: {Message}", exc.Message);
                return false;
            }
            
        }
    }
}
