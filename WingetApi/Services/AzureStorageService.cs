using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using WingetNexus.WingetApi.Settings;

namespace WingetNexus.WingetApi.Services
{
    public class AzureStorageService : IStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IOptions<AzureStorageSettings> _settings;

        public AzureStorageService(IOptions<AzureStorageSettings> settings)
        {
            _settings = settings;

            _blobServiceClient = new BlobServiceClient(_settings.Value.ConnectionString);
        }

        public async Task DeleteAsync(string key)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_settings.Value.ContainerName);
            var blobClient = containerClient.GetBlobClient(key);

            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_settings.Value.ContainerName);
            var blobs = containerClient.GetBlobsAsync();

            var keys = new List<string>();
            await foreach (var blob in blobs)
            {
                keys.Add(blob.Name);
            }

            return keys;
        }

        public async Task<Stream> GetAsync(string key)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_settings.Value.ContainerName);
            var blobClient = containerClient.GetBlobClient(key);

            var response = await blobClient.DownloadAsync();

            return response.Value.Content;
        }

        public async Task<string> GetUrlAsync(string key)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_settings.Value.ContainerName);
            var blobClient = containerClient.GetBlobClient(key);

            var uri = blobClient.Uri;

            return uri.ToString();
        }

        public async Task<string> SaveAsync(string key, Stream stream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_settings.Value.ContainerName);
            var blobClient = containerClient.GetBlobClient(key);

            await blobClient.UploadAsync(stream, true);

            return key;
        }
    }
}
