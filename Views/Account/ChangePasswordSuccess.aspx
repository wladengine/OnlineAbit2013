<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <asp:Literal ID="Head" runat="server" meta:resourcekey="Header"></asp:Literal>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="Subheader" runat="server">
    <h2><asp:Literal ID="Literal1" runat="server" meta:resourcekey="Header"></asp:Literal></h2>
</asp:Content>

<asp:Content ID="changePasswordSuccessContent" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <asp:Literal ID="Literal2" runat="server" meta:resourcekey="Success"></asp:Literal>
    </p>
</asp:Content>
