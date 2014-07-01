<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.ExtApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("ApplicationInfo", "AppDetails")%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("ApplicationInfo", "AppDetails")%></h2>
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
            $('#fileAttachment').change(ValidateInput);
            $('#MotivateAttachment').change(ValidateInput_Motivate);
            $('#EssayAttachment').change(ValidateInput_Essay);
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
        if (size > 1024*11.5*1024) {// = 12Mb
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
        if (size > 1024 * 11.5 * 1024) {// 4194304 = 5Mb
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
        if (size > 1024 * 11.5 * 1024) {// 4194304 = 5Mb
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

<a href="../../AbiturientNew/Main/"><%= GetGlobalResourceObject("ApplicationInfo", "LinkMainMenu")%></a> - 
<a href="../../Application/Index/<%= Model.CommitId.ToString("N") %>"><%= Model.CommitName %></a> - 
<%= GetGlobalResourceObject("ApplicationInfo", "AppDetails")%>
<br />
<h4><%= GetGlobalResourceObject("ApplicationInfo", "ApplicationInfoHeader")%></h4>
<hr />
<table class="paginate">
<% if (Model.Enabled)
   { %>
    <tr>
        <td width="30%" align="right"><abbr title="Наивысший приоритет равен 1"><%= GetGlobalResourceObject("PriorityChangerForeign", "Priority").ToString()%></abbr></td>
        <td align="left"><%= Html.Encode(Model.Priority)%></td>
    </tr>
<% } %>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("NewApplication", "BlockData_LicenseProgram")%></td>
        <td align="left"><%= Html.Encode(Model.Profession) %></td>
    </tr>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("NewApplication", "BlockData_ObrazProgram")%></td>
        <td align="left"><%= Html.Encode(Model.ObrazProgram) %></td>
    </tr>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("NewApplication", "BlockData_Specialization")%></td>
        <td align="left"><%= Html.Encode(Model.Specialization) %></td>
    </tr>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("NewApplication", "BlockData_StudyForm")%></td>
        <td align="left"><%= Html.Encode(Model.StudyForm) %></td>
    </tr>
    <tr>
        <td width="30%" align="right"><%= GetGlobalResourceObject("NewApplication", "BlockData_StudyBasis")%></td>
        <td align="left"><%= Html.Encode(Model.StudyBasis) %></td>
    </tr>
</table>
<br />
<% if (Model.EntryTypeId != 2 && (Model.AbiturientType == OnlineAbit2013.Models.AbitType.AG || Model.AbiturientType == OnlineAbit2013.Models.AbitType.FirstCourseBakSpec))
   { %>
   <div class="message info">
    <b><%= GetGlobalResourceObject("NewApplication", "PrintFormWarning1")%> <%= GetGlobalResourceObject("NewApplication", "PrintFormWarning2")%></b> 
   </div><br />
   <div id="map" style="width:600px;height:300px"></div>
<% } %>
<% else if (Model.EntryTypeId == 2)
   { %>
    <div class="panel">
    <h4 style="cursor:pointer;" onclick="HideMotivationMail()"> <%= GetGlobalResourceObject("ApplicationInfo", "HeaderMotivationalMail")%></h4>
    <div id="MotivationMail">
    <hr />
    <div id="MotivationInfoHelp" class="message info">
        <%= GetGlobalResourceObject("ApplicationInfo", "MotivationalMailInformation")%>
        <%= GetGlobalResourceObject("ApplicationInfo", "MotiveLetter_Link")%><%= GetGlobalResourceObject("ApplicationInfo", "MotiveLetterDetails")%></a><br />
        <%= GetGlobalResourceObject("ApplicationInfo", "MotiveLetter_Link2")%><%= GetGlobalResourceObject("ApplicationInfo", "MotiveLetterDetails2")%></a>
    </div>
    <form action="../../Application/MotivatePost" enctype="multipart/form-data" method="post">
        <input type="hidden" name="id" value="<%= Model.Id.ToString("N") %>" />
        <div class="clearfix">
            <input id="MotivateAttachment" type="file" name="File" />
        </div><br />
        <div class="clearfix">
            <input id="MotivateSubmit" type="submit" value=<%= GetGlobalResourceObject("NewApplication", "btnSubmit")%> class="button button-gray"/>
        </div>
    </form>
    </div>
    </div>
    <div class="panel">
    <h4 style="cursor:pointer;" onclick="HideEssay()"><%= GetGlobalResourceObject("ApplicationInfo", "HeaderEssay")%></h4>
    <div id="Essay">
    <hr />
    <form action="../../Application/EssayPost" enctype="multipart/form-data" method="post">
        <input type="hidden" name="id" value="<%= Model.Id.ToString("N") %>" />
        <div class="clearfix">
            <input id="EssayAttachment" type="file" name="File" />
        </div><br />
        <div class="clearfix">
            <input id="EssaySubmit" type="submit" value=<%= GetGlobalResourceObject("NewApplication", "btnSubmit")%> class="button button-gray"/>
        </div>
    </form>
    </div>
    </div>
    <div class="panel">
    <h4 onclick="HidePortfolio()" style="cursor:pointer;"><%= GetGlobalResourceObject("ApplicationInfo", "HeaderPortfolio")%> </h4>
    <div class="message info">
        <b><%= GetGlobalResourceObject("ApplicationInfo", "FilesWarning1")%> </b> 
        <a href="../../Abiturient/AddSharedFiles" style="font-weight:bold"><%= GetGlobalResourceObject("AddSharedFiles", "Header") %></a>
    </div>
    <div id="dPortfolio">
    <hr />
    <% if (Model.Files.Count > 0)
       { %>
        <table class="paginate">
        <tr>
            <th></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "FileName")%></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "Size")%></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "Comment")%></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Header")%></th>
            <th><%= GetGlobalResourceObject("AddSharedFiles", "Delete")%></th>
        </tr>    
    <% }
       else
       { %>
       <h5><%= GetGlobalResourceObject("NewApplication", "NoFiles")%></h5>
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
    <a class="button button-blue" href="../../Abiturient/FilesList?id=<%= Model.Id.ToString("N") %>" target="_blank"><%=GetGlobalResourceObject("AddSharedFiles", "FileList")%></a><br />
    <% if (Model.Enabled)
       { %>
    <br />
    <div class="panel">
    <h4><%=GetGlobalResourceObject("AddSharedFiles", "LoadedFiles")%></h4>
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
            <input id="btnSubmit" type="submit" value=<%= GetGlobalResourceObject("NewApplication", "btnSubmit")%> class="button button-gray"/>
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
    <h4><%=GetGlobalResourceObject("ApplicationInfo", "HeaderExams")%></h4>
    <table class="paginate">
        <% if (Model.Exams.Count > 0)
           { %>
            <tr>
                <th align="right"><%=GetGlobalResourceObject("ApplicationInfo", "ExamsStage")%></th>
                <th><%=GetGlobalResourceObject("ApplicationInfo", "ExamsName")%></th>
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
