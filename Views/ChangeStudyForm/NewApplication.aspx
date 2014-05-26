<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="OnlineAbit2013.Models" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Abiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<ApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Создание нового заявления
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
   <h2>Новое заявление</h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    var entry;
    $(function () {
        entry = $('#Entry').val();
        GetProfessions();
        $('#FinishBtn').hide();
    });

    function GetProfessions() {
        $('#Profs').show();
        $('#ObrazPrograms').hide();
        $('#Specs').hide();
        $('#FinishBtn').hide();
        $.post('/ChangeStudyForm/GetProfs', { studyform: $('#StudyFormId').val(), studybasis: $('#StudyBasisId').val(), entry: $('#EntryType').val(), 
            semesterId : $('#SemesterId').val() }, function (json_data) {
            var options = '';
            for (var i = 0; i < json_data.length; i++) {
                options += '<option value="' + json_data[i].Id + '">' + json_data[i].Name + '</option>';
            }
            $('#lProfession').html(options);
            $('#lObrazProgram').html('');
            $('#lSpecialization').html('');
        }, 'json');
    }

    function GetObrazPrograms() {
        var profId = $('#lProfession').val();
        var sfId = $('#StudyFormId').val();

        if (profId == null) {
            return;
        }

        $('#Profs').show();
        $('#ObrazPrograms').show();
        $('#Specs').hide();
        $('#FinishBtn').hide();

        $.post('/Recover/GetObrazPrograms', { prof: profId, studyform: sfId, studybasis: $('#StudyBasisId').val(),
            entry: $('#EntryType').val(), isParallel: $('#IsParallelHidden').val(), isReduced: $('#IsReducedHidden').val(),
            semesterId: $('#SemesterId').val()
        }, function (json_data) {
            var options = '';
            if (json_data.NoFree) {
                $('#ObrazProgramsErrors').text('Заявление уже подавалось');
                $('#lObrazProgram').attr('disabled', 'disabled').hide();
                $('#lSpecialization').html('');
            }
            else {
                $('#ObrazProgramsErrors').text('').hide();
                for (var i = 0; i < json_data.List.length; i++) {
                    options += '<option value="' + json_data.List[i].Id + '">' + json_data.List[i].Name + '</option>';
                }
                $('#lObrazProgram').html(options).removeAttr('disabled');
                $('#lSpecialization').html('');
            }
        }, 'json');
    }

    function GetSpecializations() {
        var profId = $('#lProfession').val();
        var opId = $('#lObrazProgram').val();
        var sfId = $('#StudyFormId').val();

        if (profId == null || opId == null) {
            return;
        }

        $('#Profs').show();
        $('#ObrazPrograms').show();
        $('#Specs').hide();
        $('#FinishBtn').hide();
        $.post('/Recover/GetSpecializations', { prof: profId, obrazprogram: opId, studyform: $('#StudyFormId').val(),
            studybasis: $('#StudyBasisId').val(), entry: $('#EntryType').val(), isParallel: $('#IsParallelHidden').val(),
            isReduced: $('#IsReducedHidden').val(), semesterId: $('#SemesterId').val()
        }, function (json_data) {
            var options = '';
            if (json_data.List.length == 1 && json_data.List[0].Name == 'нет') {
                $('#FinishBtn').show();
                $('#ObrazProgramsErrors').text('').hide();
            }
            else {
                if (json_data.NoFree) {
                    $('#ObrazProgramsErrors').text('Заявление уже подавалось').show();
                    $('#lSpecialization').attr('disabled', 'disabled').hide();
                }
                else {
                    for (var i = 0; i < json_data.List.length; i++) {
                        options += '<option value="' + json_data.List[i].Id + '">' + json_data.List[i].Name + '</option>';
                    }
                    $('#ObrazProgramsErrors').text('').hide();
                    $('#lSpecialization').html(options).removeAttr('disabled').show();
                    $('#Specs').show();
                }
            }
        }, 'json');
    }

    function MkBtn() {
        $('#FinishBtn').show();
    }
</script>
<% using (Html.BeginForm("NewApp", "ChangeStudyForm", FormMethod.Post))
   { 
%> 
    <%= Html.ValidationSummary() %>
    <% if (Model.EntryType == 2) //mag
       { %>
        <input type="hidden" id="hEntryType" name="EntryType" value="2" />
        <input type="hidden" id="EntryType" name="Entry" value="2" />
    <% }
       else if (Model.EntryType == 1)//1st course
       { %>
        <input type="hidden" id="hEntryType" name="EntryType" value="1" />
        <input type="hidden" id="EntryType" name="Entry" value="1" />
    <% } %>
    <p id="SForm">
        <span>Форма обучения</span><br />
        <%= Html.DropDownListFor(x => x.StudyFormId, Model.StudyForms, new SortedList<string, object>() { { "onchange", "GetProfessions()" } })%>
    </p>
    <input type="hidden" id="StudyBasisId" name="StudyBasisId" value="1" />
    <%= Html.HiddenFor(x => x.SemesterId)%>
    <p id="Profs" style="border-collapse:collapse;">
        <span>Выберите направление(специальность)</span><br />
        <select id="lProfession" size="12" name="lProfession" style="min-width:500px;" onchange="GetObrazPrograms()"></select>
    </p>
    <p id="ObrazPrograms" style="border-collapse:collapse;">
        <span>Выберите образовательную программу</span><br />
        <select id="lObrazProgram" size="5" name="lObrazProgram" style="min-width:500px;" onchange="GetSpecializations()"></select>
    </p>
    <p id="Specs" style="border-collapse:collapse;">
        <span>Выберите профиль</span><br />
        <select id="lSpecialization" size="5" name="lSpecialization" style="min-width:500px;" onchange="MkBtn()"></select>
        <br /><br /><span id="SpecsErrors" class="Red"></span>
    </p>
    <p id="Facs" style="display:none; border-collapse:collapse;">
        <span>Факультет</span><br />
        <select id="lFaculty" size="2" name="lFaculty" onchange="GetProfessions()"></select>
    </p>
    <p id="FinishBtn" style="border-collapse:collapse;">
        <input type="checkbox" name="NeedHostel" title="Нуждаюсь в общежитии на время обучения" /><span style="font-size:13px">Нуждаюсь в общежитии на время обучения</span><br /><br />
        <input id="Submit" type="submit" value="Подать заявление" class="button button-green"/>
    </p>
    <span id="ObrazProgramsErrors" class="message error" style="display:none;"></span>
<% 
   }
%>

</asp:Content>
