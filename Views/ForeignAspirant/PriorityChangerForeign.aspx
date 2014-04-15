<%@ Page Title="" Language="C#" MasterPageFile="~/Views/ForeignAbiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.MotivateMailModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("PriorityChangerForeign", "MainTitle").ToString()%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <%= GetGlobalResourceObject("PriorityChangerForeign", "MainHeader").ToString()%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

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
		$("#sortable").sortable({
			placeholder: "ui-state-highlight"
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
<h2><%= GetGlobalResourceObject("PriorityChangerForeign", "MainHeader").ToString()%></h2>
<p>
    <%= GetGlobalResourceObject("PriorityChangerForeign", "Info").ToString()%>
</p>
<form action="/Abiturient/ChangePriority" method="post">
<ul id="sortable">
<% foreach (var s in Model.Apps)
   { %>
    <li class="ui-state-default">
        <table style="font-size:0.75em;" class="nopadding" cellspacing="0" cellpadding="0">
            <tr>
                <td style="width:12em">Profession</td>
                <td><%=s.Profession%></td>
            </tr>
            <tr>
                <td style="width:12em">Educational program</td>
                <td><%=s.ObrazProgram%></td>
            </tr>
            <tr>
                <td style="width:12em">Profile</td>
                <td><%=s.Specialization%></td>
            </tr>
            <tr>
                <td style="width:12em">Study form</td>
                <td><%=s.StudyForm%></td>
            </tr>
        </table>
        <input type="hidden" name="<%= s.Id.ToString("N") %>" />
    </li>
<% } %>
</ul>
<button id="btnSave" type="submit" class="ui-widget ui-button ui-state-default ui-corner-all">Save</button>
</form>

<span id="saveStatus"></span>


</asp:Content>
