using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        // Keys
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [Required]
        [DisplayName("Team Member")]
        public string? UserId { get; set; }

        // Properties
        [Required]
        [DisplayName("Updated Item")]
        public string? Property { get; set; }

        [Required]
        [DisplayName("Previous")]
        public string? OldValue { get; set; }

        [Required]
        [DisplayName("Current")]
        public string? NewValue { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }

        [Required]
        [DisplayName("Description")]
        public string? Description { get; set; }

        // Navigation properties
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual BugTrackerUser User { get; set; } = null!;
    }
}
