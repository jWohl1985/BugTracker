using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketPriority
    {
        // Keys
        public int Id { get; set; }

        // Properties
        [DisplayName("Priority")]
        public string Name { get; set; }
    }
}
