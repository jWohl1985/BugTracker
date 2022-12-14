using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BugTracker.Extensions;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using BugTracker.Models.ViewModels;

namespace BugTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ITicketService _ticketService;
        private readonly ITicketHistoryService _historyService;
        private readonly ILookupService _lookupService;
        private readonly IFileService _fileService;
        private readonly UserManager<BugTrackerUser> _userManager;
        
        public TicketsController(
            IProjectService projectService,
            ITicketService ticketService,
            ITicketHistoryService historyService,
            ILookupService lookupService,
            IFileService fileService,
            UserManager<BugTrackerUser> userManager
            )
        {
            _projectService = projectService;
            _ticketService = ticketService;
            _historyService = historyService;
            _fileService = fileService;
            _lookupService = lookupService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> MyTickets()
        {
            BugTrackerUser currentUser = await _userManager.GetUserAsync(User);
            int companyId = User.Identity!.GetCompanyId();

            List<Ticket> userTickets = await _ticketService.GetTicketsByUserIdAsync(currentUser.Id, companyId);

            return View(userTickets);
        }

        [HttpGet]
        public async Task<IActionResult> AllTickets()
        {
            int companyId = User.Identity!.GetCompanyId();

            List<Ticket> allTickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);

            if (User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)))
            {
                return View(allTickets);
                
            }
            else
            {
                IEnumerable<Ticket> activeTickets = allTickets.Where(t => !t.Archived);
                return View(activeTickets);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ArchivedTickets()
        {
            int companyId = User.Identity!.GetCompanyId();
            List<Ticket> archivedTickets = await _ticketService.GetArchivedTicketsAsync(companyId);

            return View(archivedTickets);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> UnassignedTickets()
        {
            int companyId = User.Identity!.GetCompanyId();
            string userId = _userManager.GetUserId(User);
            List<Ticket> allUnassignedTickets = await _ticketService.GetUnassignedTicketsAsync(companyId);

            if (User.IsInRole(nameof(Roles.Admin)))
            {
                return View(allUnassignedTickets);
            }
            else // User is project manager, get the unassigned tickets for their project
            {
                List<Ticket> myUnassignedTickets = new();
                foreach(Ticket ticket in allUnassignedTickets)
                {
                    if (await _projectService.IsAssignedProjectManagerAsync(userId, ticket.ProjectId))
                        myUnassignedTickets.Add(ticket);
                }

                return View(myUnassignedTickets);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> AssignDeveloper(int id)
        {
            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket is null)
                return NotFound();

            AssignDeveloperViewModel model = new();
            model.Ticket = ticket;
            List<BugTrackerUser> developers = await _projectService.GetProjectMembersByRoleAsync(model.Ticket.ProjectId, nameof(Roles.Developer));
            model.DeveloperList = new SelectList(developers, "Id", "FullName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        {
            if (model.DeveloperId is null)
                return RedirectToAction(nameof(AssignDeveloper), new { id = model.Ticket.Id });

            BugTrackerUser user = await _userManager.GetUserAsync(User);
            Ticket? oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);

            await _ticketService.AssignTicketAsync(model.Ticket.Id, model.DeveloperId);

            Ticket? newTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);

            await _historyService.AddHistoryAsync(oldTicket!, newTicket!, user.Id);

            return RedirectToAction(nameof(Details), new { id = model.Ticket.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return NotFound();

            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket is null)
                return NotFound();

            return View(ticket);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await GenerateTicketCreationViewData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,Title,Description,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            RemoveNavigationPropertyModelErrors(ticket);

            if (!ModelState.IsValid)
            {
                await GenerateTicketCreationViewData();
                return View(ticket);
            }

            ticket.Created = DateTimeOffset.Now;
            ticket.CreatorId = (await _userManager.GetUserAsync(User)).Id;
            ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync(nameof(TickStatus.New))).Value;

            await _ticketService.AddNewTicketAsync(ticket);

            BugTrackerUser user = await _userManager.GetUserAsync(User);
            Ticket? newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
            await _historyService.AddHistoryAsync(null, newTicket!, user.Id);

            return RedirectToAction(nameof(AllTickets));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
                return NotFound();

            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket is null)
                return NotFound();

            await GenerateTicketEditViewData(ticket);
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,CreatorId,DeveloperId,Title,Description,Created,Archived")] Ticket ticket)
        {
            if (id != ticket.Id)
                return NotFound();

            RemoveNavigationPropertyModelErrors(ticket);

            if (!ModelState.IsValid)
            {
                await GenerateTicketEditViewData(ticket);
                return View(ticket);
            }

            BugTrackerUser user = await _userManager.GetUserAsync(User);
            Ticket? oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);

            try
            {
                ticket.Updated = DateTimeOffset.Now;
                await _ticketService.UpdateTicketAsync(ticket);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TicketExists(ticket.Id))
                    return NotFound();
                else
                    throw;
            }

            Ticket? newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);

            await _historyService.AddHistoryAsync(oldTicket!, newTicket!, user.Id);

            return RedirectToAction(nameof(AllTickets));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketComment([Bind("Id,TicketId,Comment")]TicketComment ticketComment)
        {
            ModelState.Remove(nameof(ticketComment.User));
            ModelState.Remove(nameof(ticketComment.Ticket));
            ModelState.Remove(nameof(ticketComment.UserId));

            if (ModelState.IsValid)
            {
                ticketComment.UserId = _userManager.GetUserId(User);
                ticketComment.Created = DateTimeOffset.Now;
                await _ticketService.AddTicketCommentAsync(ticketComment);
                await _historyService.AddHistoryAsync(ticketComment.TicketId, "comment", ticketComment.UserId);
            }

            return RedirectToAction(nameof(Details), new { id = ticketComment.TicketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment([Bind("Id,FormFile,Description,TicketId")]TicketAttachment ticketAttachment)
        {
            ModelState.Remove(nameof(ticketAttachment.FileData));
            ModelState.Remove(nameof(ticketAttachment.FileName));
            ModelState.Remove(nameof(ticketAttachment.FileType));
            ModelState.Remove(nameof(ticketAttachment.Ticket));
            ModelState.Remove(nameof(ticketAttachment.User));
            ModelState.Remove(nameof(ticketAttachment.FormFile));
            ModelState.Remove(nameof(ticketAttachment.UserId));

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Details), new { id = ticketAttachment.TicketId });
            }

            ticketAttachment.FileData = await _fileService.ConvertFileToByteArrayAsync(ticketAttachment.FormFile);
            ticketAttachment.FileName = ticketAttachment.FormFile.FileName;
            ticketAttachment.FileType = ticketAttachment.FormFile.ContentType;
            ticketAttachment.Created = DateTimeOffset.Now;
            ticketAttachment.UserId = _userManager.GetUserId(User);

            await _ticketService.AddTicketAttachmentAsync(ticketAttachment);
            await _historyService.AddHistoryAsync(ticketAttachment.TicketId, "attachment", ticketAttachment.UserId);

            return RedirectToAction(nameof(Details), new { id = ticketAttachment.TicketId });
        }

        [HttpGet]
        public async Task<IActionResult> ShowFile(int id)
        {
            TicketAttachment? ticketAttachment = await _ticketService.GetTicketAttachmentByIdAsync(id);

            if (ticketAttachment is null)
                return NotFound();

            string fileName = ticketAttachment.FileName;
            byte[] fileData = ticketAttachment.FileData;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id is null)
                return NotFound();

            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket is null)
                return NotFound();

            return View(ticket);
        }

        [HttpPost, ActionName(nameof(Archive))]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket is null)
                return NotFound();

            await _ticketService.ArchiveTicketAsync(ticket);
            return RedirectToAction(nameof(AllTickets));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id is null)
                return NotFound();

            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket is null)
                return NotFound();

            return View(ticket);
        }

        [HttpPost, ActionName(nameof(Restore))]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket is null)
                return NotFound();

            ticket.Archived = false;
            await _ticketService.UpdateTicketAsync(ticket);

            return RedirectToAction(nameof(Index));
        }

        #region Helper Methods
        private async Task<bool> TicketExists(int id)
        {
            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id);
            return ticket is not null;
        }

        private async Task GenerateTicketCreationViewData()
        {
            BugTrackerUser currentUser = await _userManager.GetUserAsync(User);
            int companyId = User.Identity!.GetCompanyId();

            if (User.IsInRole(nameof(Roles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyIdAsync(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(currentUser.Id), "Id", "Name");
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");
        }

        private async Task GenerateTicketEditViewData(Ticket ticket)
        {
            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);
        }

        private void RemoveNavigationPropertyModelErrors(Ticket ticket)
        {
            ModelState.Remove(nameof(ticket.Developer));
            ModelState.Remove(nameof(ticket.Creator));
            ModelState.Remove(nameof(ticket.Project));
            ModelState.Remove(nameof(ticket.Priority));
            ModelState.Remove(nameof(ticket.Status));
            ModelState.Remove(nameof(ticket.Type));
        }
        #endregion
    }
}
