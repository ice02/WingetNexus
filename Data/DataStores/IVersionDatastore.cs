





namespace WingetNexus.Data.DataStores
{
    public interface IVersionDatastore
    {
        Task<Shared.Entities.Version> CreateVersionAsync(Shared.Entities.Version version);
        Task DeleteVersionAsync(int id);
        Task<IEnumerable<Shared.Entities.Version>> GetAllVersionsAsync(string? versionFilter = null, int pageNumber = 1, int pageSize = 10);
        Task<Shared.Entities.Version> GetVersionByIdAsync(int id);
        Task<Shared.Entities.Version> UpdateVersionAsync(Shared.Entities.Version version);
    }
}