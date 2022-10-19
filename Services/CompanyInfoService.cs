using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class CompanyInfoService : ICompanyInfoService
    {
        private readonly ApplicationDbContext _context;

        public CompanyInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BugTrackerUser>> GetAllMembersAsync(int companyId)
        {
            return await _context.Users
                .Where(u => u.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<List<Project>> GetAllProjectsAsync(int companyId)
        {
            return await _context.Projects
                .Where(p => p.CompanyId == companyId)
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

        public async Task<List<Ticket>> GetAllTicketsAsync(int companyId)
        {
            return (await GetAllProjectsAsync(companyId))
                .SelectMany(p => p.Tickets)
                .ToList();
        }

        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            if (companyId is null)
                return new Company();

            return await _context.Companies
                .Include(c => c.Members)
                .Include(c => c.Projects)
                .Include(c => c.Invites)
                .FirstOrDefaultAsync(c => c.Id == companyId);
        }
    }
}
