using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Data.DataStores
{
    public interface IContentFilesDataStore
    {
        Task<ContentFileDto> GetContentFileByIdAsync(int id);
    }
}