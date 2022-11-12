namespace BugTracker.Models.ViewModels
{
    public class DashboardViewModel
    {
        public Company Company { get; set; } = default!;

        public List<Project> Projects { get; set; } = default!;

        public List<Ticket> Tickets { get; set; } = default!;

        public List<BugTrackerUser> Members { get; set; } = default!;
    }
}
