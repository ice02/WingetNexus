using System.Threading.Tasks;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Client.Services
{
    public interface IContentFileService
    {
        Task<ContentFileDto> GetContentFileAsync(int id);
        Task SaveContentFileAsync(int id, string content);
        Task DeleteContentFileAsync(int id);
    }
}