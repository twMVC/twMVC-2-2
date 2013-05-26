using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using LogReportingDashboard.Helpers;
using LogReportingDashboard.Models;

namespace LogReportingDashboard.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult LogOn()
        {
            bool isLogon = Utils.CheckAuthenticated();
            if (isLogon)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult LogOn(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                return Content("您輸入的帳號資料錯誤，請重新登入!");
            }
            else
            {
                string role = account.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                    ? "Admin"
                    : account.Equals("Manage", StringComparison.OrdinalIgnoreCase)
                        ? "Manage"
                        : "Other";

                string encryptTicket = Utils.SignIn(account, role);
                if (string.IsNullOrEmpty(encryptTicket))
                {
                    return Content("Faild");
                }
                else
                {
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptTicket);
                    authCookie.Expires = DateTime.Now.AddDays(1);
                    this.Response.Cookies.Add(authCookie);

                    string redirectUrl = Url.Action("Index", "Home");
                    return Content(redirectUrl);
                }
            }
        }

        public ActionResult LogOff()
        {
            //原本號稱可以清除所有 Cookie 的方法...
            FormsAuthentication.SignOut();

            //清除所有的 session
            Session.RemoveAll();

            //建立一個同名的 Cookie 來覆蓋原本的 Cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            //建立 ASP.NET 的 Session Cookie 同樣是為了覆蓋
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            //將使用者導出去
            return RedirectToAction("Index", "Home");
        }

    }

    public class Utils
    {
        /// <summary>
        /// Signs the in.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public static string SignIn(string account, string role)
        {
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                1,
                account,
                DateTime.Now,
                DateTime.Now.AddMinutes(60), // 登入時間 60 分鐘到期
                false,
                role);

            return FormsAuthentication.Encrypt(authTicket);
        }

        /// <summary>
        /// GetLogonUser
        /// </summary>
        /// <param name="encryptedTicket"></param>
        /// <returns></returns>
        public static string GetLogonUser(string encryptedTicket)
        {
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encryptedTicket);
            if (string.IsNullOrEmpty(ticket.Name))
            {
                return string.Empty;
            }
            else
            {
                return ticket.Name;
            }
        }

        /// <summary>
        /// Checks the authenticated.
        /// </summary>
        /// <returns></returns>
        public static bool CheckAuthenticated()
        {
            if (HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName] == null)
            {
                return false;
            }
            else
            {
                string encryptedTicket = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value;

                if (string.IsNullOrWhiteSpace(encryptedTicket))
                {
                    return false;
                }
                else
                {
                    string user = Utils.GetLogonUser(encryptedTicket);
                    return !string.IsNullOrWhiteSpace(user);
                }
            }
        }

    }
}
