namespace WingetNexus.WingetApi.Settings
{
    public class S3StorageSettings
    {
        public string BucketName { get; set; }
        public string Server { get; set; }
        public string RootPath { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}
