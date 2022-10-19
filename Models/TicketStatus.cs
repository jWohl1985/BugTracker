using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketStatus
    {
        // Keys
        public int Id { get; set; }

        // Properties
        [DisplayName("Status")]
        public string Name { get; set; }
    }
}
