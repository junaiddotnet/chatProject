using BlogDal.Abstract;
using blogProject.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace blogProject.Controllers
{
    public class NavController : Controller
    {
        private IpostRepository repositoty;
        // GET: Nav
        public NavController(IpostRepository prmRepositoty)
        {
            repositoty = prmRepositoty;
        }
        [ChildActionOnly]
      //  [OutputCache(Duration =60)]
        public PartialViewResult postCategory()
        {
            categoriesViewModel catViewModel = new categoriesViewModel();
            catViewModel.listCategories =  repositoty.categories.ToList();

            return  PartialView(catViewModel);
        }
        [ChildActionOnly]
      //  [OutputCache(Duration = 60)]
        public PartialViewResult postList ()
        {
            postViewModel listPosts = new postViewModel();
            listPosts.posts = repositoty.posts.OrderByDescending(c=>c.createDate).Take(10);
            return PartialView(listPosts);
        }
        [ChildActionOnly]
       // [OutputCache(Duration = 60)]
        public PartialViewResult recentComments()
        {
            commentsListViewModel commentsViewModel = new commentsListViewModel();
            commentsViewModel.commentsList = repositoty.comments.OrderByDescending(d => d.commentDate).Take(10);
            return PartialView(commentsViewModel);

        }
        [ChildActionOnly]
       // [OutputCache(Duration = 60)]
        public PartialViewResult archiveList()
        {
            archiveViewMode listViewMonth = new archiveViewMode();
            //listViewMonth.monthsList = new Dictionary<string, int>();
           // var archiveList=    repositoty.posts.Select(c => new { c.createDate.Month, c.createDate.Year }).Distinct();
            listViewMonth.listArchive = (IList<archive>)
                                        (from post in repositoty.posts
                                         select new archive {Month=  post.createDate.Month,Year=post.createDate.Year }).Distinct().ToList();

            //for (int month =1; month<=12;month++)
            //{
            //    var value = repositoty.posts.Where(p => p.createDate.Month == month).Count();
            //    listViewMonth.monthsList.Add(DateTimeFormatInfo.CurrentInfo.GetMonthName(month) , value);
            //}
            //foreach (var mon in archiveList)
            //{
            //    listViewMonth.monthsList.Add(DateTimeFormatInfo.CurrentInfo.GetMonthName(mon.Month)+"(" + mon.Year + ")"  , mon.Year);
            //}
            foreach (var arch in listViewMonth.listArchive)
            {
                arch.MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(arch.Month);
            }

           
            return PartialView(listViewMonth);
        }

        
        public PartialViewResult LogInnandOut()
        {
            
            LoguserViewModel logUser = new LoguserViewModel();
           
            if ( ControllerContext.HttpContext.Session["UserId"]!=null)
            {
                User dbuser = (User) ControllerContext.HttpContext.Session["UserId"];
                logUser.userName = dbuser.name;
                logUser.Id = dbuser.userId;
            }

            return PartialView(logUser);

        }
        
        public User getUserId ()
        {
            User dbuser = new User();
            if (ControllerContext.HttpContext.Session["UserId"] != null)
            {
                 dbuser = (User)ControllerContext.HttpContext.Session["UserId"];
              
            }
            return dbuser;

        }
        [ChildActionOnly]

        public PartialViewResult leaveComment(int postId,string postName)
        {
            leaveCommentModel dbcomment = new leaveCommentModel();
            dbcomment.postId = postId;
            dbcomment.postName = postName;
            if (ControllerContext.HttpContext.Session["UserId"] != null)
            {
                dbcomment.loged = true;
            }
            else
            {
                dbcomment.loged = false;
            }
            return PartialView(dbcomment);
        }
        public PartialViewResult AdminOptions()
        {
            return PartialView();
        }
        public PartialViewResult postFavourite()
        {
            User userInfo = getUserId();
            favouriteViewModel favPostList = new favouriteViewModel();
            if (userInfo.userId!=0)
            {
                favPostList.userId = userInfo.userId;

                favPostList.favPostList = repositoty.favPosts.Where(d => d.userId == userInfo.userId).OrderByDescending(c => c.favDate).Take(10);

            }
            else
            {
                favPostList.userId = 0;

            }

            return PartialView(favPostList);
        }
    }
}