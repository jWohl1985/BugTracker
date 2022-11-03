using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class AssignDeveloperViewModel
    {
        public Ticket Ticket { get; set; } = default!;
        public SelectList DeveloperList { get; set; } = default!;
        public string DeveloperId { get; set; } = default!;
    }
}
