using BlogDal;
using BlogDal.Abstract;
using blogProject.Helper;
using blogProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;  

namespace blogProject.Controllers
{
    public class AccountController : Controller
    {           
        // GET: Account    
        private static string SessionKey = "userId";
        private IuserRepository repository;
        private IpostRepository postRepository;
        private IgroupRepository groupRepositoty;

        public AccountController(IuserRepository prmUserRepository, IpostRepository prmPostRepository,IgroupRepository prmgroupRepository)
        {
            repository = prmUserRepository;
            postRepository = prmPostRepository;
            groupRepositoty = prmgroupRepository;
        }
     

        public ActionResult SignIn(user prmUser )
        {
            Random rnd = new Random();
            int number = rnd.Next(1, 99);
            TempData["keyCode"] = number.ToString();
            ViewBag.returnUrl = Request["returnUrl"];
            if (ModelState.IsValid)
            {
               if (prmUser.keyCode != prmUser.confirmKeyCode)
                {
                    ViewBag.invalid = "Key code not correct... ";
                    
                    return View("LogPagee", prmUser);
                }
                user dbUser = repository.verifyUser(prmUser);
             //    FormsAuthentication.SetAuthCookie("junaid",false); // This statment is for Temporarly purpose  only 
                if (dbUser!=null)
                {
                    User usr = new User();
                    usr.userId = dbUser.userId;
                    usr.name = dbUser.userName;
                    this.HttpContext.Session.Timeout = 60;
                    ControllerContext.HttpContext.Session[SessionKey] = usr;
                    var ip = this.Request.ServerVariables["REMOTE_ADDR"];
                    ip= "92.0.164.122";
                    var country= GeoLocation.IPRequestHelper(ip);
                    repository.saveCountry(dbUser.userId, country);
                    return RedirectToAction("chatRoom", "chat");
                }
                else
                {
                    ViewBag.invalid = "Details Provided are invalid .... ";
                    ViewBag.returnUrl = Request["returnUrl"];
                    return View("LogPagee", prmUser);
                }
            } // end if
            
            return View("LogPagee",prmUser);

            
        } // end of action method...
        [HttpPost]
        public ActionResult Register(user prmUser)
        {
            ViewBag.returnUrl = Request["returnUrl"];
            Random rnd = new Random();
            int number = rnd.Next(1, 99);
            TempData["keyCode"] = number.ToString();

            if (ModelState.IsValid)
            {
                if (prmUser.keyCode!=prmUser.confirmKeyCode)
                {
                    ViewBag.alreadyuser = "Key code not correct... ";

                    return View("LogPagee", prmUser);
                }
                if (prmUser.password!=prmUser.confirmPassword)
                {
                    ViewBag.alreadyuser = "Password and Confirm Password not Match  .... ";
                    return View("LogPagee", prmUser);
                }
                else if (repository.checkUser(prmUser))
                {
                    ViewBag.alreadyuser = "User name Already Exist Please seek alternative.. ";
                    return View("LogPagee", prmUser);
                }

                else
                {
                    repository.Create(prmUser);
                    User usr = new User();
                    usr.userId = prmUser.userId;
                    usr.name = prmUser.userName;
                    ControllerContext.HttpContext.Session[SessionKey] = usr;
                    //return Redirect(Request["returnUrl"]);
                    return RedirectToAction("chatRoom", "chat");
                }
                
            }
            else
            {
                return View("LogPagee", prmUser);
            }
            return View("LogPagee", prmUser);
        }
        public ActionResult LogOut(string returnUrl)
        {
            ControllerContext.HttpContext.Session.Clear();

            return Redirect(returnUrl);
        }
        public ActionResult LogPagee(string returnUrl)
        {
            //var d = repository.chats.ToList();
            ViewBag.returnUrl = returnUrl;
            Random rnd = new Random();
            int number = rnd.Next(1, 99);
            TempData["keyCode"] = number.ToString();
            return View(new user() { keyCode=number.ToString()});
        }
        [HttpPost]
        public ActionResult addNewUser(user dbUser)
        {
            User dbNew = new User(); 
            if (repository.checkUser(dbUser))
            {
                dbNew.userId = 0;
                dbNew.name = "User Alredy exist .. ";
                return Json(dbNew , JsonRequestBehavior.AllowGet);
            }
            else
            {
                repository.Create(dbUser);
                dbNew.userId = dbUser.userId;
                dbNew.name = "User Added Successfully .. ";
                return Json(dbNew, JsonRequestBehavior.AllowGet);
            }

        }
        public FileResult downloadCV(string fileName)
        {
           return new FilePathResult("~\\App_Data\\" + fileName, System.Net.Mime.MediaTypeNames.Application.Octet);

          // below is anoter way to download file as stream 

            //FileStream fs = new FileStream( (Server.MapPath("~\\App_Data\\" + fileName)), FileMode.Open);
            //FileStreamResult fsResult = new FileStreamResult(fs, "Text");
            //return fsResult;



        }
        public PartialViewResult CvView()
        {
            cvViewModel cvModel = new cvViewModel();
            // Before showing the Cv we have to fetch alrady loaded file contents ...
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~\\App_Data"));
            //var path = System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data" );

            
           // var dir = new System.IO.DirectoryInfo(path);


            System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
            List<string> fileItems = new List<string>();
            foreach (var file in fileNames)
            {
                fileItems.Add(file.Name);
            }
            cvModel.fileList = fileItems;
            if (CheckAdmin())
            {
                cvModel.IsAuthorize = true;
            }
            else
            {
                cvModel.IsAuthorize = false;
            }
            return PartialView(cvModel);
        }
        public bool CheckAdmin ()
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
        [HttpPost]
        public ActionResult saveCv(HttpPostedFileBase file)
        {
            
            if (file.ContentLength>0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~\\App_Data"), fileName);
                file.SaveAs(path);
            }
            return RedirectToAction("Profile");
        }
        public ActionResult deleteFile (string fileName)
        {
            var path = Path.Combine(Server.MapPath("~\\App_Data"), fileName);
            System.IO.File.Delete(path);
            return RedirectToAction("Profile");

        }
        public ActionResult delUser(int userId=0)
        {
            var result= repository.deleteUser(userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Profile()
        {
            return View();
        }
        public ActionResult myProfile()
        {
            //if (ControllerContext.HttpContext.Session["UserId"] == null)
            //{
            //    ViewBag.invalid = "You need to give Log Dtails to Start Chatting ....";
            //  return RedirectToAction("LogPagee", "Account",new {returnUrl= Request.RawUrl } );
            //}
            LoguserViewModel logUser = new LoguserViewModel();
            myProfileViewModel myProfile = new myProfileViewModel();
            if (ControllerContext.HttpContext.Session["UserId"] != null)
            {
                User dbuser = (User)ControllerContext.HttpContext.Session["UserId"];
                logUser.userName = dbuser.name;
                logUser.Id = dbuser.userId;
                myProfile.myUser = logUser;
                myProfile.Comments = postRepository.comments.Where(p => p.userId == logUser.Id).Distinct().ToList().OrderByDescending(c => c.commentDate);

                myProfile.chatMessagesList = groupRepositoty.groupChats.Where(c => c.userId == logUser.Id).ToList().OrderByDescending(c => c.txtDate);

            }
            else 
            { 
                 return RedirectToAction("LogPagee", "Account",new {returnUrl= Request.RawUrl } );

            }

            

            return View(myProfile);
        }
        public ActionResult userManage()
        {
            return View();
        }
        [HttpGet]
        public ActionResult jsGetUser()
        {
           var userList=  repository.users.Select(x => new {userId=x.userId,userName=x.userName,password=x.password });
            return Json(userList, JsonRequestBehavior.AllowGet);
        }
    }
}