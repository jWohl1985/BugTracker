using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IProjectService
    {
        Task AddNewProjectAsync(Project project);

        Task<bool> AddProjectManagerAsync(string userId, int projectId);

        Task<bool> AddUserToProjectAsync(string userId, int projectId);

        Task ArchiveProjectAsync(Project project);

        Task<List<Project>> GetAllProjectsByCompany(int companyId);

        Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName);

        Task<List<BugTrackerUser>> GetAllProjectMembersExceptPMAsync(int projectId);

        Task<List<Project>> GetArchivedProjectsByCompany(int companyId);

        Task<List<BugTrackerUser>> GetDevelopersOnProjectAsync(int projectId);

        Task<BugTrackerUser> GetProjectManagerAsync(int projectId);

        Task<List<BugTrackerUser>> GetProjectMembersByRoleAsync(int projectId, string role);

        Task<Project> GetProjectByIdAsync(int projectId, int companyId);

        Task<List<BugTrackerUser>> GetSubmittersOnProjectAsync(int projectId);

        Task<List<BugTrackerUser>> GetUsersNotOnProjectAsync(int projectId, int companyId);

        Task<List<Project>> GetUserProjectsAsync(string userId);

        Task<bool> IsUserOnProject(string userId, int projectId);

        Task<int> LookupProjectPriorityId(string priorityName);

        Task RemoveProjectManagerAsync(int projectId);

        Task RemoveUsersFromProjectByRoleAsync(string role, int projectId);

        Task RemoveUserFromProjectAsync(string userId, int projectId);

        Task UpdateProjectAsync(Project project);

    }
}
