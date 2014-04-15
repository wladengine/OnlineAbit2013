<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Captcha
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Captcha</h2>
<% using (Html.BeginForm("PostCaptcha", "Account", FormMethod.Post))
   { %>
    <%= Html.ValidationSummary() %>
    <%= Html.GenerateCaptcha() %>
    <button type="submit" value="Submit">Go</button>
<% } %>
</asp:Content>
