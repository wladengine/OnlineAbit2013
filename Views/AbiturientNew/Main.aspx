<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.SimplePerson>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("Main", "PageHeader")%> 
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("Main", "PageHeader")%></h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript"> 
    function DeleteMsg(id) {
        var p = new Object();
        p["id"] = id;
        $.post('/Abiturient/DeleteMsg', p, function (res) {
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
    <h2><%= Html.Encode(Model.Surname + " " + Model.Name + " " + Model.SecondName) %></h2>
<% foreach (var msg in Model.Messages)
   { %>
    <div id="<%= msg.Id %>" class="message info" style="padding:5px">
        <span class="ui-icon ui-icon-alert"></span><%= msg.Text %>
        <div style="float:right;"><span class="link" onclick="DeleteMsg('<%= msg.Id %>')"><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить" /></span></div>
    </div>
<% } %>
    <p>
        <%= GetGlobalResourceObject("Main", "TitleInfo")%> 
    </p>
    <h4> <%= GetGlobalResourceObject("Main", "HeaderActiveApps")%> </h4>
    <hr />
    <table class="paginate full">
        <thead>
            <tr>
                <th><%= GetGlobalResourceObject("Main", "AppLevel")%></th>
                <%--<th>Тип поступления</th>--%>
                <th><%= GetGlobalResourceObject("Main", "AppView")%></th>
            </tr>
        </thead>
    <% foreach (OnlineAbit2013.Models.SimpleApplicationPackage app in Model.Applications.ToList())
        { %>
         <tr>
            <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.StudyLevel) %></td>
            <%--<td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.PriemType) %></td>--%>
            <td style="vertical-align:middle; text-align:center;"><a href="<%= string.Format("../../Application/Index/{0}", app.Id.ToString("N")) %>"><%= GetGlobalResourceObject("Main", "View")%></a></td>
         </tr>
     <% } %>
     </table>
    <hr />
</asp:Content>
