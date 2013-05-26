using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demo.Controllers
{
    [HandleError]
    public class ErrorController : Controller
    {
        [PreventDirectAccess]
        public ActionResult Index()
        {
            ViewData["Description"] = "抱歉, 處理你的請求發生錯誤! Orz...";
            Response.StatusCode = 200;
            return View();
        }

        /// <summary>
        /// Pages the not found.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        [PreventDirectAccess]
        public ActionResult PageNotFound(string error)
        {
            ViewData["Description"] = "抱歉, 處理你的請求發生404錯誤!";
            Response.StatusCode = 200;
            return View();
        }


        private class PreventDirectAccessAttribute : FilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
                object value = filterContext.RouteData.Values["from_Application_Error_Event"];
                if (!(value is bool && (bool)value))
                {
                    filterContext.Result = new ViewResult { ViewName = "PageNotFound" };
                }
            }
        }
    }

}
