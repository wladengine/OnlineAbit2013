<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="OnlineAbit2013.Models" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<Mag_ApplicationModel>" %>

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
        $('#Block<%= Model.Applications.Count + 1 %>').show();
    });  
    var BlockIds = new Object();
    <% for (int i = 1; i <= Model.Applications.Count; i++ ) { %>
        BlockIds['<%= i.ToString() %>'] = '<%= Model.Applications[i - 1].Id.ToString("N") %>';
    <% } %> 

    $(function () {  
        entry = $('#Entry').val();
        GetProfessions(<%= Model.Applications.Count + 1 %>);
        $('#FinishBtn<%= Model.Applications.Count + 1 %>').hide(); 
    });

    function GetProfessions(i) {
        var CurrProfs = '#Profs'+i;
        var CurrlProfession = '#lProfession'+i;
        var CurrlSpecialization = '#lSpecialization'+i;
        var CurrObrazPrograms = '#ObrazPrograms'+i; 
        var CurrObrazProgramsErrors='#ObrazProgramsErrors'+i;
        var CurrlObrazProgram = '#lObrazProgram'+i; 
        var CurrSpecs = '#Specs'+i;
        var CurrFinishBtn = '#FinishBtn'+i; 
        var CurrGosLine = '#GosLine'+i; 

        $(CurrProfs).show();
        $(CurrObrazPrograms).hide();
        $(CurrSpecs).hide();
        $(CurrFinishBtn).hide();
        $(CurrGosLine).hide();
        $.post('/AbiturientNew/GetProfs', { studyform: $('#StudyFormId'+i).val(), studybasis: $('#StudyBasisId'+i).val(),
            entry: $('#EntryType').val(), isSecond: $('#IsSecondHidden'+i).val(), isParallel: $('#IsParallelHidden'+i).val(), isReduced : $('#IsReducedHidden'+i).val() }, function (json_data) 
        {
            var options = '';
            if (json_data.NoFree) {
                $(CurrObrazProgramsErrors).text('Нет направлений, на которые можно подать заявление').show();
                $(CurrlProfession).attr('disabled', 'disabled').hide();
                $(CurrlObrazProgram).html('');
                $(CurrProfs).hide();
            }
            else {
                $(CurrProfs).show();
                $(CurrObrazProgramsErrors).text('').hide();
                for (var i = 0; i < json_data.length; i++) {
                    options += '<option value="' + json_data[i].Id + '">' + json_data[i].Name + '</option>';
                }
                $(CurrlProfession).html(options).removeAttr('disabled').show();
                $(CurrlObrazProgram).html('');
                $(CurrlSpecialization).html('');
            }  
        }, 'json');
    }

    function GetObrazPrograms(i) {
        var CurrProfs = '#Profs'+i;
        var CurrlProfession = '#lProfession'+i;
        var CurrlSpecialization = '#lSpecialization'+i;
        var CurrObrazPrograms = '#ObrazPrograms'+i; 
        var CurrObrazProgramsErrors='#ObrazProgramsErrors'+i;
        var CurrlObrazProgram = '#lObrazProgram'+i; 
        var CurrSpecs = '#Specs'+i;
        var CurrFinishBtn = '#FinishBtn'+i; 
        var CurrGosLine = '#GosLine'+i;  
        var profId = $(CurrlProfession).val();
        var sfId = $('#StudyFormId'+i).val();

        if (profId == null){
            return;
        } 
        $(CurrProfs).show();
        $(CurrObrazPrograms).show();
        $(CurrSpecs).hide();
        $(CurrFinishBtn).hide();
        $(CurrGosLine).hide();
   
        $.post('/Recover/GetObrazPrograms', { prof: profId, studyform: sfId, studybasis: $('#StudyBasisId'+i).val(), 
            entry: $('#EntryType').val(), isParallel: $('#IsParallelHidden'+i).val(), isReduced : $('#IsReducedHidden'+i).val(), 
            semesterId : $('#SemesterId'+i).val() }, function (json_data) {
            var options = '';
            if (json_data.NoFree) {
                $(CurrObrazProgramsErrors).text('Заявление уже подавалось').show();;
                $(CurrlObrazProgram).attr('disabled', 'disabled').hide();
                $(CurrlSpecialization).html('');
            }
            else {
                $(CurrObrazProgramsErrors).text('').hide();
                for (var i = 0; i < json_data.List.length; i++) {
                    options += '<option value="' + json_data.List[i].Id + '">' + json_data.List[i].Name + '</option>';
                }
                $(CurrlObrazProgram).html(options).removeAttr('disabled');
                $(CurrlSpecialization).html('');
            }
        }, 'json');
    }

    function GetSpecializations(i) { 
        var profId = $('#lProfession'+i).val();
        var opId = $('#lObrazProgram'+i).val();
        var sfId = $('#StudyFormId'+i).val();
        var sbId = $('#StudyBasisId'+i).val()
        if (profId == null || opId == null){
            return;
        } 

        var CurrProfs = '#Profs'+i; 
        var CurrObrazPrograms = '#ObrazPrograms'+i;  
        var CurrObrazProgramsErrors='#ObrazProgramsErrors'+i;        
        var CurrSpecs = '#Specs'+i;
        var CurrlSpecialization = '#lSpecialization'+i;
        var CurrFinishBtn = '#FinishBtn'+i; 
        var CurrGosLine = '#GosLine'+i;  
        var CurrGosLineHidden = '#isGosLineHidden'+i;  
        $(CurrProfs).show();
        $(CurrObrazPrograms).show();
        $(CurrSpecs).hide();
        $(CurrFinishBtn).hide();
        $(CurrGosLine).hide();
        $.post('/Recover/GetSpecializations', { prof: profId, obrazprogram: opId, studyform: $('#StudyFormId'+i).val(), 
            studybasis: $('#StudyBasisId'+i).val(), entry: $('#EntryType').val(), CommitId: $('#CommitId').val(), isParallel: $('#IsParallelHidden'+i).val(), 
            isReduced : $('#IsReducedHidden'+i).val(), semesterId : $('#SemesterId'+i).val() }, function (json_data) {
            var options = '';
            if (sbId==1){ <!-- Бюджет -->
                if (json_data.GosLine==0) { <!-- Рф - РФ (только общий прием) -->
                    $(CurrGosLine).hide();
                    $(CurrGosLineHidden).val('0');
                } 
                else {
                    if (json_data.GosLine==11) { <!-- неРф - неРФ или неСНГ-РФ (бд 1, только гослиния)-->
                        $(CurrGosLine).hide();
                        $(CurrGosLineHidden).val('1');  
                    }
                    else { 
                        $(CurrGosLine).show();  
                    }
                }
            }
            else{
                $(CurrGosLine).hide();
                $(CurrGosLineHidden).val('0');
            } 
            
            if (json_data.ret.List.length == 1 && json_data.ret.List[0].Name == 'нет') {
                $(CurrFinishBtn).show();
                $(CurrObrazProgramsErrors).text('').hide(); 
            }
            else {
                if (json_data.ret.NoFree) {
                    $(CurrObrazProgramsErrors).text('Заявление уже подавалось').show();
                    $(CurrlSpecialization).attr('disabled', 'disabled').hide();
                    $(CurrGosLine).hide();  
                }
                else {    
                    for (var i = 0; i< json_data.ret.List.length; i++) {
                        options += '<option value="' + json_data.ret.List[i].Id.toString() + '">' + json_data.ret.List[i].Name.toString() + '</option>';
                    } 
                    $(CurrObrazProgramsErrors).text('').hide();
                    $(CurrlSpecialization).html(options).removeAttr('disabled').show();
                    $(CurrSpecs).show(); 
                }
            }
        }, 'json');
    }

    function MkBtn(i) { 
        $('#FinishBtn' + i).hide();
        currFinishButton = '#FinishBtn' + i;
        currSpecs = '#Specs' + i;  
        currObrazProgramErrors = '#ObrazProgramsErrors' + i;
        currProfile = '#Profile' + i;
        currProfessions = '#Professions' + i;
        currBlock = '#Block' + i;
        currNeedHostel = '#NeedHostel' + i;
        currBlockData = '#BlockData' + i;
        var nxt = i + 1;
        nextBlock = '#Block' + nxt; 
        currBlockData_Profession = '#BlockData_Profession' + i;
        currBlockData_ObrazProgram = '#BlockData_Profession' + i;
        currBlockData_Specialization = '#BlockData_Specialization' + i;  
      
        $.post('/AbiturientNew/CheckApplication_Mag', {
            studyform: $('#StudyFormId'+i).val(), 
            studybasis: $('#StudyBasisId'+i).val(), 
            entry: $('#EntryType').val(),
            isSecond:  $('#IsSecondHidden'+i).val(), 
            isReduced: $('#IsReducedHidden'+i).val(), 
            isParallel: $('#IsParallelHidden'+i).val(), 
            profession: $('#lProfession'+i).val(), 
            obrazprogram:  $('#lObrazProgram'+i).val(), 
            specialization: $('#lSpecialization'+i).val(), 
            NeedHostel: $('#NeedHostel' + i).is(':checked'), 
            CommitId: $('#CommitId').val()   }, 
            function(json_data) {
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
        currObrazProgramErrors = '#ObrazProgramsErrors' + i;  
        currNeedHostel = '#NeedHostel' + i;
        currGosLineHidden = '#isGosLineHidden'+i;
        currGosLine = '#isGosLine'+i;

        currBlock = '#Block' + i; 
        currBlockData = '#BlockData' + i;
        nxt = i + 1;
        nextBlock = '#Block' + nxt;

        currBlockData_StudyFormId = '#BlockData_StudyFormId' + i;
        currBlockData_StudyBasisId = '#BlockData_StudyBasisId' + i;
        currBlockData_Profession = '#BlockData_Profession' + i;
        currBlockData_ObrazProgram = '#BlockData_ObrazProgram' + i;
        currBlockData_Specialization = '#BlockData_Specialization' + i; 

        $.post('/AbiturientNew/AddApplication_Mag', { 
        studyform: $('#StudyFormId'+i).val(), 
        studybasis: $('#StudyBasisId'+i).val(), 
        entry: $('#EntryType').val(),
        isSecond:  $('#IsSecondHidden'+i).val(), 
        isReduced: $('#IsReducedHidden'+i).val(), 
        isParallel: $('#IsParallelHidden'+i).val(), 
        profession: $('#lProfession'+i).val(), 
        obrazprogram: $('#lObrazProgram'+i).val(), 
        specialization: $('#lSpecialization'+i).val(), 
        NeedHostel: $('#NeedHostel' + i).is(':checked'), 
        IsGosLine: $('#isGosLineHidden'+i).val(),
        CommitId: $('#CommitId').val() 
          }, 
          function(json_data) {
            if (json_data.IsOk) { 
                $(currBlockData_StudyFormId).text(json_data.StudyFormName);
                $(currBlockData_StudyBasisId).text(json_data.StudyBasisName);
                $(currBlockData_Profession).text(json_data.Profession);
                $(currBlockData_ObrazProgram).text(json_data.ObrazProgram);
                $(currBlockData_Specialization).text(json_data.Specialization); 
                $(currBlock).hide();
                $(currBlockData).show();
                if (BlockIds[nxt] == undefined) {
                    $(nextBlock).show(); 
                    GetProfessions(nxt);
                }
                BlockIds[i] = json_data.Id; 
            }
            else {
                $(currObrazProgramErrors).text(json_data.ErrorMessage).show();
            }
        }, 'json');
    }
    
    function ChangeGosLine(i) {
        if ($('#IsGosLine'+i).is(':checked')){
            var CurrGosLineHidden = '#isGosLineHidden'+i;  
            $(CurrGosLineHidden).val('1');
        }
        else{
            var CurrGosLineHidden = '#isGosLineHidden'+i;  
            $(CurrGosLineHidden).val('0');
        }
    }

    function ChangeEType(i) {
        entry = $('#Entry').val();
        $('#EntryType').val(entry);
        if (entry == 3) {
            $('#Second'+i).show();
            $('#Parallel'+i).show();
            $('#Reduced'+i).show();
            $('#StudyBasisId'+i).html('<option value="2">Договорная</option>');
            $('#SBasis'+i).hide();
            GetProfessions(i);
        }
        else {
            $('#IsSecond'+i).removeAttr('checked');
            $('#IsSecondHidden'+i).val('0');
            $('#Second'+i).hide();
            $('#Profs'+i).show();
            $('#StudyBasisId'+i).html('<option value="1">Госбюджетная</option><option value="2">Договорная</option>');
            $('#SBasis'+i).show();
            GetProfessions(i);
        }
    }
    function ChangeIsSecond(i) {
        if ($('#IsSecond'+i).is(':checked')) {
            $('#IsSecondHidden'+i).val('1');
            $('#SBasis'+i).hide();
            $('#StudyBasisId'+i).html('<option value="2">Договорная</option>');
        }
        else {
            $('#IsSecondHidden'+i).val('0');
            if (!$('#IsParallel'+i).is(':checked') && !$('#IsReduced'+i).is(':checked') && $('#Entry').val() != 3)
            {
                $('#StudyBasisId'+i).html('<option value="1">Госбюджетная</option><option value="2">Договорная</option>');
                $('#SBasis'+i).show();
            }
        }
        GetProfessions(i);
    }
    function ChangeIsParallel(i) {
        if ($('#IsParallel'+i).is(':checked')) {
            $('#IsParallelHidden'+i).val('1');
            $('#SBasis'+i).hide();
            $('#StudyBasisId'+i).html('<option value="2">Договорная</option>');
        }
        else {
            $('#IsParallelHidden'+i).val('0');
            if (!$('#IsReduced'+i).is(':checked') && !$('#IsSecond'+i).is(':checked') && $('#Entry').val() != 3)
            {
                $('#StudyBasisId'+i).html('<option value="1">Госбюджетная</option><option value="2">Договорная</option>');
                $('#SBasis'+i).show();
            }
        }
        GetProfessions(i);
    }
    function ChangeIsReduced(i) {
        if ($('#IsReduced'+i).is(':checked')) {
            $('#IsReducedHidden'+i).val('1');
            $('#SBasis'+i).hide(); 
            $('#StudyBasisId'+i).html('<option value="2">Договорная</option>');
        }
        else {
            $('#IsReducedHidden'+i).val('0');
            if (!$('#IsParallel'+i).is(':checked') && !$('#IsSecond'+i).is(':checked') && $('#Entry').val() != 3)
            {
                $('#StudyBasisId'+i).html('<option value="1">Госбюджетная</option><option value="2">Договорная</option>');
                $('#SBasis'+i).show();
            }
        }
        GetProfessions(i);
    }
     
    function DeleteApp(i) {
        var appId = BlockIds[i];
        nextBlock = '#Block' + i;
        nextFinishButton = '#FinishBtn' + i;
        nextSpecs = '#Specs' + i;    
        nextObrazProgramErrors = '#ObrazProgramsErrors' + i; 
        nextProfessions = '#Professions' + i;
        nextNeedHostel = '#NeedHostel' + i;
        nextBlockData = '#BlockData' + i;
        
        nextBlockData_StudyFormId = '#BlockData_StudyFormId' + i;
        nextBlockData_StudyBasisId = '#BlockData_StudyBasisId' + i;
        nextBlockData_Profession = '#BlockData_Profession' + i;
        nextBlockData_ObrazProgram = '#BlockData_ObrazProgram' + i;
        nextBlockData_Specialization = '#BlockData_Specialization' + i; 

        currObrazProgramsErrors_Block = '#ObrazProgramsErrors_Block' + i;
        $(currObrazProgramsErrors_Block).text('').hide();

        $.post('/AbiturientNew/DeleteApplication_Mag', { id : appId, CommitId : $('#CommitId').val() }, function(json_data) {
            if (json_data.IsOk) {  
                $(nextBlockData_StudyFormId).text('');
                $(nextBlockData_StudyBasisId).text('');
                $(nextBlockData_Profession).text('');
                $(nextBlockData_ObrazProgram).text('');
                $(nextBlockData_Specialization).text('');
                $(nextBlockData).hide();
                $(nextBlock).show();
                GetProfessions(i);  
            }
            else {
                $(currObrazProgramsErrors_Block).text(json_data.ErrorMessage).show();
            }
        }, 'json');
    }
</script>
<% using (Html.BeginForm("NewApp_Mag", "AbiturientNew", FormMethod.Post))
   { 
%> 
    <%= Html.ValidationSummary() %>
     <%= Html.HiddenFor(x => x.CommitId)%>
    <% if (2 == 1 && DateTime.Now < new DateTime(2012, 6, 20, 0, 0, 0))
       { %>
       <div class="message warning">Внимание! Подача заявлений на <strong style="font-size:10pt">первый курс</strong> начнётся с <strong style="font-size:11pt">20 июня 2012 года</strong></div>
    <% } %>
    
        <input type="hidden" id = "EntryType" name = "EntryType" value="2" />
        <select id="Entry" name="Entry" onchange="ChangeEType()" disabled="disabled">
            <option value="2"><%= GetGlobalResourceObject("NewApplication", "Select_Magistery")%></option>
        </select>
        <% for (int i = 1; i <= Model.Applications.Count; i++)
           { %>
           <div id="BlockData<%= i.ToString()%>" class="message info panel" style="width:450px;">
            <table class="nopadding" cellspacing="0" cellpadding="0">
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("PriorityChangerForeign", "Priority").ToString()%></td>
                <td style="font-size:1.3em;"><%= i.ToString() %></td>
            </tr>
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("NewApplication", "BlockData_StudyForm")%></td>
                <td id="BlockData_StudyFormId<%= i.ToString()%>" style="font-size:1.3em;"><%= Model.Applications[i - 1].StudyFormName  %></td>
            </tr>
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("NewApplication", "BlockData_StudyBasis")%></td>
                <td id="BlockData_StudyBasisId<%= i.ToString()%>" style="font-size:1.3em;"><%= Model.Applications[i - 1].StudyBasisName%></td>
            </tr>
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("NewApplication", "BlockData_LicenseProgram")%></td>
                <td id="BlockData_Profession<%= i.ToString()%>" ><%= Model.Applications[i - 1].ProfessionName%></td>
            </tr>
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("NewApplication", "BlockData_LicenseProgram")%></td>
                <td id="BlockData_ObrazProgram<%= i.ToString()%>" ><%= Model.Applications[i - 1].ObrazProgramName%></td>
            </tr>
            <tr>
                <td style="width:12em;"><%= GetGlobalResourceObject("NewApplication", "BlockData_Specialization")%></td>
                <td id="BlockData_Specialization<%= i.ToString()%>" ><%= Model.Applications[i - 1].SpecializationName%></td>
            </tr>
        </table>
        <button type="button" onclick="DeleteApp(<%= i.ToString()%>)" class="error"><%= GetGlobalResourceObject("NewApplication", "Delete")%></button>
        <div id="ObrazProgramsErrors_Block<%= i.ToString()%>" class="message error" style="display:none; width:450px;">
        </div>
    </div>
    <div id="Block<%= i.ToString()%>" class="message info panel" style="width:450px; display:none;">
        <p id="SForm<%= i.ToString()%>">
            <span><%= GetGlobalResourceObject("NewApplication", "BlockData_StudyForm")%></span><br /> 
            <%= Html.DropDownList("StudyFormId" + i.ToString(), Model.StudyFormList, new Dictionary<string, object>() { { "size", "1" },
                 { "style", "min-width:450px;" }, { "onchange", "GetProfessions(" + i.ToString() + ")" } })%>
        </p>
        <p id="SBasis<%= i.ToString()%>">
            <span><%= GetGlobalResourceObject("NewApplication", "BlockData_StudyBasis")%></span><br />
            <%= Html.DropDownList("StudyBasisId" + i.ToString(), Model.StudyBasisList, new Dictionary<string, object>() { { "size", "1" }, 
                { "style", "min-width:450px;" },   { "onchange", "GetProfessions(" + i.ToString() + ")" } })%>
        </p>
        <p id="Reduced<%= i.ToString()%>" style=" border-collapse:collapse;">
            <input type="checkbox" id="IsReduced<%= i.ToString()%>" name="IsReduced" title="Второе высшее" onclick="ChangeIsReduced(<%= i.ToString()%>)"/><span style="font-size:13px"><%= GetGlobalResourceObject("NewApplication", "IsReduced")%></span><br />
            <input type="hidden" name="IsReducedHidden" id="IsReducedHidden<%= i.ToString()%>" value="0"/>
        </p>
        <p id="Parallel<%= i.ToString()%>" style=" border-collapse:collapse;">
            <input type="checkbox" id="IsParallel<%= i.ToString()%>" name="IsParallel" title="Параллельное обучение" onclick="ChangeIsParallel(<%= i.ToString()%>)"/><span style="font-size:13px"><%= GetGlobalResourceObject("NewApplication", "IsParallel")%></span><br />
            <input type="hidden" name="IsParallelHidden" id="IsParallelHidden<%= i.ToString()%>" value="0"/>
        </p>
        <p id="Second<%= i.ToString()%>" style=" border-collapse:collapse;">
            <input type="checkbox" id="IsSecond<%= i.ToString()%>" name="IsSecond" title="Для лиц, имеющих ВО" onclick="ChangeIsSecond(<%= i.ToString()%>)"/><span style="font-size:13px"><%= GetGlobalResourceObject("NewApplication", "IsSecond")%></span><br />
            <input type="hidden" name="IsSecondHidden" id="IsSecondHidden<%= i.ToString()%>" value="0"/>
        </p>
        <p id="Profs<%= i.ToString()%>" style="border-collapse:collapse;">
            <span><%= GetGlobalResourceObject("NewApplication", "HeaderProfession")%></span><br />
            <select id="lProfession<%= i.ToString()%>" size="12" name="lProfession" style="min-width:450px;" onchange="GetObrazPrograms(<%= i.ToString()%>)"></select>
        </p>
        <p id="ObrazPrograms<%= i.ToString()%>" style="border-collapse:collapse;">
            <span><%= GetGlobalResourceObject("NewApplication", "HeaderObrazProgram")%></span><br />
            <select id="lObrazProgram<%= i.ToString()%>" size="5" name="lObrazProgram" style="min-width:450px;" onchange="GetSpecializations(<%= i.ToString()%>)"></select>
        </p>
        <p id="Specs<%= i.ToString()%>" style="border-collapse:collapse;">
            <span><%= GetGlobalResourceObject("NewApplication", "HeaderProfile")%></span><br />
            <select id="lSpecialization<%= i.ToString()%>" size="5" name="lSpecialization" style="min-width:450px;" onchange="MkBtn(<%= i.ToString()%>)"></select>
            <br /><br /><span id="SpecsErrors<%= i.ToString()%>" class="Red"></span>
        </p>
        <p id="Facs<%= i.ToString()%>" style="display:none; border-collapse:collapse;">
            <span><%= GetGlobalResourceObject("NewApplication", "HeaderFaculty")%></span><br />
            <select id="lFaculty<%= i.ToString()%>" size="2" name="lFaculty" onchange="GetProfessions(<%= i.ToString()%>)"></select>
        </p>
        <p id = "GosLine<%= i.ToString()%>" style="display:none;" >
             <input type="checkbox" name="isGosLine" title="Поступать по гослинии" id="IsGosLine<%= i.ToString()%>" onchange="ChangeGosLine(<%= i.ToString()%>)"/><span style="font-size:13px">Поступать по гослинии</span><br /><br />
             <input type="hidden" name="isGosLineHidden" title="Поступать по гослинии" id="isGosLineHidden<%= i.ToString()%>" ></input>
        </p>
        <p id="FinishBtn<%= i.ToString()%>" style="border-collapse:collapse;">
            <input type="checkbox" name="NeedHostel" title="Нуждаюсь в общежитии на время обучения" id="NeedHostel<%= i.ToString()%>"  /><span style="font-size:13px"><%= GetGlobalResourceObject("NewApplication", "chbNeedHostel")%></span><br /><br />
            <input id="Submit<%= i.ToString()%>" type="button" value="Добавить" onclick="SaveData(<%= i.ToString()%>)" class="button button-blue"/>
        </p><br />
        <span id="ObrazProgramsErrors<%= i.ToString()%>" class="message error" style="display:none;"></span>
        </div>
       <%} %>
    <% for (int i = Model.Applications.Count + 1; i <= Model.MaxBlocks; i++)  
       { %> 
        <div id="BlockData<%= i.ToString()%>" class="message info panel" style="width:450px; display:none;">
            <table class="nopadding" cellspacing="0" cellpadding="0">
                <tr>
                    <td style="width:12em;"><%= GetGlobalResourceObject("PriorityChangerForeign", "Priority").ToString()%></td>
                    <td style="font-size:1.3em;"><%= i.ToString() %></td>
                </tr>
                <tr>
                    <td style="width:12em;"><%= GetGlobalResourceObject("NewApplication", "BlockData_StudyForm")%></td>
                    <td id="BlockData_StudyFormId<%= i.ToString() %>" style="font-size:1.3em;"></td>
                </tr>
                <tr>
                    <td style="width:12em;"><%= GetGlobalResourceObject("NewApplication", "BlockData_StudyBasis")%></td>
                    <td id="BlockData_StudyBasisId<%= i.ToString() %>" style="font-size:1.3em;"></td>
                </tr>
                <tr>
                    <td style="width:12em;"><%= GetGlobalResourceObject("NewApplication", "BlockData_LicenseProgram")%></td>
                    <td id="BlockData_Profession<%= i.ToString() %>" ></td>
                </tr>
                <tr>
                    <td style="width:12em;"><%= GetGlobalResourceObject("NewApplication", "BlockData_ObrazProgram")%></td>
                    <td id="BlockData_ObrazProgram<%= i.ToString() %>" ></td>
                </tr>
                <tr>
                    <td style="width:12em;"><%= GetGlobalResourceObject("NewApplication", "BlockData_Specialization")%></td>
                    <td id="BlockData_Specialization<%= i.ToString() %>" ></td>
                </tr>
            </table>
            <button type="button" onclick="DeleteApp(<%= i.ToString()%>)" class="error"><%= GetGlobalResourceObject("NewApplication", "Delete")%></button>
            <div id="ObrazProgramsErrors_Block<%= i.ToString()%>" class="message error" style="display:none; width:450px;">
            </div>
        </div>
       <div id="Block<%= i.ToString()%>" class="message info panel" style="width:450px; display:none;">
        <p id="SForm<%= i.ToString()%>">
            <span>Форма обучения</span><br /> 
            <%= Html.DropDownList("StudyFormId" + i.ToString(), Model.StudyFormList, new Dictionary<string, object>() { { "size", "1" },
                 { "style", "min-width:450px;" }, { "onchange", "GetProfessions(" + i.ToString() + ")" } })%>
        </p>
        <p id="SBasis<%= i.ToString()%>">
            <span>Основа обучения</span><br />
            <%= Html.DropDownList("StudyBasisId" + i.ToString(), Model.StudyBasisList, new Dictionary<string, object>() { { "size", "1" }, 
                { "style", "min-width:450px;" },   { "onchange", "GetProfessions(" + i.ToString() + ")" } })%>
        </p>
        <p id="Reduced<%= i.ToString()%>" style=" border-collapse:collapse;">
            <input type="checkbox" id="IsReduced<%= i.ToString()%>" name="IsReduced" title="Второе высшее" onclick="ChangeIsReduced(<%= i.ToString()%>)"/><span style="font-size:13px"><%= GetGlobalResourceObject("NewApplication", "IsReduced")%></span><br />
            <input type="hidden" name="IsReducedHidden" id="IsReducedHidden<%= i.ToString()%>" value="0"/>
        </p>
        <p id="Parallel<%= i.ToString()%>" style=" border-collapse:collapse;">
            <input type="checkbox" id="IsParallel<%= i.ToString()%>" name="IsParallel" title="Параллельное обучение" onclick="ChangeIsParallel(<%= i.ToString()%>)"/><span style="font-size:13px"><%= GetGlobalResourceObject("NewApplication", "IsParallel")%></span><br />
            <input type="hidden" name="IsParallelHidden" id="IsParallelHidden<%= i.ToString()%>" value="0"/>
        </p>
        <p id="Second<%= i.ToString()%>" style=" border-collapse:collapse;">
            <input type="checkbox" id="IsSecond<%= i.ToString()%>" name="IsSecond" title="Для лиц, имеющих ВО" onclick="ChangeIsSecond(<%= i.ToString()%>)"/><span style="font-size:13px"><%= GetGlobalResourceObject("NewApplication", "IsSecond")%></span><br />
            <input type="hidden" name="IsSecondHidden" id="IsSecondHidden<%= i.ToString()%>" value="0"/>
        </p>
        <p id="Profs<%= i.ToString()%>" style="border-collapse:collapse;">
            <span><%= GetGlobalResourceObject("NewApplication", "HeaderProfession")%></span><br />
            <select id="lProfession<%= i.ToString()%>" size="12" name="lProfession" style="min-width:450px;" onchange="GetObrazPrograms(<%= i.ToString()%>)"></select>
        </p>
        <p id="ObrazPrograms<%= i.ToString()%>" style="border-collapse:collapse;">
            <span><%= GetGlobalResourceObject("NewApplication", "HeaderObrazProgram")%></span><br />
            <select id="lObrazProgram<%= i.ToString()%>" size="5" name="lObrazProgram" style="min-width:450px;" onchange="GetSpecializations(<%= i.ToString()%>)"></select>
        </p>
        <p id="Specs<%= i.ToString()%>" style="border-collapse:collapse;">
            <span><%= GetGlobalResourceObject("NewApplication", "HeaderProfile")%></span><br />
            <select id="lSpecialization<%= i.ToString()%>" size="5" name="lSpecialization" style="min-width:450px;" onchange="MkBtn(<%= i.ToString()%>)"></select>
            <br /><br /><span id="SpecsErrors<%= i.ToString()%>" class="Red"></span>
        </p>
        <p id="Facs<%= i.ToString()%>" style="display:none; border-collapse:collapse;">
            <span>Факультет</span><br />
            <select id="lFaculty<%= i.ToString()%>" size="2" name="lFaculty" onchange="GetProfessions(<%= i.ToString()%>)"></select>
        </p> 
        <p id = "GosLine<%= i.ToString()%>" style="display:none;" >
             <input type="checkbox" name="isGosLine" title="Поступать по гослинии" id="IsGosLine<%= i.ToString()%>" onchange="ChangeGosLine(<%= i.ToString()%>)"/><span style="font-size:13px">Поступать по гослинии</span><br /><br />
             <input type="hidden" name="isGosLineHidden" title="Поступать по гослинии" id="isGosLineHidden<%= i.ToString()%>" ></input>
        </p>
        <p id="FinishBtn<%= i.ToString()%>" style="border-collapse:collapse;">
            <input type="checkbox" name="NeedHostel" title="Нуждаюсь в общежитии на время обучения" id="NeedHostel<%= i.ToString()%>" /><span style="font-size:13px"><%= GetGlobalResourceObject("NewApplication", "chbNeedHostel")%></span><br /><br />
            <input id="Submit<%= i.ToString()%>" type="button" value="Добавить" onclick="SaveData(<%= i.ToString()%>)" class="button button-blue"/>
        </p><br />
        <span id="ObrazProgramsErrors<%= i.ToString()%>" class="message error" style="display:none;"></span>
        </div>
    <%} %>
    <br />
    <input id="Submit" type="submit" value="Подтвердить" class="button button-green"/>
<% 
   }
%>

</asp:Content>
