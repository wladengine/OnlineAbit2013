<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.AdminAbitsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    AbitsList
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Список абитуриентов</h2>

<table class="classic" style="border-collapse:collapse;">
<tr>
    <th>Факультет</th>
    <th>Направление</th>
    <th>Образовательная программа</th>
    <th>Профиль</th>
    <th>Заявлений на бюджет</th>
    <th>Заявлений на внебюджет</th>
    <th>Просмотр</th>
</tr>
<tbody>
<% foreach (var ap in Model.List)
   { %>
<tr>
    <td><%= ap.Faculty.Value %><br /><a class="MyLink" href="../../AbitList?faculty=<%= ap.Faculty.Key %>">Просмотреть</a></td>
    <td><%= ap.Profession.Value %><br /><a class="MyLink" href="../../AbitList?faculty=<%= ap.Faculty.Key %>&profession=<%= ap.Profession.Key %>">Просмотреть</a></td>
    <td><%= ap.ObrazProgram.Value %><br /><a class="MyLink" href="../../AbitList?faculty=<%= ap.Faculty.Key %>&profession=<%= ap.Profession.Key %>&obrazprogram=<%= ap.ObrazProgram.Key %>">Просмотреть</a></td>
    <td><%= ap.Profile.Value %></td>
    <td><%= ap.CntBudzh %></td>
    <td><%= ap.CntPlatn %></td>
    <td><a href="../../AbitList?faculty=<%= ap.Faculty.Key %>&profession=<%= ap.Profession.Key %>&obrazprogram=<%= ap.ObrazProgram.Key %>&profile=<%= ap.Profile.Key %>">Просмотреть</a></td>
</tr>
<% } %>
</tbody>
</table>


</asp:Content>
