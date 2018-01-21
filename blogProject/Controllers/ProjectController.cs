using BlogDal;
using BlogDal.Abstract;
using blogProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace blogProject.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        private IprojectRepository projectRepository;
        private static string SessionKey = "userId";

        public ProjectController(IprojectRepository prmRepository)
        {
            projectRepository = prmRepository;
        }
        public bool CheckAdmin()
        {
            if (ControllerContext.HttpContext.Session[SessionKey] != null)
            {
                User currentUser = (User)ControllerContext.HttpContext.Session[SessionKey];
                if (currentUser.name.ToLower() != "admin")
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            return false;
        }
        public ActionResult recentProject()
        {
            projectListViewModel projectList = new projectListViewModel();
            projectList.projectList = projectRepository.projects.ToList();
            return View(projectList);
        }
        public PartialViewResult projectShoot(project prmProject)
        {
            return PartialView("_projectShoot",new projectViewModel() { ProjectView= prmProject } );
        }
        public PartialViewResult uploadShoot(project prmProject)
        {
            bool IsAuth = false;
            if (CheckAdmin()) IsAuth = true;
            else IsAuth = false;
                    
            return PartialView("uploadShoot",new projectViewModel() { IsAuthorize=IsAuth,ProjectView=prmProject});
        }
        public ActionResult saveProjectImg(HttpPostedFileBase file,int ProjectId)
        {
            if (file!=null && file.ContentLength > 0)
            {
               
                var fileName =ProjectId.ToString()+ Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~\\Images"), fileName);
                file.SaveAs(path);
                project prmProject = projectRepository.projects.Where(c => c.projectId == ProjectId).FirstOrDefault();
                // Delete the already save file to replace old one
                deleteFile(prmProject.projectImg);
                prmProject.projectImg = fileName;
                projectRepository.editProject(prmProject);
            } 
            return RedirectToAction("recentProject");
        }

        public void deleteFile(string fileName)
        {
            var path = Path.Combine(Server.MapPath("~\\Images"), fileName);
            System.IO.File.Delete(path);

        }
        
    }
}