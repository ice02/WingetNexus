using System.Threading.Tasks;
using WingetNexus.Shared.Models.Dtos;
using System.Collections.Generic;

namespace WingetNexus.Client.Services
{
    public interface IPackageService
    {
        Task<ApplicationDto> GetPackageAsync(string id);
        Task<DataListDto<ApplicationDto>> GetPackagesFilteredAsync(GridDataRequestDto request);
        Task DeletePackageAsync(string id);
        Task DeletePublisherAsync(int id);
        Task<bool> CheckPackageIdentifierUniquenessAsync(string identifier);
        Task<IEnumerable<string>> SearchPublishersAsync(string value);
        Task<ApplicationDto> CreatePackageAsync(ApplicationDto application);
        Task<IEnumerable<PublisherDto>> GetPublishersAsync(string searchTerm, int page, int pageSize);

        Task<PublisherDto> CreatePublisherAsync(PublisherDto publisher);
        Task<ApplicationDto> CreateApplicationAsync(ApplicationDto application);

        Task InitializeHttpClientAsync(HttpClient httpClient);
    }
}