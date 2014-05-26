<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.MotivateMailModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("PriorityChangerForeign", "MainTitle").ToString()%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("PriorityChangerForeign", "MainHeader").ToString()%></h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<p>
    <%= GetGlobalResourceObject("PriorityChangerForeign", "Info").ToString()%>
</p>

<style>
	#sortable { list-style-type: decimal; margin: 10px; padding: 10px; width: 90%; cursor: move; }
	#sortable li { margin: 0 5px 5px 5px; padding: 5px; font-size: 1.2em; /*height: 1.5em; */}
	html>body #sortable li { /*height: 1.5em; */line-height: 1.2em; }
	.ui-state-highlight { /*height: 1.5em; */line-height: 1.2em; }
</style>
<% if (1 == 0)
   { %>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
<% } %>
<script type="text/javascript">
    $(function() {
        $('#UILink').hide();
		$("#sortable").sortable({
			placeholder: "message warning"
		});
		$("#sortable").disableSelection();
	});
    var childs = new Array(); 
    var obj;
    childs.push();
    <% foreach (var s in Model.Apps) { %> 
        <%= "obj = new Object();" %>
        <%= "obj['Id']='" + s.Id.ToString("N") + "';" %>
        <%= "obj['Val']='" + s.Priority + "';" %>
        childs.push(obj);
    <% } %> 
    var changesAllowed = true;
    function JumpPriority(idSrc)
    {
        if (!changesAllowed) {
            return;
        }
        changesAllowed = false;
        var prior = $('#' + idSrc).val();
        for (var i = 0; i < childs.length; i++)
        {
            var idDest = childs[i].Id;
            if ((idDest != idSrc) && prior == childs[i].Val)
            {
                for (var j = 0; j < childs.length; j++)
                {
                    if (childs[j].Id == idSrc)
                    {
                        $('#' + idDest).val(childs[j].Val);
                        childs[i].Val = childs[j].Val;
                        childs[j].Val = prior;
                        $('#' + idDest).addClass('border-blue');
                        setTimeout(function() { $('#' + idDest).removeClass('border-blue'); }, 5000);
                        break;
                    }
                }
            }
        }
        changesAllowed = true;
    }
    function RemoveBlue(id) {
        $('#' + id).removeClass('border-blue');
    }
</script>
<script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
<form action="/AbiturientNew/ChangePriority" method="post">
    <%= Html.HiddenFor(x => x.CommitId) %>
    <ul id="sortable">
    <% foreach (var s in Model.Apps)
       { %>
        <li class="message success">
            <table style="font-size:0.75em;" class="nopadding" cellspacing="0" cellpadding="0">
                <tr>
                    <td style="width:12em"><%= GetGlobalResourceObject("PriorityChangerForeign", "LicenseProgram").ToString()%></td>
                    <td><%=s.Profession%></td>
                </tr>
                <tr>
                    <td style="width:12em"><%= GetGlobalResourceObject("PriorityChangerForeign", "ObrazProgram").ToString()%></td>
                    <td><%=s.ObrazProgram%></td>
                </tr>
                <tr>
                    <td style="width:12em"><%= GetGlobalResourceObject("PriorityChangerForeign", "Profile").ToString()%></td>
                    <td><%=s.Specialization%></td>
                </tr>
                <tr>
                    <td style="width:12em"><%= GetGlobalResourceObject("PriorityChangerForeign", "StudyForm").ToString()%></td>
                    <td><%=s.StudyForm%></td>
                </tr>
                <tr>
                    <td style="width:12em"><%= GetGlobalResourceObject("PriorityChangerForeign", "StudyBasis").ToString()%></td>
                    <td><%=s.StudyBasis%></td>
                </tr>
            </table>
            <input type="hidden" name="<%= s.Id.ToString("N") %>" />
            <%--<a href="PersonStartPage.aspx">some path...</a>--%>
        </li>
    <% } %>
    </ul>
    <button id="btnSave" type="submit" class="button button-green">Сохранить</button><br />
</form>

<span id="saveStatus"></span>

</asp:Content>
