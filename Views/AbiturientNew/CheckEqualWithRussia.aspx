<%@ Page Title="" Language="C#" MasterPageFile="~/Views/ForeignAbiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.EqualWithRussiaModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    CheckEqualWithRussia
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Для подтверждения Вашего желания участия в равном конкурсе с гражданами РФ, введите ваш пароль от Личного Кабинета:</h2>
    <% using (Html.BeginForm("SetEqualWithRussia", "Abiturient", FormMethod.Post))
       { %>
       <% if (!string.IsNullOrEmpty(Model.Errors))
          { %>
          <div class="field-validation-error"><%= Model.Errors %></div>
       <% } %>
       <%= Html.HiddenFor(x => x.Email) %>
       <%= Html.PasswordFor(x => x.Password) %>
       <br /><br />
       <input type="submit" class="button button-green" />
    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderScriptsContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
</asp:Content>
