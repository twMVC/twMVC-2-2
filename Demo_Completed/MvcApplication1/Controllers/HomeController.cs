using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;
using NLog;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            ViewBag.Message = "歡迎使用 ASP.NET MVC!";

            //logger.Trace("this is trace");
            //logger.Debug("this is debug");
            //logger.Info("this is Info");
            //logger.Warn("this is Warn");
            //logger.Error("this is Error");
            //logger.Fatal("this is Fatal");

            return View();
        }

        public ActionResult About()
        {
            try
            {
                int a = 6;
                int b = 0;
                int result = a / b;
            }
            catch (Exception ex)
            {
                //throw ex;
                logger.ErrorException("故意的", ex);
            }
            return View();
        }

        public ActionResult Category()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                var result = db.Categories.OrderBy(x => x.CategoryID).ToList();
                ViewData.Model = result;
            }
            return View();
        }
    }
}
