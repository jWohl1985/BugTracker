using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Project
    {
        // Keys
        public int Id { get; set; }

        [DisplayName("Company")]
        public int? CompanyId { get; set; }

        [DisplayName("Priority")]
        public int? PriorityId { get; set; }

        // Properties
        [Required]
        [StringLength(50)]
        [DisplayName("Project Name")]
        public string Name { get; set; }

        public string Description { get; set; }

        [DisplayName("Start Date")]
        public DateTimeOffset StartDate { get; set; }

        [DisplayName("End Date")]
        public DateTimeOffset EndDate { get; set; }

        public bool Archived { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]

        public IFormFile ImageFile { get; set; }

        public byte[] ImageData { get; set; }

        public string ImageType { get; set; }

        [DisplayName("File Name")]
        public string ImageFileName { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; }
        public virtual ProjectPriority Priority { get; set; }
        public virtual ICollection<BugTrackerUser> Members { get; set; } = new HashSet<BugTrackerUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}
