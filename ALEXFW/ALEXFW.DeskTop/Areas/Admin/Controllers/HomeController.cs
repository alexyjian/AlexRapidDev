using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALEXFW.DeskTop.Areas.Admin.Controllers
{
    [EntityAuthorize]
    public class HomeController : Controller
    {
        public ActionResult Overview()
        {
            return View();
        }
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}