<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
        歡迎 <b><%: Page.User.Identity.Name %></b>!
        [ <%: Html.ActionLink("登出", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%: Html.ActionLink("登入", "LogOn", "Account") %> ]
<%
    }
%>
