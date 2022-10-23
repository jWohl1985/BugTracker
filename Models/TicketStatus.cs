using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketStatus
    {
        // Keys
        public int Id { get; set; }

        // Properties
        [Required]
        [DisplayName("Status")]
        public string? Name { get; set; }
    }
}
