﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.ExtApplicationCommitModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("ApplicationInfo", "Title")%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("ApplicationInfo", "Title")%></h2>
</asp:Content>

<asp:Content ID="HeaderScripts" ContentPlaceHolderID="HeaderScriptsContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% if (1 == 2)
   { %>
   <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
<% } %>
    <script type="text/javascript">
        $(function () {
            $('#fileAttachment').change(ValidateInput);
            $("#rejectBtn")
                //.button()
                .click(function () {
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
                        "Да (yes)": function () 
                        {
                            $.post('/Application/DisableFull', { id: '<%= Model.Id.ToString("N") %>' }, function (res) 
                            {
                                if (res.IsOk) 
                                {
                                    if (!res.Enabled) 
                                    {
                                        $('#appStatus').removeClass("Green").addClass("Red").text("Отозвано");
                                        $('#rejectApp').html('').hide();
                                        $("#dialog-form").dialog("close");
                                        location.reload(true);
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
                        "Нет (no)": function () 
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
        if (size > 4*1024*1024) {// 4194304 = 4Mb
            alert('Too big file for uploading (4Mb - max)');
            //Очищаем поле ввода файла
            document.getElementById('fileAttachment').parentNode.innerHTML = document.getElementById('fileAttachment').parentNode.innerHTML;
            $('#fileAttachment').change(ValidateInput);
        }
    }
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
    </script>
    <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
<style>
   td {
    padding: 1px 10px 1px 10px; 
   }
</style>
    <% if (Model.Applications.Count == 0)
   { %>
    <div class="message error">
        <b><%= GetGlobalResourceObject("ApplicationInfo", "ApplicationCanceled")%></b>
    </div>
<% } else { %>
<table>
    <tr>
        <td><a href="<%= string.Format("../../Application/GetPrint/{0}", Model.Id.ToString("N")) %>"><img src="../../Content/themes/base/images/PDF.png" alt="Скачать (PDF)" /></a></td>
        <td>
            <a href="#" id="rejectBtn">
                <img  src="../../Content/themes/base/images/Delete064.png" alt="Удалить" />
            </a>
        </td>
        <td>
            <a href="<%= string.Format("../../AbiturientNew/PriorityChanger?CommitId={0}", Model.Id.ToString("N")) %>">
                <img src="../../Content/themes/base/images/transfer-down-up.png" alt="Изменить приоритеты" />
            </a>
        </td>
        <td>
            <a href="<%= string.Format("../../AbiturientNew/ChangeApplication?Id={0}", Model.Id.ToString("N")) %>">
                <img src="../../Content/themes/base/images/File_edit064.png" alt="Редактировать заявление" />
            </a>
        </td>
    </tr>
    <tr>
        <td><%= GetGlobalResourceObject("ApplicationInfo", "Download")%></td>
        <td><%= GetGlobalResourceObject("ApplicationInfo", "ApplicationDelete")%></td>
        <td><%= GetGlobalResourceObject("ApplicationInfo", "ApplicationPriorityChange")%></td>
        <td><%= GetGlobalResourceObject("ApplicationInfo", "ApplicationChange")%></td>
    </tr>

    <tr>
        
    </tr>
</table>
<div id="dialog-form">
    <p class="errMessage"></p>
    <h5><%= GetGlobalResourceObject("ApplicationInfo", "DeleteApp_Warning1")%></h5>
    <p><%= GetGlobalResourceObject("ApplicationInfo", "DeleteApp_Warning2")%>.</p>
    <h4><%= GetGlobalResourceObject("ApplicationInfo", "DeleteApp_Warning3")%></h4>
</div>
<h4><%= GetGlobalResourceObject("ApplicationInfo", "ApplicationInfoHeader")%> </h4>
<hr />
<% foreach (var Application in Model.Applications.OrderBy(x => x.Priority).ThenBy(x => x.ObrazProgram))
   { %>
<table class="paginate" style="width: 659px;">
    <% if (Application.Enabled) { %>
    <tr>
        <td width="30%" align="right"><abbr title="Наивысший приоритет равен 1"><%= GetGlobalResourceObject("PriorityChangerForeign", "Priority").ToString()%> </abbr></td>
        <td align="left"><%= Html.Encode(Application.Priority) %></td>
    </tr>
    <% } %>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("PriorityChangerForeign", "LicenseProgram").ToString()%></td>
        <td align="left"><%= Html.Encode(Application.Profession) %></td>
    </tr>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("PriorityChangerForeign", "ObrazProgram").ToString()%></td>
        <td align="left"><%= Html.Encode(Application.ObrazProgram) %></td>
    </tr>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("PriorityChangerForeign", "Profile").ToString()%></td>
        <td align="left"><%= Html.Encode(Application.Specialization) %></td>
    </tr>
    <%  if (Application.HasManualExams)
        {%>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("PriorityChangerForeign", "ManualExam").ToString()%></td>
        <td align="left"><%= Html.Encode(Application.ManualExam) %></td>
    </tr>
    <%  } %>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("PriorityChangerForeign", "StudyForm").ToString()%></td>
        <td align="left"><%= Html.Encode(Application.StudyForm) %></td>
    </tr>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("PriorityChangerForeign", "StudyBasis").ToString()%></td>
        <td align="left"><%= Html.Encode(Application.StudyBasis) %></td>
    </tr>
    <% if (Application.IsGosLine)
       { %>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("NewApplication", "EnterGosLine").ToString()%></td>
        <td align="left">да</td>
    </tr>
    <% } %>
    <tr>
        <td width="30%" align="right"></td>
        <td align="left"><a class="button button-orange" href="../../Application/AppIndex/<%= Application.Id.ToString("N") %>"><%= GetGlobalResourceObject("ApplicationInfo", "ViewAddFiles")%>  </a></td>
    </tr>
</table>
<br />
<% } %>

<div class="panel">
    <h4 onclick="HidePortfolio()" style="cursor:pointer;"><%= GetGlobalResourceObject("AddSharedFiles", "LoadedFiles")%></h4>
    <div class="message info">
        <b><%= GetGlobalResourceObject("ApplicationInfo", "FilesWarning1")%></b> 
        <a href="../../AbiturientNew/AddSharedFiles" style="font-weight:bold"><%= GetGlobalResourceObject("AddSharedFiles", "Header")%></a>
        <br />
        <b><%= GetGlobalResourceObject("ApplicationInfo", "FilesWarning2")%></b>
    </div>
    <div id="dPortfolio">
        <hr />
    <% if (Model.Files.Count > 0)
        { %>
        <table class="paginate" style="width:99%;">
            <thead>
                <th></th>
                <th><%= GetGlobalResourceObject("AddSharedFiles", "FileName").ToString()%></th>
                <th><%= GetGlobalResourceObject("AddSharedFiles", "Size").ToString()%></th>
                <th><%= GetGlobalResourceObject("AddSharedFiles", "Comment").ToString()%></th>
                <th><%= GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Header").ToString()%></th>
                <th><%= GetGlobalResourceObject("AddSharedFiles", "Delete").ToString()%></th>
            </thead>    
    <% }
        else
        { %>
        <h5><%= GetGlobalResourceObject("AddSharedFiles", "NoFiles").ToString()%></h5>
    <% } %>
            <tbody>
        <% foreach (var file in Model.Files)
            { %>
                <tr id='<%= file.Id.ToString("N") %>'>
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
                        <span title="Данный файл можно удалить в разделе 'Общие файлы'">
                            <img src="../../Content/myimg/icon_shared3.png" />
                        </span>
                    <% } %>
                    </td>
                </tr>
        <% } %>
            </tbody>
        </table>
        <div class="panel">
            <h4><%= GetGlobalResourceObject("ApplicationInfo", "HeaderAddFile").ToString()%></h4>
            <hr />
            <form action="/Application/AddFileInCommit" method="post" enctype="multipart/form-data" class="form" id="">
                <input type="hidden" name="id" value="<%= Model.Id.ToString("N") %>" />
                <div class="clearfix">
                    <input id="fileAttachment" type="file" name="File" />
                </div><br />
                <div class="clearfix">
                    <textarea id="fileComment" class="noresize" name="Comment" maxlength="1000" cols="80" rows="5"></textarea>
                </div><br />
                <div class="clearfix">
                    <input id="btnSubmit" type="submit" value=<%= GetGlobalResourceObject("AddSharedFiles", "Submit").ToString()%> class="button button-gray"/>
                </div>
            </form>
            <br />
        </div>
    </div>
</div>
<% } %>
</asp:Content>
