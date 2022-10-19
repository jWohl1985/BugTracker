using System.ComponentModel;

namespace BugTracker.Models
{
    public class ProjectPriority
    {
        // Keys
        public int Id { get; set; }

        // Properties
        [DisplayName("Priority")]
        public string Name { get; set; }
    }
}
