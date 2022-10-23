using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class TicketAttachment
    {
        // Keys
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [Required]
        [DisplayName("Team Member")]
        public string? UserId { get; set; }

        // Properties
        [DisplayName("File Date")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("File Description")]
        public string? Description { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile FormFile { get; set; } = null!;

        [Required]
        public byte[]? FileData { get; set; }

        [Required]
        [DisplayName("File Name")]
        public string? FileName { get; set; }

        [Required]
        [DisplayName("File Extension")]
        public string? FileType { get; set; }

        // Navigation properties
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual BugTrackerUser User { get; set; } = null!;
    }
}
