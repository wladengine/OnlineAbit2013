<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AG/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.AppendFilesModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Общие файлы
</asp:Content>

<asp:Content ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("AddSharedFiles", "Header") %></h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% if (0 == 1)
   { %>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
<% } %>
<script type="text/javascript" language="javascript">
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
        if (size > 41943040) {// 41943040 = 4Mb
            alert('Too big file for uploading (4Mb - max)');
            //Очищаем поле ввода файла
            document.getElementById('fileAttachment').parentNode.innerHTML = document.getElementById('fileAttachment').parentNode.innerHTML;
        }
    }
    function CheckEmpty() {
        return true;
    }
    function DeleteFile(id) {
        var p = new Object();
        p["id"] = id;
        $.post('/Abiturient/DeleteSharedFile', p, function (res) {
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
                    tbody += '<td align="center" valign="middle"><span class="link" onclick="DeleteFile(\'' + res.Data[i].Id + '\')"><img src="../../Content/themes/base/images/delete-icon.png" alt="<%= GetGlobalResourceObject("AddSharedFiles", "Delete") %>" /></span></td>';
                    tbody += '</tr>';
                }
                $('#tblFiles tbody').html(tbody);
            }
        }, 'json');
    }
</script>
<%= Html.ValidationSummary() %>
<p class="message info">
    <asp:Literal runat="server" Text="<%$ Resources:AddSharedFiles, HelpMessage_AG %>"></asp:Literal>
</p>
<form action="/Abiturient/AddSharedFile" method="post" class="form panel" enctype="multipart/form-data" onsubmit="return CheckEmpty()">
    <fieldset>
        <div class="clearfix">
            <label for="fileAttachment"><%= GetGlobalResourceObject("AddSharedFiles", "File") %></label>
            <input id="fileAttachment" type="file" name="File" />
        </div>
        <div class="clearfix">
            <label for="fileComment"><%= GetGlobalResourceObject("AddSharedFiles", "Comment") %></label>
            <textarea id="fileComment" cols="60" rows="5" class="noresize" name="Comment" maxlength="1000"></textarea>
        </div>
        <hr />
        <div class="clearfix">
            <input id="btnSubmit" type="submit" class="button button-green" value="<%= GetGlobalResourceObject("AddSharedFiles", "Submit") %>" />
        </div>
    </fieldset>
</form>

<h4><%= GetGlobalResourceObject("AddSharedFiles", "LoadedFiles")%></h4>
<% if (Model.Files.Count > 0)
   { %>
<table id="tblFiles" class="paginate" style="width:100%;">
    <thead>
        <tr>
            <th style="width:10%;"><%= GetGlobalResourceObject("AddSharedFiles", "View") %></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "FileName") %></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "Comment") %></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "Size") %></th>
            <th style="width:10%;"><%= GetGlobalResourceObject("AddSharedFiles", "Delete") %></th>
        </tr>
    </thead>
    <tbody>
<% foreach (var file in Model.Files)
   { %>
        <tr id="<%= file.Id.ToString() %>">
            <td style="vertical-align:middle; text-align:center;"><a href="<%= "../../Abiturient/GetFile?id=" + file.Id.ToString("N") %>" target="_blank"><img src="../../Content/themes/base/images/downl1.png" alt="Скачать файл" /></a></td>
            <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(file.FileName) %></td>
            <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(file.Comment) %></td>
            <td style="vertical-align:middle; text-align:center;"><%= file.FileSize > (2 * 1024 * 1024) ?
                Math.Round(((double)file.FileSize / (1024.0 * 1024.0)), 2).ToString() + " Mb"
                :
                file.FileSize > 1024 ?
                Math.Round(((double)file.FileSize / 1024.0), 2).ToString() + " Kb"
                : file.FileSize.ToString() %></td>
            <td style="vertical-align:middle; text-align:center;">
                <span class="link" onclick="DeleteFile('<%= file.Id.ToString() %>')">
                    <img src="../../Content/themes/base/images/delete-icon.png" alt="<%= GetGlobalResourceObject("AddSharedFiles", "Delete") %>" />
                </span>
            </td>
        </tr>
<% } %>
    </tbody>
</table>
<% }
   else
   { %>
<h5><%= GetGlobalResourceObject("AddSharedFiles", "NoFiles") %></h5>
<% } %>
<br />
<%--<asp:Literal runat="server" Text="<%$ Resources:AddSharedFiles, RulesInformation %>"></asp:Literal>--%>
</asp:Content>
