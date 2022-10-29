using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Extensions;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using BugTracker.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoleService _roleService;
        private readonly ILookupService _lookupService;
        private readonly IFileService _fileService;
        private readonly IProjectService _projectService;
        private readonly ICompanyInfoService _companyInfoService;
        private readonly UserManager<BugTrackerUser> _userManager;

        public ProjectsController(ApplicationDbContext context,
            IRoleService roleService,
            ILookupService lookupService,
            IFileService fileService,
            IProjectService projectService,
            UserManager<BugTrackerUser> userManager,
            ICompanyInfoService companyInfoService)
        {
            _context = context;
            _roleService = roleService;
            _lookupService = lookupService;
            _fileService = fileService;
            _projectService = projectService;
            _userManager = userManager;
            _companyInfoService = companyInfoService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int companyId = User.Identity!.GetCompanyId();
            var companyProjects = await _companyInfoService.GetAllProjectsAsync(companyId);

            return View(companyProjects);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User);
            List<Project> userProjects = await _projectService.GetUserProjectsAsync(userId);

            return View(userProjects);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AllProjects()
        {
            int companyId = User.Identity!.GetCompanyId();
            List<Project> companyProjects;

            if (User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)))
                companyProjects = await _companyInfoService.GetAllProjectsAsync(companyId);
            else
                companyProjects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);

            return View(companyProjects);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ArchivedProjects()
        {
            int companyId = User.Identity!.GetCompanyId();
            List<Project> archivedProjects = await _projectService.GetArchivedProjectsByCompanyAsync(companyId);

            return View(archivedProjects);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return NotFound();

            int companyId = User.Identity!.GetCompanyId();

            Project? project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project is null)
                return NotFound();

            return View(project);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            AddProjectWithPMViewModel model = new();
            model.Project = new Project();

            int companyId = User.Identity!.GetCompanyId();

            List<BugTrackerUser> projectManagers = await _roleService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId);
            model.ProjectManagerList = new SelectList(projectManagers, "Id", "FullName");

            List<ProjectPriority> projectPriorities = await _lookupService.GetProjectPrioritiesAsync();
            model.PriorityList = new SelectList(projectPriorities, "Id", "Name");

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            if (model is null)
                return RedirectToAction(nameof(Create));

            int companyId = User.Identity!.GetCompanyId();
            Project project = model.Project;
            

            if (project.ImageFile is not null)
            {
                project.ImageData = await _fileService.ConvertFileToByteArrayAsync(project.ImageFile);
                project.ImageFileName = project.ImageFile.FileName;
                project.ImageType = project.ImageFile.ContentType;
            }

            project.CompanyId = companyId;

            await _projectService.AddNewProjectAsync(model.Project);

            if (!string.IsNullOrEmpty(model.ProjectManagerId))
            {
                await _projectService.AddProjectManagerAsync(model.ProjectManagerId, model.Project.Id);
            }
            
            return View(nameof(Details), project);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
                return NotFound();

            AddProjectWithPMViewModel model = new();
            int companyId = User.Identity!.GetCompanyId();
            Project? project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project is null)
                return NotFound();

            model.Project = project;

            List<BugTrackerUser> projectManagers = await _roleService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId);
            BugTrackerUser? projectManager = await _projectService.GetProjectManagerAsync(project.Id);

            if (projectManager is null)
                model.ProjectManagerList = new SelectList(projectManagers, "Id", "FullName");
            else
                model.ProjectManagerList = new SelectList(projectManagers, "Id", "FullName", projectManager.Id);

            List<ProjectPriority> projectPriorities = await _lookupService.GetProjectPrioritiesAsync();
            model.PriorityList = new SelectList(projectPriorities, "Id", "Name", project.PriorityId);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            if (model is null)
                return RedirectToAction(nameof(Create));

            Project project = model.Project;

            if (project.ImageFile is not null)
            {
                project.ImageData = await _fileService.ConvertFileToByteArrayAsync(project.ImageFile);
                project.ImageFileName = project.ImageFile.FileName;
                project.ImageType = project.ImageFile.ContentType;
            }

            await _projectService.UpdateProjectAsync(model.Project);

            if (!string.IsNullOrEmpty(model.ProjectManagerId))
            {
                await _projectService.AddProjectManagerAsync(model.ProjectManagerId, model.Project.Id);
            }

            return View(nameof(Details), project);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id is null || _context.Projects is null)
                return NotFound();

            int companyId = User.Identity!.GetCompanyId();

            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project is null || project.Archived)
                return NotFound();

            return View(project);
        }

        [Authorize]
        [HttpPost, ActionName(nameof(Archive))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            int companyId = User.Identity!.GetCompanyId();

            var project = await _projectService.GetProjectByIdAsync(id, companyId);
            if (project is null)
                return RedirectToAction(nameof(Archive));

            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id is null || _context.Projects is null)
                return NotFound();

            int companyId = User.Identity!.GetCompanyId();

            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project is null || !project.Archived)
                return NotFound();

            return View(project);
        }

        [Authorize]
        [HttpPost, ActionName(nameof(Restore))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            int companyId = User.Identity!.GetCompanyId();

            var project = await _projectService.GetProjectByIdAsync(id, companyId);
            if (project is null)
                return RedirectToAction(nameof(Archive));

            await _projectService.RestoreArchivedProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }

    }
}
