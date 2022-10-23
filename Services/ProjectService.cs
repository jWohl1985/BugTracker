using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoleService _roleService;

        public ProjectService(ApplicationDbContext context, IRoleService roleService)
        {
            _context = context;
            _roleService = roleService;
        }

        public async Task AddNewProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            BugTrackerUser? currentProjectManager = await GetProjectManagerAsync(projectId);

            if (currentProjectManager != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"*** ERROR *** Error removing current PM - {ex.Message}");
                    return false;
                }
            }

            try
            {
                await AddProjectManagerAsync(userId, projectId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** Error adding PM - {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            try
            {
                BugTrackerUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user is null)
                    return false;

                Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                if (project is null || await IsUserOnProjectAsync(userId, projectId))
                    return false;

                project.Members.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error adding user to project - {ex.Message}");
                throw;
            }
        }

        public async Task ArchiveProjectAsync(Project project)
        {
            project.Archived = true;
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BugTrackerUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<BugTrackerUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            List<BugTrackerUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
            List<BugTrackerUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

            return developers.Concat(submitters).Concat(admins).ToList();
        }

        public async Task<List<Project>> GetAllProjectsByCompanyIdAsync(int companyId)
        {
            return await _context.Projects
                .Where(p => p.CompanyId == companyId && !p.Archived)
                .Include(p => p.Members)
                .Include(p => p.Priority)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Type)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Status)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Priority)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Creator)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Developer)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Comments)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.History)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Attachments)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Notifications)
                .ToListAsync();
        }

        public async Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName)
        {
            int? priorityId = await LookupProjectPriorityIdAsync(priorityName);

            return (await GetAllProjectsByCompanyIdAsync(companyId))
                .Where(p => p.PriorityId == priorityId)
                .ToList();
        }

        public async Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId)
        {
            return (await GetAllProjectsByCompanyIdAsync(companyId))
                .Where(p => p.Archived)
                .ToList();
        }

        public async Task<List<BugTrackerUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<Project?> GetProjectByIdAsync(int projectId, int companyId)
        {
            return await _context.Projects
                .Include(p => p.Members)
                .Include(p => p.Tickets)
                .Include(p => p.Priority)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);
        }

        public async Task<BugTrackerUser?> GetProjectManagerAsync(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project is null)
                return null;

            foreach(BugTrackerUser user in project.Members)
            {
                if (await _roleService.IsUserInRoleAsync(user, Roles.ProjectManager.ToString()))
                    return user;
            }

            return null;
        }

        public async Task<List<BugTrackerUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            var project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
            List<BugTrackerUser> users = new List<BugTrackerUser>();

            if (project is null)
                return users;

            foreach(BugTrackerUser user in project.Members)
            {
                if (await _roleService.IsUserInRoleAsync(user, role))
                    users.Add(user);
            }

            return users;
        }

        public async Task<List<BugTrackerUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.Developer)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.Creator)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.Priority)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.Status)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.Type)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user is null)
                    return new List<Project>();

                return user.Projects.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error getting user projects - {ex.Message}");
                throw;
            }
        }

        public async Task<List<BugTrackerUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            return await _context.Users
                .Where(u => u.Projects.All(p => p.Id != projectId))
                .Where(u => u.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project is null)
                return false;

            return project.Members.Any(m => m.Id == userId);
        }

        public async Task<int?> LookupProjectPriorityIdAsync(string priorityName)
        {
            var projectPriority = await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName);

            if (projectPriority is null)
                return null;

            return projectPriority.Id;
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project is null)
                return;

            try
            {
                foreach (BugTrackerUser user in project.Members)
                {
                    if (await _roleService.IsUserInRoleAsync(user, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(user.Id, projectId);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error removing project manager from project - {ex.Message}");
                throw;
            }
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BugTrackerUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user is null)
                    return;

                Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                if (project is null || !(await IsUserOnProjectAsync(userId, projectId)))
                    return;

                project.Members.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error removing user from project - {ex.Message}");
                throw;
            }
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                var usersToRemove = await GetProjectMembersByRoleAsync(projectId, role);
                var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                if (project is null)
                    throw new Exception();

                foreach(BugTrackerUser user in usersToRemove)
                {
                    project.Members.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error removing users in role {role} from project - {ex.Message}");
                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
