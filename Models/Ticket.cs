using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Ticket
    {
        // Keys
        public int Id { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Type")]
        public int TicketTypeId { get; set; }

        [DisplayName("Status")]
        public int TicketStatusId { get; set; }

        [DisplayName("Priority")]
        public int TicketPriorityId { get; set; }

        [DisplayName("Creator")]
        public string? CreatorId { get; set; }

        [DisplayName("Developer")]
        public string? DeveloperId { get; set; }

        // Properties
        [Required]
        [StringLength(50)]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.Date)]
        public DateTimeOffset? Updated { get; set; }

        public bool Archived { get; set; }

        [DisplayName("Archived By Project")]
        public bool ArchivedByProject { get; set; }

        // Navigation properties
        public virtual Project? Project { get; set; }
        public virtual TicketType? Type { get; set; }
        public virtual TicketStatus? Status { get; set; }
        public virtual TicketPriority? Priority { get; set; }
        public virtual BugTrackerUser? Creator { get; set; }
        public virtual BugTrackerUser? Developer { get; set; }
        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    }
}
