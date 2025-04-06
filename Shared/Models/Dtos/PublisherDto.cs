namespace WingetNexus.Shared.Models.Dtos
{
    public class PublisherDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? GitHubUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
