using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class LookupService : ILookupService
    {
        private readonly ApplicationDbContext _context;

        public LookupService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            return await _context.ProjectPriorities.ToListAsync();
        }

        public async Task<List<TicketPriority>> GetTicketPrioritiesAsync()
        {
            return await _context.TicketPriorities.ToListAsync();
        }

        public async Task<List<TicketStatus>> GetTicketStatusesAsync()
        {
            return await _context.TicketStatuses.ToListAsync();
        }

        public async Task<List<TicketType>> GetTicketTypesAsync()
        {
            return await _context.TicketTypes.ToListAsync();
        }
    }
}
