using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public BugTrackerUser BugTrackerUser { get; set; } = default!;
        public MultiSelectList Roles { get; set; } = default!;
        public List<string> SelectedRoles { get; set; } = default!;
    }
}
