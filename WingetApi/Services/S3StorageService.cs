using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using WingetNexus.WingetApi.Settings;

namespace WingetNexus.WingetApi.Services
{
    public class S3StorageService : IStorageService
    {
        private IAmazonS3 _s3Client;
        private readonly IOptions<S3StorageSettings> _settings;

        public S3StorageService(IOptions<S3StorageSettings> settings)
        {
            _settings = settings;

            var config = new AmazonS3Config
            {
                ServiceURL = $"https://{_settings.Value.Server}",
            };

            _s3Client = new AmazonS3Client(
                settings.Value.AccessKey,
                settings.Value.SecretKey,
                config);
            
        }

        public async Task DeleteAsync(string key)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _settings.Value.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
        }

        public async Task<List<string>> GetAllAsync()
        {
            //get all files description from S3 bucket
            var request = new ListObjectsRequest
            {
                BucketName = _settings.Value.BucketName
            };

            var response = await _s3Client.ListObjectsAsync(request);

            return response.S3Objects.Select(x => x.Key).ToList();            
        }
        

        public async Task<Stream> GetAsync(string key)
        {
            var request = new GetObjectRequest
            {
                BucketName = _settings.Value.BucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(request);

            return response.ResponseStream;
        }

        public async Task<string> GetUrlAsync(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _settings.Value.BucketName,
                Key = key,
                Expires = DateTime.Now.AddMinutes(5)
            };

            return await _s3Client.GetPreSignedURLAsync(request);
        }

        public async Task<string> SaveAsync(string key, Stream stream)
        {
            var request = new PutObjectRequest
            {
                BucketName = _settings.Value.BucketName,
                Key = key,
                InputStream = stream
            };

            await _s3Client.PutObjectAsync(request);

            return key;
        }
    }
}
