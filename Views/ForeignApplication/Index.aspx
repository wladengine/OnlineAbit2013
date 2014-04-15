<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Abiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.ExtApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("ApplicationInfo", "Title") %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("ApplicationInfo", "Subheader") %></h2>
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
                        $.post('/ForeignApplication/Disable', { id: '<%= Model.Id.ToString("N") %>' }, function (res) {
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
    </script>
<h4><%= GetGlobalResourceObject("ApplicationInfo", "HeaderBaseInfo")%></h4>
<hr />
<table class="paginate" style="width:70%;">
<% if (Model.Enabled)
   { %>
    <tr>
        <td width="30%" align="right"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:ApplicationInfo, Priority %>"></asp:Literal></td>
        <td align="left"><%= Html.Encode(Model.Priority)%></td>
    </tr>
<% } %>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("ApplicationInfo", "Profession")%></td>
        <td align="left"><%= Html.Encode(Model.Profession) %></td>
    </tr>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("ApplicationInfo", "ObrazProgram")%></td>
        <td align="left"><%= Html.Encode(Model.ObrazProgram) %></td>
    </tr>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("ApplicationInfo", "Profile")%></td>
        <td align="left"><%= Html.Encode(Model.Specialization) %></td>
    </tr>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("ApplicationInfo", "StudyForm")%></td>
        <td align="left"><%= Html.Encode(Model.StudyForm) %></td>
    </tr>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("ApplicationInfo", "Download")%></td>
        <td align="left"><a href="<%= string.Format("../../ForeignApplication/GetPrint/{0}", Model.Id.ToString("N")) %>"><img src="../../Content/themes/base/images/PDF.png" alt="Скачать (PDF)" /></a></td>
    </tr>
    <tr>
        <td width="30%" align="right" valign="top"><%= GetGlobalResourceObject("ApplicationInfo", "AppStatus")%></td>
        <td align="left">
            <span id="appStatus" style="font-size:14px; font-weight:bolder;" class="<%= Model.Enabled ? "Green" : "Red" %>">
                <%= Model.Enabled ? GetGlobalResourceObject("ApplicationInfo", "StatusActive").ToString() : 
                GetGlobalResourceObject("ApplicationInfo", "StatusCancelled").ToString() + Model.DateOfDisable%>
            </span>
        </td>
    </tr>
    <tr>
        <td width="30%" align="right" valign="top"></td>
        <td align="left">
            <% if (Model.Enabled)
               { %>
            <button id="rejectBtn" type="button" class="button button-orange"><%= GetGlobalResourceObject("ApplicationInfo", "btnCancelApplication")%></button>
            <% } %>
        </td>
    </tr>
</table>

<h4><%= GetGlobalResourceObject("ApplicationInfo", "HeaderPortfolio")%></h4>
<hr />
<% if (Model.Files.Count > 0)
   { %>
    <table class="paginate" style="width:70%;">
    <thead>
        <tr>
            <th></th>
            <th><%= GetGlobalResourceObject("ApplicationInfo", "HeaderFilename")%></th>
            <th><%= GetGlobalResourceObject("ApplicationInfo", "HeaderFilesize")%></th>
            <th><%= GetGlobalResourceObject("ApplicationInfo", "HeaderFileComments")%></th>
            <th><%= GetGlobalResourceObject("ApplicationInfo", "HeaderDelete")%></th>
        </tr>
    </thead>
<% }
   else
   { %>
   <h5><%= GetGlobalResourceObject("ApplicationInfo", "HeaderDelete")%></h5>
<% } %>
<% foreach (var file in Model.Files)
   { %>
    <tr id="<%= file.Id.ToString("N") %>">
        <td>
            <a href="<%= "../../Application/GetFile?id=" + file.Id.ToString("N") %>" target="_blank">
                <img src="../../Content/themes/base/images/downl1.png" alt="Dowload the file" />
            </a>
        </td>
        <td align="right">
            <span><%= Html.Encode(file.FileName) %></span>
        </td>
        <td><%= file.FileSize > (2 * 1024 * 1024) ?
                    Math.Round(((double)file.FileSize / (1024.0 * 1024.0)), 2).ToString() + " Mb"
                    :
                    file.FileSize > 1024 ?
                    Math.Round(((double)file.FileSize / 1024.0), 2).ToString() + " Kb"
                    : file.FileSize.ToString()%>
        </td>
        <td><%= file.Comment %></td>
        <td align="center">
            <span class="link" onclick="DeleteFile('<%= file.Id.ToString("N") %>')">
                <img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить" />
            </span>
        </td>
    </tr>
<% } %>
</table>
<% if (Model.Enabled)
   { %>
<h4><%= GetGlobalResourceObject("ApplicationInfo", "HeaderAddFile")%></h4>
<hr />
<form action="/ForeignApplication/AddFile" method="post" enctype="multipart/form-data" class="form">
    <fieldset>
        <div class="clearfix">
            <input id="fileAttachment" type="file" name="File" />
        </div><br />
        <div class="clearfix">
            <textarea id="fileComment" name="Comment" maxlength="1000" rows="6" cols="60">
            </textarea>
        </div><br />
        <div class="clearfix">
            <input id="btnSubmit" type="submit" value="<%= GetGlobalResourceObject("ApplicationInfo", "btnSubmit")%>" class="button button-blue"/>
        </div>
    </fieldset>
    <input type="hidden" name="id" value="<%= Model.Id.ToString("N") %>" />
</form>
<% } %>

<h4><%= GetGlobalResourceObject("ApplicationInfo", "HeaderMotivationalMail")%></h4>
<hr />
<div id="MotivationInfoHelp" style="display:none;">
    <asp:Literal Text="<%$ Resources:ApplicationInfo, MotivationalMailInformation %>" runat="server"></asp:Literal>
</div>
<form action="../../Application/MotivatePost" method="post" class="form">
    <fieldset>
        <input type="hidden" name="id" value="<%= Model.Id.ToString("N") %>" />
        <div class="clearfix">
            <textarea class="noresize" cols="120" rows="10"><%= Model.MotivateEditText %></textarea>
        </div>
        <div class="clearfix">
            <button type="submit" class="button button-green"><%= Model.MotivateEditId == Guid.Empty ? GetGlobalResourceObject("ApplicationInfo", "btnSubmit").ToString() : GetGlobalResourceObject("ApplicationInfo", "btnSaveChanges").ToString() %></button>
            <% if (!string.IsNullOrEmpty(Model.MotivateEditText))
               { %>
            <a target="_blank" class="button button-blue" href="../../Abiturient/GetMotivationMailPDF/<%= Model.MotivateEditId.ToString("N") %>"><%= GetGlobalResourceObject("ApplicationInfo", "btnViewMotivationalMail")%></a>
            <% } %>
        </div>
    </fieldset>
    <br />
</form>

<div id="dialog-form">
    <p class="errMessage"></p>
    <asp:Literal runat="server" Text="<%$ Resources:ApplicationInfo, CancelAppMessage %>"></asp:Literal>
</div>
</asp:Content>
