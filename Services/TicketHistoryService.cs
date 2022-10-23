using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class TicketHistoryService : ITicketHistoryService
    {
        private readonly ApplicationDbContext _context;

        public TicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            if (newTicket is null)
                return;

            if (oldTicket is null)
            {
                TicketHistory history = new TicketHistory()
                {
                    TicketId = newTicket.Id,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = $"Ticket created",
                };

                await _context.TicketHistories.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (oldTicket.Title != newTicket.Title)
                {
                    TicketHistory history = new TicketHistory()
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = newTicket.Title,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket title: {newTicket.Title}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                if (oldTicket.Description != newTicket.Description)
                {
                    TicketHistory history = new TicketHistory()
                    {
                        TicketId = newTicket.Id,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = newTicket.Description,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket description: {newTicket.Description}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                {
                    TicketHistory history = new TicketHistory()
                    {
                        TicketId = newTicket.Id,
                        Property = "Priority",
                        OldValue = oldTicket.Priority.Name,
                        NewValue = newTicket.Priority.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket priority: {newTicket.Priority.Name}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                {
                    TicketHistory history = new TicketHistory()
                    {
                        TicketId = newTicket.Id,
                        Property = "Status",
                        OldValue = oldTicket.Status.Name,
                        NewValue = newTicket.Status.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket status: {newTicket.Status.Name}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                {
                    TicketHistory history = new TicketHistory()
                    {
                        TicketId = newTicket.Id,
                        Property = "Type",
                        OldValue = oldTicket.Type.Name,
                        NewValue = newTicket.Type.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket type: {newTicket.Type.Name}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                if (oldTicket.DeveloperId != newTicket.DeveloperId)
                {
                    TicketHistory history = new TicketHistory()
                    {
                        TicketId = newTicket.Id,
                        Property = "Developer",
                        OldValue = oldTicket.Developer?.FullName ?? "Not Assigned",
                        NewValue = newTicket.Developer?.FullName,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket developer: {newTicket.Developer?.FullName}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            Company? company = await _context.Companies
                                                .Include(c => c.Projects)
                                                    .ThenInclude(p => p.Tickets)
                                                        .ThenInclude(t => t.History)
                                                            .ThenInclude(h => h.User)
                                                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company is null)
                return new List<TicketHistory>();

            List<Project> companyProjects = company.Projects.ToList();
            List<Ticket> companyTickets = companyProjects.SelectMany(p => p.Tickets).ToList();
            List<TicketHistory> companyTicketHistories = companyTickets.SelectMany(t => t.History).ToList();

            return companyTicketHistories;
        }

        public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            Project? project = await _context.Projects.Where(p => p.CompanyId == companyId)
                .Include(p => p.Tickets)
                    .ThenInclude(p => p.History)
                        .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project is null)
                return new List<TicketHistory>();

            List<Ticket> projectTickets = project.Tickets.ToList();
            List<TicketHistory> projectTicketHistories = projectTickets.SelectMany(t => t.History).ToList();

            return projectTicketHistories;
        }
    }
}
