using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Company
    {
        public int Id { get; set; }


        [DisplayName("Company Name")]
        public string Name { get; set; }

        [DisplayName("Company Description")]
        public string Description { get; set; }


        // Navigation properties
        public ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        public ICollection<BugTrackerUser> Members { get; set; } = new HashSet<BugTrackerUser>();
    }
}
