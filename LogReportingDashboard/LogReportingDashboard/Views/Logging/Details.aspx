<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<LogReportingDashboard.Models.LogEvent>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details</h2>

    <p>        
        <%: Html.ActionLink("Back to List", "Index") %>
    </p>

    <fieldset>
        <legend>Fields</legend>
        
        <div class="display-label">Id</div>
        <div class="display-field"><%: Model.Id %></div>
        
        <div class="display-label">LogDate</div>
        <div class="display-field"><%:Model.LogDate.ToLocalTime() %></div>
        
        <div class="display-label">Name</div>
        <div class="display-field"><%: Model.LoggerProviderName %></div>
        
        <div class="display-label">Source</div>
        <div class="display-field"><%: Model.Source %></div>
        
        <div class="display-label">MachineName</div>
        <div class="display-field"><%: Model.MachineName %></div>
        
        <div class="display-label">Type</div>
        <div class="display-field"><%: Model.Type %></div>
        
        <div class="display-label">Level</div>
        <div class="display-field"><%: Model.Level %></div>
        
        <div class="display-label">Message</div>
        <div class="display-field">
            <pre><%: Model.Message.WordWrap(80) %></pre>
        </div>
        
        <div class="display-label">StackTrace</div>
        <div class="display-field"><pre><%: Model.StackTrace %></pre></div>                      
        
    </fieldset>

    <% =FormsHelper.OutputXmlTableForLogging(Model.AllXml) %>

    <p>        
        <%: Html.ActionLink("Back to List", "Index") %>
    </p>

</asp:Content>

