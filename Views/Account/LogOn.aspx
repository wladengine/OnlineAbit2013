<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("LogOn", "HeaderLogOn").ToString()%>
</asp:Content>

<asp:Content ID="SubheaderContent" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("LogOn", "HeaderLogOn").ToString()%></h2>
</asp:Content>

<asp:Content ContentPlaceHolderID="NavigationList" runat="server">
    <ul class="clearfix">
        <li class="active"><a href="../../AbiturientNew/Main"><%= GetGlobalResourceObject("Common", "MainNavLogon").ToString()%></a></li>
        <li><a href="../../Account/Register"><%= GetGlobalResourceObject("Common", "MainNavRegister").ToString()%></a></li>
    </ul>
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <a href="../../Account/Register" style="float:right; text-decoration:underline; color:#888;font-weight:400; font-size:1.3em;">
        <%: GetGlobalResourceObject("LogOn", "Register").ToString() %>
    </a>
    <%--<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>--%>
    <script type="text/javascript">
        $(function mkTimeClient() {
            var time = new Date();
            var value = time.getHours() + ":" + time.getMinutes() + ":" + time.getSeconds() + " " + time.toDateString();
            $('#time').val(value);
        });
    </script>
    <div class="grid">
        <div class="wrapper">
        <div class=" grid_5 first">
            <form class="form" method="post" action="../../Account/LogOn">
                <%: Html.ValidationSummary(true, GetGlobalResourceObject("LogOn", "ValidationSummaryHeader").ToString()) %>
                <fieldset>
                    <h5><%= GetGlobalResourceObject("LogOn", "HeaderAccountInformation").ToString()%></h5>
                    <hr />
                    <div class="clearfix">
                        <%= Html.LabelFor(m => m.Email, GetGlobalResourceObject("LogOn", "Email").ToString()) %>
                        <%: Html.TextBoxFor(m => m.Email, new SortedList<string, object>() { {"required", "required" } })%>
                        <%: Html.ValidationMessageFor(m => m.Email, GetGlobalResourceObject("LogOn", "EmailValidationMessage").ToString())%>
                    </div><br />
                    <div class="clearfix">
                        <%: Html.LabelFor(m => m.Password, GetGlobalResourceObject("LogOn", "Password").ToString())%>
                        <%: Html.PasswordFor(m => m.Password) %><br />
                        <a href="../../Account/ForgotPassword"><%= GetGlobalResourceObject("LogOn", "ForgotPassword").ToString()%></a><br />
                        <%: Html.ValidationMessageFor(m => m.Password, GetGlobalResourceObject("LogOn", "PasswordValidationMessage").ToString())%>
                    </div><br />
                    <div class="clearfix">
                        <%: Html.LabelFor(m => m.RememberMe, GetGlobalResourceObject("LogOn", "RememberMe").ToString())%>
                        <%: Html.CheckBoxFor(m => m.RememberMe) %>
                    </div>
                    
                    <input id="time" name="time" type="hidden" />
                    <hr />
                    <div class="clearfix">
                        <input id="submitBtn" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("LogOn", "btnLogOn").ToString() %>" />
                    </div>
                </fieldset>
            </form>
        </div>
        </div>
    </div>
    
</asp:Content>
