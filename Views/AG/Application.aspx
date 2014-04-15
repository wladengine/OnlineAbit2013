<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AG/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.ExtApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Application
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% if (1 == 2)
   { %>
   <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
<% } %>
<% if (Model.Enabled)
   { %>
    <script type="text/javascript" language="javascript">
        $(function () {
            $('#UILink').hide();
            $('#fileAttachment').change(ValidateInput);
            $("#rejectBtn")
                .button().click(function () {
                    $("#dialog-form").dialog("open");
                });
            $("#dialog:ui-dialog").dialog("destroy");
            $('#fileAttachment').change(ValidateInput);
            $("#dialog-form").dialog(
                {
                    autoOpen: false,
                    height: 400,
                    width: 350,
                    modal: true,
                    buttons:
                    {
                        "Да": function () {
                            $.post('/AG/DisableApp', { id: '<%= Model.Id.ToString("N") %>' }, function (res) {
                                if (res.IsOk) {
                                    if (!res.Enabled) {
                                        $('#appStatus').removeClass("Green").addClass("Red").text("Отозвано");
                                        $('#rejectApp').html('').hide();
                                        $("#dialog-form").dialog("close");
                                    }
                                }
                                else {
                                    //message to the user
                                    $('#errMessage').text(res.ErrorMessage).addClass("ui-state-highlight");
                                    setTimeout(function () {
                                        $('#errMessage').removeClass("ui-state-highlight", 1500);
                                    }, 500);
                                }
                            }, 'json');
                        },
                        "Нет": function () {
                            $(this).dialog("close");
                        }
                    }
                });
        });
        function ValidateInput() {
            if ($.browser.msie) {
                var myFSO = new ActiveXObject("Scripting.FileSystemObject");
                var filepath = document.getElementById('fileAttachment').value;
                var thefile = myFSO.getFile(filepath);
                var size = thefile.size;
            } else {
                var fileInput = $("#fileAttachment")[0];
                var size = fileInput.files[0].size; // Size returned in bytes.
            }
            if (size > 4194304) {// 4194304 = 5Mb
                alert('To big file for uploading (4Mb - max)');
                //Очищаем поле ввода файла
                document.getElementById('fileAttachment').parentNode.innerHTML = document.getElementById('fileAttachment').parentNode.innerHTML;
            }
        }
    </script>
    <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
<% } %>
    <script type="text/javascript">
        function DeleteFile(id) {
            var p = new Object();
            p["id"] = id;
            $.post('/Application/DeleteFile', p, function (res) {
                if (res.IsOk) {
                    $('#' + id).hide(250).html("");
                }
                else {
                    if (res != undefined) {
                        alert(res.ErrorMessage);
                    }
                }
            }, 'json');
        }
        var portfolioHidden = false;
        function HidePortfolio() {
            if (!portfolioHidden) {
                $('#dPortfolio').hide(200);
                portfolioHidden = true;
            }
            else {
                $('#dPortfolio').show(200);
                portfolioHidden = false;
            }
        }
        var motivMailHelpHidden = true;
        function ShowMotivMailHelp() {
            if (motivMailHelpHidden) {
                $('#MotivationInfoHelp').show(200);
                motivMailHelpHidden = false;
            }
            else {
                $('#MotivationInfoHelp').hide(200);
                motivMailHelpHidden = true;
            }
        }
        var motivMailHidden = false;
        function HideMotivationMail() {
            if (!motivMailHidden) {
                $('#MotivationMail').hide(200);
                motivMailHidden = true;
            }
            else {
                $('#MotivationMail').show(200);
                motivMailHidden = false;
            }
        }
    </script>
<h4>Основные сведения</h4>
<hr />
<table class="paginate">
    <tr>
        <td width="30%" align="right">Направление</td>
        <td align="left"><%= Html.Encode(Model.Profession) %></td>
    </tr>
    <tr>
        <td width="30%" align="right">Образовательная программа</td>
        <td align="left"><%= Html.Encode(Model.ObrazProgram) %></td>
    </tr>
    <tr>
        <td width="30%" align="right">Профиль</td>
        <td align="left"><%= Html.Encode(Model.Specialization) %></td>
    </tr>
    <tr>
        <td width="30%" align="right">Скачать заявление</td>
        <td align="left"><a href="<%= string.Format("../../Application/GetPrint/{0}", Model.Id.ToString("N")) %>"><img src="../../Content/themes/base/images/PDF.png" alt="Скачать (PDF)" /></a></td>
    </tr>
    <tr>
        <td width="30%" align="right" valign="top">Статус заявления:</td>
        <td align="left">
            <span id="appStatus" style="font-size:14px" class="<%= Model.Enabled ? "Green" : "Red" %>">
                <%= Model.Enabled ? "Подано" : "Отозвано " + Model.DateOfDisable %>
            </span>
            <br /><br />
            <% if (Model.Enabled)
               { %>
            <p id="rejectApp">
                <button id="rejectBtn" class="button button-orange">Забрать заявление</button>
                <div id="dialog-form">
                    <p class="errMessage"></p>
                    <h5>Внимание!</h5>
                    <p>Отзывая данное заявление, вы отказываетесь от участия в конкурсе на указанную образовательную программу.Восстановить данное заявление будет уже невозможно.</p>
                    <h4>Вы хотите отказаться от участия в конкурсе?</h4>
                 </div>
            </p>
            <% } %>
        </td>
    </tr>
</table>
<br />
<div class="panel">
<h4 onclick="HidePortfolio()" style="cursor:pointer;">Портфолио</h4>
<div id="dPortfolio">
<hr />
<% if (Model.Files.Count > 0)
   { %>
    <table class="paginate" style="width:90%">
    <tr>
        <th></th>
        <th>Имя файла</th>
        <th>Размер</th>
        <th>Комментарий</th>
        <th>Удалить</th>
    </tr>    
<% }
   else
   { %>
   <h5>В портфолио нет файлов</h5>
<% } %>
    <tbody>
<% foreach (var file in Model.Files)
   { %>
    <tr id="<%= file.Id.ToString("N") %>">
        <td>
            <a href="<%= "../../Application/GetFile?id=" + file.Id.ToString("N") %>" target="_blank">
                <img src="../../Content/themes/base/images/downl1.png" alt="Скачать файл" />
            </a>
        </td>
        <td style="text-align:center; vertical-align:middle;">
            <span><%= Html.Encode(file.FileName) %></span>
        </td>
        <td style="text-align:center; vertical-align:middle;">
        <%= file.FileSize > (2 * 1024 * 1024) ?
            Math.Round(((double)file.FileSize / (1024.0 * 1024.0)), 2).ToString() + " Mb"
            :
            file.FileSize > 1024 ?
            Math.Round(((double)file.FileSize / 1024.0), 2).ToString() + " Kb"
            : file.FileSize.ToString()%>
        </td>
        <td style="text-align:center; vertical-align:middle;"><%= file.Comment %></td>
        <td style="text-align:center; vertical-align:middle;">
        <% if (!file.IsShared)
           { %>
            <span class="link" onclick="DeleteFile('<%= file.Id.ToString("N") %>')">
                <img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить" />
            </span>
        <% }
           else
           { %>
           <img src="../../Content/myimg/icon_shared3.png" />
        <% } %>
        </td>
    </tr>
<% } %>
</tbody>
</table><br />
<a class="button button-blue" href="../../Abiturient/FilesList?id=<%= Model.Id.ToString("N") %>" target="_blank">Опись поданных документов</a><br />
<% if (Model.Enabled)
   { %>
<br />
<h4>Добавить файл</h4>
<hr />
<form action="/Application/AddFile" method="post" enctype="multipart/form-data" class="form">
    <input type="hidden" name="id" value="<%= Model.Id.ToString("N") %>" />
    <div class="clearfix">
        <input id="fileAttachment" type="file" name="File" />
    </div><br />
    <div class="clearfix">
        <textarea id="fileComment" class="noresize" name="Comment" maxlength="1000" cols="80" rows="5"></textarea>
    </div><br />
    <div class="clearfix">
        <input id="btnSubmit" type="submit" value="Отправить" class="button button-gray"/>
    </div>
</form>
<% } %>
<br />
</div>
</div>
<% if (0 == 1) //no exams 'till now
   { %>
<h4>Экзамены</h4>
<table class="paginate">
    <% if (Model.Exams.Count > 0)
       { %>
        <tr>
            <th align="right">Состояние</th>
            <th>Название</th>
        </tr>
        <% foreach (string exam in Model.Exams)
           { %>
            <tr>
                <td width="30%" align="right"><%= Html.Encode("<ok when passed>")%></td>
                <td><%= exam %></td>
            </tr>
        <% } %>
    <% } %>
</table>
<% } %>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
</asp:Content>
