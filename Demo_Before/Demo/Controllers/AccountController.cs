using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Demo.Models;
using Demo.Misc;

namespace Demo.Controllers
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
        public ActionResult LogOn(string UserName, string Password)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                return Content("您輸入的帳號資料錯誤，請重新登入!");
            }
            else
            {
                //這邊可以改用自己的權限驗證模組
                string role = UserName.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                    ? "Admin"
                    : UserName.Equals("Manage", StringComparison.OrdinalIgnoreCase)
                        ? "Manage"
                        : "Other";

                string encryptTicket = Utils.SignIn(UserName, role);
                if (string.IsNullOrEmpty(encryptTicket))
                {
                    return Content("Faild");
                }
                else
                {
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptTicket);
                    authCookie.Expires = DateTime.Now.AddDays(1);
                    this.Response.Cookies.Add(authCookie);

                    return RedirectToAction("Index", "Home");
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

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // 嘗試註冊使用者
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // 在某些失敗狀況中，ChangePassword 會擲回例外狀況，
                // 而不是傳回 false。
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "目前密碼不正確或是新密碼無效。");
                }
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // 請參閱 http://go.microsoft.com/fwlink/?LinkID=177550 了解
            // 狀態碼的完整清單。
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "使用者名稱已經存在。請輸入不同的使用者名稱。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "該電子郵件地址的使用者名稱已經存在。請輸入不同的電子郵件地址。";

                case MembershipCreateStatus.InvalidPassword:
                    return "所提供的密碼無效。請輸入有效的密碼值。";

                case MembershipCreateStatus.InvalidEmail:
                    return "所提供的電子郵件地址無效。請檢查這項值，然後再試一次。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "所提供的密碼擷取解答無效。請檢查這項值，然後再試一次。";

                case MembershipCreateStatus.InvalidQuestion:
                    return "所提供的密碼擷取問題無效。請檢查這項值，然後再試一次。";

                case MembershipCreateStatus.InvalidUserName:
                    return "所提供的使用者名稱無效。請檢查這項值，然後再試一次。";

                case MembershipCreateStatus.ProviderError:
                    return "驗證提供者傳回錯誤。請確認您的輸入，然後再試一次。如果問題仍然存在，請聯繫您的系統管理員。";

                case MembershipCreateStatus.UserRejected:
                    return "使用者建立要求已取消。請確認您的輸入，然後再試一次。如果問題仍然存在，請聯繫您的系統管理員。";

                default:
                    return "發生未知的錯誤。請確認您的輸入，然後再試一次。如果問題仍然存在，請聯繫您的系統管理員。";
            }
        }
        #endregion
    }
}
