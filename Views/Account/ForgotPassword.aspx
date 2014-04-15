<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("ForgotPassword", "Header").ToString() %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% if (0 == 1)
   { %>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
<% } %>
<script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
<script type="text/javascript">
    $(function() {
        $('#BirthDate').datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: "focus",
                yearRange: '1920:2000',
                defaultDate: '-17y',
            });
    });
    function UserRequest() {
        var email = $('#email').val();
        $.post('/Account/PasswordRestore', { email: email }, function (res) {
            if (res.NeedInfo) {
                $('#NoEmailMsg').hide();
                $('#AddInfo').show();
                $('#btnEmailRequest').hide();
            }
            else if (res.NoEmail) {
                $('#AddInfo').hide();
                $('#NoEmailMsg').show();
                $('#btnEmailRequest').show();
            }
            else if (res.IsOk) {
                $('#NoEmailMsg').hide();
            }
        }, 'json');
    }
    function DataRequest() {
        $.post('/Account/RestoreByData',
        { email: $('#email').val(), surname: $('#Surname').val(), birthdate: $('#BirthDate').val() },
        function (res) {
            if (res.IsOk) {
                $('#NoDataFound').hide();
                $('#EmailSent').show();
            }
            else if (!res.Email) {
                $('#EmailFail').show();
                $('#EmailSent').hide();
            }
            else {
                $('#NoDataFound').show();
                $('#EmailSent').hide();
            }
        }, 'json');
    }
</script>
    <p><%= GetGlobalResourceObject("ForgotPassword", "Info").ToString()%></p>
    <div class="form">
        <div class="clearfix">
            <label for="email">Email</label>
            <input id="email" type="text" placeholder="your email" />
        </div><br />
        <div id="btnEmailRequest" class="clearfix">
            <button class="button button-gray" onclick="UserRequest()"><%= GetGlobalResourceObject("ForgotPassword", "btnSubmit").ToString()%></button>
        </div><br />
        <div id="AddInfo" style="display:none;">
            <div class="message info"><%= GetGlobalResourceObject("ForgotPassword", "msgNeedConfirm").ToString() %></div>
            <div class="clearfix">
                <label for="Surname"><%= GetGlobalResourceObject("ForgotPassword", "Surname").ToString() %></label>
                <input id="Surname" type="text" />
            </div>
            <div class="clearfix">
                <label for="BirthDate"><%= GetGlobalResourceObject("ForgotPassword", "BirthDate").ToString() %></label>
                <input id="BirthDate" type="text" class="date" />
            </div><br />
            <div class="clearfix">
                <button class="button button-blue" onclick="DataRequest()"><%= GetGlobalResourceObject("ForgotPassword", "btnSubmit").ToString()%></button>
            </div>
            <div id="NoDataFound" class="message error" style="display:none;">
                <%= GetGlobalResourceObject("ForgotPassword", "msgWrongData").ToString()%>
            </div>
        </div><br />
        <div id="NoEmailMsg" class="message error" style="display:none;">
            <%= GetGlobalResourceObject("ForgotPassword", "MessageNoEmail").ToString()%>
        </div>
        <div id="EmailFail" class="message error" style="display:none;">
            <%= GetGlobalResourceObject("ForgotPassword", "MessageNoSent").ToString()%>
        </div>
        <div id="EmailSent" class="message success" style="display:none;">
            <%= GetGlobalResourceObject("ForgotPassword", "EmailSent").ToString()%>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="NavigationList" runat="server">
    <ul class="clearfix">
        <li><a href="../../Abiturient/Main"><%= GetGlobalResourceObject("Common", "MainNavLogon").ToString()%></a></li>
        <li><a href="../../Account/Register"><%= GetGlobalResourceObject("Common", "MainNavRegister").ToString()%></a></li>
        <li class="active"><a><%= GetGlobalResourceObject("ForgotPassword", "Header").ToString()%></a></li>
    </ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("ForgotPassword", "Header").ToString()%></h2>
</asp:Content>
