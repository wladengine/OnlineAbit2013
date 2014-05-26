<%@ Import Namespace="OnlineAbit2013" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Abiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.PersonalOffice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Личный кабинет
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script>
    $('#UILink').hide();
</script>
<% if (1 == 0)/* типа затычка, чтобы VS видела скрипты */
   {
%>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
<% } %>
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
                return ret;
            }
            function CheckSchoolExitYear() {
                var ret = true;
                if ($('#EducationInfo_SchoolExitYear').val() == '') {
                    ret = false;
                    $('#EducationInfo_SchoolExitYear').addClass('input-validation-error');
                    $('#EducationInfo_SchoolExitYear_Message').show();
                }
                else {
                    $('#EducationInfo_SchoolExitYear').removeClass('input-validation-error');
                    $('#EducationInfo_SchoolExitYear_Message').hide();
                }
                var regex = /^\d{4}$/i;
                var val = $('#EducationInfo_SchoolExitYear').val();
                if (!regex.test(val)) {
                    $('#EducationInfo_SchoolExitYear_MessageFormat').show();
                    ret = false;
                }
                else {
                    $('#EducationInfo_SchoolExitYear_MessageFormat').hide();
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
            $(function () {
                fStartOne();
                fStartTwo();

                $('#EducationInfo_AttestatRegion').keyup(function () { setTimeout(CheckAttestatRegion); });
                $('#EducationInfo_AttestatRegion').blur(function () { setTimeout(CheckAttestatRegion); });
                $('#EducationInfo_SchoolExitYear').keyup(function () { setTimeout(CheckSchoolExitYear); });
                $('#EducationInfo_SchoolExitYear').blur(function () { setTimeout(CheckSchoolExitYear); });
                $('#EducationInfo_SchoolName').keyup(function () { setTimeout(CheckSchoolName); });
                $('#EducationInfo_SchoolName').blur(function () { setTimeout(CheckSchoolName); });
            });

            function fStartOne() {
                LoadAutoCompleteValues();
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

                var cachedVuzNames = false;
                var VuzNamesCache;
                var EmptySource = [];
                function LoadAutoCompleteValues() {
                    var vals = new Object();
                    vals["schoolType"] = 4//$('#EducationInfo_SchoolTypeId').val();
                    if (!cachedVuzNames) {
                        $.post('/Abiturient/LoadVuzNames', vals, function (res) {
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
                function loadFormValues() {
                    var existingCerts = '';
                    var exams_html = '';
                    $.getJSON("Abiturient/GetAbitCertsAndExams", null, function (res) {
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
			    allFields = $([]).add(certificateNumber).add(examName).add(examMark),
			    tips = $(".validateTips");

                function updateTips(t) {
                    tips
				.text(t)
				.addClass("ui-state-highlight");
                    setTimeout(function () {
                        tips.removeClass("ui-state-highlight", 1500);
                    }, 500);
                }
                function checkLength() {
                    if (certificateNumber.val().length > 15 || certificateNumber.val().length < 15) {
                        certificateNumber.addClass("ui-state-error");
                        updateTips("Номер сертификата должен быть 15-значным в формате РР-ХХХХХХХХ-ГГ");
                        return false;
                    } else {
                        return true;
                    }
                }
                function checkVal() {
                    var val = examMark.val();
                    if (val < 1 || val > 100) {
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
                    height: 400,
                    width: 350,
                    modal: true,
                    buttons: {
                        "Добавить": function () {
                            var bValid = true;
                            allFields.removeClass("ui-state-error");

                            bValid = bValid && checkLength();
                            bValid = bValid && checkVal();
                            bValid = bValid && checkRegexp(certificateNumber, /^\d{2}\-\d{9}\-\d{2}$/i, "Номер сертификата должен быть 15-значным в формате РР-ХХХХХХХХХ-ГГ");

                            if (bValid) {
                                //add to DB
                                var parm = new Object();
                                parm["certNumber"] = certificateNumber.val();
                                parm["examName"] = examName.val();
                                parm["examValue"] = examMark.val();
                                $.post("Abiturient/AddMark", parm, function (res) {
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
                $.post("Abiturient/DeleteEgeMark", data, function r(res) {
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
                    <form id="form" class="form panel" action="Abiturient/NextStep" method="post" onsubmit="return CheckForm();">
                        <h3>Данные об образовании</h3>
                        <hr />
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <fieldset><br />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolTypeId, GetGlobalResourceObject("PersonalOffice_Step4", "SchoolTypeId").ToString())%>
                            <%= Html.DropDownListFor(x => x.EducationInfo.SchoolTypeId, Model.EducationInfo.SchoolTypeList) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolName, GetGlobalResourceObject("PersonalOffice_Step4", "SchoolName").ToString())%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolName)%>
                            <br />
                            <span id="EducationInfo_SchoolName_Message" class="Red" style="display:none">Укажите название образовательного учреждения</span>
                        </div>
                        <div id="_SchoolNumber" class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolNumber, "Номер школы") %>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolNumber) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolCity, "Населённый пункт") %>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolCity) %>
                        </div>
                        <div class="clearfix" style="display:none">
                            
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolExitYear, GetGlobalResourceObject("EducationInfo", "SchoolExitYear").ToString())%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolExitYear)%>
                            <br />
                            <span id="EducationInfo_SchoolExitYear_Message" class="Red" style="display:none; border-collapse:collapse;">Укажите год окончания обучения</span>
                            <span id="EducationInfo_SchoolExitYear_MessageFormat" class="Red" style="display:none; border-collapse:collapse;">Укажите год в 4-значном формате</span>
                        </div>
                        <div id="AvgMark" class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.AvgMark, GetGlobalResourceObject("PersonalOffice_Step4", "AvgMark").ToString())%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.AvgMark) %>
                        </div>
                        <div id="_IsExcellent" class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.IsExcellent, "Медалист (красный диплом)") %>
                            <%= Html.CheckBoxFor(x => x.EducationInfo.IsExcellent)%>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.CountryEducId, GetGlobalResourceObject("PersonalOffice_Step4", "CountryEducId").ToString())%>
                            <%= Html.DropDownListFor(x => x.EducationInfo.CountryEducId, Model.EducationInfo.CountryList) %>
                        </div>
                        <div id="CountryMessage" class="message info" style="display:none; border-collapse:collapse;">
                            Пожалуйста, укажите в названии ВУЗа страну, где Вы обучались (например, "Oxford, UK", "Oberwolfach, Germany")
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.LanguageId, GetGlobalResourceObject("PersonalOffice_Step4", "LanguageId").ToString())%>
                            <%= Html.DropDownListFor(x => x.EducationInfo.LanguageId, Model.EducationInfo.LanguageList) %>
                        </div>
                        <div id="EnglishMark" class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.EnglishMark, "Итоговая оценка по английскому языку (если изучался)") %>
                            <%= Html.TextBoxFor(x => x.EducationInfo.EnglishMark) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.StartEnglish, "Желаю изучать английский в СПбГУ 'с нуля'")%>
                            <%= Html.CheckBoxFor(x => x.EducationInfo.StartEnglish)%>
                        </div>
                        <h4>Документ об образовании</h4>
                        <hr />
                        <div id="_AttRegion" class="clearfix" style="display:none">
                            <%= Html.LabelFor(x => x.EducationInfo.AttestatRegion, "Регион (для российских аттестатов)")%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.AttestatRegion) %>
                            <span id="EducationInfo_AttestatRegion_Message" class="Red" style="display:none; border-collapse:collapse;">Укажите номер региона</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.DiplomSeries, GetGlobalResourceObject("EducationInfo", "DiplomSeries").ToString()) %>
                            <%= Html.TextBoxFor(x => x.EducationInfo.DiplomSeries) %>
                            <br />
                            <span id="EducationInfo_DiplomSeries_Message" class="Red" style="display:none">Укажите серию документа</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.DiplomNumber, GetGlobalResourceObject("EducationInfo", "DiplomNumber").ToString())%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.DiplomNumber)%>
                            <br />
                            <span id="EducationInfo_DiplomNumber_Message" class="Red" style="display:none">Укажите номер документа</span>
                        </div>
                        <div id="HEData">
                            <h4>Данные о высшем образовании</h4>
                            <hr />
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.ProgramName, GetGlobalResourceObject("EducationInfo", "PersonSpecialization").ToString())%>
                                <%= Html.TextBoxFor(x => x.EducationInfo.ProgramName)%>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.PersonStudyForm, GetGlobalResourceObject("EducationInfo", "PersonStudyForm").ToString()) %>
                                <%= Html.DropDownListFor(x => x.EducationInfo.PersonStudyForm, Model.EducationInfo.StudyFormList) %>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.PersonQualification, GetGlobalResourceObject("EducationInfo", "PersonQualification").ToString()) %>
                                <%= Html.DropDownListFor(x => x.EducationInfo.PersonQualification, Model.EducationInfo.QualificationList) %>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.DiplomTheme, GetGlobalResourceObject("EducationInfo", "DiplomTheme").ToString()) %>
                                <%= Html.TextBoxFor(x => x.EducationInfo.DiplomTheme) %>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.HEEntryYear, "Год начала обучения") %>
                                <%= Html.TextBoxFor(x => x.EducationInfo.HEEntryYear) %>
                            </div>
                            <%--<div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.HEExitYear, "Год окончания обучения") %>
                                <%= Html.TextBoxFor(x => x.EducationInfo.HEExitYear) %>
                            </div>--%>
                        </div>
                        <div id="EGEData" class="clearfix">
                            <h6>Баллы ЕГЭ</h6>
                            <% if (Model.EducationInfo.EgeMarks.Count == 0)
                               { 
                            %>
                                <h6 id="noMarks">Нет баллов по ЕГЭ</h6>
                            <%
                               }
                               else
                               {
                            %>
                            <table id="tblEGEData" class="paginate" style="width:400px">
                                <thead>
                                <tr>
                                    <th>Номер сертификата</th>
                                    <th>Предмет</th>
                                    <th>Балл</th>
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
                            <button type="button" id="create-ege" class="button button-blue">Добавить оценку</button>
                            <div id="dialog-form">
                                <p id="validation_info">Все поля обязательны для заполнения</p>
	                            <hr />
                                <fieldset>
                                    <div class="clearfix">
                                        <label for="EgeCert">Номер сертификата</label><br />
		                                <input type="text" id="EgeCert" /><br />
                                    </div>
                                    <div class="clearfix">
                                        <label for="EgeExam">Предмет</label><br />
		                                <select id="EgeExam" ></select><br />
                                    </div>
                                    <div class="clearfix">
                                        <label for="EgeMark">Балл</label><br />
		                                <input type="text" id="EgeMark" value="" /><br />
                                    </div>
	                            </fieldset>
                            </div>
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
                        <li><a href="../../Abiturient?step=1"><%= GetGlobalResourceObject("PersonInfo", "Step1")%></a></li>
                        <li><a href="../../Abiturient?step=2"><%= GetGlobalResourceObject("PersonInfo", "Step2")%></a></li>
                        <li><a href="../../Abiturient?step=3"><%= GetGlobalResourceObject("PersonInfo", "Step3")%></a></li>
                        <li><a href="../../Abiturient?step=4"><%= GetGlobalResourceObject("PersonInfo", "Step4")%></a></li>
                        <li><a href="../../Abiturient?step=5"><%= GetGlobalResourceObject("PersonInfo", "Step5")%></a></li>
                        <li><a href="../../Abiturient?step=6"><%= GetGlobalResourceObject("PersonInfo", "Step6")%></a></li>
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
