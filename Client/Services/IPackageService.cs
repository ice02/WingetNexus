using System.Threading.Tasks;
using WingetNexus.Shared.Models.Dtos;
using System.Collections.Generic;

namespace WingetNexus.Client.Services
{
    public interface IPackageService
    {
        Task<ApplicationDto> GetPackageAsync(string id);
        Task<PackageListDto> GetPackagesFilteredAsync(GridDataRequestDto request);
        Task DeletePackageAsync(string id);
        Task<bool> CheckPackageIdentifierUniquenessAsync(string identifier);
        Task<IEnumerable<string>> SearchPublishersAsync(string value);
        Task<ApplicationDto> CreatePackageAsync(ApplicationDto application);

        Task<PublisherDto> CreatePublisherAsync(PublisherDto publisher);
        Task<ApplicationDto> CreateApplicationAsync(ApplicationDto application, string versionNumber);
    }
}