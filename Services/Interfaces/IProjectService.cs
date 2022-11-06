using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IProjectService
    {
        Task AddNewProjectAsync(Project project);

        Task<bool> AddProjectManagerAsync(string userId, int projectId);

        Task<bool> AddUserToProjectAsync(string userId, int projectId);

        Task ArchiveProjectAsync(Project project);

        Task<List<Project>> GetAllProjectsByCompanyIdAsync(int companyId);

        Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName);

        Task<List<BugTrackerUser>> GetAllProjectMembersExceptPMAsync(int projectId);

        Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId);

        //Task<List<BugTrackerUser>> GetDevelopersOnProjectAsync(int projectId);

        Task<BugTrackerUser?> GetProjectManagerAsync(int projectId);

        Task<List<BugTrackerUser>> GetProjectMembersByRoleAsync(int projectId, string role);

        Task<Project?> GetProjectByIdAsync(int projectId, int companyId);

        //Task<List<BugTrackerUser>> GetSubmittersOnProjectAsync(int projectId);

        Task<List<Project>> GetUnassignedProjectsAsync(int companyId);

        Task<List<BugTrackerUser>> GetUsersNotOnProjectAsync(int projectId, int companyId);

        Task<List<Project>> GetUserProjectsAsync(string userId);

        Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId);

        Task<bool> IsUserOnProjectAsync(string userId, int projectId);

        Task<int?> LookupProjectPriorityIdAsync(string priorityName);

        Task RemoveProjectManagerAsync(int projectId);

        Task RemoveUsersFromProjectByRoleAsync(string role, int projectId);

        Task RemoveUserFromProjectAsync(string userId, int projectId);

        Task RestoreArchivedProjectAsync(Project project);

        Task UpdateProjectAsync(Project project);

    }
}
