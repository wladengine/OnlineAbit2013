<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="OnlineAbit2013.Models" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/SPO/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<SPO_NewApplicationModel>" %>

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
        $('#UILink').hide();
        <% if (Model.CanChooseExitClass)
           { %>
        $('#SForm').hide();
        $('#ObrazProgram').change(GetStudyForms);
        <% }
           else
           { %>
        GetStudyForms();
        <% } %>
        $('#FinishBtn').hide();
        $('#SBasis').hide();
        $('#Profs').hide();
        $('#StudyForm').change(GetStudyBasises);
        $('#StudyBasis').change(GetProfessions);
        $('#Profs').change(CheckProfession);
    });

    function GetStudyForms() {
        $('#ObrazProgramsErrors').hide();
        $('#SForm').show();
        $('#SBasis').hide();
        $('#FinishBtn').hide();
        $('#Profs').hide();
        $.post('/SPO/GetStudyForms', { obrazProgramId: $('#ObrazProgram').val() }, function (json_data) {
            var sforms = json_data.Data;
                var info = '';
                for (var i = 0; i < sforms.length; i++) {
                    info += '<option value="' + sforms[i].Value + '">' + sforms[i].Name + '</option>';
                }
                $('#StudyForm').html(info).show();
        }, 'json');
    }
    function GetStudyBasises() {
        $('#ObrazProgramsErrors').hide();
        $('#SBasis').show();
        $('#FinishBtn').hide();
        $('#Profs').hide();
        $.post('/SPO/GetStudyBasises', { obrazProgramId: $('#ObrazProgram').val(), studyFormId : $('#StudyForm').val() }, function (json_data) {
            var sforms = json_data.Data;
                var info = '';
                for (var i = 0; i < sforms.length; i++) {
                    info += '<option value="' + sforms[i].Value + '">' + sforms[i].Name + '</option>';
                }
                $('#StudyBasis').html(info).show();
        }, 'json');
    }
    function GetProfessions() {
        $('#ObrazProgramsErrors').hide();
        $('#Profs').show();
        $('#FinishBtn').hide();
        $.post('/SPO/GetProfessions', { obrazProgramId: $('#ObrazProgram').val(), studyFormId : $('#StudyForm').val(), studyBasisId : $('#StudyBasis').val() }, function (json_data) {
            var sforms = json_data.Data;
                var info = '';
                for (var i = 0; i < sforms.length; i++) {
                    info += '<option value="' + sforms[i].Value + '">' + sforms[i].Name + '</option>';
                }
                $('#Profession').html(info).show();
        }, 'json');
    }
    function CheckProfession() {
        $('#ObrazProgramsErrors').hide();
        $('#SBasis').show();
        $('#FinishBtn').hide();
        $.post(
            '/SPO/CheckProfession', 
            { obrazProgramId: $('#ObrazProgram').val(), studyFormId : $('#StudyForm').val(), studyBasisId : $('#StudyBasis').val(), professionId : $('#Profession').val() }, 
            function (json_data) {
                if (json_data.IsOk) {
                    $('#ObrazProgramsErrors').text('').hide();
                    $('#FinishBtn').show();
                }
                else {
                    $('#FinishBtn').hide();
                    $('#ObrazProgramsErrors').text(json_data.ErrorMessage).show();
                }
            }, 'json');
    }

    function mkButton() {
        $('#FinishBtn').show();
    }
</script>
<% using (Html.BeginForm("NewApp", "SPO", FormMethod.Post))
   { 
%> 
    <%= Html.ValidationSummary() %>
    <% if (!Model.CanChooseExitClass)
       { %>
    <div class="message info" style="width:450px;">
        Согласно данным анкеты, Вы поступаете на образовательную программу <strong><%= Model.ObrazProgramName%></strong>
    </div>
    <br />
    <input id="ObrazProgram" name="ObrazProgram" type="hidden" value="<%= Model.ObrazProgramId %>" />
    <% }
       else
       {
    %>
    <p id="ObrProg">
        <span>Выберите образовательную программу</span><br />
        <%= Html.DropDownList("ObrazProgram", Model.ObrazPrograms, new Dictionary<string, object>() { { "size", "2" }, { "style", "min-width:450px;" } }) %>
    </p>
    <% } %>
    <p id="SForm">
        <span>Форма обучения</span><br />
        <select id="StudyForm" name="StudyForm" size="3" style="min-width:450px;"></select>
    </p>
    <p id="SBasis">
        <span>Основа обучения</span><br />
        <select id="StudyBasis" name="StudyBasis" size="3" style="min-width:450px;"></select>
    </p>
    <p id="Profs">
        <span>Направление</span><br />
        <select id="Profession" name="Profession" size="5" style="min-width:450px;"></select>
    </p>
    <span id="ObrazProgramsErrors" class="message error" style="display:none;"></span>
    <p id="FinishBtn">
        <input type="checkbox" name="NeedHostel" /><span style="font-size:13px">Нуждаюсь в общежитии на время обучения</span><br /><br />
        <input id="Submit" type="submit" value="Подать заявление" class="button button-green"/>
    </p>
<% 
   }
%>

</asp:Content>
