using WingetNexus.Shared.Models.Db;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Data.DataStores
{
    public interface IWingetNexusDataStore
    {
        Installer CreateInstaller(InstallerDto installerForm);
    }
}