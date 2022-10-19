using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;

namespace BugTracker.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNewProjectAsync(Project project)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task ArchiveProjectAsync(Project project)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BugTrackerUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BugTrackerUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<BugTrackerUser> GetProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BugTrackerUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BugTrackerUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BugTrackerUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> LookupProjectPriorityIdAsync(string priorityName)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            throw new NotImplementedException();
        }
    }
}
