using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoleService _roleService;
        private readonly IProjectService _projectService;

        public TicketService(ApplicationDbContext context, IRoleService roleService, IProjectService projectService)
        {
            _context = context;
            _roleService = roleService;
            _projectService = projectService;
        }

        public async Task AddNewTicketAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            ticket.Archived = true;
            await UpdateTicketAsync(ticket);
        }

        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket is null)
                return;

            ticket.DeveloperId = userId;
            ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value;

            await UpdateTicketAsync(ticket);
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            return await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                    .Include(t => t.Priority)
                    .Include(t => t.Status)
                    .Include(t => t.Type)
                    .Include(t => t.History)
                    .Include(t => t.Comments)
                    .Include(t => t.Attachments)
                    .Include(t => t.Project)
                    .Include(t => t.Developer)
                    .Include(t => t.Creator)
                    .ToListAsync();
        }

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            int? priorityId = await LookupTicketPriorityIdAsync(priorityName);

            if (priorityId is null)
                return new List<Ticket>();

            return await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                    .Include(t => t.Priority)
                    .Include(t => t.Status)
                    .Include(t => t.Type)
                    .Include(t => t.History)
                    .Include(t => t.Comments)
                    .Include(t => t.Attachments)
                    .Include(t => t.Project)
                    .Include(t => t.Developer)
                    .Include(t => t.Creator)
                .Where(t => t.TicketPriorityId == priorityId)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            int? statusId = await LookupTicketStatusIdAsync(statusName);

            if (statusId is null)
                return new List<Ticket>();

            return await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                    .Include(t => t.Priority)
                    .Include(t => t.Status)
                    .Include(t => t.Type)
                    .Include(t => t.History)
                    .Include(t => t.Comments)
                    .Include(t => t.Attachments)
                    .Include(t => t.Project)
                    .Include(t => t.Developer)
                    .Include(t => t.Creator)
                .Where(t => t.TicketStatusId == statusId)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            int? typeId = await LookupTicketTypeIdAsync(typeName);

            if (typeId is null)
                return new List<Ticket>();

            return await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                    .Include(t => t.Priority)
                    .Include(t => t.Status)
                    .Include(t => t.Type)
                    .Include(t => t.History)
                    .Include(t => t.Comments)
                    .Include(t => t.Attachments)
                    .Include(t => t.Project)
                    .Include(t => t.Developer)
                    .Include(t => t.Creator)
                .Where(t => t.TicketTypeId == typeId)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
            List<Ticket> archivedTickets = companyTickets.Where(t => t.Archived).ToList();

            return archivedTickets;
        }

        public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            return (await GetAllTicketsByPriorityAsync(companyId, priorityName))
                .Where(t => t.ProjectId == projectId)
                .ToList();
        }

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            return (await GetTicketsByRoleAsync(role, userId, companyId))
                .Where(t => t.ProjectId == projectId)
                .ToList();
        }

        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            return (await GetAllTicketsByStatusAsync(companyId, statusName))
                .Where(t => t.ProjectId == projectId)
                .ToList();
        }

        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            return (await GetAllTicketsByTypeAsync(companyId, typeName))
                .Where(t => t.ProjectId == projectId)
                .ToList();
        }

        public async Task<Ticket?> GetTicketByIdAsync(int ticketId)
        {
            Ticket? ticket = await _context.Tickets
                    .Include(t => t.Priority)
                    .Include(t => t.Status)
                    .Include(t => t.Type)
                    .Include(t => t.Project)
                    .Include(t => t.Developer)
                    .Include(t => t.Creator)
                    .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket is null)
                return null;

            return ticket;
        }

        public async Task<BugTrackerUser?> GetTicketDeveloperAsync(int ticketId, int companyId)
        {
            Ticket? ticket = (await GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(t => t.Id == ticketId);

            return ticket?.Developer;
        }

        public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
            List<Ticket> roleTickets = new List<Ticket>();

            if (role == Roles.Admin.ToString())
            {
                roleTickets = companyTickets;
            }
            else if (role == Roles.Developer.ToString())
            {
                roleTickets = companyTickets
                        .Where(t => t.DeveloperId == userId)
                        .ToList();
            }
            else if (role == Roles.ProjectManager.ToString())
            {
                roleTickets = companyTickets
                    .Where(t => t.CreatorId == userId)
                    .ToList();
            }
            else if (role == Roles.Submitter.ToString())
            {
                roleTickets = await GetTicketsByUserIdAsync(userId, companyId);
            }

            return roleTickets;
        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            BugTrackerUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            List<Ticket> userTickets = new List<Ticket>();

            if (user is null)
                return userTickets;

            if (await _roleService.IsUserInRoleAsync(user, Roles.Admin.ToString()))
            {
                userTickets = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId))
                    .SelectMany(p => p.Tickets)
                    .ToList();
            }
            else if (await _roleService.IsUserInRoleAsync(user, Roles.Developer.ToString()))
            {
                userTickets = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId))
                    .SelectMany(p => p.Tickets)
                    .Where(t => t.DeveloperId == userId)
                    .ToList();
            }
            else if (await _roleService.IsUserInRoleAsync(user, Roles.ProjectManager.ToString()))
            {
                userTickets = (await _projectService.GetUserProjectsAsync(userId))
                    .SelectMany(p => p.Tickets)
                    .ToList();
            }
            else if (await _roleService.IsUserInRoleAsync(user, Roles.Submitter.ToString()))
            {
                userTickets = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId))
                    .SelectMany(p => p.Tickets)
                    .Where(t => t.CreatorId == userId)
                    .ToList();
            }

            return userTickets;
        }

        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            TicketPriority? priority = await _context.TicketPriorities.FirstOrDefaultAsync(p => p.Name == priorityName);

            if (priority is null)
                return null;

            return priority.Id;
        }

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            TicketStatus? status = await _context.TicketStatuses.FirstOrDefaultAsync(p => p.Name == statusName);

            if (status is null)
                return null;

            return status.Id;
        }

        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            TicketType? type = await _context.TicketTypes.FirstOrDefaultAsync(p => p.Name == typeName);

            if (type is null)
                return null;

            return type.Id;
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }
    }
}
