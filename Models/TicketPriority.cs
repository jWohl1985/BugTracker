using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketPriority
    {
        // Keys
        public int Id { get; set; }

        // Properties
        [Required]
        [DisplayName("Priority")]
        public string? Name { get; set; }
    }
}
