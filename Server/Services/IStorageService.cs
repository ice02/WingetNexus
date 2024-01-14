namespace WingetNexus.Server.Services
{
    public interface IStorageService
    {
        Task DeleteAsync(string key);
        Task<List<string>> GetAllAsync();
        Task<Stream> GetAsync(string key);
        Task<string> GetUrlAsync(string key);
        Task<string> SaveAsync(string key, Stream stream);
    }
}