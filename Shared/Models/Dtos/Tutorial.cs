namespace WingetNexus.Shared.Models.Dtos
{
    public class Tutorial
    {
        public int Id { get; set; }
        public string? Title { get; set; } // Marked as nullable
        public string? Content { get; set; } // Marked as nullable
        public bool Dismissed { get; set; }
    }
}