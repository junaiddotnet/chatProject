using BlogDal;
using BlogDal.Abstract;
using blogProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace blogProject.Controllers
{
    public class HomeController : Controller
    {
           private IpostRepository repository;

         
        // GET: Home
        
       
        public HomeController(IpostRepository IpostRepository)
        {
            repository = IpostRepository;
        }
        public ActionResult Index()
        {
            post latestPost= repository.posts.OrderByDescending(c=>c.createDate).FirstOrDefault();
            postdetailViewMode postView = new postdetailViewMode();
            postView.postDetails = latestPost;
            postView.currentUser = new blogProject.User();

            //return RedirectToAction("viewPost","Post",new {postId=latestPost.postId });

            return RedirectToAction("ChatRoom", "Chat");
           // return View(postView);
           
        }
        public PartialViewResult welcomeScreen()
        {
            return PartialView("_welcomeScreen");
        }


    }
}