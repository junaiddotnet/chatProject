using BlogDal;
using BlogDal.Abstract;
using blogProject.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace blogProject.Controllers
{
    public class PostController : Controller
    {
        private IpostRepository repository;
        // GET: Post
        public PostController(IpostRepository prmRepository)
        {
            repository = prmRepository;
        }
        [HttpPost, ValidateInput(false)]
        public RedirectToRouteResult postComment(int postId,string postName,string commenttext)
        {
            User logUser =(User) ControllerContext.HttpContext.Session["userId"];
            TempData["newComment"] = "New comment added Successfully .... ";
            repository.addComment(new comment() {
                postId = postId,
                commentDate = DateTime.Now,
                commentTxt = commenttext,
                userId = logUser.userId
            });
            ////------------------ This Part is for Test Purpose /only 
            blogProject.Views.Dashboard.dashMessage tmessage = new blogProject.Views.Dashboard.dashMessage();
            tmessage.commentId = 0;
            tmessage.commentTxt = commenttext;
            tmessage.commentDate = DateTime.Now;
            tmessage.postId = postId;
            tmessage.postName = postName;
            tmessage.userName = logUser.name;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<blogProject.Views.Dashboard.dashHub>();
            hubContext.Clients.All.displayMessage(tmessage);
            //---------------------------------------------------
            ////------------------ This Part is for Test Purpose /only 
            //blogProject.Views.Chat.message tmessage = new blogProject.Views.Chat.message();
            //tmessage.uid = 0;
            //tmessage.msg = commenttext;
            //tmessage.time = DateTime.Now;
            //tmessage.group = 1;
            //tmessage.name = "junaid";
            //tmessage.receiverId = 0;

            //var hubContext = GlobalHost.ConnectionManager.GetHubContext<blogProject.Views.Chat.PlaneHub>();
            //hubContext.Clients.All.displayMessage(tmessage);
            //---------------------------------------------------


            return RedirectToAction("viewPost", new { postId = postId });
        }
        
           
        
        public ActionResult viewPost(int postId)
        {
            int a = postId;
            postdetailViewMode selectPost = new postdetailViewMode();


            selectPost.postDetails = repository.posts.Where(p => p.postId == postId).FirstOrDefault();
            selectPost.postDetails.favPosts.Where(c => c.userId == 1 && postId == a);
            if (ControllerContext.HttpContext.Session["UserId"] != null)
            {
                User dbuser = (User)ControllerContext.HttpContext.Session["UserId"];
                selectPost.currentUser = dbuser;
                
            }
            else
            {
                selectPost.currentUser = new blogProject.User();
                
                
            }
            if (selectPost.postDetails==null)
            {
              return RedirectToAction("Index", "Home");
            }
            return View(selectPost);
        }
        public ActionResult postList(int categoryId)
        {
            categoryPostsViewModel postListViewModel = new categoryPostsViewModel();
            postListViewModel.posts = repository.posts.Where(p => p.categoryId == categoryId).ToList();
            if (postListViewModel.posts==null)
            {

            }
            postListViewModel.postCategory = repository.categories.Where(s => s.categoryId == categoryId).Select(c=>c.categoryName).FirstOrDefault();
            
            return View(postListViewModel);
        }
        
        public ActionResult viewPostbyMonth(string monthId,int year)
        {
            
            Dictionary<string, int> monthList = new Dictionary<string, int>();
            int month;
            for (int mon = 1; mon <= 12; mon++)
            {
                monthList.Add(DateTimeFormatInfo.CurrentInfo.GetMonthName(mon),mon );
                    
            }
            if (monthList.ContainsKey(monthId))
            {
                month = monthList[monthId];

            }
            else
            {
                month = DateTime.Now.Month;
                monthId = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
            }
            

            monthSelectPostViewModel viewPostbyMonth = new monthSelectPostViewModel();
            viewPostbyMonth.posts = repository.posts.Where(c => c.createDate.Month == month && c.createDate.Year==year ).ToList();
            viewPostbyMonth.monthName = monthId;
            viewPostbyMonth.year = year;
            TempData["selectMonth"] = monthId;
            return View(viewPostbyMonth);
        }
        public ActionResult recentPosts()
        {
            postViewModel listPosts = new postViewModel();
            listPosts.posts = repository.posts.OrderByDescending(c => c.createDate).ToList().Take(5);
            return View(listPosts);
        }
        public ActionResult recentComments()
        {
            commentsListViewModel commentsViewModel = new commentsListViewModel();
            commentsViewModel.commentsList = repository.comments.OrderByDescending(d => d.commentDate).Take(10);
            return View(commentsViewModel);

        }
        public ActionResult addToFavourite(int postId,int userId)
        {
            repository.addPosttoFavourite(userId, postId);
            return RedirectToAction("viewPost", new { postId = postId });

        }
        public ActionResult RemoveFromFavourite(int postId,int userId)
        {
            repository.RemovePosttoFavourite(userId, postId);
            return RedirectToAction("viewPost", new { postId = postId });

        }
        public ActionResult delComment(int commentId,string url)
        {
           repository.delComment(commentId);

           return Redirect(url);

        }
    }
}