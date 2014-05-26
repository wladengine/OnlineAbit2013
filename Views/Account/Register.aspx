<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.RegisterModel>" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("Registration", "PageHeader").ToString()%>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Subheader" runat="server">
   <h2><%= GetGlobalResourceObject("Registration", "PageHeader").ToString()%></h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="NavigationList" runat="server">
    <ul class="clearfix">
        <li><a href="../../AbiturientNew/Main"><%= GetGlobalResourceObject("Common", "MainNavLogon").ToString()%></a></li>
        <li class="active"><a href="../../Account/Register"><%= GetGlobalResourceObject("Common", "MainNavRegister").ToString()%></a></li>
    </ul>
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <% if (0 == 1)
       { %>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
    <% } %>

    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            var time = new Date();
            var value = time.getHours() + ":" + time.getMinutes() + ":" + time.getSeconds() + " " + time.toDateString();
            $('#time').val(value);
        });
        var InfoShow = false;
    </script>
<% if (System.Threading.Thread.CurrentThread.CurrentUICulture == System.Globalization.CultureInfo.GetCultureInfo("ru-RU")) { %>
    
<% } %>
    <form action="../../Account/Register" method="post" class="panel form grid_5">
        <%: Html.ValidationSummary(true, GetGlobalResourceObject("Registration", "HeaderErrors").ToString())%>
        <div>
            <fieldset>
                <h3><%= GetGlobalResourceObject("Registration", "HeaderAccountInfo").ToString()%></h3>
                <hr />
                <p><%= GetGlobalResourceObject("Registration", "HeaderMinPasswordLength").ToString()%></p>
                <div class="clearfix">
                    <%: Html.LabelFor(m => m.Email, GetGlobalResourceObject("Registration", "Email").ToString())%>
                    <%: Html.TextBoxFor(m => m.Email) %>
                    <%: Html.ValidationMessageFor(m => m.Email) %>
                </div>
                <div class="clearfix">
                    <%: Html.LabelFor(m => m.Password, GetGlobalResourceObject("Registration", "Password").ToString())%>
                    <%: Html.PasswordFor(m => m.Password) %>
                    <%: Html.ValidationMessageFor(m => m.Password) %>
                </div>
                <div class="clearfix">
                    <%: Html.LabelFor(m => m.ConfirmPassword, GetGlobalResourceObject("Registration", "ConfirmPassword").ToString())%>
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                <hr />
                <div class="clearfix">
                    <span><%= GetGlobalResourceObject("Registration", "CaptchaHeader").ToString() %></span>
                    <br />
                    <%= Html.GenerateCaptcha(Guid.NewGuid().ToString("N"), "clean") %>
                </div>
                <input id="time" name="time" type="hidden" />
                <hr />
                <div class="clearfix">
                    <input id="btnSubmit" type="submit" value="<%= GetGlobalResourceObject("Registration", "btnRegister").ToString() %>" class="button button-green"/>
                </div>
            </fieldset>
        </div>
    </form>
</asp:Content>
