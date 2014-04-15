<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.DormsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Электронная очередь на поселение
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
<script type="text/javascript" src="../../Scripts/jquery.ui.datepicker-ru.js"></script>
<script type="text/javascript">
    var LockedId = '';
    var CellId = '';
    var updsec = 30;
    var maxVal = 2;
    $(function () {
        var minDate = new Date(<%= Model.isSPB ? "2012, 9, 1" : "2012, 7, 25" %>);
        var vDate = new Date();
        var nowDate = new Date(vDate.getYear(), vDate.getMonth(), vDate.getDate());
        if (nowDate > minDate)
            minDate = nowDate;
        $('#dtDate').datepicker({
            changeMonth: true,
            showOn: "focus",
            defaultDate: minDate,
            minDate: minDate,
            maxDate: new Date(2012, 9, 25),
            onSelect: function (dateText, inst) {
                nullTimetable();
                $.post('/Dorms/GetTimetable', { dormsId: $('#dormsId').val(), date: $('#dtDate').val() }, function (data) {
                    if (data.IsOk) {
                        for (var i = 0; i < data.Values.length; i++) {
                            var val = data.Values[i];
                            var cellname = '#H' + val.H + 'M' + val.M;
                            $(cellname).removeClass('redcell').removeClass('greencell').removeClass('yellowcell');
                            if (val.Cnt >= val.Max) {
                                $(cellname).addClass('redcell');
                            }
                            else {
                                $(cellname).addClass('greencell');
                            }
                            $(cellname + '_text').text(val.Cnt + '/' + val.Max);
                        }
                        if (data.RegH > 0 && data.RegM > 0) {
                            $('#H' + data.RegH + 'M' + data.RegM).removeClass('redcell').removeClass('greencell').addClass('yellowcell');
                        }
                    }
                }, 'json');
            }
        });
        $.datepicker.regional["ru"];
        setTimeout(onStart, 0);
    });
    function onStart() {
        nullTimetable();
        getTimetable();
        $('#msgErrors').hide();
        $('#msgPreLock').hide();
    }
    function nullTimetable() {
        for (var h = 8; h <= 20; h++) {
            for (var m = 0; m < 10; m++) {
                var cellname = '#H' + h + 'M' + m * 5;
                $(cellname).removeClass('yellowcell').removeClass('redcell').removeClass('greencelldark').addClass('greencell');
                $(cellname + '_text').text('0/' + maxVal);
            }
        }
    }
    function getTimetable() {
        nullTimetable();
        $(CellId).removeClass('greencell').addClass('greencelldark');
        $.post('/Dorms/GetTimetable', { dormsId: $('#dormsId').val(), date: $('#dtDate').val() }, function (data) {
            if (data.IsOk) {
                for (var i = 0; i < data.Values.length; i++) {
                    var val = data.Values[i];
                    var cellname = '#H' + val.H + 'M' + val.M;
                    $(cellname).removeClass('redcell').removeClass('greencell').removeClass('yellowcell');
                    if (val.Cnt >= val.Max) {
                        $(cellname).addClass('redcell');
                        maxVal = val.Max;
                    }
                    else {
                        $(cellname).addClass('greencell');
                    }
                    $(cellname + '_text').text(val.Cnt + '/' + val.Max);
                }
                if (data.RegH > 0 && data.RegM > 0) {
                    $('#H' + data.RegH + 'M' + data.RegM).removeClass('redcell').removeClass('greencell').addClass('yellowcell');
                }
            }
        }, 'json');
        setTimeout(getTimetable, updsec * 1000);
    }
    function check(h, m) {
        var cellName = '#H' + h + "M" + m;
        
        $.post('/Dorms/CheckTimeAndLock', { dormsId: $('#dormsId').val(), date: $('#dtDate').val(), h: h, m: m }, function (res) {
            if (res.IsOk) {
                $('#msgErrors').hide(100);
                $('#msgErrorsText').text('');
                $(CellId).removeClass('greencelldark').addClass('greencell');
                CellId = cellName;
                if (res.Action == 'yellow') {
                    $(CellId).removeClass('greencell').addClass('greencelldark');
                    LockedId = res.Id;
                    $('#msgPreLockBtn').show();
                    $('#msgPreLockUnregBtn').hide();
                    $('#msgPreLockText').text('Вы действительно хотите занять очередь на ' + $('#dtDate').val() + ' в ' + h + ':' + (m < 10 ? '0' + m : m) + '?');
                    $('#msgPreLock').show(100);
                    if (res.Cnt != undefined && res.Max != undefined) {
                        $(cellName + '_text').text(res.Cnt + '/' + res.Max);
                    }
                }
                else if (res.Action == 'unregister') {
                    $('#msgPreLockBtn').hide();
                    $('#msgPreLockUnregBtn').show();
                    $('#msgPreLockText').text('Вы действительно хотите снять регистрацию с ' + $('#dtDate').val() + ' в ' + h + ':' + (m < 10 ? '0' + m : m) + '?');
                    $('#msgPreLock').show(100);
                    if (res.Cnt != undefined && res.Max != undefined) {
                        $(cellName + '_text').text(res.Cnt + '/' + res.Max);
                    }
                    LockedId = res.Id;
                }
            }
            else {
                $('#msgPreLock').hide(100);
                $('#msgErrorText').text(res.Message);
                $('#msgErrors').show(100);
                if (res.Action == 'red') {
                    $(cellName).removeClass('greencell').addClass('redcell');
                    $(cellName + '_text').text(res.Cnt + '/' + res.Max);
                }
                
                else {
                    $(cellName).removeClass('redcell').addClass('greencell');
                    if (res.Cnt != undefined && res.Max != undefined) {
                        $(cellName + '_text').text(res.Cnt + '/' + res.Max);
                    }
                }
            }
        }, 'json');
    }
    function register() {
        $('#msgErrors').hide(100);
        $.post('/Dorms/Register', { Id: LockedId }, function (res) {
            if (res.IsOk) {
                var cellname = '#H' + res.H + 'M' + res.M;
                $('#msgPreLockBtn').hide();
                $('#msgPreLockText').text('Вы успешно зарегистрировались на ' + res.H + ':' + res.M + '. Чтобы отменить регистрацию, просто щёлкните по данному времени ещё раз.');
                $(cellname).removeClass('redcell').removeClass('greencell').addClass('yellowcell');
            }
            else {
                $('#msgErrorText').text(res.Message);
                $('#msgErrors').show();
            }
        }, 'json');
    }
    function unregister() {
        $('#msgErrors').hide(100);
        $.post('/Dorms/UnRegister', { Id: LockedId }, function (res) {
            if (res.IsOk) {
                var cellname = '#H' + res.H + 'M' + res.M;
                $('#msgPreLockBtn').hide();
                $('#msgPreLockText').text('Вы сняли регистрацию.');
                $(cellname).removeClass('redcell').removeClass('yellowcell').addClass('greencell');
                $(cellname + '_text').text(res.Cnt + '/' + res.Max);
            }
            else {
                $('#msgErrorText').text(res.Message);
                $('#msgErrors').show();
            }
        }, 'json');
    }
</script>
<%  if (!Model.isRegistered)
    {
%>
    <h3>Ограниченный доступ</h3><br />
    <div class="message warning">
        Вы не авторизованы на сервере. <a href="../../Account/LogOn">Войдите</a> под своей учётной записью.<br />
        Если Вы не регистрировались в Личном кабинете поступающего - нажмите <a href="../../Account/Data">сюда</a>.
    </div>
<%  }
    else
    {
%>
<% 
        if (!Model.hasInEntered)
        {
%>
    <h3>Ограниченный доступ</h3><br />
    <div class="message warning">
        Данные вашей учётной записи не найдены в списке поступивших.<br />
        Если Вы не регистрировались в Личном кабинете поступающего - нажмите <a href="../../Account/Data">сюда</a>.<br />
        Если же Вы регистрировались, то проверьте данные учётной записи.
    </div>
<% 
        }
        else
        {
%>
    <div class="message info" id="msgSelectDate">
        Выберите желаемую дату и студгородок.
        ВНИМАНИЕ! Поселение студентов-первокурсников из Санкт-Петербурга начнется с 1 октября 2012 года!
    </div>
    <div class="clearfix">
        <select id="dormsId" onchange="onStart()">
            <option value="1">Студгородок ПУНК</option>
            <option value="2">Студгородок ВУНК</option>
        </select>
    </div><br />
    <div class="clearfix">
        <input id="dtDate" type="text" value="<%= Model.regDate.HasValue ? Model.regDate.Value.Date.ToString("dd.MM.yyyy") : "" %>" />
        <br /><br />
    </div>
    <div class="message info" id="msgSelectTime">
        В таблице указано количество абитуриентов, записавшихся на указанное время.<br />
        <span style="color:#99CC66; text-shadow: none;">Зелёным цветом</span> выделены ещё не заполненные до конца временные промежутки.<br />
        <span style="color:#FF3300; text-shadow: none;">Красным цветом</span> выделены уже заполненные до конца временные промежутки.<br />
    </div>
    <div id="msgPreLock" class="message warning" style="display:none; border-collapse:collapse;">
        <span id="msgPreLockText"></span>
        <br /><br /><button id="msgPreLockBtn" style="display:none; border-collapse:collapse;" class="button button-orange" onclick="register()">Занять очередь</button>
        <button id="msgPreLockUnregBtn" style="display:none; border-collapse:collapse;" class="button button-orange" onclick="unregister()">Снять регистрацию</button>
    </div>
    <div id="msgErrors" class="message warning" style="display:none; border-collapse:collapse;">
        <span id="msgErrorText"></span>
    </div>
    <table class="timetable">
    <thead>
        <tr>
            <th>Время</th>
            <th>00 мин</th>
            <th>05 мин</th>
            <th>10 мин</th>
            <th>15 мин</th>
            <th>20 мин</th>
            <th>25 мин</th>
            <th>30 мин</th>
            <th>35 мин</th>
            <th>40 мин</th>
            <th>45 мин</th>
            <th>Резерв</th>
        </tr>
    </thead>
    <tbody>
    <% foreach (var rw in Model.Rows)
       { 
    %>
        <tr>
            <td style="text-align:right"><%= rw.Hour.ToString("D2") %> часов</td>
    <% 
           foreach (var cell in rw.Cells)
           {
    %>
            <td id="<%= "H" + rw.Hour.ToString() + "M" + cell.Minute.ToString() %>" onclick="check(<%= rw.Hour.ToString() + "," + cell.Minute.ToString() %>)" class="<%= cell.isRegistered ? "yellowcell" : (cell.isLocked ? "redcell" : "greencell") %> cell">
                <span id="<%= "H" + rw.Hour.ToString() + "M" + cell.Minute.ToString() + "_text" %>"><%= cell.CountAbits%>/2</span>
                <span style="font-size:6pt;"><%= rw.Hour.ToString() + ":" + cell.Minute.ToString("D2")%></span>
            </td>
    <% 
           }
    %>  
            <td class="ReservedTimetable"><%= "C " + rw.Hour.ToString("D2") + ":50 по " + rw.Hour.ToString("D2") + ":59" %></td>
        </tr>
    <% } %>
    </tbody>
    </table>
<%
        }
    } 
%>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="NavigationList" runat="server">
    <ul class="clearfix">
        <li><a href="../../Abiturient/Main">Личный кабинет</a></li>
        <li class="active"><a href="../../Account/Register"><%= GetGlobalResourceObject("Common", "MainNavDorms").ToString()%></a></li>
        <li><a href="../../Account/LogOff">Выйти</a></li>
    </ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
    Электронная очередь на поселение
</asp:Content>
