using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Demo.Models;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

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
                throw ex;
            }

            return View();
        }

        public ActionResult Category()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                var result = db.Categories.OrderBy(x => x.CategoryID);
                ViewData.Model = result.ToList();
            }
            return View();
        }

        public ActionResult Products(int? id)
        {
            if (!id.HasValue)
            {
                throw new ArgumentNullException("id", "please input Category ID.");
            }

            using (NorthwindEntities db = new NorthwindEntities())
            {
                var result = db.Products.Where(x => x.CategoryID == id).OrderBy(x => x.ProductID);
                ViewData.Model = result.ToList();
            }
            return View();
        }
    }
}
