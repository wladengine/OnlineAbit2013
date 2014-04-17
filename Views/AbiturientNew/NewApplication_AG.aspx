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
        $('#UILink').hide();
        $('#FinishBtn').hide();
        $('#Specs').hide();
        $('#ManualExam').hide();
        $('#Professions').change(GetSpecializations);
        $('#Profile').change(CheckSpecialization);
        $('#Exams').change(mkButton);
    });

    function GetSpecializations() {
        $('#FinishBtn').hide();
        $('#Specs').hide();
        $('#ManualExam').hide();
        $.post('/AG/GetSpecializations', { classid: $('#EntryClassId').val(), programid: $('#Professions').val() }, function (json_data) {
            if (json_data.IsOk) {
                $('#ObrazProgramsErrors').text('').hide();
                if (json_data.HasProfileExams) {
                    $('#ManualExam').show();
                    var exams = json_data.Exams;
                    var info = '';
                    for (var i = 0; i < exams.length; i++) {
                        info += '<option value="' + exams[i].Value + '">' + exams[i].Name + '</option>';
                    }
                    $('#Exams').html(info).show();
                }
                else {
                    if (json_data.Data == undefined || json_data.Data.length == 0 || (json_data.Data.length == 1 && json_data.Data[0].Id == 0)) {
                        $('#FinishBtn').show();
                    }
                    else {
                        $('#Specs').show();
                        $('#FinishBtn').hide();
                        var opts = '';
                        for (var i = 0; i < json_data.Data.length; i++) {
                            opts += '<option value="' + json_data.Data[i].Id + '">' + json_data.Data[i].Name + '</option>';
                        }
                        $('#Profile').html(opts);
                    }
                }
            }
            else {
                $('#ObrazProgramsErrors').text(json_data.ErrorMessage).show();
            }
        }, 'json');
    }

    function CheckSpecialization() {
        $('#FinishBtn').hide();
        $('#ManualExam').hide();
        $.post('/AG/CheckSpecializations', { classid: $('#EntryClassId').val(), programid: $('#Professions').val(), specid: $('#Profile').val() }, function (json_data) {
            if (json_data.IsOk) {
                $('#ObrazProgramsErrors').text('').hide();
                if (json_data.HasProfileExams) {
                    $('#ManualExam').show();
                    var exams = json_data.Exams;
                    var info = '';
                    for (var i = 0; i < exams.length; i++) {
                        info += '<option value="' + exams[i].Value + '">' + exams[i].Name + '</option>';
                    }
                    $('#Exams').html(info).show();
                }
                else {
                    $('#FinishBtn').show();
                }
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
<% using (Html.BeginForm("NewAppAG", "AbiturientNew", FormMethod.Post))
   { 
%> 
    <%= Html.ValidationSummary() %>
    <% if (DateTime.Now >= new DateTime(2013, 6, 23, 0, 0, 0))
       { %>
       <div class="message error" style="width:450px;">
        <strong style="font-size:10pt">Внимание! Приём документов в АГ СПбГУ закрыт.</strong>
       </div>
    <% } %>
    <% if (!Model.Enabled) {%> Для вас прием закрыт<% }
       else
       {%>
    <div class="message info" style="width:450px;">
        Согласно данным анкеты, Вы поступаете в <strong><%= Model.EntryClassName%></strong>
    </div>
    <br />
    <%= Html.HiddenFor(x => x.EntryClassId)%>
    <h5>Выберите направление подготовки</h5>
    <p id="Profs">
        <span>Направление</span><br />
        <%= Html.DropDownList("Professions", Model.Professions, new Dictionary<string, object>() { { "size", "5" }, { "style", "min-width:450px;" } })%>
    </p>
    <p id="Specs">
        <span>Специализация</span><br />
        <select id="Profile" name="Profile" size="3" style="min-width:450px;"></select>
    </p>
    <p id="ManualExam">
        <span>Экзамен по выбору</span><br />
        <select id="Exams" name="Exam" size="3" style="min-width:450px;"></select>
    </p>
    <p id="FinishBtn" style="display:none;">
        <input type="checkbox" name="NeedHostel" /><span style="font-size:13px">Нуждаюсь в общежитии на время обучения</span><br /><br />
        <input id="Submit" type="submit" value="Подать заявление" class="button button-green"/>
    </p>
    <div id="ObrazProgramsErrors" class="message error" style="display:none; width:450px;">
    </div>
<% 
       }
   }
%>

</asp:Content>
