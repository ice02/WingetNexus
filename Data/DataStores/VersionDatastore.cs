using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WingetNexus.Data.DataStores
{
    public class VersionDatastore : IVersionDatastore
    {
        private readonly WingetNexusCtx _context;
        private readonly ILogger<VersionDatastore> _logger;

        public VersionDatastore(WingetNexusCtx context, ILogger<VersionDatastore> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WingetNexus.Shared.Entities.Version> CreateVersionAsync(WingetNexus.Shared.Entities.Version version)
        {
            _context.Versions.Add(version);
            await _context.SaveChangesAsync();
            return version;
        }

        public async Task<WingetNexus.Shared.Entities.Version> GetVersionByIdAsync(int id)
        {
            var version = await _context.Versions.Include(v => v.Application).FirstOrDefaultAsync(v => v.Id == id);
            if (version == null)
            {
                throw new KeyNotFoundException($"Version with id {id} not found.");
            }

            if (!string.IsNullOrEmpty(version.DatasJson))
            {
                var manifestVersion = new System.Version(version.ManifestVersion);
                var typeName = $"WingetNexus.Shared.Models.Yaml.v{manifestVersion.Major}._{manifestVersion.Minor}.InstallerClass, WingetNexus.Shared";
                var type = Type.GetType(typeName);
                if (type != null)
                {
                    version.Datas = JsonSerializer.Deserialize(version.DatasJson, type);
                }
            }

            return version;
        }

        public async Task<IEnumerable<WingetNexus.Shared.Entities.Version>> GetAllVersionsAsync(string? versionFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Versions.Include(v => v.Application).AsQueryable();

            if (!string.IsNullOrEmpty(versionFilter))
            {
                query = query.Where(v => v.VersionNumber.Contains(versionFilter));
            }

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<WingetNexus.Shared.Entities.Version> UpdateVersionAsync(WingetNexus.Shared.Entities.Version version)
        {
            _context.Versions.Update(version);
            await _context.SaveChangesAsync();
            return version;
        }

        public async Task DeleteVersionAsync(int id)
        {
            var version = await _context.Versions.FindAsync(id);
            if (version != null)
            {
                _context.Versions.Remove(version);
                await _context.SaveChangesAsync();
            }
        }
    }
}
