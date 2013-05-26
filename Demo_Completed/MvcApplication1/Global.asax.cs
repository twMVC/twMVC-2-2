using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MvcApplication1.Controllers;
using MvcApplication1.Misc;
using NLog.Config;

namespace MvcApplication1
{
    // 注意: 如需啟用 IIS6 或 IIS7 傳統模式的說明，
    // 請造訪 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.IgnoreRoute("elmah.axd");
            routes.IgnoreRoute("ooxx.axd");

            routes.MapRoute(
                "Default", // 路由名稱
                "{controller}/{action}/{id}", // URL 及參數
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // 參數預設值
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("utc_date", typeof(UtcDateRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("web_variables", typeof(WebVariablesRenderer));
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            // 發生未處理錯誤時執行的程式碼

            var app = (MvcApplication)sender;
            var ex = app.Server.GetLastError();

            var context = app.Context;
            context.Response.Clear();
            context.ClearError();

            var httpException = ex as HttpException;
            if (httpException == null)
            {
                httpException = new HttpException(null, ex);
            }

            var routeData = new RouteData();

            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "Index";

            routeData.Values["exception"] = ex;
            routeData.Values["from_Application_Error_Event"] = true;

            if (httpException != null)
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        routeData.Values["action"] = "PageNotFound";
                        break;
                    case 500:
                        routeData.Values["action"] = "InternalError";
                        break;
                    default:
                        routeData.Values["action"] = "GenericError";
                        break;
                }
            }

            // Pass exception details to the target error View.
            routeData.Values.Add("error", ex.Message);

            // Avoid IIS7 getting in the middle
            context.Response.TrySkipIisCustomErrors = true;
            IController controller = new ErrorController();
            controller.Execute(new RequestContext(new HttpContextWrapper(context), routeData));
        }

        /// <summary>
        /// Handles the Filtering event of the ErrorLog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Elmah.ExceptionFilterEventArgs"/> instance containing the event data.</param>
        void ErrorLog_Filtering(object sender, Elmah.ExceptionFilterEventArgs e)
        {
            if (e.Exception.GetBaseException() is HttpRequestValidationException)
            {
                e.Dismiss();
            }

            var httpException = e.Exception as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404)
            {
                e.Dismiss();
            }
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

    }
}