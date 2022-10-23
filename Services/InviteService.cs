using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class InviteService : IInviteService
    {
        private readonly ApplicationDbContext _context;

        public InviteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            Invite? invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);

            if (invite is null)
                return false;

            invite.IsValid = false;
            invite.RecipientId = userId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddNewInviteAsync(Invite invite)
        {
            await _context.Invites.AddAsync(invite);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyInviteAsync(Guid token, string email, int companyId)
        {
            return await _context.Invites.Where(i => i.Id == companyId)
                                            .AnyAsync(i => (i.CompanyToken == token) && (i.RecipientEmail == email));
        }

        public async Task<Invite?> GetInviteAsync(int inviteId, int companyId)
        {
            return await _context.Invites.Where(i => i.CompanyId == companyId)
                                            .Include(i => i.Company)
                                            .Include(i => i.Project)
                                            .Include(i => i.Sender)
                                            .FirstOrDefaultAsync(i => i.Id == inviteId);
        }

        public async Task<Invite?> GetInviteAsync(Guid token, string email, int companyId)
        {
            return await _context.Invites.Where(i => i.CompanyId == companyId)
                                            .Include(i => i.Company)
                                            .Include(i => i.Project)
                                            .Include(i => i.Sender)
                                            .FirstOrDefaultAsync(i => (i.CompanyToken == token) && (i.RecipientEmail == email));
        }

        public async Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            if (token is null) 
                return false;

            Invite? invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);
            if (invite is null) 
                return false;

            int inviteAgeInDays = (DateTime.Now - invite.SendDate.DateTime).Days;
            if (inviteAgeInDays > 7)
                return false;

            return invite.IsValid;
        }
    }
}
