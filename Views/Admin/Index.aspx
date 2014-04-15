<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Admin PowerTools</h2>
    <ul>
        <li><a href="../../Admin/AbitCountList?entrytype=1">Списки абитуриентов 1 курс</a></li>
        <li><a href="../../Admin/AbitCountList?entrytype=2">Списки абитуриентов магистратура</a></li>
        <li><a href="../../Admin/UserList">Списки пользователей</a></li>
        <li><a href="../../Admin/EntryList">Списки учебных планов</a></li>
    </ul>
</asp:Content>
