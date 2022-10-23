using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IRoleService _roleService;

        public NotificationService(ApplicationDbContext context, 
            IEmailSender emailSender,
            IRoleService roleService)
        {
            _context = context;
            _emailSender = emailSender;
            _roleService = roleService;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Include(n => n.Recipient)
                .Include(n => n.Sender)
                .Include(n => n.Ticket)
                    .ThenInclude(t => t.Project)
                .Where(n => n.RecipientId == userId)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Include(n => n.Recipient)
                .Include(n => n.Sender)
                .Include(n => n.Ticket)
                    .ThenInclude(t => t.Project)
                .Where(n => n.SenderId == userId)
                .ToListAsync();
        }

        public async Task<bool> SendEmailNotificationAsync(Notification notification, string? emailSubject)
        {
            BugTrackerUser? recipient = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);

            if (recipient is null)
                return false;

            try
            {
                await _emailSender.SendEmailAsync(recipient.Email, emailSubject, notification.Message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
        {
            try
            {
                List<BugTrackerUser> roleMembers = await _roleService.GetUsersInRoleAsync(role, companyId);
                foreach(BugTrackerUser user in roleMembers)
                {
                    notification.RecipientId = user.Id;
                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task SendMembersEmailNotificationAsync(Notification notification, List<BugTrackerUser> members)
        {
            try
            {
                foreach (BugTrackerUser user in members)
                {
                    notification.RecipientId = user.Id;
                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
