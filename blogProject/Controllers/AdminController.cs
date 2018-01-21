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
  
    public class AdminController : Controller
    {
        private static string SessionKey = "userId";
          
        public class postValue
        {
            public int postId { get; set; }
            public string postText { get; set; }
            public string postName { get; set; }
            public int categoryId { get; set; }
        }
        public class categoryValue
        {
            public int categoryId { get; set; }
            public string categoryName { get; set; }

        }
        // GET: Admin
        private IpostRepository Postrepository;
        private IadminRepository adminRepository;
        public AdminController(IpostRepository prmRepository,IadminRepository prmadminRepository)
        {
            Postrepository = prmRepository;
            adminRepository = prmadminRepository;
        }
        public ActionResult PostList()
        {
            postViewModel postListViewModel = new postViewModel();
            if (ControllerContext.HttpContext.Session[SessionKey]==null)
            {
                return RedirectToAction("LogPagee", "Account", new { returnUrl =Request.RawUrl});
            }
            if (ControllerContext.HttpContext.Session[SessionKey]!=null)
            {
                User currentUser = (User)ControllerContext.HttpContext.Session[SessionKey];
                if (currentUser.name.ToLower()!="admin")
                {
                    return RedirectToAction("greetMessage", new messageViewModel() { userName = currentUser.name, message = " You Have No access for this section of web site ???" });
                }

            }
            postListViewModel.posts = Postrepository.posts.ToList();
            return View();
        }

        public ActionResult EditPost(int postid=0)
        {
            var value =Postrepository.posts.Where(c => c.postId == postid).Select(x=> new {postId=x.postId,postText=x.postText }).FirstOrDefault();
            postValue EditPostViewModel = new postValue();
            EditPostViewModel.postId = value.postId;
            EditPostViewModel.postText = value.postText;
            return View(EditPostViewModel);
        }

        [HttpPost]
        public ActionResult EditPosts(postValue value)
        {
            post dbPost = new BlogDal.post() { postId = value.postId, postName = value.postName, postText = value.postText, categoryId = value.categoryId };
            Postrepository.Save(dbPost);
            //return RedirectToAction("PostList","Admin");
            var postView = Postrepository.posts.Select(x => new { postId = x.postId, postname = x.postName, posttext = x.postText, cratedOn = x.createDate, commentCount = x.comments.Count() }).Where(c => c.postId == dbPost.postId).FirstOrDefault();

            return Json(postView, JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public ActionResult saveCategory(categoryValue catValue)
        {
            categorie dbCategory = new categorie();
            dbCategory.categoryId = catValue.categoryId;
            dbCategory.categoryName = catValue.categoryName;
            Postrepository.savePostCategory(dbCategory);
            catValue.categoryId = dbCategory.categoryId;
            return Json(catValue, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult jsViewPost ( int postId)
        {
            var postDetails = Postrepository.posts.Where(c => c.postId == postId).Select(x => new {postId=x.postId,postname=x.postName,posttext=x.postText, categoryId=x.categoryId }).FirstOrDefault();
            return Json(postDetails, JsonRequestBehavior.AllowGet);
        }
        public ActionResult jsGetCategory ()
        {
            var cateoriesList = Postrepository.categories.Select(x => new {categoryId=x.categoryId,categoryName=x.categoryName });
            return Json(cateoriesList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult show()
        {
            return View();
        }
        public ActionResult jsViewPostList()
        {
            var PostListView = Postrepository.posts.Select(x => new { postId = x.postId, postname = x.postName, posttext = x.postText,cratedOn=x.createDate,commentCount=x.comments.Count() }).ToList();
            return Json(PostListView, JsonRequestBehavior.AllowGet);

        }
        public ActionResult jsViewComments(int postId)
        {
            var postComments = Postrepository.comments.Where(c => c.postId == postId).Select(x => new {commentId=x.commentId,commentTxt=x.commentTxt,commentDate=x.commentDate }).ToList();
            return Json(postComments, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult deleteComments(int commentId=0)
        {
            Postrepository.delComment(commentId);
            return Json(commentId.ToString(), JsonRequestBehavior.AllowGet);

        }
       
        public ActionResult greetMessage(messageViewModel greetMessage)
        {
            return View("messagePage", greetMessage);
        }
        public ActionResult Contact(contactus ContactUs )
        {
            ContactUs.contactDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                adminRepository.ContactUs(ContactUs);
                messageViewModel greetMessage = new messageViewModel();
                greetMessage.userName = ContactUs.contactName;
                greetMessage.message = "Thaks for Your Message .. I will contact you soon ??";
                // Save the Message to database and then redirect the page to GreetingPage
                return RedirectToAction("greetMessage", greetMessage);
            }
            else
            {
                return View(ContactUs);
            }
           
        }
       

    }
}