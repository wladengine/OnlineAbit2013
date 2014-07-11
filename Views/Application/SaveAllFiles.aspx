<%@ Page Title="" Language="C#"  MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master"  Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.FileListChecker>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%//= GetGlobalResourceObject("ApplicationInfo", "Title")%>
    Проверка файлов: мотивационных писем и эссе
</asp:Content>
<asp:Content ID="SubheaderContent" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Журналистика</h2>
    <%//= GetGlobalResourceObject("ApplicationInfo", "Title")%> 
</asp:Content>
 

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% if (1 == 2)
   { %>
   <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
<% } %>
    <script type="text/javascript">
        function CheckMark(i) {
            var val = $('#Mark'+i).val();
            var regex = /^([0-9])+$/i;
            if (val != '') {
                if (!regex.test(val)) { 
                    return false;
                }
                else {
                }
            }
            return true;
        }
        function SaveMark(i) {
            if (CheckMark(i)) {
                var FileId = $('#FileId' + i).val();
                var Mark = $('#Mark' + i).val();
                $.post('/Application/AddMark', { FileId: FileId, Mark: Mark }, function (json_data) {
                    if (json_data.IsOk) { }
                    else { alert(json_data.ErrorMessage); }
                }, 'json');
            }
        }
    </script>
    <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script> 
<style> 
   .grid_2
   {
       width: 0px;
       display: none;
   }
   .wrapper
   {
       width: 1290px;
   }
   .grid_6
   {
       width: 1290px;
   }
   .first 
   {
       width: 1290px;
   }
</style>
     <h3>Журналистика (Глобальная коммуникация и международная журналистика)</h3>
    <div class="panel" style="background:#eaeaea !important;">
    <% if (Model.Files.Count > 0)
        { %>
        <table id="tblFiles" class="paginate" style="width:99%;">
            <thead>
                <th>№</th>
                <th>Скачать</th>
                <th><%= GetGlobalResourceObject("SaveAllFiles", "FileAuthor").ToString()%></th>
                <th><%= GetGlobalResourceObject("SaveAllFiles", "FileName").ToString()%></th>
                <th><%= GetGlobalResourceObject("SaveAllFiles", "Comment").ToString()%></th>
                <th><%= GetGlobalResourceObject("SaveAllFiles", "FileType").ToString()%></th>
                <th><%= GetGlobalResourceObject("SaveAllFiles", "FileLocation").ToString()%></th>
                <th><%= GetGlobalResourceObject("SaveAllFiles", "ApprovalStatus_Header").ToString()%></th>
                <th><%= GetGlobalResourceObject("SaveAllFiles", "FileMark").ToString()%></th>
                <th><%= GetGlobalResourceObject("SaveAllFiles", "SaveMark").ToString()%></th>
            </thead>    
    <% }
        else
        { %>
        <h5><%= GetGlobalResourceObject("AddSharedFiles", "NoFiles").ToString()%></h5>
    <% } %>
            <tbody>
        <% int i = 0; foreach (var file in Model.Files)
            { i++;
               %> 
                <tr> 
                    <td style="display: none;"> <input type="hidden" id = "FileId<%=i.ToString() %>"value="<%=file.Id.ToString("N")%>" /></td>
                    <td style="text-align:center; vertical-align:middle; width: 15px;"> <% =i.ToString() %></td> 
                    <td style="text-align:center; vertical-align:middle; width: 15px;">
                        <a href="<%= "../../Application/GetFile?id=" + file.Id.ToString("N") %>" target="_blank">
                            <img src="../../Content/themes/base/images/downl1.png" alt="Скачать файл" />
                        </a>
                    </td>
                     <td style="text-align:center; vertical-align:middle; width: 217px;">
                        <span><%= Html.Encode(file.Author)%></span>
                    </td> 
                    <td style="text-align:center; vertical-align:middle; width: 304px;">
                        <span><%= Html.Encode(file.FileName)%></span>
                    </td> 
                    <td style="text-align:center; vertical-align:middle; width: 450px;"><%= file.Comment%></td>
                    <td style="text-align:center; vertical-align:middle; width: 325px;"><%= file.FileType%></td>
                    <td style="text-align:center; vertical-align:middle; width: 110px;" ><%= file.AddInfo%></td>
                    <td style="text-align:center; vertical-align:middle; width: 50px;" <%= file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Approved ? "class=\"Green\"" : file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Rejected ? "class=\"Red\"" : "class=\"Blue\"" %>  >
                        <span style="font-weight:bold">
                        <%= file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Approved ?
                                    GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Approved") :
                                            file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Rejected ? GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Rejected") :
                                            GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_NotSet")
                        %>
                        </span>
                    </td>
                    <td style="text-align:center; vertical-align:middle; width: 15px;"><input type='text' id='Mark<%=i.ToString()%>' value='<%= file.Mark%>' style="min-width: 15px; width: 20px;"/></td>
                    <td style="text-align:center; vertical-align:middle; width: 15px;"><button value="save" onclick ="SaveMark(<%=i.ToString()%>)">Save</button> </td>
                </tr>
        <% } %>
            </tbody>
        </table> 
        </div>
</asp:Content>
