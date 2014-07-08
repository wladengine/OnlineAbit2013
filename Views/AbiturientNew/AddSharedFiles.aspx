<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.AppendFilesModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("AddSharedFiles", "Header") %>
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
<script type="text/javascript">
    $(function () {
        SetTitle();
        $('#fileAttachment').change(ValidateInput);
        $('#FileTypeId').change(SetTitle);
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
        if (size > 4 * 1024 * 1024) {// 4194304 = 4Mb
            alert('Too big file for uploading (4Mb - max)');
            //Очищаем поле ввода файла
            document.getElementById('fileAttachment').parentNode.innerHTML = document.getElementById('fileAttachment').parentNode.innerHTML;
            $('#fileAttachment').change(ValidateInput);
        }
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
        var HideSomeFiles = $('#HideSomeFiles').is(':checked');
        $.post('/AbiturientNew/GetFileList', null, function (res) {
            if (res.IsOk) {
                var tbody = '';
                for (var i = 0; i < res.Data.length; i++) {
                    if (!HideSomeFiles || !res.Data[i].IsReadOnly) {
                        tbody += '<tr id="' + res.Data[i].Id + '">';
                        tbody += '<td style="vertical-align:middle; text-align:center;"><a href="../../AbiturientNew/GetFile?id=' + res.Data[i].Id + '" target="_blank"><img src="../../Content/themes/base/images/downl1.png" alt="Скачать файл" /></a></td>';
                        tbody += '<td style="vertical-align:middle; text-align:center;">' + res.Data[i].FileName + '</td>';
                        tbody += '<td style="vertical-align:middle; text-align:center;">' + res.Data[i].FileType + '</td>';
                        tbody += '<td style="vertical-align:middle; text-align:center;">' + res.Data[i].Comment + '</td>';
                        tbody += '<td style="vertical-align:middle; text-align:center;">';
                        if (res.Data[i].FileSize > (2 * 1024 * 1024)) {
                            tbody += " " + parseFloat(res.Data[i].FileSize / (1024.0 * 1024.0)).toFixed(2) + " Mb";
                        }
                        else {
                            if (res.Data[i].FileSize > 1024)
                                tbody += " " + parseFloat(res.Data[i].FileSize / 1024.0).toFixed(2) + " Kb";
                            else tbody += res.Data[i].FileSize + '</td>';
                        }
                        tbody += '<td style="vertical-align:middle; text-align:center;"><span style="font-weight:bold" ';
                        if (res.Data[i].IsApproved == 0) {
                            tbody += ' class="Green" >' + '<%=GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Approved")%>';
                        }
                        else {
                            if (res.Data[i].IsApproved == 1) {
                                tbody += ' class="Red" >' + '<%=GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Rejected")%>';
                            }
                            else
                                tbody += ' class="Blue" >' + '<%=GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_NotSet")%>';
                        }
                        tbody += "</td>";
                        tbody += '<td style="vertical-align:middle; text-align:center;"><span class="link" onclick="DeleteFile(\'' + res.Data[i].Id + '\')"><img src="../../Content/themes/base/images/delete-icon.png" alt="<%= GetGlobalResourceObject("AddSharedFiles", "Delete") %>" /></span></td>';
                        tbody += '</tr>';
                    }
                }
                $('#tblFiles tbody').html(tbody);
            }
        }, 'json');
    }
    function SetTitle() {
        var txt = $("#FileTypeId option:selected").text();
        $('#FileTypeId').attr("title", txt);
    }
</script>
<%= Html.ValidationSummary() %>
<p class="message info">
    <asp:Literal runat="server" Text="<%$ Resources:AddSharedFiles, HelpMessage_Foreign %>"></asp:Literal>
</p>
<form name = "filesform" action="/AbiturientNew/AddSharedFile" method="post" class="form panel" enctype="multipart/form-data">
    <fieldset>
        <div class="clearfix">
            <label for="fileAttachment"><%= GetGlobalResourceObject("AddSharedFiles", "File") %></label>
            <input id="fileAttachment" type="file" name="File" />
        </div>
        <br/> 
        <div class="clearfix">
        <label for="FileTypeId"><%=GetGlobalResourceObject("AddSharedFiles", "FileType").ToString()%></label> 
        <div style="width:200px; height:30px; overflow: hidden;">
         <%= Html.DropDownList("FileTypeId", Model.FileTypes )%>
        </div>
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
<input type="checkbox" id="HideSomeFiles" onClick="GetList();"><%= GetGlobalResourceObject("AddSharedFiles", "HideSomeFiles") %></input>
<div id = "divFiles" style="width: 664px; overflow-x: scroll; ">
<table id="tblFiles" class="paginate" style="width:100%;">
    <thead>
        <tr>
            <th style="width:10%;"><%= GetGlobalResourceObject("AddSharedFiles", "View") %></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "FileName") %></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "FileType") %></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "Comment") %></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "Size") %></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Header")%></th>
            <th style="width:10%;"><%= GetGlobalResourceObject("AddSharedFiles", "Delete") %></th>
        </tr>
    </thead>
    <tbody>
<% foreach (var file in Model.Files)
   { %>
        
        <tr id="<%= file.Id.ToString() %>">
            <td style="vertical-align:middle; text-align:center;"><a href="<%= "../../AbiturientNew/GetFile?id=" + file.Id.ToString("N") %>" target="_blank"><img src="../../Content/themes/base/images/downl1.png" alt="Скачать файл" /></a></td>
            <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(file.FileName) %></td>
            <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(file.FileType) %></td>
            <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(file.Comment) %></td>
            <td style="vertical-align:middle; text-align:center;"><%= file.FileSize > (2 * 1024 * 1024) ?
                Math.Round(((double)file.FileSize / (1024.0 * 1024.0)), 2).ToString() + " Mb"
                :
                file.FileSize > 1024 ?
                Math.Round(((double)file.FileSize / 1024.0), 2).ToString() + " Kb"
                : file.FileSize.ToString() %></td>
            <td style="vertical-align:middle; text-align:center;">
                <span style="font-weight:bold" <%= file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Approved ? "class=\"Green\"" : file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Rejected ? "class=\"Red\"" : "class=\"Blue\"" %>>
                <%= file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Approved ?
                       GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Approved") :
                       file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Rejected ? 
                       GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Rejected") :
                       GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_NotSet")
                %>
                </span>
            </td>
            <td style="vertical-align:middle; text-align:center;">
                <span class="link" onclick="DeleteFile('<%= file.Id.ToString() %>')">
                    <img src="../../Content/themes/base/images/delete-icon.png" alt="<%= GetGlobalResourceObject("AddSharedFiles", "Delete") %>" />
                </span>
            </td>
        </tr>
<% } %>
    </tbody>
</table>
</div>
<% }
   else
   { %>
<h5><%= GetGlobalResourceObject("AddSharedFiles", "NoFiles") %></h5>
<% } %>
<br />
<asp:Literal runat="server" Text="<%$ Resources:AddSharedFiles, RulesInformation %>"></asp:Literal>
</asp:Content>
