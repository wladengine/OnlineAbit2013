<%@ Import Namespace="OnlineAbit2013" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Abiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.PersonalOffice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Личный кабинет
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script>
    $('#UILink').hide();
</script>
<% if (1 == 0)/* типа затычка, чтобы VS видела скрипты */
   {
%>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
<% } %>
    <script type="text/javascript">
        $(function () {
            $('#WorkPlace').keyup(function () {
                var str = $('#WorkPlace').val();
                if (str != "") {
                    $('#validationMsgPersonWorksPlace').text('');
                }
                else {
                    $('#validationMsgPersonWorksPlace').text('Введите место работы');
                }
            });
            $('#WorkProf').keyup(function () {
                var str = $('#WorkProf').val();
                if (str != "") {
                    $('#validationMsgPersonWorksLevel').text('');
                }
                else {
                    $('#validationMsgPersonWorksLevel').text('Введите должность');
                }
            });
            $('#WorkSpec').keyup(function () {
                var str = $('#WorkSpec').val();
                if (str != "") {
                    $('#validationMsgPersonWorksDuties').text('');
                }
                else {
                    $('#validationMsgPersonWorksDuties').text('Введите должностные обязанности');
                }
            });
        });
        function UpdScWorks() {
            if ($('#ScWorkInfo').val() == '') {
                return false;
            }
            var params = new Object();
            params['ScWorkInfo'] = $('#ScWorkInfo').val();
            params['ScWorkType'] = $('#WorkInfo_ScWorkId').val();
            $.post('Abiturient/UpdateScienceWorks', params, function (res) {
                if (res.IsOk) {
                    var output = '';
                    output += '<tr id=\'' + res.Data.Id + '\'><td>';
                    output += res.Data.Type + '</td>';
                    output += '<td>' + res.Data.Info + '</td>';
                    output += '<td><span class="link" onclick="DeleteScWork(\'' + res.Data.Id + '\')" ><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить оценку" /><span></td>';
                    output += '</tr>';
                    $('#ScWorks tbody').append(output);
                }
                else {
                    alert(res.ErrorMsg);
                }
            }, 'json');
        }
        function DeleteScWork(id) {
            var param = new Object();
            param['id'] = id;
            $.post('Abiturient/DeleteScienceWorks', param, function (res) {
                if (res.IsOk) {
                    $("#" + id).hide(250).html("");
                }
                else {
                    alert(res.ErrorMessage);
                }
            }, 'json');
        }
        function AddWorkPlace() {
            var params = new Object();
            params['WorkStag'] = $('#WorkStag').val();
            params['WorkPlace'] = $('#WorkPlace').val();
            params['WorkProf'] = $('#WorkProf').val();
            params['WorkSpec'] = $('#WorkSpec').val();
            var Ok = true;
            if (params['WorkPlace'] == "") {
                $('#validationMsgPersonWorksPlace').text('Введите место работы');
                Ok = false;
            }
            if (params['WorkPlace'] == "") {
                $('#validationMsgPersonWorksLevel').text('Введите должность');
                Ok = false;
            }
            if (params['WorkSpec'] == "") {
                $('#validationMsgPersonWorksDuties').text('Введите должностные обязанности');
                Ok = false;
            }
            if (Ok) {
                $.post('Abiturient/AddWorkPlace', params, function (res) {
                    if (res.IsOk) {
                        $('#NoWorks').hide();
                        var info = '<tr id="' + res.Data.Id + '">';
                        info += '<td>' + res.Data.Place + '</td>';
                        info += '<td>' + res.Data.Stag + '</td>';
                        info += '<td>' + res.Data.Level + '</td>';
                        info += '<td>' + res.Data.Duties + '</td>';
                        info += '<td><span class="link" onclick="DeleteWorkPlace(\'' + res.Data.Id + '\')"><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить" /></span></td>';
                        $('#PersonWorks tbody').append(info);
                    }
                    else {
                        alert(res.ErrorMessage);
                    }
                }, 'json');
            }
        }
        function DeleteWorkPlace(id) {
            var parm = new Object();
            parm["wrkId"] = id;
            $.post('Abiturient/DeleteWorkPlace', parm, function (res) {
                if (res.IsOk) {
                    $('#' + id).hide(250).html('');
                }
                else {
                    alert(res.ErrorMessage);
                }
            }, 'json');
        }
        function CheckRegExp() {
            var val = $('#WorkStag').val();
            var regex = /^([0-9])+$/i;
            if (!regex.test(val)) {
                $('#btnAddProfs').hide();
                $('#validationMsgPersonWorksExperience').text('Введите целое число').show();
            }
            else {
                $('#btnAddProfs').show();
                $('#validationMsgPersonWorksExperience').hide();
            }
        }
    </script>
    <div class="grid">
        <div class="wrapper">
            <div class="grid_4 first">
            <% if (!Model.Enabled)
                { %>
                <div id="Message" class="message warning">
                    <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                </div>
            <% } %>
                <h3>Участие в научно-исследовательской работе:</h3>
                <hr />
                <p>Участие в научных конференциях (укажите тему конференции, дату и место ее проведения; тему доклада/выступления), опубликованные научные статьи, работа в научных лабораториях, работа в проектных группах.</p>
                <p>Напоминаем, что Вам необходимо предоставить документы, <b>подтверждающие:</b> </p>
                <ul>
                    <li>Ваше участие в конференциях, семинарах, круглых столах и прочих научных и научно-практических мероприятиях. В качестве таковых могут выступать опубликованные тезисы доклада и программа  мероприятия.</li>
                    <li>Ваше  участие в <b>исследовательских проектах, поддержанных грантами,</b> а также подтверждающие полученные Вами результаты.</li>
                    <li>Вашу работу (в том числе и эффективность  деятельности).</li>
                </ul>
                <div class="form">
                    <div class="clearfix">
                        <%= Html.DropDownListFor(x => x.WorkInfo.ScWorkId, Model.WorkInfo.ScWorks)%>
                    </div>
                    <div class="clearfix">
                        <textarea class="noresize" id="ScWorkInfo" rows="5" cols="80"></textarea>
                    </div>
                    <br />
                    <div class="clearfix">
                        <button id="btnAddScWork" onclick="UpdScWorks()" class="button button-blue">Добавить</button>
                    </div>
                    <br /><br />
                    <table id="ScWorks" class="paginate" style="width:100%;">
                        <thead>
                            <tr>
                                <th>Тип</th>
                                <th>Текст</th>
                                <th>Удалить</th>
                            </tr>
                        </thead>
                        <tbody>
                        <% foreach (var scWork in Model.WorkInfo.pScWorks)
                            {
                        %>
                            <tr>
                            <%= Html.Raw(string.Format(@"<tr id=""{0}"">", scWork.Id)) %>
                                <td><%= Html.Encode(scWork.ScienceWorkType) %></td>
                                <td><%= Html.Encode(scWork.ScienceWorkInfo) %></td>
                                <td><%= Html.Raw("<span class=\"link\" onclick=\"DeleteScWork('" + scWork.Id.ToString() + "')\" ><img src=\"../../Content/themes/base/images/delete-icon.png\" alt=\"Удалить\" /></span>") %></td>
                            </tr>
                        <% } %>
                        </tbody>
                    </table>
                </div>
                <br /><br />
                <h3>Опыт работы (практики):</h3>
                <hr />
                <div class="form">
                    <div class="clearfix">
                        <label for="WorkStag">Стаж (полных лет):</label>
                        <input id="WorkStag" onkeyup="CheckRegExp()" type="text" class="text ui-widget-content ui-corner-all"/><br /><span id="validationMsgPersonWorksExperience" class="Red"></span>
                    </div>
                    <div class="clearfix">
                        <label for="WorkPlace">Место работы(практики):</label>
                        <input id="WorkPlace" type="text" /><br /><span id="validationMsgPersonWorksPlace" class="Red"></span>
                    </div>
                    <div class="clearfix">
                        <label for="WorkProf">Должность:</label>
                        <input id="WorkProf" type="text" /><br /><span id="validationMsgPersonWorksLevel" class="Red"></span>
                    </div>
                    <div class="clearfix">
                        <label for="WorkSpec">Должностные обязанности:</label>
                        <textarea id="WorkSpec" cols="80" rows="4" ></textarea><br /><span id="validationMsgPersonWorksDuties" class="Red"></span>
                    </div>
                </div>
                <div class="clearfix">
                    <button id="btnAddProfs" onclick="AddWorkPlace()" class="button button-blue">Добавить</button>
                </div>
                <br /><br />
                <table id="PersonWorks" class="paginate" style="width:100%;">
                    <thead>
                        <tr>
                            <th>Место работы</th>
                            <th>Стаж</th>
                            <th>Должность</th>
                            <th>Должностные обязанности</th>
                            <th>Удалить</th>
                        </tr>
                    </thead>
                    <tbody>
                    <% foreach (var wrk in Model.WorkInfo.pWorks)
                        {
                    %>
                        <tr>
                        <%= Html.Raw(string.Format(@"<tr id=""{0}"">", wrk.Id.ToString())) %>
                            <td><%= Html.Encode(wrk.Place) %></td>
                            <td><%= Html.Encode(wrk.Stag) %></td>
                            <td><%= Html.Encode(wrk.Level) %></td>
                            <td><%= Html.Encode(wrk.Duties) %></td>
                            <td><%= Html.Raw(string.Format(@"<span class=""link"" onclick=""DeleteWorkPlace('{0}')""><img src=""../../Content/themes/base/images/delete-icon.png"" alt=""Удалить"" /></span>", wrk.Id.ToString()))%></td>
                        </tr>
                    <% } %>
                    </tbody>
                </table>
                <% if (Model.WorkInfo.pWorks.Count == 0)
                    {
                %>
                    <h5 id="NoWorks">Нет</h5>
                <% } %>

                <br /><hr style="color:#A6C9E2;" />

                <% using (Html.BeginForm("NextStep", "Abiturient", FormMethod.Post))
                    {
                %>
                    <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                    <input id="Submit4" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                <% } %>
            </div>
            <div class="grid_2">
                <ol>
                    <li><a href="../../Abiturient?step=1"><%= GetGlobalResourceObject("PersonInfo", "Step1")%></a></li>
                    <li><a href="../../Abiturient?step=2"><%= GetGlobalResourceObject("PersonInfo", "Step2")%></a></li>
                    <li><a href="../../Abiturient?step=3"><%= GetGlobalResourceObject("PersonInfo", "Step3")%></a></li>
                    <li><a href="../../Abiturient?step=4"><%= GetGlobalResourceObject("PersonInfo", "Step4")%></a></li>
                    <li><a href="../../Abiturient?step=5"><%= GetGlobalResourceObject("PersonInfo", "Step5")%></a></li>
                    <li><a href="../../Abiturient?step=6"><%= GetGlobalResourceObject("PersonInfo", "Step6")%></a></li>
                </ol>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderScriptsContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Анкета</h2>
</asp:Content>
