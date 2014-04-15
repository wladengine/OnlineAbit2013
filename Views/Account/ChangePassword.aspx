<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="OnlineAbit2013.Models" %>
<%@ Page Language="C#" MasterPageFile="~/Views/Abiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("ChangePassword", "Header").ToString() %>
</asp:Content>

<asp:Content ContentPlaceHolderID="Subheader" runat="server">
    <h2>
    <asp:Label ID="Label0" Text="<%$Resources:ChangePassword, Header%>" runat="server"></asp:Label></h2>
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <p>
        <asp:Label ID="Label2" Text="<%$Resources:ChangePassword, FormHeader%>" runat="server"></asp:Label>
    </p>
    <p>
        <asp:Label ID="Label3" Text="<%$Resources:ChangePassword, HeaderMinPasswordLength %>" runat="server"></asp:Label>
    </p>

    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, GetGlobalResourceObject("ChangePassword", "FailMessage").ToString())%>
        <div class="form">
            <fieldset>
                <legend><%= GetGlobalResourceObject("ChangePassword", "AccountInformationLegend").ToString()%></legend>
                <hr />
                <div class="clearfix">
                    <%: Html.LabelFor(m => m.OldPassword, GetGlobalResourceObject("ChangePassword", "OldPassword").ToString())%>
                    <%: Html.PasswordFor(m => m.OldPassword) %>
                    <%: Html.ValidationMessageFor(m => m.OldPassword) %>
                </div>
                <div class="clearfix">
                    <%: Html.LabelFor(m => m.NewPassword, GetGlobalResourceObject("ChangePassword", "NewPassword").ToString())%>
                    <%: Html.PasswordFor(m => m.NewPassword) %>
                    <%: Html.ValidationMessageFor(m => m.NewPassword) %>
                </div>
                <div class="clearfix">
                    <%: Html.LabelFor(m => m.ConfirmPassword, GetGlobalResourceObject("ChangePassword", "ConfirmPassword").ToString())%>
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                
                <p>
                    <input class="button button-green" type="submit" value="<%= GetGlobalResourceObject("ChangePassword", "btnSubmit").ToString() %>" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
