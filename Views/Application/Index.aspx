<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Abiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.ExtApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Просмотр заявления
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Просмотр заявления</h2>
</asp:Content>

<asp:Content ID="HeaderScripts" ContentPlaceHolderID="HeaderScriptsContent" runat="server">
    <script src="https://api-maps.yandex.ru/2.0/?load=package.full&lang=ru-RU"
            type="text/javascript"></script>
    <script type="text/javascript">
        ymaps.ready(init);

        function init() {
            var myMap = new ymaps.Map("map", {
                center: [<%= Model.ComissionYaCoord %>],
                zoom: 16
            });

            myMap.controls.add('typeSelector');

            myMap.balloon.open(
                [<%= Model.ComissionYaCoord %>], {
                    contentBody: '<%= Model.ComissionAddress %>'
                }, {
                    closeButton: false
                });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% if (1 == 2)
   { %>
   <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
<% } %>
<% if (Model.Enabled)
   { %>
    <script type="text/javascript">
        $(function () {
            $('#UILink').hide();
            $('#fileAttachment').change(ValidateInput);
            $('#MotivateAttachment').change(ValidateInput_Motivate);
            $('#EssayAttachment').change(ValidateInput_Essay);
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
                        "Да": function () 
                        {
                            $.post('/Application/Disable', { id: '<%= Model.Id.ToString("N") %>' }, function (res) 
                            {
                                if (res.IsOk) 
                                {
                                    if (!res.Enabled) 
                                    {
                                        $('#appStatus').removeClass("Green").addClass("Red").text("Отозвано");
                                        $('#rejectApp').html('').hide();
                                        $("#dialog-form").dialog("close");
                                    }
                                }
                                else 
                                {
                                    //message to the user
                                    $('#errMessage').text(res.ErrorMessage).addClass("ui-state-highlight");
                                    setTimeout(function () 
                                    {
                                        $('#errMessage').removeClass("ui-state-highlight", 1500);
                                    }, 500);
                                }
                            }, 'json');
                        },
                        "Нет": function () 
                        {
                            $(this).dialog("close");
                        }
                    }
                });
        }); 
    function ValidateInput() {
        var size = 0;
        if ($.browser.msie) {
            var myFSO = new ActiveXObject("Scripting.FileSystemObject");
            var filepath = document.getElementById('fileAttachment').value;
            var thefile = myFSO.getFile(filepath);
            size = thefile.size;
        } else {
            var fileInput = $("#fileAttachment")[0];
            if (fileInput.files[0] != undefined) {
                size = fileInput.files[0].size; // Size returned in bytes.
            }
        }
        if (size > 4194304) {// 4194304 = 5Mb
            alert('To big file for uploading (4Mb - max)');
            //Очищаем поле ввода файла
            document.getElementById('fileAttachment').parentNode.innerHTML = document.getElementById('fileAttachment').parentNode.innerHTML;
        }
    }

    function ValidateInput_Essay() {
        var size = 0;
        if ($.browser.msie) {
            var myFSO = new ActiveXObject("Scripting.FileSystemObject");
            var filepath = document.getElementById('EssayAttachment').value;
            var thefile = myFSO.getFile(filepath);
            size = thefile.size;
        } else {
            var fileInput = $("#EssayAttachment")[0];
            if (fileInput.files[0] != undefined) {
                size = fileInput.files[0].size; // Size returned in bytes.
            }
        }
        if (size > 4194304) {// 4194304 = 5Mb
            alert('To big file for uploading (4Mb - max)');
            //Очищаем поле ввода файла
            document.getElementById('EssayAttachment').parentNode.innerHTML = document.getElementById('EssayAttachment').parentNode.innerHTML;
        }
    }

    function ValidateInput_Motivate() {
        var size = 0;
        if ($.browser.msie) {
            var myFSO = new ActiveXObject("Scripting.FileSystemObject");
            var filepath = document.getElementById('MotivateAttachment').value;
            var thefile = myFSO.getFile(filepath);
            size = thefile.size;
        } else {
            var fileInput = $("#MotivateAttachment")[0];
            if (fileInput.files[0] != undefined) {
                size = fileInput.files[0].size; // Size returned in bytes.
            }
        }
        if (size > 4194304) {// 4194304 = 5Mb
            alert('To big file for uploading (4Mb - max)');
            //Очищаем поле ввода файла
            document.getElementById('MotivateAttachment').parentNode.innerHTML = document.getElementById('MotivateAttachment').parentNode.innerHTML;
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
    var essayHidden = false;
    function HideEssay() {
        if (!essayHidden) {
            $('#Essay').hide(200);
            essayHidden = true;
        }
        else {
            $('#Essay').show(200);
            essayHidden = false;
        }
    }
    </script>
    
<h4>Основные сведения</h4>
<hr />
<table class="paginate">
<% if (Model.Enabled)
   { %>
    <tr>
        <td width="30%" align="right"><abbr title="Наивысший приоритет равен 1">Приоритет</abbr></td>
        <td align="left"><%= Html.Encode(Model.Priority)%></td>
    </tr>
<% } %>
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
        <td width="30%" align="right">Форма обучения</td>
        <td align="left"><%= Html.Encode(Model.StudyForm) %></td>
    </tr>
    <tr>
        <td width="30%" align="right">Основа обучения</td>
        <td align="left"><%= Html.Encode(Model.StudyBasis) %></td>
    </tr>
    <tr>
        <td width="30%" align="right">Скачать заявление</td>
        <td align="left"><a href="<%= string.Format("../../Application/GetPrint/{0}", Model.Id.ToString("N")) %>"><img src="../../Content/themes/base/images/PDF.png" alt="Скачать (PDF)" /></a></td>
    </tr>
    
    <tr>
        <td width="30%" align="right" valign="top">Статус заявления:</td>
        <td align="left">
            <span id="appStatus" style="font-size:14px" class="<%= Model.Enabled ? "Green" : "Red" %>">
                <%= Model.Enabled ? "Подано" : "Отозвано " + Model.DateOfDisable%>
            </span>
            <br /><br />
            <% if (Model.Enabled)
               { %>
                <% if (Model.EntryTypeId != 2)
                   { %>
            <p id="rejectApp">
                <button id="rejectBtn" class="button button-orange">Удалить заявление</button>
                <div id="dialog-form">
                    <p class="errMessage"></p>
                    <h5>Внимание!</h5>
                    <p>Вы собираетесь удалить данное заявление. Данное действие нельзя будет отменить.</p>
                    <h4>Вы хотите удалить заявление?</h4>
                 </div>
            </p>
                <% } %>
                <% if (Model.EntryTypeId == 2)
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
            <% } %>
        </td>
    </tr>
    
</table>
<br />
<% if (Model.EntryTypeId != 2 && Model.AbiturientTypeId == 1)
   { %>
   <div class="message info">
    <b>Вам следует <a href="<%= string.Format("../../Application/GetPrint/{0}", Model.Id.ToString("N")) %>">распечатать заявление</a> (или запомнить штрих-код и данные по заявлению) и прийти с ним в приёмную комиссию</b> 
   </div>
   <div id="map" style="width:600px;height:300px"></div>
<% } %>
<% if (Model.EntryTypeId == 2)
   { %>

    <div class="panel">
    <h4 style="cursor:pointer;" onclick="HideMotivationMail()">Мотивационное письмо</h4>
    <div id="MotivationMail">
    <hr />
    <div id="MotivationInfoHelp" class="message info">
        <b>В мотивационном письме должны содержаться:</b> 
        <ul>
            <li>необходимые сведения об опыте профессиональной подготовки/деятельности;</li>
            <li>сведения, подтверждающие необходимость получения знаний/навыков, освоение/приобретение которых возможно в период обучения на выбранной программе;</li>
            <li>перспективы/планы реализации полученных знаний/навыков в будущей профессиональной деятельности.</li>
        </ul>
    </div>
    <form action="../../Application/MotivatePost" enctype="multipart/form-data" method="post">
        <input type="hidden" name="id" value="<%= Model.Id.ToString("N") %>" />
        <div class="clearfix">
            <input id="MotivateAttachment" type="file" name="File" />
        </div><br />
        <div class="clearfix">
            <input id="MotivateSubmit" type="submit" value="Отправить" class="button button-gray"/>
        </div>
    </form>
    </div>
    </div>

    <div class="panel">
    <h4 style="cursor:pointer;" onclick="HideEssay()">Эссе</h4>
    <div id="Essay">
    <hr />
    <form action="../../Application/EssayPost" enctype="multipart/form-data" method="post">
        <input type="hidden" name="id" value="<%= Model.Id.ToString("N") %>" />
        <div class="clearfix">
            <input id="EssayAttachment" type="file" name="File" />
        </div><br />
        <div class="clearfix">
            <input id="EssaySubmit" type="submit" value="Отправить" class="button button-gray"/>
        </div>
    </form>
    </div>
    </div>

    <div class="panel">
    <h4 onclick="HidePortfolio()" style="cursor:pointer;">Портфолио</h4>
    <div class="message info">
        <b>Пожалуйста, прикрепляйте те файлы, которые общие для каждого заявления (сканы паспорта/документа об образовании и т.п.) в раздел </b> 
        <a href="../../Abiturient/AddSharedFiles" style="font-weight:bold">Общие файлы</a>
    </div>
    <div id="dPortfolio">
    <hr />
    <% if (Model.Files.Count > 0)
       { %>
        <table class="paginate">
        <tr>
            <th></th>
            <th>Имя файла</th>
            <th>Размер</th>
            <th>Комментарий</th>
            <th>Статус</th>
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
                <span><%= Html.Encode(file.FileName)%></span>
            </td>
            <td style="text-align:center; vertical-align:middle;"><%= file.FileSize > (2 * 1024 * 1024) ?
                        Math.Round(((double)file.FileSize / (1024.0 * 1024.0)), 2).ToString() + " Mb"
                        :
                        file.FileSize > 1024 ?
                        Math.Round(((double)file.FileSize / 1024.0), 2).ToString() + " Kb"
                        : file.FileSize.ToString()%>
            </td>
            <td style="text-align:center; vertical-align:middle;"><%= file.Comment%></td>
            <td style="text-align:center; vertical-align:middle;" <%= file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Approved ? "class=\"Green\"" : file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Rejected ? "class=\"Red\"" : "class=\"Blue\"" %>  >
                <span style="font-weight:bold">
                <%= file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Approved ?
                           GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Approved") :
                                  file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Rejected ? GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Rejected") :
                                  GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_NotSet")
                %>
                </span>
            </td>
            <td  style="text-align:center; vertical-align:middle;">
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
    <div class="panel">
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
    </div>
<% } %>
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
