using BlogDal;
using BlogDal.Abstract;
using blogProject.Helper;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace blogProject.Controllers
{
    public class CustomJsonResult : JsonResult
    {
        private const string _dateFormat = "yyyy-MM-dd HH:mm:ss";

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                // Using Json.NET serializer
                var isoConvert = new IsoDateTimeConverter();
                isoConvert.DateTimeFormat = _dateFormat;
                response.Write(JsonConvert.SerializeObject(Data, isoConvert));
            }
        }
    }// end of class
    public class grouoMessageViewModel
    {
        public int groupChatId { get; set; }
        public int uid { get; set; }
        public string name { get; set; }
        public string msg { get; set; }
        public string groupd { get; set; }
        public string time { get; set; }
        public string txtType { get; set; }
    }
    public class ChatController : Controller
    {
        public IgroupRepository gRepository;
        private IuserRepository repository;

        // GET: Chat
        private static string SessionKey = "userId";

        public  ChatController(IgroupRepository prmRepository,IuserRepository prmUserRepository)
        {
            gRepository = prmRepository;
            repository = prmUserRepository;
        }
        [HttpGet]
        public  ActionResult getFiles(int groupId)
        {
            var values = (from x in gRepository.groupChats
                       where (x.groupId == groupId && x.txtType == "imagetxt")
                       select new {userId = x.userId, fileName = x.chatTxt }).ToList();

            //var values= gRepository.getFileNames(groupId);
            return Json(values, JsonRequestBehavior.AllowGet);

        }
        

        [HttpGet]
        public ActionResult getChat(int groupId,int pageCount)
        {
            int groupToMestalsages = gRepository.GroupMessageTotal(groupId);
            int skippTotal = pageCount * 30;
            if (skippTotal<groupToMestalsages && (groupToMestalsages-skippTotal)<30 )
            {
                skippTotal = groupToMestalsages;

            }
            var groupChat = (from p in gRepository.groupChats
                             where p.groupId == groupId
                             select new
                             {
                                 groupChatId = p.groupChatId,
                                 uid = p.userId,
                                 name = p.users.userName,
                                 msg = p.chatTxt,
                                 groupd = p.groups.groupName,
                                 time = p.txtDate,
                                 txtType = p.txtType
                             }).
                            ToList()
                            .Select(x => new grouoMessageViewModel()
                            {
                                groupChatId=x.groupChatId,
                                uid=x.uid,
                                name=x.name,
                                msg=x.msg,
                                groupd=x.groupd,
                                time=x.time.ToString("s"),
                                txtType=x.txtType


                            }).ToList().Skip(groupToMestalsages-skippTotal).Take(skippTotal);
                            

            
               
             
              //  Select(x => new { groupChatId = x.groupChatId, uid = x.users.userId, name = x.users.userName, msg = x.chatTxt, group = x.groups.groupName, time = x.txtDate, txtType = x.txtType });
            return Json(groupChat, JsonRequestBehavior.AllowGet);
           // return new CustomJsonResult { Data = new { chats = groupChat } };

        }
        [HttpPost]
        public JsonResult SaveFiles(string description)
        {
            string Message, fileName, actualFileName;
            Message = fileName = actualFileName = string.Empty;
            bool flag = false;
            string path = "";
            if (Request.Files != null)
            {
                var file = Request.Files[0];
                actualFileName = file.FileName;
                fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                int size = file.ContentLength;
                try
                {
                    path = Server.MapPath("~/Images");
                    file.SaveAs(Path.Combine(path, fileName));

                    Message = fileName;
                    flag = true;

                }
                catch (Exception)
                {
                    Message = "Failed";

                    flag = false;
                }


            }

            return new JsonResult { Data = new { Message = Message, Status = flag } };
        }
        public ActionResult testing()
        {
            return View();
        }
        [HttpGet]
        [Route("ChatController")]

        public ActionResult getUserId ()
        {
            var userDetails = ControllerContext.HttpContext.Session[SessionKey];
            return Json(userDetails, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult verifyUserAndGetUser(string userName, string password, HttpRequestMessage request)
        {
          // var d= request.GetClientIpAddress();
            user prmUser = new user
            {
                userName = userName,
                password = password
            };
            user dbUser = repository.verifyUser(prmUser);
            //    FormsAuthentication.SetAuthCookie("junaid",false); // This statment is for Temporarly purpose  only 
            if (dbUser != null)
            {
                User usr = new User();
                usr.userId = dbUser.userId;
                usr.name = dbUser.userName;
                this.HttpContext.Session.Timeout = 60;
                ControllerContext.HttpContext.Session[SessionKey] = usr;

                
                 var ip = this.Request.ServerVariables["REMOTE_ADDR"];
                //ip = "92.0.164.122";
                var country = Helper.GeoLocation.IPRequestHelper(ip);
                repository.saveCountry(dbUser.userId, country);
            }
            var userDetails = ControllerContext.HttpContext.Session[SessionKey];
            if (userDetails!=null)
            {
               return  RedirectToAction("ChatRoom", "chat");

            }

            return Json(userDetails, JsonRequestBehavior.AllowGet);


        } // end of action Method
        [HttpGet]
        public ActionResult registerNewUser(string name,string password)
        {
            User usr = new User();

            user prmUser = new user()
            {
                userName=name,
                password=password
            };
            if (repository.checkUser(prmUser))
            {
                
            }
            else
            {
                repository.Create(prmUser);
                usr.userId = prmUser.userId;
                usr.name = prmUser.userName;
                ControllerContext.HttpContext.Session[SessionKey] = usr;
            }
            return Json(usr, JsonRequestBehavior.AllowGet);


        }
        [HttpGet]
        public ActionResult getId()
        {
            var userDetails = ControllerContext.HttpContext.Session[SessionKey];
            return Json(userDetails, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChatRoom()
        {
            //if (ControllerContext.HttpContext.Session["UserId"] == null)
            //{
            //    ViewBag.invalid = "You need to give Log Dtails to Start Chatting ....";
            //  return RedirectToAction("LogPagee", "Account",new {returnUrl= Request.RawUrl } );
            //}
            //else 
            return View();
        }
        public ActionResult chatManage()
        {
            return View();
        }
        public ActionResult postChat(int postId)
        {
            ControllerContext.HttpContext.Session["postId"] = postId;
            return View();
        }
        public ActionResult mapChat()
        {
            return View();
        }
    }
}