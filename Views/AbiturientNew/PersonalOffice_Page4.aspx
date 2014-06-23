<%@ Import Namespace="OnlineAbit2013" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.PersonalOffice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("Main", "PersonalOfficeHeader").ToString()%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% if (1 == 0)/* типа затычка, чтобы VS видела скрипты */
   {
%>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
<% } %>
    <script type="text/javascript" src="../../Scripts/jquery.ui.datepicker-ru.js"></script>
    <style>
	    .ui-autocomplete {
		    max-height: 200px;
		    max-width: 400px;
		    overflow-y: auto;
		    /* prevent horizontal scrollbar */
		    overflow-x: hidden;
		    /* add padding to account for vertical scrollbar */
		    padding-right: 20px;
	    }
	    /* IE 6 doesn't support max-height
	        * we use height instead, but this forces the menu to always be this tall
	        */
	    * html .ui-autocomplete {
		    height: 200px;
	    }
    </style> 
        <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
        <script type="text/javascript">
            $(function () { setTimeout(GetCities, 50) });
            $(function () { setTimeout(GetProfessions, 50) });

            <% if (!String.IsNullOrEmpty(Model.CurrentEducation.HiddenObrazProgramId)) { %>
            $(function () { setTimeout(GetObrazPrograms, 50) });
            <% } %>

            function Myfun() {
                var profId = $('#CurrentEducation_LicenseProgramId').val();
                $('#CurrentEducation_HiddenLicenseProgramId').val(profId);
                var obrzId = $('#CurrentEducation_ObrazProgramId').val();
                $('#CurrentEducation_HiddenObrazProgramId').val(obrzId);
            }
             
            function GetProfessions() {
                if ($('#EducationInfo_VuzAdditionalTypeId').val() == 2) {
                    var CurLevelId = $('#CurrentEducation_StudyLevelId').val();
                    if (CurLevelId == 15) CurLevelId = 4;
                    else if (CurLevelId == 16) CurLevelId = 1;
                    else if (CurLevelId == 17) CurLevelId = 2;
                    else if (CurLevelId == 18) CurLevelId = 1;
                    var CurrlObrazProgram = '#CurrentEducation_ObrazProgramId';
                    var CurrlProfession = '#CurrentEducation_LicenseProgramId';
                    var sfId = $('#CurrentEducation_StudyFormId').val();
                     
                    var curSemester = $('#CurrentEducation_SemesterId').val();
                     
                    $.post('/AbiturientNew/GetProfs', { studyform: sfId, studybasis: $('#CurrentEducation_StudyBasisId').val(),
                        entry: CurLevelId,
                        semesterId: curSemester
                    }, function (json_data) {
                        var options = '';
                        if (json_data.NoFree) {
                            $('#CurrentEducation_LicenseProgramId').html('');
                            $('#CurrentEducation_ObrazProgramId').html('');
                        }
                        else {
                            for (var i = 0; i < json_data.length; i++) {
                                options += '<option value="' + json_data[i].Id +'"';
                                if (json_data[i].Id == $('#CurrentEducation_HiddenLicenseProgramId').val())
                                    options += ' selected="true" ';
                                options += ' >' + json_data[i].Name + '</option>';
                            }
                            $('#CurrentEducation_LicenseProgramId').html(options);
                            $('#CurrentEducation_ObrazProgramId').html('');
                        }
                    }, 'json');
                }
            }
            function GetObrazPrograms() {
                var CurrlObrazProgram = '#CurrentEducation_ObrazProgramId'; 
                var profId = $('#CurrentEducation_LicenseProgramId').val();
                var sfId = $('#CurrentEducation_StudyFormId').val();
                if (profId == null){
                    profId = $('#CurrentEducation_HiddenLicenseProgramId').val();
                }
                $('#_ObrazProg').show();
                var CurLevelId = $('#CurrentEducation_StudyLevelId').val();
                if (CurLevelId == 15) CurLevelId = 4;
                else if (CurLevelId == 16) CurLevelId = 1;
                else if (CurLevelId == 17) CurLevelId = 2;
                else if (CurLevelId == 18) CurLevelId = 1;
                var curSemester = $('#CurrentEducation_SemesterId').val();
                $.post('/Transfer/GetObrazPrograms', { prof: profId, studyform: sfId, studybasis: $('#CurrentEducation_StudyBasisId').val(),
                    entry: CurLevelId, semesterId: curSemester
                }, function (json_data) {
                    var options = '';
                    if (json_data.NoFree) {
                        $('#CurrentEducation_ObrazProgramId').html('');
                    }
                    else {
                        for (var i = 0; i < json_data.List.length; i++) {
                            options += '<option value="' + json_data.List[i].Id +'"';
                            if (json_data.List[i].Id == $('#CurrentEducation_HiddenObrazProgramId').val())
                                options += ' selected="true" ';
                            options += ' >' + json_data.List[i].Name + '</option>';
                        }
                        $('#CurrentEducation_ObrazProgramId').html(options).removeAttr('disabled').show();
                    }
                }, 'json');
            }

            function GetCities() {
                if ($('#EducationInfo_CountryEducId').val() == '193') {
                    $.post('../../AbiturientNew/GetCityNames', { regionId: $('#EducationInfo_RegionEducId').val() }, function (data) {
                        if (data.IsOk) {
                            $('#EducationInfo_SchoolCity').autocomplete({
                                source: data.List
                            });
                        }
                    }, 'json');
                }
            }
            function CheckForm() {
                var ret = true;
                if (!CheckSchoolName()) { ret = false; }
                if (!CheckSchoolExitYear()) { ret = false; }
                if (!CheckAttestatRegion()) { ret = false; }
                return ret;
            }

            function CheckSchoolName() {
                var ret = true;
                if ($('#EducationInfo_SchoolName').val() == '') {
                    ret = false;
                    $('#EducationInfo_SchoolName').addClass('input-validation-error');
                    $('#EducationInfo_SchoolName_Message').show();
                }
                else {
                    $('#EducationInfo_SchoolName').removeClass('input-validation-error');
                    $('#EducationInfo_SchoolName_Message').hide();
                }
                if ($('#EducationInfo_SchoolName').val() == "Санкт-Петербургский государственный  университет (СПбГУ)") {
                    $('#_AccreditationInfo').hide();
                    $('#EducationInfo_SchoolCity').val("Санкт-Петербург");
                }
                else {
                    $('#_AccreditationInfo').show();
                    $('#EducationInfo_SchoolCity').val('');
                }
                var SchoolTypeId = $('#EducationInfo_SchoolTypeId').val();
                if (SchoolTypeId == '4') {
                    if (($('#EducationInfo_VuzAdditionalTypeId').val() == 2)||($('#EducationInfo_VuzAdditionalTypeId').val() == 3)){
                        $('#EducationInfo_SchoolName').val("Санкт-Петербургский государственный  университет (СПбГУ)");
                        $('#EducationInfo_SchoolCity').val("Санкт-Петербург"); 
                      }
                }
                return ret;
            }
            function CheckSchoolExitYear() {
                var ret = true; 
                if ($('#EducationInfo_SchoolExitYear').val() == '') {
                    ret = false;
                    $('#EducationInfo_SchoolExitYear').addClass('input-validation-error');
                    $('#EducationInfo_SchoolExitYear_Message').show();
                    $('#EducationInfo_SchoolExitYear_MessageFormat').hide();
                }
                else {
                    $('#EducationInfo_SchoolExitYear').removeClass('input-validation-error');
                    $('#EducationInfo_SchoolExitYear_Message').hide(); 
                    var regex = /^\d{4}$/i;
                    var val = $('#EducationInfo_SchoolExitYear').val();
                    if (!regex.test(val)) {
                        $('#EducationInfo_SchoolExitYear').addClass('input-validation-error');
                        $('#EducationInfo_SchoolExitYear_MessageFormat').show();
                        ret = false;
                    }
                    else {
                        $('#EducationInfo_SchoolExitYear').removeClass('input-validation-error');
                        $('#EducationInfo_SchoolExitYear_MessageFormat').hide();
                    }
                }
                return ret;
            } 
            function CheckDisorderInfoYear() {
                var ret = true;
                if ($('#DisorderInfo_YearOfDisorder').val() == '') {
                    ret = false;
                    $('#DisorderInfo_YearOfDisorder').addClass('input-validation-error');
                    $('#DisorderInfo_Year_Message').show();
                    $('#DisorderInfo_Year_MessageFormat').hide();
                }
                else {
                    $('#DisorderInfo_YearOfDisorder').removeClass('input-validation-error');
                    $('#DisorderInfo_Year_Message').hide(); 
                    var regex = /^\d{4}$/i;
                    var val = $('#DisorderInfo_YearOfDisorder').val();
                    if (!regex.test(val)) {
                        $('#DisorderInfo_YearOfDisorder').addClass('input-validation-error');
                        $('#DisorderInfo_Year_MessageFormat').show();
                        ret = false;
                    }
                    else {
                        $('#EducationInfo_SchoolExitYear').removeClass('input-validation-error');
                        $('#DisorderInfo_Year_MessageFormat').hide();
                    }
                }
                return ret;
            }
            function CheckAttestatRegion() {
                var ret = true;

                if ($('#EducationInfo_SchoolTypeId').val() != 1) {
                    return true;
                }

                $('#EducationInfo_AttestatRegion').removeClass('input-validation-error');
                var val = $('#EducationInfo_AttestatRegion').val();

                if ($('#EducationInfo_SchoolTypeId').val() == 1 && $('#EducationInfo_CountryEducId').val() == 1 && (val == undefined || val == '')) {
                    $('#EducationInfo_AttestatRegion').addClass('input-validation-error');
                    $('#EducationInfo_AttestatRegion_Message').show();
                    return false;
                }

                if (val == undefined || val == '') {
                    return ret;
                }
                var regex = /^\d{2}$/i;
                if (!regex.test(val)) {
                    $('#EducationInfo_AttestatRegion_Message').show();
                    $('#EducationInfo_AttestatRegion').addClass('input-validation-error');
                    ret = false;
                }
                else {
                    $('#EducationInfo_AttestatRegion_Message').hide();
                    $('#EducationInfo_AttestatRegion').removeClass('input-validation-error');
                }
                return ret;
            }

            function UpdateAfterSchooltype() {
                $('#LabelEducationInfo_VuzExitYear').hide();
                $('#LabelEducationInfo_SchoolExitYear').show();
                $('#LabelEducationInfo_SchoolCurName').hide();
                $('#LabelEducationInfo_SchoolName').show();
                $('#LabelEducationInfo_CountryCurEducId').hide();
                $('#LabelEducationInfo_CountryEducId').show();
                var SchoolTypeId = $('#EducationInfo_SchoolTypeId').val();
                if (SchoolTypeId == '1') {
                    $('#_vuzAddType').hide();
                    $('#_schoolExitClass').show();
                    $('#_TransferData').hide();
                    $('#_DisorderInfo').hide();
                    $('#_mainpriem1').show();
                    $('#_mainpriem2').show();
                }
                else if (SchoolTypeId == '4') {
                    $('#_vuzAddType').show();
                    $('#_schoolExitClass').hide();
                    UpdateVuzAddType();
                }
                else {
                    $('#_vuzAddType').hide();
                    $('#_schoolExitClass').hide();
                    $('#_TransferData').hide();
                    $('#_DisorderInfo').hide();
                    $('#_mainpriem1').show();
                    $('#_mainpriem2').show();
                }
            }
            function UpdateVuzAddType() {
                if ($('#EducationInfo_VuzAdditionalTypeId').val() == 2) {
                    $('#_TransferData').show();
                    $('#_TransferHasScholarship').show(); 
                    $('#EducationInfo_SchoolCity').val("Санкт-Петербург");
                    $('#EducationInfo_SchoolName').val("Санкт-Петербургский государственный  университет (СПбГУ)");
                    $('#_AccreditationInfo').hide();
                    $('#_ForeignCountryEduc').hide();
                    $('#_TransferSPBUAddInfo').show();
                    $('#_DisorderInfo').hide();
                    $('#_mainpriem1').hide();
                    $('#_mainpriem2').hide();
                    $('#LabelEducationInfo_VuzExitYear').show();
                    $('#LabelEducationInfo_SchoolExitYear').hide();
                    $('#LabelEducationInfo_SchoolCurName').show();
                    $('#LabelEducationInfo_SchoolName').hide();
                    $('#LabelEducationInfo_CountryCurEducId').show();
                    $('#LabelEducationInfo_CountryEducId').hide();
                }
                else { 
                    if ($('#EducationInfo_VuzAdditionalTypeId').val() == 4) {
                        $('#_TransferData').show();
                        if ($('#EducationInfo_CountryEducId').val() == '193') { $('#_TransferHasScholarship').show(); }
                        else { $('#_TransferHasScholarship').hide(); }
                        if ($('#EducationInfo_SchoolName').val() == "Санкт-Петербургский государственный  университет (СПбГУ)") {
                            $('#_AccreditationInfo').hide(); 
                        }
                        else {
                            $('#_AccreditationInfo').show();
                        } 
                        $('#_TransferSPBUAddInfo').hide();
                        $('#_DisorderInfo').hide();
                        $('#_mainpriem1').hide();
                        $('#_mainpriem2').hide();
                        $('#LabelEducationInfo_VuzExitYear').show();
                        $('#LabelEducationInfo_SchoolExitYear').hide();
                        $('#LabelEducationInfo_SchoolCurName').show();
                        $('#LabelEducationInfo_SchoolName').hide();
                        $('#LabelEducationInfo_CountryCurEducId').show();
                        $('#LabelEducationInfo_CountryEducId').hide();
                    }
                    else {
                        $('#_TransferData').hide();
                        if ($('#EducationInfo_VuzAdditionalTypeId').val() == 4) {
                            $('#EducationInfo_SchoolName').val("Санкт-Петербургский государственный  университет (СПбГУ)");
                            $('#EducationInfo_SchoolCity').val("Санкт-Петербург");
                            $('#LabelEducationInfo_VuzExitYear').show();
                            $('#LabelEducationInfo_SchoolExitYear').hide();
                            $('#LabelEducationInfo_SchoolCurName').show();
                            $('#LabelEducationInfo_SchoolName').hide();
                            $('#_DisorderInfo').show();
                            $('#_mainpriem1').hide();
                            $('#_TransferSPBUAddInfo').hide();
                            $('#_mainpriem2').hide();
                            $('#LabelEducationInfo_CountryCurEducId').show();
                            $('#LabelEducationInfo_CountryEducId').hide();
                        }
                        else
                            if ($('#EducationInfo_VuzAdditionalTypeId').val() == 3) {
                                $('#EducationInfo_SchoolName').val("Санкт-Петербургский государственный  университет (СПбГУ)");
                                $('#EducationInfo_SchoolCity').val("Санкт-Петербург");
                                $('#LabelEducationInfo_VuzExitYear').show();
                                $('#LabelEducationInfo_SchoolExitYear').hide();
                                $('#LabelEducationInfo_SchoolCurName').show();
                                $('#LabelEducationInfo_SchoolName').hide();
                                $('#_DisorderInfo').show();
                                $('#_mainpriem1').hide();
                                $('#_mainpriem2').hide();
                                $('#LabelEducationInfo_CountryCurEducId').show();
                                $('#LabelEducationInfo_CountryEducId').hide();
                            }
                            else { 
                                $('#LabelEducationInfo_SchoolCurName').hide();
                                $('#LabelEducationInfo_SchoolName').show();
                                $('#LabelEducationInfo_VuzExitYear').hide();
                                $('#LabelEducationInfo_SchoolExitYear').show();
                                $('#_DisorderInfo').hide();
                                $('#_mainpriem1').show();
                                $('#_mainpriem2').show();
                                $('#LabelEducationInfo_CountryCurEducId').hide();
                                $('#LabelEducationInfo_CountryEducId').show();
                            }
                    }
                }
                $('#_CountryEduc').show();
                $('#_regionEduc').show();
                if ($('#EducationInfo_CountryEducId').val() != "193"){
                    $('#_regionEduc').hide();
                }
                if (($('#EducationInfo_VuzAdditionalTypeId').val() == 2)||($('#EducationInfo_VuzAdditionalTypeId').val() == 3)){
                    $('#_CountryEduc').hide();
                    $('#_regionEduc').hide();
                }
            }

            $(function () {
                fStartOne();
                fStartTwo();
                UpdateAfterSchooltype();
                $('#EducationInfo_AttestatRegion').keyup(function () { setTimeout(CheckAttestatRegion); });
                $('#EducationInfo_AttestatRegion').blur(function () { setTimeout(CheckAttestatRegion); });
                $('#EducationInfo_SchoolExitYear').keyup(function () { setTimeout(CheckSchoolExitYear); });
                $('#EducationInfo_SchoolExitYear').blur(function () { setTimeout(CheckSchoolExitYear); });
                $('#EducationInfo_SchoolName').keyup(function () { setTimeout(CheckSchoolName); });
                $('#EducationInfo_SchoolName').change(function () { setTimeout(CheckSchoolName); });
                $('#EducationInfo_SchoolName').blur(function () { setTimeout(CheckSchoolName); });
                $('#EducationInfo_RegionEducId').change(function () { setTimeout(GetCities); });
                $('#EducationInfo_SchoolTypeId').change(function () { setTimeout(UpdateAfterSchooltype) });
                $('#EducationInfo_VuzAdditionalTypeId').change(function () { setTimeout(UpdateVuzAddType) });
                $('#DisorderInfo_YearOfDisorder').change(function () { setTimeout(CheckDisorderInfoYear) });
                $('#CurrentEducation_StudyLevelId').change(function () { setTimeout(GetProfessions) });
                $('#CurrentEducation_StudyFormId').change(function () { setTimeout(GetProfessions) });
                $('#CurrentEducation_StudyBasisId').change(function () { setTimeout(GetProfessions) });
                $('#CurrentEducation_SemesterId').change(function () { setTimeout(GetProfessions) });
                $('#CurrentEducation_LicenseProgramId').change(function () { setTimeout(Myfun) });
                $('#CurrentEducation_ObrazProgramId').change(function () { setTimeout(Myfun) });
            });
            

            function fStartOne() {
                LoadAutoCompleteValues();
                updateRegionEduc();
                updateForeignCountryEduc();
                if ($('#EducationInfo_SchoolTypeId').val() != 4) {
                    $('#HEData').hide();
                    $('#EGEData').show();
                    $('#_AttRegion').show();
                }
                else {
                    $('#HEData').show();
                    $('#EGEData').hide();
                    $('#_AttRegion').hide();
                }

                if ($('#EducationInfo_SchoolTypeId').val() == 1) {
                    $('#_SchoolNumber').show();
                }
                else {
                    $('#_SchoolNumber').hide();
                }

                function updateRegionEduc() {
                    if ($('#EducationInfo_CountryEducId').val() == '193') {
                        $('#_regionEduc').show();
                    }
                    else {
                        $('#_regionEduc').hide();
                        if ($('#EducationInfo_SchoolName').val() == "Санкт-Петербургский государственный  университет (СПбГУ)") {
                            $('#EducationInfo_SchoolName').val('');
                            $('#EducationInfo_SchoolCity').val('');
                        }
                    }
                    UpdateAfterSchooltype();
                }
                function updateForeignCountryEduc() {
                    if ($('#EducationInfo_CountryEducId').val() != '193') {
                        $('#_ForeignCountryEduc').show();
                    }
                    else {
                        $('#_ForeignCountryEduc').hide();
                    }
                }

                $('#EducationInfo_SchoolTypeId').change(function changeTbls() {
                    if ($('#EducationInfo_SchoolTypeId').val() != 4) {
                        $('#HEData').hide();
                        $('#EGEData').show();
                        LoadAutoCompleteValues();
                    }
                    else {
                        $('#HEData').show();
                        $('#EGEData').hide();
                        LoadAutoCompleteValues();
                    }
                    if ($('#EducationInfo_SchoolTypeId').val() == 1) {
                        $('#_AttRegion').show();
                        $('#_SchoolNumber').show();
                    }
                    else {
                        $('#_AttRegion').hide();
                        $('#_SchoolNumber').hide();
                    }
                });
                $('#EducationInfo_CountryEducId').change(updateRegionEduc);
                $('#EducationInfo_CountryEducId').change(updateForeignCountryEduc);

                var cachedVuzNames = false;
                var VuzNamesCache;
                var EmptySource = [];
                function LoadAutoCompleteValues() {
                    var vals = new Object();
                    vals["schoolType"] = 4//$('#EducationInfo_SchoolTypeId').val();
                    if (!cachedVuzNames) {
                        $.post('/AbiturientNew/LoadVuzNames', vals, function (res) {
                            if (res.IsOk) {
                                VuzNamesCache = res.Values;
                                cachedVuzNames = true;
                                if ($('#EducationInfo_SchoolTypeId').val() == 4) {
                                    $('#EducationInfo_SchoolName').autocomplete({
                                        source: res.Values
                                    });
                                }
                                else {
                                    $('#EducationInfo_SchoolName').autocomplete({
                                        source: EmptySource
                                    });
                                }
                            }
                        }, 'json');
                    }
                    else {
                        if ($('#EducationInfo_SchoolTypeId').val() == 4) {
                            $('#EducationInfo_SchoolName').autocomplete({
                                source: VuzNamesCache
                            });
                        }
                        else {
                            $('#EducationInfo_SchoolName').autocomplete({
                                source: EmptySource
                            });
                        }
                    }
                }

                $('#EducationInfo_CountryEducId').change(function () {
                    if ($('#EducationInfo_CountryEducId').val() != 6) {
                        $('#CountryMessage').hide();
                    }
                    else {
                        $('#CountryMessage').show();
                    }
                });
            }
        </script>
        <script type="text/javascript">
            function updateIs2014() {
                if ($("#Is2014").is(':checked')) {
                    $('#EgeCert').attr('disabled', 'disabled');
                }
                else {
                    $('#EgeCert').removeAttr('disabled');
                }
            }
            function updateIsSecondWave() {
                if ( ($("#IsInUniversity").is(':checked')) || ($("#IsSecondWave").is(':checked'))) {
                    $('#EgeCert').attr('disabled', 'disabled');
                    $('#_EgeMark').hide();
                }
                else { 
                     if ($("#Is2014").is(':checked')) {
                        $('#EgeCert').attr('disabled', 'disabled');
                    }
                    else {
                        $('#EgeCert').removeAttr('disabled');
                    }
                    $('#_EgeMark').show();  
                }
            }
            function updateIsInUniversity() {
                if ( ($("#IsInUniversity").is(':checked')) || ($("#IsSecondWave").is(':checked'))) {
                    $('#EgeCert').attr('disabled', 'disabled');
                    $('#_EgeMark').hide();
                }
                else { 
                     if ($("#Is2014").is(':checked')) {
                        $('#EgeCert').attr('disabled', 'disabled');
                    }
                    else {
                        $('#EgeCert').removeAttr('disabled');
                    }
                    $('#_EgeMark').show();  
                }
            }

            function fStartTwo() {
                $("#dialog:ui-dialog").dialog("destroy");
                $('form').submit(function () {
                    return CheckForm();
                });

                <% if (!Model.Enabled)
                   { %>
                $('input').attr('readonly', 'readonly');
                $('select').attr('disabled', 'disabled');
                <% } %>

                <% if (Model.Enabled)
                 { %>
                    $("#CurrentEducation_AccreditationDate").datepicker({
                        changeMonth: true,
                        changeYear: true,
                        showOn: "focus",
                        yearRange: '1920:2014',
                        defaultDate: '-1y',
                    });
                    $.datepicker.regional["ru"];
                <% } %> 
                
                function loadFormValues() {
                    var existingCerts = '';
                    var exams_html = '';
                    $.getJSON("AbiturientNew/GetAbitCertsAndExams", null, function (res) {
                        existingCerts = res.Certs;
                        for (var i = 0; i < res.Exams.length; i++) {
                            exams_html += '<option value="' + res.Exams[i].Key + '">' + res.Exams[i].Value + '</option>';
                        }
                        $("#EgeExam").html(exams_html);
                        $("#EgeCert").autocomplete({
                            source: existingCerts
                        });
                    });
                }

                var certificateNumber = $("#EgeCert"),
			    examName = $("#EgeExam"),
			    examMark = $("#EgeMark"),
                Is2014 = $("#Is2014"),
                IsSecondWave = $("#IsSecondWave"),
                IsInUniversity = $("#IsInUniversity"),

			    allFields = $([]).add(certificateNumber).add(examName).add(examMark),
			    tips = $(".validateTips");

                function updateTips(t) {
                    tips.text(t).addClass("ui-state-highlight");
                    setTimeout(function () {
                        tips.removeClass("ui-state-highlight", 1500);
                    }, 500);
                }
                function checkLength() {
                    if ((certificateNumber.val().length > 15 || certificateNumber.val().length < 15) && (!$("#Is2014").is(':checked'))) {
                        certificateNumber.addClass("ui-state-error");
                        updateTips("Номер сертификата должен быть 15-значным в формате РР-ХХХХХХХХ-ГГ");
                        return false;
                    } else {
                        return true;
                    }
                }
                function checkVal() {
                    var val = examMark.val();
                    if ((val < 1 || val > 100) && (!$("#IsSecondWave").is(':checked')) && (!$("#IsInUniversity").is(':checked'))) {
                        updateTips("Экзаменационный балл должен быть от 1 до 100");
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                function checkRegexp(o, regexp, n) {
                    if (!(regexp.test(o.val()))) {
                        o.addClass("ui-state-error");
                        updateTips(n);
                        return false;
                    } else {
                        return true;
                    }
                }
                

                $("#dialog-form").dialog({
                    autoOpen: false,
                    height: 430,
                    width: 350,
                    modal: true,
                    buttons: {
                        "Добавить": function () {
                            var bValid = true;
                            allFields.removeClass("ui-state-error");

                            bValid = bValid && checkLength();
                            bValid = bValid && checkVal();

                            if (bValid) {
                                //add to DB
                                var parm = new Object();
                                parm["certNumber"] = certificateNumber.val();
                                parm["examName"] = examName.val();
                                parm["examValue"] = examMark.val();
                                parm["Is2014"] = $("#Is2014").is(':checked');
                                parm["IsInUniversity"] = $("#IsInUniversity").is(':checked');
                                parm["IsSecondWave"] = $("#IsSecondWave").is(':checked');

                                $.post("AbiturientNew/AddMark", parm, function (res) {
                                    //add to table if ok
                                    if (res.IsOk) {
                                        $("#tblEGEData tbody").append('<tr id="' + res.Data.Id + '">' +
							            '<td>' + res.Data.CertificateNumber + '</td>' +
							            '<td>' + res.Data.ExamName + '</td>' +
							            '<td>' + res.Data.ExamMark + '</td>' +
                                        '<td><span class="link" onclick="DeleteMrk(\'' + res.Data.Id + '\')"><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить оценку" /></span></td>' +
						                '</tr>');
                                        $("#noMarks").html("").hide();
                                        $("#dialog-form").dialog("close");
                                    }
                                    else {
                                        updateTips(res.ErrorMessage);
                                    }
                                }, "json");
                            }
                        },
                        "Отменить": function () {
                            $(this).dialog("close");
                        }
                    },
                    close: function () {
                        allFields.val("").removeClass("ui-state-error");
                    }
                });

                $("#create-ege").button().click(function () {
                    loadFormValues();
                    $("#dialog-form").dialog("open");
                });
            }
            function DeleteMrk(id) {
                var data = new Object();
                data['mId'] = id;
                $.post("AbiturientNew/DeleteEgeMark", data, function r(res) {
                    if (res.IsOk) {
                        $("#" + id.toString()).html('').hide();
                    }
                    else {
                        alert("Ошибка при удалении оценки:\n" + res.ErrorMsg);
                    }
                }, 'json');
            } 
	</script>
        <div class="grid">
            <div class="wrapper">
                <div class="grid_4 first">
                    <% if (!Model.Enabled)
                       { %>
                        <div id="Message" class="message warning">
                            <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                        </div>
                    <% } %>
                    <form id="form" class="form panel" action="AbiturientNew/NextStep" method="post" onsubmit="return CheckForm();">
                        <h3><%= GetGlobalResourceObject("PersonalOffice_Step4", "EducationHeader")%></h3>
                        <hr />
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <fieldset><br />
                        <div class="clearfix">
                            <label for="EducationInfo_SchoolTypeId" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                            <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step4, SchoolTypeId %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                            </label> 
                            <%= Html.DropDownListFor(x => x.EducationInfo.SchoolTypeId, Model.EducationInfo.SchoolTypeList) %>
                        </div>

                        <div id="_vuzAddType" class="clearfix" style="display:none">
                            <div class="clearfix">
                                <label for="EducationInfo_VuzAdditionalTypeId" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                                <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step4, VuzAdditionalTypeId %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                                </label> 
                                <%= Html.DropDownListFor(x => x.EducationInfo.VuzAdditionalTypeId, Model.EducationInfo.VuzAdditionalTypeList) %>
                            </div>
                        </div>
                        <div id="_schoolExitClass" class="clearfix" style="display:none">
                            <div class="clearfix">
                                <label for="EducationInfo_SchoolExitClassId" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                                <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step4, SchoolExitClass %>"></asp:Literal><asp:Literal  runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                                </label>
                                <%= Html.DropDownListFor(x => x.EducationInfo.SchoolExitClassId, Model.EducationInfo.SchoolExitClassList) %>
                            </div>
                        </div>
                        
                        <div class="clearfix" id="_CountryEduc">
                            <label id="LabelEducationInfo_CountryCurEducId" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                                <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:PersonalOffice_Step4, CountryCurEducId %>"></asp:Literal><asp:Literal ID="Literal2" runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                            </label> 
                            <label id="LabelEducationInfo_CountryEducId" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                                <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step4, CountryEducId %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                            </label> 
                            <%= Html.DropDownListFor(x => x.EducationInfo.CountryEducId, Model.EducationInfo.CountryList) %>
                        </div>
                        <div id="_regionEduc" class="clearfix">
                            <div class="clearfix">
                                <label for="EducationInfo_RegionEducId" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                                <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step4, RegionEducId %>"></asp:Literal><asp:Literal  runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                                </label>  
                                <%= Html.DropDownListFor(x => x.EducationInfo.RegionEducId, Model.EducationInfo.RegionList) %>
                            </div>
                        </div>
                        <div class="clearfix">
                            <label id="LabelEducationInfo_SchoolCurName" style="display:none;" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                                <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step4, SchoolCurName %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                            </label>  
                            <label id="LabelEducationInfo_SchoolName" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                                <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step4, SchoolName %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                            </label>  
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolName)%>
                            <br /><p></p>
                            <span id="EducationInfo_SchoolName_Message" class="Red" style="display:none">  <%= GetGlobalResourceObject("PersonalOffice_Step4", "EducationInfo_SchoolName_Message").ToString()%> </span>
                        </div>
                        
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolCity, GetGlobalResourceObject("PersonalOffice_Step4", "ResidentialPlace").ToString())%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolCity) %>
                        </div>
                        <div class="clearfix">
                            <label id="LabelEducationInfo_VuzExitYear" style="display: none;" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                                <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step4, VuzExitYear %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                            </label>
                            <label id="LabelEducationInfo_SchoolExitYear" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                                <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step4, SchoolExitYear %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                            </label>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolExitYear)%>
                            <br /><p></p>
                            <span id="EducationInfo_SchoolExitYear_Message" class="Red" style="display:none; border-collapse:collapse;"><%=GetGlobalResourceObject("PersonalOffice_Step4", "SchoolExitYear_Message").ToString()%></span>
                            <span id="EducationInfo_SchoolExitYear_MessageFormat" class="Red" style="display:none; border-collapse:collapse;"><%=GetGlobalResourceObject("PersonalOffice_Step4", "EducationInfo_SchoolExitYear_MessageFormat").ToString()%></span>
                        </div>
                        <div id="_mainpriem1">
                            <div id="_SchoolNumber" class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.SchoolNumber, GetGlobalResourceObject("PersonalOffice_Step4", "SchoolNumber").ToString())%>
                                <%= Html.TextBoxFor(x => x.EducationInfo.SchoolNumber) %>
                            </div>
                            <div id="AvgMark" class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.AvgMark, GetGlobalResourceObject("PersonalOffice_Step4", "AvgMark").ToString())%>
                                <%= Html.TextBoxFor(x => x.EducationInfo.AvgMark) %>
                            </div>
                            <div id="_IsExcellent" class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.IsExcellent, GetGlobalResourceObject("PersonalOffice_Step4", "RedDiploma").ToString())%>
                                <%= Html.CheckBoxFor(x => x.EducationInfo.IsExcellent)%>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.LanguageId, GetGlobalResourceObject("PersonalOffice_Step4", "LanguageId").ToString())%>
                                <%= Html.DropDownListFor(x => x.EducationInfo.LanguageId, Model.EducationInfo.LanguageList) %>
                            </div>
                            <div id="EnglishMark" class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.EnglishMark, GetGlobalResourceObject("PersonalOffice_Step4", "EnglishMark").ToString())%>
                                <%= Html.TextBoxFor(x => x.EducationInfo.EnglishMark) %>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.StartEnglish, GetGlobalResourceObject("PersonalOffice_Step4", "EnglishNull").ToString())%>
                                <%= Html.CheckBoxFor(x => x.EducationInfo.StartEnglish)%>
                            </div>
                            <div id = "_EducationDocument" >
                                <h4><%=GetGlobalResourceObject("PersonalOffice_Step4", "EducationDocumentHeader").ToString()%></h4>
                                <hr />
                                <div id="_AttRegion" class="clearfix" style="display:none">
                                    <%= Html.LabelFor(x => x.EducationInfo.AttestatRegion, GetGlobalResourceObject("PersonalOffice_Step4", "AttestatRegion").ToString())%>
                                    <%= Html.TextBoxFor(x => x.EducationInfo.AttestatRegion) %>
                                    <span id="EducationInfo_AttestatRegion_Message" class="Red" style="display:none; border-collapse:collapse;"><%=GetGlobalResourceObject("PersonalOffice_Step4", "AttestatRegion_Message").ToString()%> </span>
                                </div>
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.EducationInfo.DiplomSeries, GetGlobalResourceObject("PersonalOffice_Step4", "DiplomSeries").ToString())%>
                                    <%= Html.TextBoxFor(x => x.EducationInfo.DiplomSeries) %>
                                    <br /><p></p>
                                    <span id="EducationInfo_DiplomSeries_Message" class="Red" style="display:none"><%=GetGlobalResourceObject("PersonalOffice_Step4", "DiplomSeries_Message").ToString()%></span>
                                </div>
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.EducationInfo.DiplomNumber, GetGlobalResourceObject("PersonalOffice_Step4", "DiplomNumber").ToString())%>
                                    <%= Html.TextBoxFor(x => x.EducationInfo.DiplomNumber)%>
                                    <br /><p></p>
                                    <span id="EducationInfo_DiplomNumber_Message" class="Red" style="display:none"><%=GetGlobalResourceObject("PersonalOffice_Step4", "DiplomNumber_Message").ToString()%></span>
                                </div>
                            </div>
                        </div>
                        <div id="_ForeignCountryEduc" class="clearfix" style="display:none">
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.IsEqual, GetGlobalResourceObject("PersonalOffice_Step4", "IsEqual").ToString())%>
                                <%= Html.CheckBoxFor(x => x.EducationInfo.IsEqual)%>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.EqualityDocumentNumber, GetGlobalResourceObject("PersonalOffice_Step4", "EqualityDocumentNumber").ToString())%>
                                <%= Html.TextBoxFor(x => x.EducationInfo.EqualityDocumentNumber) %>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.HasTRKI, GetGlobalResourceObject("PersonalOffice_Step4", "HasTRKI").ToString())%>
                                <%= Html.CheckBoxFor(x => x.EducationInfo.HasTRKI)%>
                            </div>
                            <div class="clearfix" id="TRKI">
                                <%= Html.LabelFor(x => x.EducationInfo.TRKICertificateNumber, GetGlobalResourceObject("PersonalOffice_Step4", "TRKICertificateNumber").ToString())%>
                                <%= Html.TextBoxFor(x => x.EducationInfo.TRKICertificateNumber) %>
                            </div>
                        </div>
                        <div id="_mainpriem2">
                            <div id="HEData">
                                <h4><% =GetGlobalResourceObject("PersonalOffice_Step4", "HEDataHeader").ToString()%></h4>
                                <hr />
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.EducationInfo.ProgramName, GetGlobalResourceObject("PersonalOffice_Step4", "PersonSpecialization").ToString())%>
                                    <%= Html.TextBoxFor(x => x.EducationInfo.ProgramName)%>
                                </div>
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.EducationInfo.PersonStudyForm, GetGlobalResourceObject("PersonalOffice_Step4", "PersonStudyForm").ToString())%>
                                    <%= Html.DropDownListFor(x => x.EducationInfo.PersonStudyForm, Model.EducationInfo.StudyFormList) %>
                                </div>
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.EducationInfo.PersonQualification, GetGlobalResourceObject("PersonalOffice_Step4", "PersonQualification").ToString())%>
                                    <%= Html.DropDownListFor(x => x.EducationInfo.PersonQualification, Model.EducationInfo.QualificationList) %>
                                </div>
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.EducationInfo.DiplomTheme, GetGlobalResourceObject("PersonalOffice_Step4", "DiplomTheme").ToString())%>
                                    <%= Html.TextBoxFor(x => x.EducationInfo.DiplomTheme) %>
                                </div>
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.EducationInfo.HEEntryYear, GetGlobalResourceObject("PersonalOffice_Step4", "HEEntryYear").ToString())%>
                                    <%= Html.TextBoxFor(x => x.EducationInfo.HEEntryYear) %>
                                </div> 
                            </div>
                            <div id="EGEData" class="clearfix">
                                <h6><%=GetGlobalResourceObject("PersonalOffice_Step4", "EGEmarks").ToString()%></h6>
                                <% if (Model.EducationInfo.EgeMarks.Count == 0)
                                   { 
                                %>
                                    <h6 id="noMarks"><%=GetGlobalResourceObject("PersonalOffice_Step4", "EGEnomarks").ToString()%></h6>
                                <%
                                   }
                                   else
                                   {
                                %>
                                <table id="tblEGEData" class="paginate" style="width:400px">
                                    <thead>
                                    <tr>
                                        <th><%=GetGlobalResourceObject("PersonalOffice_Step4", "EGEsert").ToString()%></th>
                                        <th><%=GetGlobalResourceObject("PersonalOffice_Step4", "EGEsubject").ToString()%></th>
                                        <th><%=GetGlobalResourceObject("PersonalOffice_Step4", "EGEball").ToString()%></th>
                                        <th></th>
                                    </tr>
                                    </thead>
                                    <tbody>
                                <%
                                       foreach (var mark in Model.EducationInfo.EgeMarks)
                                       {
                                %>
                                    <tr id="<%= mark.Id.ToString() %>">
                                        <td><span><%= mark.CertificateNum%></span></td>
                                        <td><span><%= mark.ExamName%></span></td>
                                        <td><span><%= mark.Value%></span></td>
                                        <td><%= Html.Raw("<span class=\"link\" onclick=\"DeleteMrk('" + mark.Id.ToString() + "')\"><img src=\"../../Content/themes/base/images/delete-icon.png\" alt=\"Удалить оценку\" /></span>")%></td>
                                    </tr>
                                <%
                                       }
                                %>
                                    </tbody>
                                </table>
                                <% } %>
                                <br />
                                <button type="button" id="create-ege" class="button button-blue"><%=GetGlobalResourceObject("PersonalOffice_Step4", "AddMark").ToString()%></button>
                                <div id="dialog-form">
                                    <p id="validation_info">Все поля обязательны для заполнения</p>
	                                <hr />
                                    <fieldset>
                                        <div id="_CertNum" class="clearfix">
                                            <label for="EgeCert"><%=GetGlobalResourceObject("PersonalOffice_Step4", "EGEsert").ToString()%></label><br />
		                                    <input type="text" id="EgeCert" disabled="disabled"/><br />
                                        </div>
                                        <div class="clearfix">
                                            <label for="Is2014"><%=GetGlobalResourceObject("PersonalOffice_Step4", "CurrentYear").ToString()%></label><br />
		                                    <input type="checkbox" id="Is2014" checked="checked" onchange="updateIs2014()" /><br />
                                        </div>
                                        <div class="clearfix">
                                            <label for="EgeExam"><%=GetGlobalResourceObject("PersonalOffice_Step4", "EGEsubject").ToString()%></label><br />
		                                    <select id="EgeExam" ></select><br />
                                        </div>
                                        <div id="_EgeMark" class="clearfix" >
                                            <label for="EgeMark"><%=GetGlobalResourceObject("PersonalOffice_Step4", "EGEball").ToString()%></label><br />
		                                    <input type="text" id="EgeMark" value="" /><br />
                                        </div>
                                        <div id="_IsSecondWave" class="clearfix" >
                                            <label for="IsSecondWave"><%=GetGlobalResourceObject("PersonalOffice_Step4", "SecondWave").ToString()%></label><br />
		                                    <input type="checkbox" id="IsSecondWave" onchange="updateIsSecondWave()" /><br />
                                        </div>
                                        <br />
                                        <div id="_IsInUniversity" class="clearfix">
                                            <label for="IsInUniversity"><%=GetGlobalResourceObject("PersonalOffice_Step4", "PassInSPbSU").ToString()%></label><br />
		                                    <input type="checkbox" id="IsInUniversity" onchange="updateIsInUniversity()" /><br />
                                        </div>
	                                </fieldset>
                                </div>
                            </div>
                        </div>
                        <div id="_TransferData" class="clearfix" style="display:none">
                            <div id = "_AccreditationInfo">
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.CurrentEducation.HasAccreditation, "Имеет гос. аккредитацию")%>
                                    <%= Html.CheckBoxFor(x => x.CurrentEducation.HasAccreditation)%>
                                </div>
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.CurrentEducation.AccreditationNumber, "Номер аккредитации")%>
                                    <%= Html.TextBoxFor(x => x.CurrentEducation.AccreditationNumber)%>
                                </div>
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.CurrentEducation.AccreditationDate, "Дата аккредитации") %>
                                    <%= Html.TextBoxFor(x => x.CurrentEducation.AccreditationDate)%> 
                                </div>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.CurrentEducation.StudyLevelId, "Уровень обучения")%>
                                <%= Html.DropDownListFor(x => x.CurrentEducation.StudyLevelId, Model.CurrentEducation.StudyLevelList, new SortedList<string, object>() { })%>
                            </div>  
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.CurrentEducation.SemesterId, "Семестр")%>
                                <%= Html.DropDownListFor(x => x.CurrentEducation.SemesterId, Model.CurrentEducation.SemesterList, new SortedList<string, object>() {  })%>
                            </div>
                            <div id="_TransferSPBUAddInfo" class="clearfix" >
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.CurrentEducation.StudyFormId, "Форма обучения")%>
                                    <%= Html.DropDownListFor(x => x.CurrentEducation.StudyFormId, Model.EducationInfo.StudyFormList, new SortedList<string, object>() {  })%>
                                </div>
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.CurrentEducation.StudyBasisId, "Основа обучения") %>
                                    <%= Html.DropDownListFor(x => x.CurrentEducation.StudyBasisId, Model.EducationInfo.StudyBasisList, new SortedList<string, object>() { })%>
                                </div>
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.CurrentEducation.LicenseProgramId, "Текущее направление (специальность)") %>
                                    <%= Html.HiddenFor(x=> x.CurrentEducation.HiddenLicenseProgramId) %>
                                    <select id="CurrentEducation_LicenseProgramId" size="12" name="lProfession" style="width:459px;" onchange="GetObrazPrograms()"></select> 
                                </div>

                                <div class="clearfix" id="_ObrazProg" <% if (String.IsNullOrEmpty(Model.CurrentEducation.HiddenObrazProgramId)) { %>style="display:none;"<% } %>>
                                    <%= Html.LabelFor(x => x.CurrentEducation.ObrazProgramId, "Текущая образовательная программа")%>
                                    <%= Html.HiddenFor(x => x.CurrentEducation.HiddenObrazProgramId)%>
                                    <select id="CurrentEducation_ObrazProgramId"  <%if (!Model.Enabled){ %> disabled="true"<%} %> size="5" name="CurObrazProgram" style="width:459px;"></select>
                                </div>
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.CurrentEducation.ProfileName, "Текущий профиль (специализация)")%>
                                    <%= Html.TextBoxFor(x => x.CurrentEducation.ProfileName)%>
                                </div>
                            </div>
                            <div id="_TransferHasScholarship">
                                <div class="clearfix">
                                    <%= Html.LabelFor(x => x.CurrentEducation.HasScholarship, "Получаю академическую стипендию") %>
                                    <%= Html.CheckBoxFor(x => x.CurrentEducation.HasScholarship)%>
                                </div>
                            </div>
                        </div>
                        <div id = "_DisorderInfo" style="display:none;">
                            <h3>Данные об отчислении из СПбГУ</h3>
                            <hr /> 
                            <fieldset><br />
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.DisorderInfo.YearOfDisorder, "Год") %>
                                <%= Html.TextBoxFor(x => x.DisorderInfo.YearOfDisorder)%>
                                <br /><p></p>
                                <span id="DisorderInfo_Year_Message" class="Red" style="display:none; border-collapse:collapse;"><%=GetGlobalResourceObject("PersonalOffice_Step4", "SchoolExitYear_Message").ToString()%></span>
                                <span id="DisorderInfo_Year_MessageFormat" class="Red" style="display:none; border-collapse:collapse;"><%=GetGlobalResourceObject("PersonalOffice_Step4", "EducationInfo_SchoolExitYear_MessageFormat").ToString()%></span>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.DisorderInfo.EducationProgramName, "Название образовательной программы")%>
                                <%= Html.TextBoxFor(x => x.DisorderInfo.EducationProgramName)%>
                                <br /> 
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.DisorderInfo.IsForIGA, "Восстанавливаюсь для ИГА")%>
                                <%= Html.CheckBoxFor(x => x.DisorderInfo.IsForIGA)%>
                                <br /> 
                            </div>
                            </fieldset>
                        </div>
                        </fieldset>
                        <hr />
                        <div class="clearfix">
                            <input id="Submit3" type="submit" class="button button-green" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                        </div>
                    </form>
                </div>
                <div class="grid_2">
                    <ol>
                        <li><a href="../../AbiturientNew?step=1"><%= GetGlobalResourceObject("PersonInfo", "Step1")%></a></li>
                        <li><a href="../../AbiturientNew?step=2"><%= GetGlobalResourceObject("PersonInfo", "Step2")%></a></li>
                        <li><a href="../../AbiturientNew?step=3"><%= GetGlobalResourceObject("PersonInfo", "Step3")%></a></li>
                        <li><a href="../../AbiturientNew?step=4"><%= GetGlobalResourceObject("PersonInfo", "Step4")%></a></li>
                        <li><a href="../../AbiturientNew?step=5"><%= GetGlobalResourceObject("PersonInfo", "Step5")%></a></li>
                        <li><a href="../../AbiturientNew?step=6"><%= GetGlobalResourceObject("PersonInfo", "Step6")%></a></li>
                    </ol>
                </div>
            </div>
        </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderScriptsContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Анкета</h2>
</asp:Content>
