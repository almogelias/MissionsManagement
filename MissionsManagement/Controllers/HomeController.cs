using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MissionsManagement.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Register()
        {
            ViewBag.Title = "Register Page";

            return View();
        }
        public ActionResult Login()
        {
            ViewBag.Title = "Login Page";

            return View();
        }


        [Authorize]
        public ActionResult Logout()
        {
            
            return View();
        }
        [Authorize]
        public ActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";

            return View();
        }

        [Authorize]
        public ActionResult Statistics()
        {
            ViewBag.Title = "Statistics";

            return View();
        }
    }
}
