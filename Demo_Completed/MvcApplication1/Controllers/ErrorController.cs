using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    [HandleError]
    public class ErrorController : Controller
    {
        /// <summary>
        /// Indexes the specified error.
        /// </summary>
        /// <returns></returns>
        [PreventDirectAccess]
        public ActionResult Index(Exception exception = null)
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
        public ActionResult PageNotFound(string error, Exception exception)
        {
            ViewData["Description"] = "抱歉, 處理你的請求發生404錯誤!";
            Response.StatusCode = 404;
            return View();
        }

        /// <summary>
        /// Internals the error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public ActionResult InternalError(string error)
        {
            ViewData["Title"] = "抱歉, 處理你的請求發生500錯誤";
            ViewData["Description"] = error;
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
