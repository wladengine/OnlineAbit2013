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
       width: 1190px;
   }
   .grid_6
   {
       width: 1190px;
   }
   .first 
   {
       width: 1190px;
   }
</style>
     <h3>Журналистика (Глобальная коммуникация и международная журналистика)</h3>
    <div class="panel">
    <% if (Model.Files.Count > 0)
        { %>
        <table id="tblFiles" class="paginate" style="width:99%;">
            <thead>
                <th>№</th>
                <th>Скачать</th>
                <th><%//= GetGlobalResourceObject("AddSharedFiles", "FileName").ToString()%>Автор файла</th>
                <th><%= GetGlobalResourceObject("AddSharedFiles", "FileName").ToString()%></th>
                <th><%= GetGlobalResourceObject("AddSharedFiles", "Comment").ToString()%></th>
                <th><%//= GetGlobalResourceObject("AddSharedFiles", "Comment").ToString()%>Тип файла</th>
                <th><%//= GetGlobalResourceObject("AddSharedFiles", "Comment").ToString()%>Файл приложен</th>
                <th><%= GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Header").ToString()%></th>
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
                <tr id='<%= file.Id.ToString("N") %>' > 
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
                    <td style="text-align:center; vertical-align:middle; width: 350px;"><%= file.Comment%></td>
                    <td style="text-align:center; vertical-align:middle; width: 350px;"><%= file.FileType%></td>
                    <td style="text-align:center; vertical-align:middle; width: 110px;" ><%= file.AddInfo%></td>
                    <td style="text-align:center; vertical-align:middle; width: 100px;" <%= file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Approved ? "class=\"Green\"" : file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Rejected ? "class=\"Red\"" : "class=\"Blue\"" %>  >
                        <span style="font-weight:bold">
                        <%= file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Approved ?
                                    GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Approved") :
                                            file.IsApproved == OnlineAbit2013.Models.ApprovalStatus.Rejected ? GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_Rejected") :
                                            GetGlobalResourceObject("AddSharedFiles", "ApprovalStatus_NotSet")
                        %>
                        </span>
                    </td>
                </tr>
        <% } %>
            </tbody>
        </table> 
        </div>
</asp:Content>
