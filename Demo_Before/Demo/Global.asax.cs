using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using Demo.Controllers;

namespace Demo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// Handles the AuthenticateRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            bool hasUser = HttpContext.Current.User != null;
            bool isAuthenticated = hasUser && HttpContext.Current.User.Identity.IsAuthenticated;
            bool isIdentity = isAuthenticated && HttpContext.Current.User.Identity is FormsIdentity;

            if (isIdentity)
            {
                // 取得表單認證目前這位使用者的身份
                FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;

                // 取得 FormsAuthenticationTicket 物件
                FormsAuthenticationTicket ticket = id.Ticket;

                // 取得 UserData 欄位資料 (這裡我們儲存的是角色)
                string userData = ticket.UserData;

                // 如果有多個角色可以用逗號分隔
                string[] roles = userData.Split(',');

                // 賦予該使用者新的身份 (含角色資訊)
                HttpContext.Current.User = new GenericPrincipal(id, roles);
            }
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    var app = (MvcApplication)sender;
        //    var ex = app.Server.GetLastError();

        //    var context = app.Context;
        //    context.Response.Clear();
        //    context.ClearError();

        //    var httpException = ex as HttpException;
        //    var routeData = new RouteData();

        //    routeData.Values["controller"] = "Error";
        //    routeData.Values["action"] = "Index";

        //    routeData.Values["exception"] = ex;
        //    routeData.Values["from_Application_Error_Event"] = true;

        //    if (httpException != null)
        //    {
        //        switch (httpException.GetHttpCode())
        //        {
        //            case 404:
        //                routeData.Values["action"] = "PageNotFound";
        //                break;
        //            default:
        //                routeData.Values["action"] = "Index";
        //                break;
        //        }
        //    }
        //    // Pass exception details to the target error View.
        //    routeData.Values.Add("Error", ex.Message);

        //    // Avoid IIS7 getting in the middle
        //    context.Response.TrySkipIisCustomErrors = true;
        //    IController controller = new ErrorController();
        //    controller.Execute(new RequestContext(new HttpContextWrapper(context), routeData));
        //}



    }
}