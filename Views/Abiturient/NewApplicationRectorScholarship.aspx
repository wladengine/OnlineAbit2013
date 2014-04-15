<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Abiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.NewApplicationRectorScholarshipModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Заявление на подачу ректорской стипендии
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Ректорская стипендия</h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        Заявление на подачу ректорской стипендии
    </p>
    <% if (!string.IsNullOrEmpty(Model.Message))
       {
    %>
        <div class="Red"><%= Model.Message %></div><br />
    <% 
       }
    %>
    
    <form action="../../Abiturient/NewAppRectorScholarship">
        <input id="Submit" type="submit" value="Подать заявление" class="button button-green"/>
    </form>
    <br />
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
           <img src="../../Content/myimg/icon_shared3.png" />
        <% } %>
        </td>
    </tr>
<% } %>
</tbody>
</table><br />
    <div class="panel">
<h4>Добавить файл</h4>
<hr />
<form action="/Abiturient/NewApplicationRectorScholarshipAddFile" method="post" enctype="multipart/form-data" class="form">
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
<br />
</div>

</asp:Content>


