using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BugTracker.Extensions;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BugTrackerUser> _userManager;
        private readonly IProjectService _projectService;
        private readonly ILookupService _lookupService;
        private readonly ITicketService _ticketService;

        public TicketsController(ApplicationDbContext context,
            UserManager<BugTrackerUser> userManager,
            IProjectService projectService,
            ILookupService lookupService,
            ITicketService ticketService)
        {
            _context = context;
            _userManager = userManager;
            _projectService = projectService;
            _lookupService = lookupService;
            _ticketService = ticketService;
        }


        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tickets.Include(t => t.Creator).Include(t => t.Developer).Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status).Include(t => t.Type);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Developer)
                .Include(t => t.Priority)
                .Include(t => t.Project)
                .Include(t => t.Status)
                .Include(t => t.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await GenerateTicketCreationViewData();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,Title,Description,TicketTypeId,TicketPriorityId,")] Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                await GenerateTicketCreationViewData();
                return View(ticket);
            }

            ticket.Created = DateTimeOffset.Now;
            ticket.CreatorId = (await _userManager.GetUserAsync(User)).Id;
            ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync(nameof(TickStatus.New))).Value;

            await _ticketService.AddNewTicketAsync(ticket);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null || _context.Tickets is null)
                return NotFound();

            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket is null)
                return NotFound();

            await GenerateTicketEditViewData(ticket);
            return View(ticket);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,CreatorId,DeveloperId,Title,Description,Created,Archived")] Ticket ticket)
        {
            if (id != ticket.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await GenerateTicketEditViewData(ticket);
                return View(ticket);
            }

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

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id is null)
                return NotFound();

            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket is null)
                return NotFound();

            return View(ticket);
        }

        [Authorize]
        [HttpPost, ActionName(nameof(Archive))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket is null)
                return NotFound();

            await _ticketService.ArchiveTicketAsync(ticket);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id is null)
                return NotFound();

            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket is null)
                return NotFound();

            return View(ticket);
        }

        [Authorize]
        [HttpPost, ActionName(nameof(Restore))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            Ticket? ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket is null)
                return NotFound();

            ticket.Archived = false;
            await _ticketService.UpdateTicketAsync(ticket);

            return RedirectToAction(nameof(Index));
        }

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
    }
}
