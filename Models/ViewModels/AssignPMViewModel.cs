using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class AssignPMViewModel
    {
        public Project Project { get; set; } = default!;
        public SelectList ProjectManagers { get; set; } = default!;
        public string ProjectManagerId { get; set; } = default!;
    }
}
