<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
        Добро пожаловать, <strong><%: Page.User.Identity.Name %></strong>!
        [ <%: Html.ActionLink("Выйти", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%: Html.ActionLink("Войти", "LogOn", "Account") %> ]
<%
    }
%>
