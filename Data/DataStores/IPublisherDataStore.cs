using WingetNexus.Shared.Entities;

namespace WingetNexus.Data.DataStores
{
    public interface IPublisherDataStore
    {
        Task<IEnumerable<Publisher>> SearchPublishersAsync(string query);
        Task<Publisher> CreatePublisherAsync(Publisher publisher);
        Task<Publisher> GetPublisherByIdAsync(int id);
        Task<IEnumerable<Publisher>> GetAllPublishersAsync(string? nameFilter = null, int pageNumber = 1, int pageSize = 10);
        Task<Publisher> UpdatePublisherAsync(Publisher publisher);
        Task DeletePublisherAsync(int id);
    }
}