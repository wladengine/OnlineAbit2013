<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
        Welcome, <strong><%: Page.User.Identity.Name %></strong>!
        [ <%: Html.ActionLink("LogOff", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%: Html.ActionLink("LogOn", "LogOn", "Account") %> ]
<%
    }
%>
