using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcApplication1.Models
{

    #region 模型

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "目前密碼")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼與確認密碼不相符。")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "使用者名稱")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Display(Name = "記住我?")]
        public bool RememberMe { get; set; }
    }


    public class RegisterModel
    {
        [Required]
        [Display(Name = "使用者名稱")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "電子郵件地址")]
        public string Email { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
        public string ConfirmPassword { get; set; }
    }
    #endregion

    #region Services
    // FormsAuthentication 型別是密封的而且含有靜態成員，所以很難
    // 對呼叫其成員的程式碼進行單元測試。下列的介面和 Helper 類別示範
    // 如何建立抽象包裝函式包住這種型別，以便讓 AccountController
    // 程式碼能進行單元測試。

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不可為 null 或空白。", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("值不可為 null 或空白。", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不可為 null 或空白。", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("值不可為 null 或空白。", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("值不可為 null 或空白。", "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不可為 null 或空白。", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("值不可為 null 或空白。", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("值不可為 null 或空白。", "newPassword");

            // 在某些失敗情況下，基礎 ChangePassword() 將會擲回例外狀況
            // 而不是傳回 false。
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不可為 null 或空白。", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // 請參閱 http://go.microsoft.com/fwlink/?LinkID=177550 了解
            // 狀態碼的完整清單。
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "使用者名稱已經存在。請輸入不同的使用者名稱。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "擁有該電子郵件地址的使用者名稱已經存在。請輸入不同的電子郵件地址。";

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
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _defaultErrorMessage = "'{0}' 的長度至少要有 {1} 個字元。";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), _minCharacters, int.MaxValue)
            };
        }
    }
    #endregion

}
