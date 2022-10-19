using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    public class InvitesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvitesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Invites
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invites.Include(i => i.Company).Include(i => i.Project).Include(i => i.Recipient).Include(i => i.Sender);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Invites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Invites == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites
                .Include(i => i.Company)
                .Include(i => i.Project)
                .Include(i => i.Recipient)
                .Include(i => i.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        // GET: Invites/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            ViewData["RecipientId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Invites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyId,ProjectId,SenderId,RecipientId,SendDate,JoinDate,CompanyToken,RecipientEmail,RecipientFirstName,RecipientLastName,IsValid")] Invite invite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", invite.CompanyId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", invite.ProjectId);
            ViewData["RecipientId"] = new SelectList(_context.Users, "Id", "Id", invite.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", invite.SenderId);
            return View(invite);
        }

        // GET: Invites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Invites == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites.FindAsync(id);
            if (invite == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", invite.CompanyId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", invite.ProjectId);
            ViewData["RecipientId"] = new SelectList(_context.Users, "Id", "Id", invite.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", invite.SenderId);
            return View(invite);
        }

        // POST: Invites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,ProjectId,SenderId,RecipientId,SendDate,JoinDate,CompanyToken,RecipientEmail,RecipientFirstName,RecipientLastName,IsValid")] Invite invite)
        {
            if (id != invite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InviteExists(invite.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", invite.CompanyId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", invite.ProjectId);
            ViewData["RecipientId"] = new SelectList(_context.Users, "Id", "Id", invite.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", invite.SenderId);
            return View(invite);
        }

        // GET: Invites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Invites == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites
                .Include(i => i.Company)
                .Include(i => i.Project)
                .Include(i => i.Recipient)
                .Include(i => i.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        // POST: Invites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Invites == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Invites'  is null.");
            }
            var invite = await _context.Invites.FindAsync(id);
            if (invite != null)
            {
                _context.Invites.Remove(invite);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InviteExists(int id)
        {
          return _context.Invites.Any(e => e.Id == id);
        }
    }
}
