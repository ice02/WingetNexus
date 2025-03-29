using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Entities;

namespace WingetNexus.Data.DataStores
{
    public interface IWingetAppDatastore
    {
        Task<Application> CreateApplicationAsync(Application application);
        Task<Application> GetApplicationByIdAsync(int id);
        Task<Application> GetApplicationByPackageIdentifierAsync(string packageIdentifier);
        Task<int> GetApplicationCountAsync(string? filter = null);
        Task<IEnumerable<Application>> GetAllApplicationsAsync(string? filter = null, int? pageNumber = 1, int? pageSize = 10, string? orderby = null, string? orderway="DESC");
        Task<Application> GetApplicationByPublisherAndNameAsync(string publisher, string name);
        Task<Application> UpdateApplicationAsync(Application application);
        Task DeleteApplicationAsync(string packageIdentifier);
    }
}
