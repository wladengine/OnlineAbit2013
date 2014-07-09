<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.PriorityChangerApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Смена приоритетов внутри конкурса
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<style>
	#sortable { list-style-type: decimal; margin: 10px; padding: 10px; width: 90%; cursor: move; }
	#sortable li { margin: 0 5px 5px 5px; padding: 5px; font-size: 1.2em; /*height: 1.5em; */}
	html>body #sortable li { /*height: 1.5em; */line-height: 1.2em; }
	.ui-state-highlight { /*height: 1.5em; */line-height: 1.2em; }
</style>
<script>
    $(function () {
        $("#sortable").sortable({
            placeholder: "message warning"
        });
        $("#sortable").disableSelection();
    });
</script>
<script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
    <a href="../AbiturientNew/PriorityChanger?ComId=<%= Model.CommitId.ToString("N") %>"><%= Model.CommitName %></a>
    -> 
    <span><%= GetGlobalResourceObject("PriorityChangerForeign", "ObrazPr_Message1").ToString()%></span>
    <p class="message info">
        <%= GetGlobalResourceObject("PriorityChangerForeign", "ObrazPr_Message2").ToString()%>
    </p>
    <form action="../AbiturientNew/PriorityChangeApplication" method="post">
        <%= Html.HiddenFor(x => x.ApplicationId) %>
        <%= Html.HiddenFor(x => x.ApplicationVersionId) %>
        <%= Html.HiddenFor(x => x.CommitId) %>
        <%= Html.HiddenFor(x => x.CommitName) %>
        <ul id="sortable">
        <% for (int i = 0; i < Model.lstObrazPrograms.Count; i++) { %>
            <li class="message success">
                <table style="font-size:0.75em;" class="nopadding" cellspacing="0" cellpadding="0">
                    <tr>
                        <td style="width:12em"><%= GetGlobalResourceObject("PriorityChangerForeign", "ObrazProgram").ToString()%></td>
                        <td><%= Model.lstObrazPrograms[i].Value.Name %></td>
                    </tr>
                </table>
                <input type="hidden" name="<%= Model.lstObrazPrograms[i].Key.ToString("N") %>" />
                <% if (Model.lstObrazPrograms[i].Value.HasProfileInObrazProgramInEntry) { %>
                <a style="text-decoration:underline" href="../AbiturientNew/PriorityChangerProfile?AppId=<%= Model.ApplicationId.ToString("N") %>&OPIE=<%= Model.lstObrazPrograms[i].Key.ToString("N") %>&V=<%= Model.ApplicationVersionId.ToString("N") %>">Для данной программы Вы можете указать приоритетность профилей</a>
                <% } %>
            </li>
        <% } %>
        </ul>
        <button id="btnSave" type="submit" class="button button-green"><%= GetGlobalResourceObject("PriorityChangerForeign", "BtnSave").ToString()%></button><br />
    </form>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("PriorityChangerForeign", "ObrazPr_Message1").ToString()%></h2>
</asp:Content>
