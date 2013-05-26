using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using LogReportingDashboard.Models;
using LogReportingDashboard.Services;
using NLog;
using NLog.Config;
using LogReportingDashboard.Services.Logging.NLog;

namespace LogReportingDashboard
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

            routes.MapRoute(
                "Default", // 路由名稱
                "{controller}/{action}/{id}", // URL 及參數
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // 參數預設值
            );

            routes.MapRoute(
                "Log", // Route name
                "{controller}/{action}/{LoggerProviderName}/{id}", // URL with parameters
                new { controller = "Logging", action = "Details" } // Parameter defaults
            );
        }

        public override void Init()
        {
            this.AuthenticateRequest += new EventHandler(MvcApplication_AuthenticateRequest);
            this.PostAuthenticateRequest += new EventHandler(MvcApplication_PostAuthenticateRequest);
            base.Init();
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                string encTicket = authCookie.Value;
                if (!String.IsNullOrEmpty(encTicket))
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encTicket);
                    FormsIdentity id = new FormsIdentity(ticket);
                    GenericPrincipal prin = new GenericPrincipal(id, null);
                    HttpContext.Current.User = prin;
                }
            }

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

        void MvcApplication_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            // Setup our custom controller factory so that the [HandleErrorWithElmah] attribute
            // is automatically injected into all of the controllers
            ControllerBuilder.Current.SetControllerFactory(new ErrorHandlingControllerFactory());

            // Register custom NLog Layout renderers
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("utc_date", typeof(UtcDateRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("web_variables", typeof(WebVariablesRenderer));

        }

    }
}