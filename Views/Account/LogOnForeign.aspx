<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SiteForeign.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.LogOnForeignModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Log on
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    Log on
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Log on</h2>
    <p>
        Please, type user name and password or <%: Html.ActionLink("register", "RegisterFor") %>, if you don't have account on this site.
    </p>

    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function mkTimeClient() {
            var time = new Date();
            var value = time.getHours() + ":" + time.getMinutes() + ":" + time.getSeconds() + " " + time.toDateString();
            $('#time').val(value);
        });
    </script>

    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, "There are errors while login:") %>
        <div>
            <fieldset>
                <legend>Account information</legend>
                
                <div class="editor-label">
                    <%= Html.LabelFor(m => m.UserName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.UserName) %>
                    <%: Html.ValidationMessageFor(m => m.UserName) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Password) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.Password) %>
                    <%: Html.ValidationMessageFor(m => m.Password) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.CheckBoxFor(m => m.RememberMe) %>
                    <%: Html.LabelFor(m => m.RememberMe) %>
                </div>
                <input id="time" name="time" type="hidden" />
                <p>
                    <input class="button-gray button" id="submitBtn" type="submit" value="Войти" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
