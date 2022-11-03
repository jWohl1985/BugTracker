using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BugTracker.Extensions;

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
        [DisplayName("Select a file")]
        [MaxFileSize(1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf"} )]
        public IFormFile FormFile { get; set; } = null!;

        public byte[] FileData { get; set; } = null!;

        [DisplayName("File Name")]
        public string FileName { get; set; } = default!;

        [DisplayName("File Extension")]
        public string FileType { get; set; } = default!;

        // Navigation properties
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual BugTrackerUser User { get; set; } = null!;
    }
}
