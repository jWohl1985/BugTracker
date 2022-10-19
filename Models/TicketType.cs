using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketType
    {
        // Keys
        public int Id { get; set; }

        // Properties
        [DisplayName("Ticket Type")]
        public string Name { get; set; }

    }
}
