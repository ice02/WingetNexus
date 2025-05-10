using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.WingetApi.Services
{
    public interface ILocalCacheService
    {
        Task GenerateSQLiteDatabaseAsync(string databasePath);
        Task<List<ApplicationDto>> GetAllApplicationsAsync();
        Task<string> GetSource2MsixAsync();
        Task InitializeAsync();
    }
}