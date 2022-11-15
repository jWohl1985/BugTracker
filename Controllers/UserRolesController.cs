using BugTracker.Extensions;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly ICompanyService _companyInfoService;

        public UserRolesController(IRoleService roleService, ICompanyService companyInfoService)
        {
            _roleService = roleService;
            _companyInfoService = companyInfoService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new();

            if (User.Identity is null)
                return NotFound();

            int companyId = User.Identity.GetCompanyId();

            List<BugTrackerUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            foreach(BugTrackerUser user in users)
            {
                IEnumerable<string> selected = await _roleService.GetUserRolesAsync(user);

                model.Add(new ManageUserRolesViewModel()
                {
                    BugTrackerUser = user,
                    Roles = new MultiSelectList(await _roleService.GetRolesAsync(), "Name", "Name", selected),
                });
            }
            
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            if (User.Identity is null)
                return NotFound();

            int companyId = User.Identity.GetCompanyId();

            BugTrackerUser? user = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BugTrackerUser.Id);

            if (user is null)
                return NotFound();

            IEnumerable<string> oldRoles = await _roleService.GetUserRolesAsync(user);
            string? newRole = member.SelectedRoles.FirstOrDefault();

            if (newRole is null)
                return NotFound();

            if (await _roleService.RemoveUserFromRolesAsync(user, oldRoles))
                await _roleService.AddUserToRoleAsync(user, newRole);

            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}
