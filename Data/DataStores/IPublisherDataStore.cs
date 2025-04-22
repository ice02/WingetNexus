using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Data.DataStores
{
    public interface IPublisherDataStore
    {
        Task<IEnumerable<PublisherDto>> SearchPublishersAsync(string query);
        Task<PublisherDto> CreatePublisherAsync(PublisherDto publisher);
        Task<PublisherDto> GetPublisherByIdAsync(int id);
        Task<IEnumerable<PublisherDto>> GetAllPublishersAsync(string? nameFilter = null, int pageNumber = 1, int pageSize = 10);
        Task<PublisherDto> UpdatePublisherAsync(PublisherDto publisher);
        Task DeletePublisherAsync(int id);
        Task<PublisherDto?> GetPublisherByNameAsync(string name);
    }
}