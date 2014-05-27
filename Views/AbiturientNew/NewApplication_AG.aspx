<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="OnlineAbit2013.Models" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<AG_ApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("NewApplication", "PageTitle")%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
   <h2> <%= GetGlobalResourceObject("NewApplication", "PageSubheader")%></h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<style>
    button.error {
        background: -moz-linear-gradient(center top , #FAE2E2, #F2CACB) repeat scroll 0 0 rgba(0, 0, 0, 0);
        border: 1px solid #EEB7BA;
        color: #BE4741;
        text-shadow: 0 1px 0 #FFFFFF;
    }
</style>
<script type="text/javascript">
    var entry;
    $(function () {
        $('#Block<%= Model.Applications.Count + 1 %>').show();
    });
    var BlockIds = new Object();
    <% for (int i = 1; i <= Model.Applications.Count; i++ ) { %>
        BlockIds['<%= i.ToString() %>'] = '<%= Model.Applications[i - 1].Id.ToString("N") %>';
    <% } %>

    var currObrazProgramErrors = '#ObrazProgramsErrors';
    var currFinishButton = '#FinishBtn';
    var currSpecs = '#Specs';
    var currManualExam = '#ManualExam';
    var currProfessions = '#Professions';
    var currExams = '#Exams';
    var currManualExam = '#ManualExam';
    var currProfile = '#Profile';
    var currBlock = '#Block';
    var currBlockData = '#BlockData';
    var currNeedHostel = '#NeedHostel'
    var currBlockData_Profession = '#BlockData_Profession';
    var currBlockData_Specialization = '#Specialization';
    var currBlockData_ManualExam = '#ManualExam';

    var nextObrazProgramErrors = '#ObrazProgramsErrors';
    var nextFinishButton = '#FinishBtn';
    var nextSpecs = '#Specs';
    var nextManualExam = '#ManualExam';
    var nextProfessions = '#Professions';
    var nextExams = '#Exams';
    var nextManualExam = '#ManualExam';
    var nextProfile = '#Profile';
    var nextBlock = '#Block';
    var nextBlockData = '#BlockData';
    var nextNeedHostel = '#NeedHostel'
    var nextBlockData_Profession = '#BlockData_Profession';
    var nextBlockData_Specialization = '#Specialization';
    var nextBlockData_ManualExam = '#ManualExam';

    

    function GetSpecializations(i) {
        $('#ObrazProgramsErrors').text('').hide();
        $('#FinishBtn' + i).hide();
        currFinishButton = '#FinishBtn' + i;
        currSpecs = '#Specs' + i;
        currManualExam = '#ManualExam' + i;
        currFinishButton = '#FinishBtn' + i;
        currExams = '#Exams' + i;
        currManualExam = '#ManualExam' + i;
        currObrazProgramErrors = '#ObrazProgramsErrors' + i;
        currProfile = '#Profile' + i;
        currProfessions = '#Professions' + i;

        $(currFinishButton).hide();
        $(currSpecs).hide();
        $(currManualExam).hide();
        $(currProfile).html('');
        $(currExams).html('');
        $.post('/AG/GetSpecializations', { classid: $('#EntryClassId').val(), programid: $(currProfessions).val(), CommitId : $('#CommitId').val() }, function (json_data, i) {
            if (json_data.IsOk) {
                $(currObrazProgramErrors).text('').hide();
                if (json_data.HasProfileExams) {
                    $(currManualExam).show();
                    var exams = json_data.Exams;
                    var info = '';
                    for (var i = 0; i < exams.length; i++) {
                        info += '<option value="' + exams[i].Value + '">' + exams[i].Name + '</option>';
                    }
                    $(currExams).html(info).show();
                }
                else {
                    if (json_data.Data == undefined || json_data.Data.length == 0 || (json_data.Data.length == 1 && json_data.Data[0].Id == 0)) {
                        $(currFinishButton).show();
                    }
                    else {
                        $(currSpecs).show();
                        $(currFinishButton).hide();
                        var opts = '';
                        for (var i = 0; i < json_data.Data.length; i++) {
                            opts += '<option value="' + json_data.Data[i].Id + '">' + json_data.Data[i].Name + '</option>';
                        }
                        $(currProfile).html(opts);
                    }
                }
            }
            else {
                $(currObrazProgramErrors).text(json_data.ErrorMessage).show();
            }
        }, 'json');
    }

    function CheckSpecialization(i) {
        $('#ObrazProgramsErrors').text('').hide();
        $('#FinishBtn' + i).hide();
        currFinishButton = '#FinishBtn' + i;
        currSpecs = '#Specs' + i;
        currManualExam = '#ManualExam' + i;
        currFinishButton = '#FinishBtn' + i;
        currExams = '#Exams' + i;
        currManualExam = '#ManualExam' + i;
        currObrazProgramErrors = '#ObrazProgramsErrors' + i;
        currProfile = '#Profile' + i;
        currProfessions = '#Professions' + i;

        $(currFinishButton).hide();
        $(currManualExam).hide();
        $(currExams).html('');

        $.post('/AG/CheckSpecializations', { classid: $('#EntryClassId').val(), programid: $(currProfessions).val(), specid: $(currProfile).val(), CommitId : $('#CommitId').val() }, function (json_data) {
            if (json_data.IsOk) {
                $(currObrazProgramErrors).text('').hide();
                if (json_data.HasProfileExams) {
                    $(currManualExam).show();
                    var exams = json_data.Exams;
                    var info = '';
                    for (var i = 0; i < exams.length; i++) {
                        info += '<option value="' + exams[i].Value + '">' + exams[i].Name + '</option>';
                    }
                    $(currExams).html(info).show();
                }
                else {
                    $(currFinishButton).show();
                }
            }
            else {
                $(currFinishButton).hide();
                $(currObrazProgramErrors).text(json_data.ErrorMessage).show();
            }
        }, 'json');
    }

    function mkButton(i) {
        $('#ObrazProgramsErrors').text('').hide();
        $('#FinishBtn' + i).hide();
        currFinishButton = '#FinishBtn' + i;
        currSpecs = '#Specs' + i;
        currManualExam = '#ManualExam' + i;
        currFinishButton = '#FinishBtn' + i;
        currExams = '#Exams' + i;
        currManualExam = '#ManualExam' + i;
        currObrazProgramErrors = '#ObrazProgramsErrors' + i;
        currProfile = '#Profile' + i;
        currProfessions = '#Professions' + i;
        currBlock = '#Block' + i;
        currNeedHostel = '#NeedHostel' + i;
        currBlockData = '#BlockData' + i;
        var nxt = i + 1;
        nextBlock = '#Block' + nxt;

        currBlockData_Profession = '#BlockData_Profession' + i;
        currBlockData_Specialization = '#BlockData_Specialization' + i;
        currBlockData_ManualExam = '#BlockData_ManualExam' + i;

        $.post('/AbiturientNew/CheckApplication_AG', { Entryclass: $('#EntryClassId').val(), profession: $(currProfessions).val(), profileid: $(currProfile).val(), manualExam : $(currManualExam).val(), NeedHostel: $(currNeedHostel).is(':checked'), CommitId : $('#CommitId').val() }, function(json_data) {
            if (json_data.IsOk) {
                $('#FinishBtn' + i).show();
            }
            else {
                $(currObrazProgramErrors).text(json_data.ErrorMessage).show();
            }
        }, 'json');
    }

    var nxt = 1;
    function SaveData(i) {
        $('#ObrazProgramsErrors').text('').hide();
        currFinishButton = '#FinishBtn' + i;
        currSpecs = '#Specs' + i;
        currManualExam = '#ManualExam' + i;
        currFinishButton = '#FinishBtn' + i;
        currExams = '#Exams' + i;
        currManualExam = '#ManualExam' + i;
        currObrazProgramErrors = '#ObrazProgramsErrors' + i;
        currProfile = '#Profile' + i;
        currProfessions = '#Professions' + i;
        currBlock = '#Block' + i;
        currNeedHostel = '#NeedHostel' + i;
        currBlockData = '#BlockData' + i;
        nxt = i + 1;
        nextBlock = '#Block' + nxt;

        currBlockData_Profession = '#BlockData_Profession' + i;
        currBlockData_Specialization = '#BlockData_Specialization' + i;
        currBlockData_ManualExam = '#BlockData_ManualExam' + i;

        $.post('/AbiturientNew/AddApplication_AG', { Entryclass: $('#EntryClassId').val(), profession: $(currProfessions).val(), profileid: $(currProfile).val(), manualExam : $(currExams).val(), NeedHostel: $(currNeedHostel).is(':checked'), CommitId : $('#CommitId').val() }, function(json_data) {
            if (json_data.IsOk) {
                $(currBlockData_Profession).text(json_data.Profession);
                $(currBlockData_Specialization).text(json_data.Specialization);
                $(currBlockData_ManualExam).text(json_data.ManualExam);
                $(currBlock).hide();
                $(currBlockData).show();
                if (BlockIds[nxt] == undefined) {
                    $(nextBlock).show();
                }
                BlockIds[i] = json_data.Id;
            }
            else {
                $(currObrazProgramErrors).text(json_data.ErrorMessage).show();
            }
        }, 'json');
    }

    var currObrazProgramsErrors_Block = '#ObrazProgramsErrors_Block';
    function DeleteApp(i) {
        var appId = BlockIds[i];
        nextBlock = '#Block' + i;
        nextFinishButton = '#FinishBtn' + i;
        nextSpecs = '#Specs' + i;
        nextManualExam = '#ManualExam' + i;
        nextFinishButton = '#FinishBtn' + i;
        nextExams = '#Exams' + i;
        nextManualExam = '#ManualExam' + i;
        nextObrazProgramErrors = '#ObrazProgramsErrors' + i;
        nextProfile = '#Profile' + i;
        nextProfessions = '#Professions' + i;
        nextNeedHostel = '#NeedHostel' + i;
        nextBlockData = '#BlockData' + i;
        nextBlockData_Profession = '#BlockData_Profession' + i;
        nextBlockData_Specialization = '#BlockData_Specialization' + i;
        nextBlockData_ManualExam = '#BlockData_ManualExam' + i;

        currObrazProgramsErrors_Block = '#ObrazProgramsErrors_Block' + i;
        $(currObrazProgramsErrors_Block).text('').hide();

        $.post('/AbiturientNew/DeleteApplication_AG', { id : appId, CommitId : $('#CommitId').val() }, function(json_data) {
            if (json_data.IsOk) {
                $(nextBlockData_ManualExam).text('');
                $(nextBlockData_Profession).text('');
                $(nextBlockData_Specialization).text('');
                $(nextBlockData).hide();
                $(nextBlock).show();
            }
            else {
                $(currObrazProgramsErrors_Block).text(json_data.ErrorMessage).show();
            }
        }, 'json');
    }
</script>
<% using (Html.BeginForm("NewAppAG", "AbiturientNew", FormMethod.Post))
   { 
%> 
    <%= Html.ValidationSummary()%>
    <%= Html.HiddenFor(x => x.CommitId)%>
    <% if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
       { %>
       <div class="message error" style="width:450px;">
           <strong style="font-size:10pt"><%= GetGlobalResourceObject("NewApplication", "AG_PriemClosed").ToString()%></strong>
       </div>
    <% } %>
    <div class="message info" style="width:450px;">
        <%= GetGlobalResourceObject("NewApplication", "HeaderFirstCourse_1").ToString()%> <strong><%= Model.EntryClassName%></strong>
    </div>
    <br />
    <%= Html.HiddenFor(x => x.EntryClassId)%>

    <% for (int i = 1; i <= Model.Applications.Count; i++)
       { %>
    <div id="BlockData<%= i.ToString()%>" class="message info panel" style="width:450px;">
        <table class="nopadding" cellspacing="0" cellpadding="0">
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("PriorityChangerForeign", "Priority").ToString()%></td>
                <td style="font-size:1.3em;"><%= i.ToString() %></td>
            </tr>
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("PriorityChangerForeign", "LicenseProgram").ToString()%></td>
                <td id="BlockData_Profession<%= i.ToString()%>" style="font-size:1.3em;"><%= Model.Applications[i - 1].ProgramName%></td>
            </tr>
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("PriorityChangerForeign", "Profile").ToString()%></td>
                <td id="BlockData_Specialization<%= i.ToString()%>" style="font-size:1.3em;"><%= Model.Applications[i - 1].ProfileName%></td>
            </tr>
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("PriorityChangerForeign", "ManualExam").ToString()%></td>
                <td id="BlockData_ManualExam<%= i.ToString()%>" ><%= Model.Applications[i - 1].ManualExamName%></td>
            </tr>
        </table>
        <button type="button" onclick="DeleteApp(<%= i.ToString()%>)" class="error"><%= GetGlobalResourceObject("PriorityChangerForeign", "Delete").ToString()%></button>
        <div id="ObrazProgramsErrors_Block<%= i.ToString()%>" class="message error" style="display:none; width:450px;">
        </div>
    </div>
    <div id="Block<%= i.ToString()%>" style="display:none; width:500px;" class="panel">
        <h5><%= GetGlobalResourceObject("NewApplication", "AGHeaderProgram").ToString()%></h5>
        <p id="Profs<%= i.ToString()%>">
            <span><%= GetGlobalResourceObject("NewApplication", "AGProgram").ToString()%></span><br />
            <%= Html.DropDownList("Professions" + i.ToString(), Model.Professions, new SortedList<string, object>() { { "size", "5" }, 
{ "style", "min-width:450px;" }, { "onchange", "GetSpecializations(" + i.ToString() + ")"} })%>
        </p>
        <p id="Specs<%= i.ToString()%>" style="display:none;">
            <span><%= GetGlobalResourceObject("NewApplication", "AGSpecialization").ToString()%></span><br />
            <select id="Profile<%= i.ToString()%>" name="Profile" size="3" style="min-width:450px;" onchange="CheckSpecialization(<%= i.ToString()%>)"></select>
        </p>
        <p id="ManualExam<%= i.ToString()%>" style="display:none;">
            <span><%= GetGlobalResourceObject("NewApplication", "AGExams").ToString()%></span><br />
            <select id="Exams<%= i.ToString()%>" name="Exam" size="3" style="min-width:450px;" onchange="mkButton(<%= i.ToString()%>)"></select>
        </p>
        <p id="FinishBtn<%= i.ToString()%>" style="display:none;">
            <input type="checkbox" name="NeedHostel" id="Checkbox1" /><span style="font-size:13px"><%= GetGlobalResourceObject("NewApplication", "chbNeedHostel").ToString()%></span><br /><br />
            <input id="Submit<%= i.ToString()%>" type="button" value="Добавить" onclick="SaveData(<%= i.ToString()%>)" class="button button-blue"/>
        </p>
        <div id="ObrazProgramsErrors<%= i.ToString()%>" class="message error" style="display:none; width:450px;">
        </div>
    </div>
    <% } %>

    <% for (int i = Model.Applications.Count + 1; i <= Model.MaxBlocks; i++)
       { %>
    <div id="BlockData<%= i.ToString()%>" class="message info panel" style="width:450px; display:none;">
        <table class="nopadding" cellspacing="0" cellpadding="0">
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("PriorityChangerForeign", "Priority").ToString()%></td>
                <td style="font-size:1.3em;"><%= i.ToString() %></td>
            </tr>
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("PriorityChangerForeign", "LicenseProgram").ToString()%></td>
                <td id="BlockData_Profession<%= i.ToString()%>" style="font-size:1.3em;"></td>
            </tr>
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("PriorityChangerForeign", "Profile").ToString()%></td>
                <td id="BlockData_Specialization<%= i.ToString()%>" style="font-size:1.3em;"></td>
            </tr>
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("PriorityChangerForeign", "ManualExam").ToString()%></td>
                <td id="BlockData_ManualExam<%= i.ToString()%>" ></td>
            </tr>
        </table>
        <button type="button" onclick="DeleteApp(<%= i.ToString()%>)" class="error"><%= GetGlobalResourceObject("NewApplication", "Delete").ToString()%></button>
        <div id="ObrazProgramsErrors_Block<%= i.ToString()%>" class="message error" style="display:none; width:450px;">
    </div>
    </div>
    <div id="Block<%= i.ToString()%>" style="display:none; width:500px;" class="panel">
        <h5><%= GetGlobalResourceObject("NewApplication", "AGHeaderProgram").ToString()%></h5>
        <p id="Profs<%= i.ToString()%>">
            <span><%= GetGlobalResourceObject("NewApplication", "AGProgram").ToString()%></span><br />
            <%= Html.DropDownList("Professions" + i.ToString(), Model.Professions, new SortedList<string, object>() { { "size", "5" }, 
{ "style", "min-width:450px;" }, { "onchange", "GetSpecializations(" + i.ToString() + ")"} })%>
        </p>
        <p id="Specs<%= i.ToString()%>" style="display:none;">
            <span><%= GetGlobalResourceObject("NewApplication", "AGSpecialization").ToString()%></span><br />
            <select id="Profile<%= i.ToString()%>" name="Profile" size="3" style="min-width:450px;" onchange="CheckSpecialization(<%= i.ToString()%>)"></select>
        </p>
        <p id="ManualExam<%= i.ToString()%>" style="display:none;">
            <span><%= GetGlobalResourceObject("NewApplication", "AGExams").ToString()%></span><br />
            <select id="Exams<%= i.ToString()%>" name="Exam" size="3" style="min-width:450px;" onchange="mkButton(<%= i.ToString()%>)"></select>
        </p>
        <p id="FinishBtn<%= i.ToString()%>" style="display:none;">
            <input type="checkbox" name="NeedHostel" id="NeedHostel<%= i.ToString()%>" /><span style="font-size:13px"><%= GetGlobalResourceObject("NewApplication", "chbNeedHostel").ToString()%></span><br /><br />
            <input id="Submit<%= i.ToString()%>" type="button" value="Добавить" onclick="SaveData(<%= i.ToString()%>)" class="button button-blue"/>
        </p>
        <div id="ObrazProgramsErrors<%= i.ToString()%>" class="message error" style="display:none; width:450px;">
        </div>
    </div>
    <% } %>
    <br />
    <input id="Submit" type="submit" value="Подтвердить" class="button button-green"/>
<% 
   }
%>

</asp:Content>
