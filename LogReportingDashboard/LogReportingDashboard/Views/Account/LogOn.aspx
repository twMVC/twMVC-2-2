<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<LogReportingDashboard.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    登入
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>登入</h2>
    <p>
        請輸入您的使用者名稱和密碼。
    </p>

    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, "登入失敗。請更正錯誤後再試一次。") %>
        <div>
            <fieldset>
                <legend>帳戶資訊</legend>
                
                <div class="editor-label">
                    Account
                </div>
                <div class="editor-field">
                    <%= Html.TextBox("Account", null, new { id="Account" }) %>
                </div>
                
                <div class="editor-label">
                    Password
                </div>
                <div class="editor-field">
                    <%= Html.Password("Password", null, new { id="Password" }) %>
                </div>
                <p>
                    <input type="button" name="ButtonLogon" id="ButtonLogon" value="LogOn" />
                </p>
            </fieldset>
        </div>
    <% } %>

<% if (1 > 2){ %> <script language="javascript" type="text/javascript" src="../../Scripts/jquery-1.7.2.min.js"></script><% } %>
<script language="javascript" type="text/javascript">
<!--
    $(document).ready(function () {
        $('#Account').focus();
        $('#ButtonLogon').bind('click', function () { ExecuteLogOn(); });
    });

    function ExecuteLogOn() {
        var account = $.trim($('#Account').val());
        var password = $.trim($('#Password').val());

        if (account.length == 0 || password.length == 0) {
            alert('input error');
            return false;
        }
        else {
            $.ajax({
                url: '<%= Url.Action("LogOn", "Account") %>',
                data: { account: account, password: password },
                type: 'post',
                async: false,
                cache: false,
                success: function (data) {
                    if (data == 'Faild') {
                        alert('Logon Faild');
                        return false;
                    }
                    else {
                        alert('Logon Success');
                        location.href = data;
                    }
                }
            });
        }
    }

-->
</script>

</asp:Content>
