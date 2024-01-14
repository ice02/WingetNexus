using Microsoft.Extensions.Options;
using WingetNexus.Server.Settings;

namespace WingetNexus.Server.Services
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _basePath;
        private readonly IHostEnvironment _env;

        public LocalStorageService(IOptions<LocalStorageSettings> settings, IHostEnvironment env)
        {
            _env = env;
            _basePath = settings.Value.BasePath ?? Path.Combine(_env.ContentRootPath, _env.EnvironmentName, "upload");
        }

        public Task DeleteAsync(string key)
        {
            var path = Path.Combine(_basePath, key);
            File.Delete(path);

            return Task.CompletedTask;
        }

        public Task<List<string>> GetAllAsync()
        {
            // get all files description from local storage
            var files = Directory.GetFiles(_basePath, "*", SearchOption.AllDirectories);
            var keys = files.Select(x => x.Replace(_basePath, string.Empty).TrimStart('\\')).ToList();

            return Task.FromResult(keys);
        }

        public Task<Stream> GetAsync(string key)
        {
            var path = Path.Combine(_basePath, key);
            var stream = File.OpenRead(path);

            return Task.FromResult<Stream>(stream);
        }

        public Task<string> GetUrlAsync(string key)
        {
            var path = Path.Combine(_basePath, key);
            var url = new Uri(path).ToString();

            return Task.FromResult(url);
        }

        public Task<string> SaveAsync(string key, Stream stream)
        {
            var path = Path.Combine(_basePath, key);
            var directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var fileStream = File.Create(path);
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fileStream);

            return Task.FromResult(path);
        }
    }
}
