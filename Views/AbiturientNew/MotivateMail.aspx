<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.MotivateMailModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Создание мотивационного письма
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Создание мотивационного письма</h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% if (0 == 1)
   { %>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
<% } %>
<script type="text/javascript">
    $(function () {
        $('#saveStatus').hide();
        $('#btnGetFile').hide();
    });
    var curId;
    function SendMail() {
        var text = $('#MailInfo').val();
        if (text == '') {
            alert('Вы должны указать текст мотивационного письма!')
        }
        $.post('/Abiturient/SendMotivationMail', { "info": text, "appId": curId }, function (res) {
            if (res.IsOk) {
                $('#saveStatus').text("Сохранено").addClass("Green").show();
                setTimeout(function () { $('#saveStatus').removeClass("Green").hide(200); $('#btnGetFile').show(200); }, 5000);
                curId = res.Id;
            }
            else {
                $('#saveStatus').text(res.ErrorMessage).addClass("Red").show();
                setTimeout(function () { $('#saveStatus').removeClass("Red").hide(200); }, 3000);
            }
        }, 'json');
    }
    function GetMail(applicationId) {
        curId = applicationId;
        $.post('/Abiturient/GetMotivationMail', { "appId": applicationId }, function (res) {
            $('#saveStatus').hide();
            if (res.IsOk) {
                $('#MailInfo').val(res.Text);
                $('#btnGetFile').html('<a href="../Abiturient/GetMotivationMailPDF/' + res.Id + '" class="ui-button-text" target="_blank">Просмотреть письмо</a>').show();
            }
            else {
                $('#MailInfo').val('');
                $('#btnGetFile').hide();
            }
        }, 'json');
    }
</script>
<b>В мотивационном письме должны содержаться:</b> 
<ul>
    <li>необходимые сведения об опыте профессиональной подготовки/деятельности;</li>
    <li>сведения, подтверждающие необходимость получения знаний/навыков, освоение/приобретение которых возможно в период обучения на выбранной программе;</li>
    <li>перспективы/планы реализации полученных знаний/навыков в будущей профессиональной деятельности.</li>
</ul>
<h3>Выберите одно из своих заявлений:</h3>
<table class="stripy">
<% foreach (var s in Model.Apps)
   { %>
    <tr>
        <td><input type="radio" id="<%= s.Id.ToString() %>" name="Id" onclick="GetMail('<%= s.Id.ToString() %>')"/></td>
        <td><%= s.Profession %></td><td><%= s.ObrazProgram %></td><td><%= s.Specialization %></td>
    </tr>
<% } %>
</table>
<textarea id="MailInfo" cols="80" rows="10" class="ui-widget-content ui-corner-all noresize"></textarea><br /><br />
<button type="button" id="btnSendMail" onclick="SendMail()" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" style="font-size:12px">
    <span class="ui-button-text">Отправить</span>
</button>
<span id="btnGetFile" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" style="font-size:12px">
</span>
<span id="saveStatus"></span>
</asp:Content>