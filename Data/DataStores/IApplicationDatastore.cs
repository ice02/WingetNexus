
using WingetNexus.Data.Models;

namespace WingetNexus.Data.DataStores
{
    public interface IApplicationDatastore
    {
        List<string> GetUserAcl(string userId);
        IQueryable<UserRole> GetAllUsers();
        Task<bool> SetUserAcl(string userId, string role);
    }
}