<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.MotivateMailModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("PriorityChangerForeign", "MainTitle").ToString()%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("PriorityChangerForeign", "MainHeader").ToString()%></h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<p class="message info">
    <%= GetGlobalResourceObject("PriorityChangerForeign", "Info").ToString()%>
</p>

<style>
	.ui-sortable { list-style-type: none; margin-left: 10px; margin-right: 10px; padding-left: 10px; padding-right:10px; min-height: 15px; padding-top: 5px; width: 90%; cursor: move; } 
	.ui-sortable li { margin: 0 5px 5px 5px; padding: 5px; font-size: 1.2em; /*height: 1.5em; */}
    #sortable { list-style-type: none; margin-left: 10px; margin-right: 10px; padding-left: 10px; padding-right:10px; width: 90%; cursor: move; } 
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
 
    var childs = new Array(); 
    var obj;
    childs.push();
    <% foreach (var s in Model.Apps) { %> 
        <%= "obj = new Object();" %>
        <%= "obj['Id']='" + s.Id.ToString("N") + "';" %>
        <%= "obj['Val']='" + s.Priority + "';" %>
        childs.push(obj);
    <% }  %> 
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
    <%= Html.HiddenFor(x => x.CommitId)%>
    <% if (!String.IsNullOrEmpty(Model.OldCommitId)){ %><%= Html.HiddenFor(x => x.OldCommitId)%><%} %>
    <ul id ="sortable" >
    <% bool flag = true; int i = 0; List<int> MaxElement = new List<int>();
       int prev_prior = 0;
       foreach (var s in Model.Apps)
       {
           if ((s.Enabled) && (flag == true))
           {
               flag = false; i++; MaxElement.Add(1); %> 
               </ul><ul id="sortable<%=i.ToString() %>" >
           <%}
           else
           {
               if ((s.Enabled) && (flag == false))
               {
               }
                else
                {
                    if ((!s.Enabled) && (flag == false))
                    {
                        flag = true;
                        if (s.Priority - prev_prior > 1)
                        {    
                            MaxElement[i - 1] = s.Priority - prev_prior - 1;
                            
                        }
                        prev_prior = s.Priority;%> 
                        </ul><ul id='sortable'>
                    <%}
                    else
                    {
                        if (s.Priority - prev_prior > 1)
                        {
                            MaxElement.Add(1);
                            MaxElement[i] = s.Priority - prev_prior - 1;
                            prev_prior = s.Priority; i++;
                            %></ul><ul id="sortable<%=i.ToString() %>"></ul><ul id='sortable'><%
                        } 
                    } 
               }   
           }   
        %>
        <li class="message success" <% if (!s.Enabled){ %>style="opacity:0.5;" id = "cancelItems" title="заявление невозможно изменить"<%}%> >
            <table style="font-size:0.75em;" class="nopadding" cellspacing="0" cellpadding="0">
                <tr>
                    <td style="width:12em"><%= GetGlobalResourceObject("NewApplication", "ApplicationLevel").ToString()%></td>
                    <td><%=s.StudyLevelGroupName%></td>
                </tr>
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
                <% if (!String.IsNullOrEmpty(s.SemesterName)){%><tr>
                    <td style="width:12em"><%= GetGlobalResourceObject("NewApplication", "Semester").ToString()%></td>
                    <td><%=s.SemesterName%></td>
                </tr>
                <%} %>
                <% if (s.HasManualExams)
                   { %>
                    <tr>
                        <td style="width:12em"><%= GetGlobalResourceObject("PriorityChangerForeign", "ManualExam").ToString()%></td>
                        <td><%=s.ManualExam%></td>
                    </tr>
                <% } %>
                <% if (s.IsGosLine)
                   { %>
                    <tr>
                        <td style="width:12em"><%= GetGlobalResourceObject("NewApplication", "BlockData_GosLine").ToString()%></td>
                        <td><%= GetGlobalResourceObject("NewApplication", "Yes").ToString()%></td>
                    </tr>
                <% } %>
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
            <% if (s.Enabled){ %>
                <% if (s.HasSeparateObrazPrograms) { %>
                    <% if (s.ObrazProgramInEntryId.HasValue && s.ObrazProgramInEntryId.Value != Guid.Empty) { %>
                    <a href="../AbiturientNew/PriorityChangerProfile?AppId=<%= s.Id.ToString("N") %>&OPIE=<%= s.ObrazProgramInEntryId.Value.ToString("N") %>&V=<%= Model.VersionId %>">
                        <%= GetGlobalResourceObject("ApplicationInfo", "AppInnerProirityProfile").ToString()%></a>
                    <% } else
                       { %>
                    <a href="../AbiturientNew/PriorityChangerApplication?AppId=<%= s.Id.ToString("N") %>&V=<%= Model.VersionId %>">
                        <%= GetGlobalResourceObject("ApplicationInfo", "AppInnerProirity").ToString()%></a>
                    <% } %>
                <% } %>
            <% } %>
        </li>
    <% } %>
    </ul>
    <%if (flag)
      {
          i++; %>
          <ul id="sortable<%=i.ToString() %>" style="min-height: 10px;"></ul>
    <%} %>
    <button id="btnSave" type="submit" class="button button-green"><%= GetGlobalResourceObject("PriorityChangerForeign", "BtnSave").ToString()%></button><br />
</form>

<span id="saveStatus"></span>
<script type="text/javascript">
    $(function() {
    <% for (int s = 0; s<i; s++) {%>
		$("#sortable<%=(s+1).ToString()%>").sortable({
            connectWith : '.ui-sortable',
			placeholder: "message warning",
            cancel: "#cancelItems",
            dropOnEmpty: true,
            receive : function(event, ui) {
            // so if > 10
            <% if (s<i-1){ %>
            if ($(this).children().length > <%=MaxElement[s] %>) {
                $("#dialog-form").dialog(
                    {
                        autoOpen: false,
                        height: 400,
                        width: 350,
                        modal: true, 
                        buttons:
                        { 
                            "Okay": function () { $(this).dialog("close"); }
                        } 
                    });
                $("#dialog-form").dialog("open");
                $(ui.sender).draggable('revert');
                $(ui.sender).sortable('cancel'); 
            }
            <%} %>
        }
		}); 
		$("#sortable<%=s.ToString()%>").disableSelection();
        <%} %>
	});
    </script>
    <div id="dialog-form" style="display:none;">
    <p class="errMessage"></p>
    <p>Изменить приоритет таким образом невозможно: будет нарушены значения приоритетов заблокированных конкурсов. Перемещение отменено.</p>
</div>
</asp:Content>
