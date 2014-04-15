<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.AppendFilesModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Добавить файлы
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% if (0 == 1)
   { %>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
<% } %>
<script type="text/javascript">
    $(function () {
        $('#fileAttachment').change(ValidateInput);
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
        if (size > 5242880) {// 52428800 = 5Mb
            alert('To big file for uploading (5Mb - max)');
            //Очищаем поле ввода файла
            document.getElementById('fileAttachment').parentNode.innerHTML = document.getElementById('fileAttachment').parentNode.innerHTML;
        }
    }
    function DeleteFile(id) {
        var p = new Object();
        p["id"] = id;
        $.post('/Abiturient/DeleteFile', p, function (res) {
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
    function GetList() {
        $.post('/Abiturient/GetFileList', null, function (res) {
            if (res.IsOk) {
                var tbody = '';
                for (var i = 0; i < res.Data.length; i++) {
                    tbody += '<tr id="' + res.Data[i].Id + '">';
                    tbody += '<td align="center" valign="middle"><a href="' + res.Data[i].Path + '"><img src="../../Content/themes/base/images/downl1.png" alt="Скачать файл" /></a></td>';
                    tbody += '<td>' + res.Data[i].FileName + '</td>';
                    tbody += '<td>' + res.Data[i].FileSize + '</td>';
                    tbody += '<td align="center" valign="middle"><span class="link" onclick="DeleteFile(\'' + res.Data[i].Id + '\')"><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить" /></span></td>';
                    tbody += '</tr>';
                }
                $('#tblFiles tbody').html(tbody);
            }
        }, 'json');
    }
</script>
<%= Html.ValidationSummary() %>
<h2>Добавление файлов</h2>

<form action="/Abiturient/AddFile" method="post" enctype="multipart/form-data">
    <table>
        <tr>
            <td>Файл</td>
            <td><input id="fileAttachment" type="file" name="File" /></td>
        </tr>
        <tr>
            <td>Комментарий</td>
            <td><input id="fileComment" type="text" name="Comment" maxlength="1000"/></td>
        </tr>
        <tr>
            <td></td>
            <td><input id="btnSubmit" type="submit" value="Отправить" /></td>
        </tr>
    </table>
</form>

<h4>Загруженные файлы:</h4>
<table id="tblFiles">

<% if (Model.Files.Count > 0)
   { %>
   <thead>
    <tr class="ui-widget-header">
        <th>Просмотр</th>
        <th>Имя файла</th>
        <th>Комментарий</th>
        <th>Размер</th>
        <th>Удалить</th>
    </tr>
    </thead>
    <tbody>
<% foreach (var file in Model.Files)
   { %>
    <tr id="<%= file.Id.ToString() %>">
        <td align="center" valign="middle"><a href="<%= "../../Abiturient/GetFile?id=" + file.Id.ToString("N") %>" target="_blank"><img src="../../Content/themes/base/images/downl1.png" alt="Скачать файл" /></a></td>
        <td><%= Html.Encode(file.FileName) %></td>
        <td><%= Html.Encode(file.Comment) %></td>
        <td><%= file.FileSize > (2 * 1024 * 1024) ?
            Math.Round(((double)file.FileSize / (1024.0 * 1024.0)), 2).ToString() + " Mb"
            :
            file.FileSize > 1024 ?
            Math.Round(((double)file.FileSize / 1024.0), 2).ToString() + " Kb"
            : file.FileSize.ToString() %></td>
        <td align="center" valign="middle"><span class="link" onclick="DeleteFile('<%= file.Id.ToString() %>')"><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить" /></span></td>
    </tr>
<% } %>
    </tbody>
<% }
   else
   { %>
    <h5>Нет файлов</h5>
<% } %>
</table><br />
<h3>Информация для поступающих:</h3>
В соответствии с пунктом 2.21 Правил приема в Санкт-Петербургский государственный университет на основные образовательные программы высшего профессионального образования (программы бакалавриата, программы подготовки специалиста, программы магистратуру) в 2011 году (далее – Правила приема) для участия в конкурсе на основные образовательные программы магистратуры Вам необходимо представить следующие документы:
    <ul>
        <li> копию документа, удостоверяющего личность и гражданство поступающего;</li>
        <li> заявление о приеме на образовательную программу по форме, установленной Приемной комиссией;</li>
        <li> 6 (шесть) фотографий 3х4 см (фотографии должны быть сделаны в текущем календарном году);</li>
        <li> документ об образовании (или справку из вуза об успешном освоении образовательной программы высшего профессионального образования и прохождении итоговой государственной аттестации).</li>
    </ul><br/>
В соответствии с пунктом 7.15 Правил приема в качестве документов, рассматриваемых Приемной комиссией при проведении конкурса документов (портфолио) поступающих на основные образовательные программы магистратуры, могут учитываться следующие документы:
    <ul>
        <li> мотивационное письмо, подготовленное и подписанное поступающим в соответствии с требованиями, установленными в Критериях проведения конкурса документов (портфолио), объемом до 600 слов;</li>
        <li>дипломы победителей и лауреатов конкурсов научных, проектных работ и студенческих олимпиад разных уровней;</li>
        <li>документы, подтверждающие назначение именных стипендий министерств, ведомств, фондов, образовательных учреждений и др.;</li>
        <li>документы, свидетельствующие о повышении профессиональной квалификации, знании иностранного языка;</li>
        <li>научные или творческие работы поступающего (в зависимости от направленности образовательной программы);</li>
        <li>документы, подтверждающие наличие опыта работы в сфере практической деятельности.</li>
    </ul>
</asp:Content>
