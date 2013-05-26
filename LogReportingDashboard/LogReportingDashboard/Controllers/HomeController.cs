using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogReportingDashboard.Services.Logging;

namespace LogReportingDashboard.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC - Log Reporting Dashboard";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Confused()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View("Error");
        }

        protected override void HandleUnknownAction(string actionName)
        {
            throw new HttpException(404, "Action not found");
        }
    }
}
