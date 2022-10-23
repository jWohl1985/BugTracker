using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Invite
    {
        // Keys
        public int Id { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [Required]
        [DisplayName("Sender")]
        public string? SenderId { get; set; }

        [Required]
        [DisplayName("Recipient")]
        public string? RecipientId { get; set; }

        // Properties
        [DisplayName("Date Sent")]
        [DataType(DataType.Date)]
        public DateTimeOffset SendDate { get; set; }

        [DisplayName("Join Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset JoinDate { get; set; }

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }

        [Required]
        [DisplayName("Recipient Email")]
        public string? RecipientEmail { get; set; }

        [Required]
        [DisplayName("Recipient First Name")]
        public string? RecipientFirstName { get; set; }

        [Required]
        [DisplayName("Recipient Last Name")]
        public string? RecipientLastName { get; set; }

        public bool IsValid { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
        public virtual BugTrackerUser Sender { get; set; } = null!;
        public virtual BugTrackerUser Recipient { get; set; } = null!;
    }
}
