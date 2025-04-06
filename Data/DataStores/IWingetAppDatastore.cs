using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Data.DataStores
{
    public interface IWingetAppDatastore
    {
        Task<List<string>> SearchPublishersAsync(string search);

        Task<ApplicationDto> CreateApplicationAsync(ApplicationDto application);
        Task<ApplicationDto> GetApplicationByIdAsync(int id);
        Task<ApplicationDto> GetApplicationByPackageIdentifierAsync(string packageIdentifier);
        Task<int> GetAppCountByIdentyifierAsync(string identifier);
        Task<int> GetApplicationCountAsync(string? filter = null);
        Task<IEnumerable<ApplicationDto>> GetAllApplicationsAsync(FilterDto filterDto);
        Task<ApplicationDto> GetApplicationByPublisherAndNameAsync(string publisher, string name);
        Task<ApplicationDto> UpdateApplicationAsync(ApplicationDto application);
        Task DeleteApplicationAsync(string packageIdentifier);
    }
}
