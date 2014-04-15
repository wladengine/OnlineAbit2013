<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>

<asp:Content ID="errorTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("ErrorPage", "Header")  %>
</asp:Content>



<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Во время запроса произошла ошибка. Приносим свои извинения за доставленные неудобства.
    </h2>
</asp:Content>
