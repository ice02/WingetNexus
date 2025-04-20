using System.ComponentModel.DataAnnotations;

namespace WingetNexus.Data.Models
{
    public class TutorialDismissedState
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int TutorialId { get; set; }

        [Required]
        public bool IsDismissed { get; set; }
    }
}