using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketComment
    {
        // Keys
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [Required]
        [DisplayName("Team Member")]
        public string? UserId { get; set; }

        // Properties
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        [Required]
        public string? Comment { get; set; }

        // Navigation properties
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual BugTrackerUser User { get; set; } = null!;

    }
}
