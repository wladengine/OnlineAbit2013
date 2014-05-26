<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SiteForeign.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.RegisterForeignModel>" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Registration
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Registration a new user</h2>
<p>
        Type all fields to create an account
    </p>
    <p>
        Password length must be at least <%: Membership.MinRequiredPasswordLength %> characters long.
    </p>
    <% if (0 == 1)
       { %>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
    <% } %>

    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function mkTimeClient() {
            var time = new Date();
            var value = time.getHours() + ":" + time.getMinutes() + ":" + time.getSeconds() + " " + time.toDateString();
            $('#time').val(value);
            $("#btnSubmit").mouseover(function () {
                $("#btnSubmit").addClass("ui-state-hover")
            });
            $("#btnSubmit").mouseleave(function () {
                $("#btnSubmit").removeClass("ui-state-hover")
            });
            $("#IsComputer").change(function () {
                var f = $("#IsComputer").attr('checked');
                if (f == true) {
                    $('#IsComputerMsg').show();
                }
                else {
                    $('#IsComputerMsg').hide();
                }
            });
        });
        
    </script>

    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, "There was a problem while creating account:") %>
        <div>
            <fieldset>
                <legend>Account information</legend>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.UserName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.UserName, new System.Collections.Generic.SortedList<string, object>() { {"class", "ui-widget-content ui-corner-all"} })%>
                    <%: Html.ValidationMessageFor(m => m.UserName) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Email) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.Email, new System.Collections.Generic.SortedList<string, object>() { { "class", "ui-widget-content ui-corner-all" } })%>
                    <%: Html.ValidationMessageFor(m => m.Email) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Password) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.Password, new System.Collections.Generic.SortedList<string, object>() { { "class", "ui-widget-content ui-corner-all" } })%>
                    <%: Html.ValidationMessageFor(m => m.Password) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.ConfirmPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.ConfirmPassword, new System.Collections.Generic.SortedList<string, object>() { { "class", "ui-widget-content ui-corner-all" } })%>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                <div class="editor-field">
                    <span>Please, type the following words:</span>
                    <br />
                    <%= Html.GenerateCaptcha(Guid.NewGuid().ToString("N"), "clean")%>
                </div>
                <input id="time" name="time" type="hidden" />
                <p>
                    <input id="btnSubmit" type="submit" value="Register" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only"/>
                </p>
            </fieldset>
        </div>
    <% } %>

</asp:Content>
