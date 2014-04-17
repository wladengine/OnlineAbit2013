<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="OnlineAbit2013.Models" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<AG_ApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Создание нового заявления
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
   <h2>Новое заявление</h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script language="javascript" type="text/javascript">
    var entry;
    $(function () {
        $('#Block1').show();
    });

    var currObrazProgramErrors = '#ObrazProgramsErrors';
    var currFinishButton = '#FinishBtn';
    var currSpecs = '#Specs';
    var currManualExam = '#ManualExam';
    var currProfessions = '#Professions';
    var currExams = '#Exams';
    var currManualExam = '#ManualExam';
    var currProfile = '#Profile';

    function GetSpecializations(i) {
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
        $.post('/AG/GetSpecializations', { classid: $('#EntryClassId').val(), programid: $(currProfessions).val() }, function (json_data, i) {
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
        $.post('/AG/CheckSpecializations', { classid: $('#EntryClassId').val(), programid: $(currProfessions).val(), specid: $(currProfile).val() }, function (json_data) {
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
        $('#FinishBtn' + i).show();
    }
</script>
<% using (Html.BeginForm("NewApp", "AG", FormMethod.Post))
   { 
%> 
    <%= Html.ValidationSummary() %>
    <% if (DateTime.Now >= new DateTime(2013, 6, 23, 0, 0, 0))
       { %>
       <div class="message error" style="width:450px;">
        <strong style="font-size:10pt">Внимание! Приём документов в АГ СПбГУ закрыт.</strong>
       </div>
    <% } %>
    <div class="message info" style="width:450px;">
        Согласно данным анкеты, Вы поступаете в <strong><%= Model.EntryClassName %></strong>
    </div>
    <br />
    <%= Html.HiddenFor(x => x.EntryClassId) %>
    <% for (int i = 1; i <= Model.MaxBlocks; i++) { %>
    <div id="BlockData<%= i.ToString() %>" class="message info" style="width:450px; ">
        <table style="font-size:0.75em;" class="nopadding" cellspacing="0" cellpadding="0">
            <tr>
                <td style="width:12em"><%= GetGlobalResourceObject("PriorityChangerForeign", "LicenseProgram").ToString()%></td>
                <td id="BlockData_Profession<%= i.ToString() %>"></td>
            </tr>
            <tr>
                <td style="width:12em"><%= GetGlobalResourceObject("PriorityChangerForeign", "Profile").ToString()%></td>
                <td id="BlockData_Specialization<%= i.ToString() %>"></td>
            </tr>
            <tr>
                <td style="width:12em"><%= GetGlobalResourceObject("PriorityChangerForeign", "ManualExam").ToString()%></td>
                <td id="BlockData_ManualExam<%= i.ToString() %>"></td>
            </tr>
        </table>
    </div>
    <div id="Block<%= i.ToString() %>" style="display:none;">
        <h5>Выберите направление подготовки</h5>
        <p id="Profs<%= i.ToString() %>">
            <span>Направление</span><br />
            <%= Html.DropDownList("Professions" + i.ToString(), Model.Professions, new Dictionary<string, object>() { { "size", "5" }, 
{ "style", "min-width:450px;" }, { "onchange", "GetSpecializations(" + i.ToString() + ")"} }) %>
        </p>
        <p id="Specs<%= i.ToString() %>" style="display:none;">
            <span>Специализация</span><br />
            <select id="Profile<%= i.ToString() %>" name="Profile" size="3" style="min-width:450px;" onchange="CheckSpecialization(<%= i.ToString() %>)"></select>
        </p>
        <p id="ManualExam<%= i.ToString() %>" style="display:none;">
            <span>Экзамен по выбору</span><br />
            <select id="Exams<%= i.ToString() %>" name="Exam" size="3" style="min-width:450px;" onchange="mkButton(<%= i.ToString() %>)"></select>
        </p>
        <p id="FinishBtn<%= i.ToString() %>" style="display:none;">
            <input type="checkbox" name="NeedHostel" id="NeedHostel<%= i.ToString() %>" /><span style="font-size:13px">Нуждаюсь в общежитии на время обучения</span><br /><br />
            <input id="Submit<%= i.ToString() %>" type="button" value="Добавить" onclick="SaveData(<%= i.ToString() %>)" class="button button-blue"/>
        </p>
        <div id="ObrazProgramsErrors<%= i.ToString() %>" class="message error" style="display:none; width:450px;">
        </div>
        
    </div>
    <% } %>
    <br />
    <input id="Submit" type="submit" value="Подать заявление" class="button button-green"/>
<% 
   }
%>

</asp:Content>
