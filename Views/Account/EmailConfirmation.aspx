<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.EmailConfirmationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("EmailConfirmation", "Header").ToString()%>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="NavigationList" runat="server">
    <ul class="clearfix">
        <li><a href="../../Abiturient/Main"><%= GetGlobalResourceObject("Common", "MainNavLogon").ToString()%></a></li>
        <li><a href="../../Account/Register"><%= GetGlobalResourceObject("Common", "MainNavRegister").ToString()%></a></li>
        <li class="active"><a><%= GetGlobalResourceObject("EmailConfirmation", "Header").ToString()%></a></li>
    </ul>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% if (Model.RegStatus == OnlineAbit2013.Models.EmailConfirmationStatus.Confirmed)
   { %>
<h3>
    <%= GetGlobalResourceObject("EmailConfirmation", "Success").ToString()%><br />
    <asp:Literal runat="server" Text="<%$Resources:EmailConfirmation, SuccessLink %>"></asp:Literal>
</h3>
<% }
   else if (Model.RegStatus == OnlineAbit2013.Models.EmailConfirmationStatus.WrongTicket)
   { %>
    <h3> <%= GetGlobalResourceObject("EmailConfirmation", "WrongTicket").ToString()%> </h3>
<% }
   else if (Model.RegStatus == OnlineAbit2013.Models.EmailConfirmationStatus.WrongEmail)
   { %>
    <h3><%= GetGlobalResourceObject("EmailConfirmation", "WrongEmail").ToString()%></h3>
<% }
   else if (Model.RegStatus == OnlineAbit2013.Models.EmailConfirmationStatus.FirstEmailSent)
   { %>
    <h3><%= GetGlobalResourceObject("EmailConfirmation", "FirstMail1").ToString()%>&nbsp; 
    <%= Html.Encode(Model.Email) %> &nbsp;
    <%= GetGlobalResourceObject("EmailConfirmation", "FirstMail2").ToString()%></h3>
<% } %>

</asp:Content>