<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Abiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.ForeignNewApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("NewApplication", "PageTitle").ToString()%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("NewApplication", "PageSubheader").ToString()%></h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% if (0 == 1)
   { %>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
<% } %>
<script type="text/javascript">
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
        $.post('/Abiturient/GetProfs', { studyform: $('#StudyFormId').val(), studybasis: $('#StudyBasisId').val(), entry: $('#Entry').val() }, function (json_data) {
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
        $('#Profs').show();
        $('#ObrazPrograms').show();
        $('#Specs').hide();
        $('#FinishBtn').hide();
        $.post('/Abiturient/GetObrazPrograms', { prof: $('#lProfession').val(), studyform: $('#StudyFormId').val(), studybasis: $('#StudyBasisId').val(), entry: $('#Entry').val() }, function (json_data) {
            var options = '';
            if (json_data.NoFree) {
                $('#ObrazProgramsErrors').text('<%= GetGlobalResourceObject("NewApplication", "ErrorHasApplication").ToString()%>');
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
        $('#Profs').show();
        $('#ObrazPrograms').show();
        $('#Specs').hide();
        $('#FinishBtn').hide();
        $.post('/Abiturient/GetSpecializations', { prof: $('#lProfession').val(), obrazprogram: $('#lObrazProgram').val(), studyform: $('#StudyFormId').val(), studybasis: $('#StudyBasisId').val(), entry: $('#Entry').val() },
        function (json_data) {
            var options = '';
            if (json_data.List.length == 1 && json_data.List[0].Name == 'нет') {
                $('#FinishBtn').show();
                $('#ObrazProgramsErrors').text('').hide();
            }
            else {
                if (json_data.NoFree) {
                    $('#ObrazProgramsErrors').text('<%= GetGlobalResourceObject("NewApplication", "ErrorHasApplication").ToString()%>').show();
                    $('#lProfiles').hide();
                }
                else {
                    $('#ObrazProgramsErrors').text('').hide();
                    for (var j = 0; j < json_data.List.length; j++) {
                        options += '<option value="' + json_data.List[j].Id + '">' + json_data.List[j].Name + '</option>';
                    }
                    $('#Specs').show();
                    $('#lProfiles').html(options).show();
                }
            }
        }, 'json');
    }

    function MkBtn() {
        $('#FinishBtn').show();
    }
</script>
<form action="/ForeignApplication/NewApp" method="post" onsubmit="return f();">
    <%= Html.ValidationSummary() %>
    <fieldset>
        <p class="message info">
            <%= GetGlobalResourceObject("NewApplication", "HeaderObtainingLevel").ToString()%>&nbsp;<%= Model.ObtainingLevel %>
        </p>
        <%--<select id="StudyForm" name="StudyForm" onchange="GetFacs()">
            <% foreach (var sf in Model.StudyForms)
               { %>
                <option value="<%= sf.Key.ToString() %>"><%= sf.Value %></option>
            <% } %>
        </select>--%>
        
        <p id="SForm">
            <span><%= GetGlobalResourceObject("PriorityChangerForeign", "StudyForm").ToString()%></span><br />
            <%= Html.DropDownListFor(x => x.StudyFormId, Model.StudyForms, new Dictionary<string, object>() { { "onchange", "GetProfessions()" } })%>
        </p>
        <p id="SBasis">
            <span><%= GetGlobalResourceObject("PriorityChangerForeign", "StudyBasis").ToString()%></span><br />
            <%= Html.DropDownListFor(x => x.StudyBasisId, Model.StudyBasises, new Dictionary<string, object>() { { "onchange", "GetProfessions()" } })%>
        </p>
        <input type="hidden" id="Entry" name="EntryType" value="<%= Model.EntryType.ToString() %>" />
        <div id="Faculties" class="clearfix" style="display:none;">
            <h6><%= GetGlobalResourceObject("NewApplication", "HeaderFaculty").ToString()%></h6>
            <select id="lFacs" name="Faculty" size="5"></select>
        </div>
        <div id="Profs" class="clearfix" style="display:none;">
            <h6><%= GetGlobalResourceObject("NewApplication", "HeaderProfession").ToString()%></h6>
            <select id="lProfession" name="lProfession" style="min-width:450px;" size="12" onchange="GetObrazPrograms()"></select>
        </div>
        <div id="ObrazPrograms" class="clearfix" style="display:none;">
            <h6><%= GetGlobalResourceObject("NewApplication", "HeaderObrazProgram").ToString()%></h6>
            <select id="lObrazProgram" name="lObrazProgram" style="min-width:450px;" size="5" onchange="GetSpecializations()"></select>
        </div>
        <br /><span id="ObrazProgramsErrors" class="message error" style="display:none; border-collapse:collapse;" onchange="GetSpecializations()"></span>
        <div id="Specs" class="clearfix" style="display:none;">
            <h6><%= GetGlobalResourceObject("NewApplication", "HeaderProfile").ToString()%></h6>
            <select id="lProfiles" name="lSpecialization" size="5" onchange="MkBtn()" style="min-width:450px; border-collapse:collapse;"></select>
        </div>
        <div id="FinishBtn" class="clearfix" style="display:none; border-collapse:collapse;">
            <div class="clearfix">
                <input type="checkbox" name="HostelEduc" />
                <span style="font-size:1.2em;"><%= GetGlobalResourceObject("NewApplication", "chbNeedHostel").ToString()%></span>
            </div><br />
            <div class="clearfix">
                <input type="submit" value="<%= GetGlobalResourceObject("NewApplication", "btnCreateApp").ToString()%>" class="button button-green" />
            </div>
        </div>
    </fieldset>
</form>

</asp:Content>
