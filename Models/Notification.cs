using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Notification
    {
        // Keys
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [Required]
        [DisplayName("Recipient")]
        public string? RecipientId { get; set; }

        [Required]
        [DisplayName("Sender")]
        public string? SenderId { get; set; }

        // Properties
        [Required]
        [DisplayName("Title")]
        public string? Title { get; set; }

        [Required]
        [DisplayName("Message")]
        public string? Message { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Has been viewed")]
        public bool Viewed { get; set; }

        // Navigation properties
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual BugTrackerUser Recipient { get; set; } = null!;
        public virtual BugTrackerUser Sender { get; set; } = null!;
    }
}
