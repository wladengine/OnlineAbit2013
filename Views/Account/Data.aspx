<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.DataRegModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Для незарегистрированных пользователей
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.ui.datepicker-ru.js"></script>
    <script type="text/javascript">
        var PID = '';
        var Type = '';
        $(function () {
            $('#regBirthDate').datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: "focus",
                defaultDate: '-18y',
                maxDate: '-15y'
            });
            $.datepicker.regional["ru"];
        });
        function sendPersData() {
            var type = $(":radio[name=Course]").filter(":checked").val();
            $.post('/Account/DataReg', { surname: $('#regSurname').val(), name: $('#regName').val(), secondName: $('#regSecondName').val(), date: $('#regBirthDate').val(), type: type }, function (res) {
                if (res.IsOk) {
                    PID = res.Id;
                    Type = res.Type;
                    $('#btnSendPersData').hide();
                    $('#emailForm').show(100);
                }
                else {
                    if (res.Action == 'yellow') {
                        $('#emailForm').hide();
                        $('#addRegInfo').show();
                        $('#errMsgText').text(res.Message);
                        $('#errMsg').show();
                        $('#errMsg').removeClass('warning').addClass('error');
                        setTimeout(function () { $('#errMsg').removeClass('error').addClass('warning'); }, 2000);
                        $('#btnSendPersData').hide();
                    }
                    else {
                        $('#emailForm').hide();
                        $('#addRegInfo').hide();
                        $('#errMsgText').text(res.Message);
                        $('#errMsg').show();
                        $('#errMsg').removeClass('warning').addClass('error');
                        setTimeout(function () { $('#errMsg').removeClass('error').addClass('warning'); }, 2000);
                    }
                }
            }, 'json');
        }
        function sendPersDataExt() {
            var type = $(":radio[name=Course]").filter(":checked").val();
            $.post('/Account/DataRegExt', { surname: $('#regSurname').val(), name: $('#regName').val(), secondName: $('#regSecondName').val(), date: $('#regBirthDate').val(), type: type, RegionId: $('#Region').val() }, function (res) {
                if (res.IsOk) {
                    PID = res.Id;
                    Type = res.Type;
                    $('#errMsg').hide();
                    $('#btnSendPersData').hide();
                    $('#btnSendPersDataExt').hide();
                    $('#emailForm').show(100);
                }
                else {
                    $('#emailForm').hide();
                    $('#errMsgText').text(res.Message);
                    $('#errMsg').show();
                    $('#errMsg').removeClass('warning').addClass('error');
                    setTimeout(function () { $('#errMsg').removeClass('error').addClass('warning'); }, 2000);
                }
            }, 'json');
        }
        function registerUser() {
            $.post('/Account/RegisterSimple', { id: PID, email: $('#email').val(), type: Type }, function (res) {
                if (res.IsOk) {
                    $('#errMsg').show();
                    $('#errMsg').removeClass('warning').addClass('success');
                    $('#errMsgText').text(res.Message);
                }
                else {
                    $('#errMsg').show();
                    $('#errMsg').removeClass('warning').addClass('error');
                    $('#errMsgText').text('На указанный вами адрес электронной почты было отправлено письмо с инструкциями по продолжению регистрации');
                    setTimeout(function () { $('#errMsg').removeClass('error').addClass('warning'); }, 2000);
                }
            }, 'json');
        }
    </script>
    <p class="message info">
       Если Вы НЕ регистрировались в Личном Кабинете поступающего на первый курс и поступали в СПбГУ после 2008 года, то укажите свои данные в форме ниже.
    </p>
    <br />
    <div id="regInfo" class="form" style="border-collapse:collapse;">
        <hr />
        <div class="clearfix">
            <label for="regSurname">Фамилия</label>
            <input type="text" id="regSurname"/>
        </div>
        <div class="clearfix">
            <label for="regName">Имя</label>
            <input type="text" id="regName"/>
        </div>
        <div class="clearfix">
            <label for="regSecondName">Отчество</label>
            <input type="text" id="regSecondName"/>
        </div>
        <div class="clearfix">
            <label for="regBirthDate">Дата рождения</label>
            <input type="text" id="regBirthDate"/>
        </div>
        <br />
        <div class="clearfix">
            <span>Поступал(а) в 2012 году</span><input type="radio" id="isFirstCourse" name="Course" value="0" />
        </div>
        <br />
        <div class="clearfix">
            <span>Поступал(а) ранее</span><input type="radio" id="AfterCourse" name="Course" value="1" />
        </div>
        <br /><hr />
        <div class="clearfix" id="btnSendPersData">
            <button type="button" class="button button-green" onclick="sendPersData()">Проверить данные</button>
        </div>
    </div>
    <div id="addRegInfo" class="form" style=" display:none; border-collapse:collapse; margin: 5px 0px 5px 0px;">
        <div class="clearfix">
            <label for="Region">Регион</label>
            <%= Html.DropDownList("Region", Model.listRegion) %>
        </div><br />
        <div class="clearfix" id="btnSendPersDataExt">
            <button type="button" class="button button-green" onclick="sendPersDataExt()">Отправить</button>
        </div>
    </div>
    <div id="emailForm" class="form" style="display:none; border-collapse:collapse; margin: 5px 0px 5px 0px;">
        <div class="clearfix">
            <label for="email">Укажите E-mail для регистрации</label>
            <input type="text" id="email" />
        </div><br />
        <div class="clearfix">
            <button type="button" onclick="registerUser()" class="button button-green">Зарегистрироваться</button>
        </div>
    </div>
    <div id="errMsg" class="message warning" style="display:none; border-collapse:collapse; margin: 5px 0px 5px 0px;">
        <span id="errMsgText"></span>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="NavigationList" runat="server">
    <ul class="clearfix">
        <li><a href="../../Abiturient/Main"><%= GetGlobalResourceObject("Common", "MainNavLogon").ToString()%></a></li>
        <li class="active"><a href="../../Account/Data">Для незарегистрированных пользователей</a></li>
    </ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
    Для незарегистрированных пользователей
</asp:Content>
