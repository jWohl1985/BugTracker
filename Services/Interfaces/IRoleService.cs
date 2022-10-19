using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IRoleService
    {
        Task<bool> IsUserInRoleAsync(BugTrackerUser user, string roleName);

        Task<IEnumerable<string>> GetUserRolesAsync(BugTrackerUser user);

        Task<bool> AddUserToRoleAsync(BugTrackerUser user, string roleName);

        Task<bool> RemoveUserFromRoleAsync(BugTrackerUser user, string roleName);

        Task<bool> RemoveUserFromRolesAsync(BugTrackerUser user, IEnumerable<string> roles);

        Task<List<BugTrackerUser>> GetUsersInRoleAsync(string roleName, int companyId);

        Task<List<BugTrackerUser>> GetUsersNotInRoleAsync(string roleName, int companyId);

        Task<string> GetRoleNameByIdAsync(string roleId);
    }
}
