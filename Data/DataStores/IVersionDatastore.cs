using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Data.DataStores
{
    public interface IVersionDatastore
    {
        Task<VersionDto> CreateVersionAsync(VersionDto version);
        Task<bool> DeleteVersionAsync(int id);
        Task<IEnumerable<VersionDto>> GetAllVersionsAsync(string? versionFilter = null, int pageNumber = 1, int pageSize = 10);
        Task<VersionDto> GetVersionByIdAsync(int id);
        Task<VersionDto> UpdateVersionAsync(VersionDto version);
    }
}