using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Company> GetCompanyInfoByIdAsync(int? companyId);

        Task<List<BugTrackerUser>> GetAllMembersAsync(int companyId);

        Task<List<Project>> GetAllProjectsAsync(int companyId);

        Task<List<Ticket>> GetAllTicketsAsync(int companyId);
    }
}
