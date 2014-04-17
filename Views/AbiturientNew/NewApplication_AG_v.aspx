<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<AG_ApplicationModel>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
    Создание нового заявления
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
   <h2>Новое заявление</h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% using (Html.BeginForm("NewApp", "AbiturientNew", FormMethod.Post))
   { 
%> 
    <%= Html.ValidationSummary() %>
    <div class="message info" style="width:450px;">
        Согласно данным анкеты, Вы поступаете в <strong><%= Model.EntryClassName %></strong>
    </div>
    <br />
    
<% 
   }
%>

</asp:Content>




