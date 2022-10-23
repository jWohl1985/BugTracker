using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace BugTracker.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BugTrackerUser> _userManager;

        public RoleService(ApplicationDbContext context, 
            RoleManager<IdentityRole> roleManager, 
            UserManager<BugTrackerUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<bool> AddUserToRoleAsync(BugTrackerUser user, string roleName)
        {
            return (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
        }

        public async Task<string?> GetRoleNameByIdAsync(string roleId)
        {
            IdentityRole? role = _context.Roles.Find(roleId);

            if (role == null)
                return null;

            return await _roleManager.GetRoleNameAsync(role);
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(BugTrackerUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<List<BugTrackerUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            return (await _userManager.GetUsersInRoleAsync(roleName))
                .Where(u => u.CompanyId == companyId)
                .ToList();
        }

        public async Task<List<BugTrackerUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            return _context.Users
                .Except(usersInRole)
                .Where(u => u.CompanyId == companyId)
                .ToList();
        }

        public async Task<bool> IsUserInRoleAsync(BugTrackerUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<bool> RemoveUserFromRoleAsync(BugTrackerUser user, string roleName)
        {
            return (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
        }

        public async Task<bool> RemoveUserFromRolesAsync(BugTrackerUser user, IEnumerable<string> roles)
        {
            return (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;
        }
    }
}
