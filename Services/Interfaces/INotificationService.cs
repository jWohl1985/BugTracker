using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface INotificationService
    {
        Task AddNotificationAsync(Notification notification);

        Task<List<Notification>> GetReceivedNotificationsAsync(string userId);

        Task<List<Notification>> GetSentNotificationsAsync(string userId);

        Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role);

        Task SendMembersEmailNotificationAsync(Notification notification, List<BugTrackerUser> members);

        Task<bool> SendEmailNotificationAsync(Notification notification, string? emailSubject);
    }
}
