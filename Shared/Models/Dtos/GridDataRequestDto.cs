namespace WingetNexus.Shared.Models.Dtos
{
    public class GridDataRequestDto
    {
        public string? SearchTerm { get; set; } = null;
        public int Page { get; set; } = 0; // The page number for the data we're requesting
        public int PageSize { get; set; } = 10; // The number of items per page
    }
}