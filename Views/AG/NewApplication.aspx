<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="OnlineAbit2013.Models" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AG/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<AG_ApplicationModel>" %>

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

    function GetSpecializations(i) {
        $('#FinishBtn' + i).hide();
        $('#Specs' + i).hide();
        $('#ManualExam' + i).hide();
        $.post('/AG/GetSpecializations', { classid: $('#EntryClassId' + i).val(), programid: $('#Professions' + i).val() }, function (json_data) {
            if (json_data.IsOk) {
                $('#ObrazProgramsErrors' + i).text('').hide();
                if (json_data.HasProfileExams) {
                    $('#ManualExam' + i).show();
                    var exams = json_data.Exams;
                    var info = '';
                    for (var i = 0; i < exams.length; i++) {
                        info += '<option value="' + exams[i].Value + '">' + exams[i].Name + '</option>';
                    }
                    $('#Exams' + i).html(info).show();
                }
                else {
                    if (json_data.Data == undefined || json_data.Data.length == 0 || (json_data.Data.length == 1 && json_data.Data[0].Id == 0)) {
                        $('#FinishBtn' + i).show();
                    }
                    else {
                        $('#Specs' + i).show();
                        $('#FinishBtn' + i).hide();
                        var opts = '';
                        for (var i = 0; i < json_data.Data.length; i++) {
                            opts += '<option value="' + json_data.Data[i].Id + '">' + json_data.Data[i].Name + '</option>';
                        }
                        $('#Profile' + i).html(opts);
                    }
                }
            }
            else {
                $('#ObrazProgramsErrors' + i).text(json_data.ErrorMessage).show();
            }
        }, 'json');
    }

    function CheckSpecialization(i) {
        $('#FinishBtn' + i).hide();
        $('#ManualExam' + i).hide();
        $.post('/AG/CheckSpecializations', { classid: $('#EntryClassId' + i).val(), programid: $('#Professions' + i).val(), specid: $('#Profile' + i).val() }, function (json_data) {
            if (json_data.IsOk) {
                $('#ObrazProgramsErrors' + i).text('').hide();
                if (json_data.HasProfileExams) {
                    $('#ManualExam' + i).show();
                    var exams = json_data.Exams;
                    var info = '';
                    for (var i = 0; i < exams.length; i++) {
                        info += '<option value="' + exams[i].Value + '">' + exams[i].Name + '</option>';
                    }
                    $('#Exams' + i).html(info).show();
                }
                else {
                    $('#FinishBtn' + i).show();
                }
            }
            else {
                $('#FinishBtn' + i).hide();
                $('#ObrazProgramsErrors' + i).text(json_data.ErrorMessage).show();
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
    <% for (int i = 1; i < Model.MaxBlocks; i++) { %>
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
            <input id="Submit<%= i.ToString() %>" type="button" value="Добавить" class="button button-green"/>
        </p>
        <div id="ObrazProgramsErrors<%= i.ToString() %>" class="message error" style="display:none; width:450px;">
        </div>
        
    </div>
    <% } %>
    <input id="Submit" type="submit" value="Подать заявление" class="button button-green"/>
<% 
   }
%>

</asp:Content>
