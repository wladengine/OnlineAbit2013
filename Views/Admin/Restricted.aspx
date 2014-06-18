<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Restricted
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>403</h1>
<h2>Доступ только администраторам. Извините.</h2>
<h2>Restricted part of the site.</h2>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="NavigationList" runat="server">
    <ul class="clearfix">
        <li><a href="../../AbiturientNew/Main">Вам сюда</a></li>
        <li><a href="../../Account/LogOn">Или даже сюда</a></li>
    </ul>
</asp:Content>
