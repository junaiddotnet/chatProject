using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace blogProject.Controllers
{
    public class PodcastController : Controller
    {
        // GET: Podcast
        public ActionResult Index()
        {
            return View();
        }
    }
}