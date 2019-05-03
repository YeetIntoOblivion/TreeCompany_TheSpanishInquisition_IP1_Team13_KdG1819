using System;
using System.Collections;
using System.Collections.Generic;
using BL;
using DAL.Data_Transfer_Objects;
using Domain.Identity;
using Domain.Projects;
using Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UIMVC.Models;

namespace UIMVC.Controllers
{
    public class ProjectController : Controller
    {
        private ProjectManager _mgr;
        private PlatformManager _mgrPlatform;
        private readonly UserManager<UIMVCUser> _userManager;

        public ProjectController(UserManager<UIMVCUser> userManager)
        {
            _mgr = new ProjectManager();
            _mgrPlatform = new PlatformManager();
            _userManager = userManager;
        }

        #region Index

        public ActionResult Index()
        {
            var projects = _mgr.GetProjects();
            return View(projects);
        }

        public IActionResult Details(int id)
        {
            Project project = _mgr.GetProject(id, false);

            return View(project);
        }


        // GET /Project
        /*[Route("Project/Index/{id}")]
        public IActionResult Index(int id)
        {
            Project project = _mgr.GetProject(id, false);
            if (project == null)
            {
                return RedirectToAction("HandleErrorCode", "Errors", 404);
            }

            return View(project);
        }*/

        /*[HttpPost]
        public ActionResult Index(Project emp, List<Phase> dept)
        {
            emp.Phases = dept;
            return View(emp);
        }*/

        #endregion


        #region Create

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["platforms"] = _mgrPlatform.ReadAllPlatforms();

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(ProjectViewModel pvm, string user)
        {
            UIMVCUser projectUser = _userManager.GetUserAsync(HttpContext.User).Result;

            if (pvm == null)
            {
                return BadRequest("Project cannot be null");
            }

            /*pvm.Project.CurrentPhase = pvm.Phases.Find(e => e.IsCurrentPhase);

            Project project = new Project()
            {
                User = projectUser,
                CurrentPhase = pvm.Project.CurrentPhase,
                EndDate = pvm.Project.EndDate,
                StartDate = pvm.Project.StartDate,
                Title = pvm.Project.Title,
                Platform = new Platform() {Id = Int32.Parse(Request.Form["Platform"].ToString())},
                Status = pvm.Project.Status.ToUpper(),
                LikeVisibility = 1,
            };
            
            Project newProject = _mgr.MakeProject(project);
          
            foreach (var phase in pvm.Phases)
            {
                project.Phases.Add(phase);
                phase.Project = project;
                _mgr.MakePhase(phase, newProject.Id);
            }*/



            Project project1 = pvm.Project;
            project1.Platform = new Platform() {Id = Int32.Parse(Request.Form["Platform"].ToString())};
            project1.User = projectUser;
            project1.CurrentPhase = pvm.Phases.Find(e => e.IsCurrentPhase);

            if (project1.CurrentPhase == null)
            {
                return BadRequest("Phase cannot be null");
            }

            project1.Status = project1.Status.ToUpper();
            project1.LikeVisibility = 1;
            Project newProject = _mgr.MakeProject(project1);

            foreach (var newPhase in pvm.Phases)
            {
                newProject.Phases.Add(newPhase);
                newPhase.Project = newProject;
                _mgr.MakePhase(newPhase, project1.Id);
            }

            return RedirectToAction("Details", new {id = newProject.Id});
        }

        #endregion


        #region Update

        //[Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Project project = _mgr.GetProject(id, false);

            if (project == null)
            {
                return RedirectToAction("HandleErrorCode", "Errors", 404);
            }

            return View(project);
        }

        [HttpPost]
        //[Authorize]
        public IActionResult Edit(Project project)
        {
            _mgr.EditProject(project);
            // TODO: make the redirect work
            return RedirectToAction("Index");
        }

        #endregion


        //GET: /Project/Detail/<project_id>


        public IActionResult Delete(object id)
        {
            throw new System.NotImplementedException();
        }
    }
}