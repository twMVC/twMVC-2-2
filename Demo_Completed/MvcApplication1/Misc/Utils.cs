using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MvcApplication1.Misc
{
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