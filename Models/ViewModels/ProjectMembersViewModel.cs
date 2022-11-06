using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class ProjectMembersViewModel
    {
        public Project Project { get; set; } = default!;

        public MultiSelectList PossibleMembers { get; set; } = default!;

        public List<string> Members { get; set; } = default!;
    }
}
