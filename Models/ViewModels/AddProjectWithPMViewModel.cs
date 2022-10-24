using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class AddProjectWithPMViewModel
    {
        public Project Project { get; set; } = default!;
        public SelectList ProjectManagerList { get; set; } = default!;
        public string ProjectManagerId { get; set; } = default!;
        public SelectList PriorityList { get; set; } = default!;
    }
}
