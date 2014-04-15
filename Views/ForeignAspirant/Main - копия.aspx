<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Abiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.ForeignMain>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("Main", "PersonalOfficeHeader").ToString()%>
</asp:Content>

<asp:Content ID="id1" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("Main", "PersonalOfficeHeader").ToString()%></h2>
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
<h2>
    <%= Html.Encode(Model.Surname + " " + Model.Name) %>
</h2>

<% foreach (var msg in Model.Messages)
   { %>
    <div id="<%= msg.Id %>" class="message info" style="padding:5px">
        <span class="ui-icon ui-icon-alert"></span><%= msg.Text %>
        <div style="float:right;"><span class="link" onclick="DeleteMsg('<%= msg.Id %>')"><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить" /></span></div>
    </div>
<% } %>
<p>
    <asp:Label ID="Label1" Text="<%$Resources:Main, TitleInfo%>" runat="server"></asp:Label>
</p>
<h3><%= GetGlobalResourceObject("Main", "HeaderActiveApps").ToString()%></h3>
<hr />
<% if (Model.lApps.FindAll(x => x.Enabled == true).Count > 0)
   { %>
    <table class="paginate">
        <thead>
            <tr>
                <th><%= GetGlobalResourceObject("PriorityChangerForeign", "LicenseProgram").ToString()%></th>
                <th><%= GetGlobalResourceObject("PriorityChangerForeign", "ObrazProgram").ToString()%></th>
                <th><%= GetGlobalResourceObject("PriorityChangerForeign", "Profile").ToString()%></th>
                <th><%= GetGlobalResourceObject("PriorityChangerForeign", "StudyForm").ToString()%></th>
                <th><%= GetGlobalResourceObject("PriorityChangerForeign", "StudyBasis").ToString()%></th>
                <th><%= GetGlobalResourceObject("Main", "ShowApplication").ToString()%></th>
            </tr>
        </thead>
    <% foreach (var app in Model.lApps.FindAll(x => x.Enabled == true))
       { %>
        <tr>
            <td style="vertical-align:middle; text-align:center;"><%= app.Profession %></td>
            <td style="vertical-align:middle; text-align:center;"><%= app.ObrazProgram %></td>
            <td style="vertical-align:middle; text-align:center;"><%= app.Specialization %></td>
            <td style="vertical-align:middle; text-align:center;"><%= app.StudyForm %></td>
            <td style="vertical-align:middle; text-align:center;"><%= app.StudyBasis %></td>
            <td style="vertical-align:middle; text-align:center;"><a href="../../ForeignApplication/Index/<%= app.Id.ToString("N") %>"><%= GetGlobalResourceObject("Main", "LinkApplicationInfo").ToString()%></a></td>
        </tr>
    <% } %>
    </table>
<% }
   else
   { %>
    <h4><asp:Label ID="Label2" Text="<%$Resources:Main, HeaderNoActiveApps%>" runat="server"></asp:Label></h4>
<% } %>
<br />
<h3><%= GetGlobalResourceObject("Main", "HeaderCancelledApps").ToString()%></h3>
<hr />
<% if (Model.lApps.FindAll(x => x.Enabled == false).Count > 0)
   { %>
    <table class="paginate">
        <thead>
            <tr>
                <th><%= GetGlobalResourceObject("PriorityChangerForeign", "LicenseProgram").ToString()%></th>
                <th><%= GetGlobalResourceObject("PriorityChangerForeign", "ObrazProgram").ToString()%></th>
                <th><%= GetGlobalResourceObject("PriorityChangerForeign", "Profile").ToString()%></th>
                <th><%= GetGlobalResourceObject("PriorityChangerForeign", "StudyForm").ToString()%></th>
                <th><%= GetGlobalResourceObject("PriorityChangerForeign", "StudyBasis").ToString()%></th>
                <th><%= GetGlobalResourceObject("Main", "ShowApplication").ToString()%></th>
            </tr>
        </thead>
    <% foreach (var app in Model.lApps.FindAll(x => x.Enabled == false))
       { %>
        <tr>
            <td style="vertical-align:middle; text-align:center;"><%= app.Profession %></td>
            <td style="vertical-align:middle; text-align:center;"><%= app.ObrazProgram %></td>
            <td style="vertical-align:middle; text-align:center;"><%= app.Specialization %></td>
            <td style="vertical-align:middle; text-align:center;"><%= app.StudyForm %></td>
            <td style="vertical-align:middle; text-align:center;"><%= app.StudyBasis %></td>
            <td style="vertical-align:middle; text-align:center;"><a href="../../ForeignApplication/Index/<%= app.Id.ToString("N") %>"><%= GetGlobalResourceObject("Main", "LinkApplicationInfo").ToString()%></a></td>
        </tr>
    <% } %>
    </table>
<% }
   else
   { %>
    <h4><%= GetGlobalResourceObject("Main", "HeaderNoCancelledApps").ToString()%></h4>
<% } %>

</asp:Content>
