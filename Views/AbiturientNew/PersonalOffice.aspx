<%@ Import Namespace="OnlineAbit2013" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.PersonalOffice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("Main", "PersonalOfficeHeader").ToString()%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Анкета</h2>
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
<% if (Model.Stage == 1)
   {
%>
    <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.ui.datepicker-ru.js"></script>
    <script type="text/javascript">
        $(function () {
            <% if (!Model.Enabled)
               { %>
            $('input').attr('readonly', 'readonly');
            $('select').attr('disabled', 'disabled');
            <% } %>
            
            <% if (Model.Enabled)
               { %>
            $("#PersonInfo_BirthDate").datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: "focus",
                yearRange: '1920:2000',
                defaultDate: '-17y',
            });
            $.datepicker.regional["ru"];
            <% } %>

            $('#PersonInfo_Surname').keyup( function() { setTimeout(CheckSurname) });
            $('#PersonInfo_Name').keyup( function() { setTimeout(CheckName) });
            $('#PersonInfo_SecondName').keyup( function() { setTimeout(CheckSecondName) });
            $('#PersonInfo_BirthDate').keyup( function() { setTimeout(CheckBirthDate) });
            $('#PersonInfo_BirthPlace').keyup( function() { setTimeout(CheckBirthPlace) });
            $('#PersonInfo_Surname').blur( function() { setTimeout(CheckSurname) });
            $('#PersonInfo_Name').blur( function() { setTimeout(CheckName) });
            $('#PersonInfo_SecondName').blur( function() { setTimeout(CheckSecondName) });
            $('#PersonInfo_BirthDate').blur( function() { setTimeout(CheckBirthDate) });
            $('#PersonInfo_BirthPlace').blur( function() { setTimeout(CheckBirthPlace) });
        });

        function CheckForm() {
            var res = true;
            if (!CheckSurname()) { res = false; }
            if (!CheckName()) { res = false; }
            if (!CheckBirthPlace()) { res = false; }
            if (!CheckBirthDate()) { res = false; }
            return res;
        }
    </script>
    <script type="text/javascript">
        var PersonInfo_Surname_Message = $('#PersonInfo_Surname_Message').text();
        var PersonInfo_Name_Message = $('#PersonInfo_Name_Message').text();
        var regexp = /^[А-Яа-яё\-\'\s]+$/i;
        function CheckSurname() {
            var ret = true;
            var val = $('#PersonInfo_Surname').val();
            if (val == '') {
                ret = false;
                $('#PersonInfo_Surname').addClass('input-validation-error');
                $('#PersonInfo_Surname_Message').show();
            }
            else {
                $('#PersonInfo_Surname').removeClass('input-validation-error');
                $('#PersonInfo_Surname_Message').hide();
                if (!regexp.test(val)) {
                    ret = false;
                    $('#PersonInfo_Surname_Message').text('Использование латинских символов не допускается');
                    $('#PersonInfo_Surname_Message').show();
                    $('#PersonInfo_Surname').addClass('input-validation-error');
                }
                else {
                    $('#PersonInfo_Surname_Message').text(PersonInfo_Surname_Message);
                    $('#PersonInfo_Surname_Message').hide();
                    $('#PersonInfo_Surname').removeClass('input-validation-error');
                }
            }
            return ret;
        }
        function CheckName() {
            var ret = true;
            var val = $('#PersonInfo_Name').val();
            if (val == '') {
                ret = false;
                $('#PersonInfo_Name').addClass('input-validation-error');
                $('#PersonInfo_Name_Message').show();
            }
            else {
                $('#PersonInfo_Name').removeClass('input-validation-error');
                $('#PersonInfo_Name_Message').hide();
                if (!regexp.test(val)) {
                    $('#PersonInfo_Name_Message').text('Использование латинских символов не допускается');
                    $('#PersonInfo_Name_Message').show();
                    $('#PersonInfo_Name').addClass('input-validation-error');
                    ret = false;
                }
                else {
                    $('#PersonInfo_Name_Message').text(PersonInfo_Name_Message);
                    $('#PersonInfo_Name_Message').hide();
                    $('#PersonInfo_Name').removeClass('input-validation-error');
                }
            }
            return ret;
        }
        function CheckSecondName() {
            var val = $('#PersonInfo_SecondName').val();
            if (!regexp.test(val)) {
                $('#PersonInfo_SecondName_Message').show();
                $('#PersonInfo_SecondName').addClass('input-validation-error');
                ret = false;
            }
            else {
                $('#PersonInfo_SecondName_Message').hide();
                $('#PersonInfo_SecondName').removeClass('input-validation-error');
            }
        }
        function CheckBirthDate() {
            var ret = true;
            if ($('#PersonInfo_BirthDate').val() == '') {
                ret = false;
                $('#PersonInfo_BirthDate').addClass('input-validation-error');
                $('#PersonInfo_BirthDate_Message').show();
            }
            else {
                $('#PersonInfo_BirthDate').removeClass('input-validation-error');
                $('#PersonInfo_BirthDate_Message').hide();
            }
            return ret;
        }
        function CheckBirthPlace() {
            var ret = true;
            if ($('#PersonInfo_BirthPlace').val() == '') {
                ret = false;
                $('#PersonInfo_BirthPlace').addClass('input-validation-error');
                $('#PersonInfo_BirthPlace_Message').show();
            }
            else {
                $('#PersonInfo_BirthPlace').removeClass('input-validation-error');
                $('#PersonInfo_BirthPlace_Message').hide();
            }
            return ret;
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
                <h4><%= GetGlobalResourceObject("PersonInfo", "HeaderPersonalInfo").ToString()%></h4>
                <hr />
                <%= Html.ValidationSummary(GetGlobalResourceObject("PersonInfo", "ValidationSummaryHeader").ToString())%>
                <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                <input name="Enabled" type="hidden" value="<%= Model.Enabled %>" />
                <fieldset>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.Surname, GetGlobalResourceObject("PersonInfo", "Surname").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.Surname)%>
                        <br />
                        <span id="PersonInfo_Surname_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_Surname_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.Name, GetGlobalResourceObject("PersonInfo", "Name").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.Name)%>
                        <br />
                        <span id="PersonInfo_Name_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_Name_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.SecondName, GetGlobalResourceObject("PersonInfo", "SecondName").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.SecondName)%>
                        <span id="PersonInfo_SecondName_Message" class="Red" style="display:none">
                            Использование латинских символов не допускается
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.Sex, GetGlobalResourceObject("PersonInfo", "Sex").ToString())%>
                        <%= Html.DropDownListFor(x => x.PersonInfo.Sex, Model.PersonInfo.SexList)%>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.BirthDate, GetGlobalResourceObject("PersonInfo", "BirthDate").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.BirthDate)%>
                        <br />
                        <span id="PersonInfo_BirthDate_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_BirthDate_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.BirthPlace, GetGlobalResourceObject("PersonInfo", "BirthPlace").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.BirthPlace)%>
                        <br />
                        <span id="PersonInfo_BirthPlace_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_BirthPlace_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.Nationality, GetGlobalResourceObject("PersonInfo", "Nationality").ToString())%>
                        <%= Html.DropDownListFor(x => x.PersonInfo.Nationality, Model.PersonInfo.NationalityList)%>
                    </div>
                </fieldset>
                <hr />
                <div class="clearfix">
                    <input id="btnSubmit" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
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
<%  }
    if (Model.Stage == 2)
    {
%>
        <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
        <script type="text/javascript" src="../../Scripts/jquery.ui.datepicker-ru.js"></script>
        <script type="text/javascript">
            $(function () {
                <% if (!Model.Enabled)
                   { %>
                $('input').attr('readonly', 'readonly');
                $('select').attr('disabled', 'disabled');
                <% } %>
                <% if (Model.Enabled)
                   { %>
                $("#PassportInfo_PassportDate").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    showOn: "focus",
                    yearRange: '1967:2012',
                    maxDate: "+1D",
                    defaultDate: '-3y',
                });
                $.datepicker.regional["ru"];
                <% } %>

                $("form").submit(function () {
                    return CheckForm();
                });
                $('#PassportInfo_PassportType').change(CheckSeries);
                $('#PassportInfo_PassportSeries').keyup( function() { setTimeout(CheckSeries); });
                $('#PassportInfo_PassportNumber').keyup( function() { setTimeout(CheckNumber); });
                $('#PassportInfo_PassportAuthor').keyup( function() { setTimeout(CheckAuthor); });
                $('#PassportInfo_PassportDate').keyup( function() { setTimeout(CheckDate); });
                $('#PassportInfo_PassportSeries').blur( function() { setTimeout(CheckSeries); });
                $('#PassportInfo_PassportNumber').blur( function() { setTimeout(CheckNumber); });
                $('#PassportInfo_PassportAuthor').blur( function() { setTimeout(CheckAuthor); });
                $('#PassportInfo_PassportDate').blur( function() { setTimeout(CheckDate); });
            });
            function CheckForm() {
                var res = true;
                if (!CheckSeries()) { res = false; }
                if (!CheckNumber()) { res = false; }
                if (!CheckAuthor()) { res = false; }
                if (!CheckDate()) { res = false; }
                return res;
            }
        </script>
        <script type="text/javascript">
            var PassportInfo_PassportSeries_Message = $('#PassportInfo_PassportSeries_Message').text();
            var PassportInfo_PassportNumber_Message = $('#PassportInfo_PassportNumber_Message').text();
            var PassportInfo_PassportDate_Message = $('#PassportInfo_PassportDate_Message').text();
            function CheckSeries() {
                var ret = true;
                var val = $('#PassportInfo_PassportSeries').val();
                var ruPassportRegex = /^\d{4}$/i;
                if ($('#PassportInfo_PassportType').val() == '1' && val == '') {
                    ret = false;
                    $('#PassportInfo_PassportSeries').addClass('input-validation-error');
                    $('#PassportInfo_PassportSeries_Message').show();
                }
                else {
                    $('#PassportInfo_PassportSeries').removeClass('input-validation-error');
                    $('#PassportInfo_PassportSeries_Message').hide();
                    if ($('#PassportInfo_PassportType').val() == '1' && !ruPassportRegex.text(val)) {
                        $('#PassportInfo_PassportSeries').addClass('input-validation-error');
                        $('#PassportInfo_PassportSeries_Message').text('Серия паспорта РФ должна состоять из 4 цифр без пробелов');
                        $('#PassportInfo_PassportSeries_Message').show();
                    }
                    else {
                        $('#PassportInfo_PassportSeries').removeClass('input-validation-error');
                        $('#PassportInfo_PassportSeries_Message').hide();
                        $('#PassportInfo_PassportSeries_Message').text(PassportInfo_PassportSeries_Message);
                        if (val.length > 10) {
                            $('#PassportInfo_PassportSeries').addClass('input-validation-error');
                            $('#PassportInfo_PassportSeries_Message').text('Слишком длинное значение');
                            $('#PassportInfo_PassportSeries_Message').show();
                        }
                        else {
                            $('#PassportInfo_PassportSeries').removeClass('input-validation-error');
                            $('#PassportInfo_PassportSeries_Message').hide();
                            $('#PassportInfo_PassportSeries_Message').text(PassportInfo_PassportSeries_Message);
                        }
                    }
                }
                return ret;
            }
            function CheckNumber() {
                var ret = true;
                var val = $('#PassportInfo_PassportNumber').val();
                var ruPassportRegex = /^\d{6}$/i;
                if ($('#PassportInfo_PassportNumber').val() == '') {
                    ret = false;
                    $('#PassportInfo_PassportNumber').addClass('input-validation-error');
                    $('#PassportInfo_PassportNumber_Message').show();
                }
                else {
                    $('#PassportInfo_PassportNumber').removeClass('input-validation-error');
                    $('#PassportInfo_PassportNumber_Message').hide();
                    if ($('#PassportInfo_PassportType').val() == '1' && !ruPassportRegex.text(val)) {
                        $('#PassportInfo_PassportNumber').addClass('input-validation-error');
                        $('#PassportInfo_PassportNumber_Message').text('Номер паспорта РФ должен состоять из 6 цифр без пробелов');
                        $('#PassportInfo_PassportNumber_Message').show();
                    }
                    else {
                        $('#PassportInfo_PassportNumber').removeClass('input-validation-error');
                        $('#PassportInfo_PassportNumber_Message').hide();
                        $('#PassportInfo_PassportNumber_Message').text(PassportInfo_PassportNumber_Message);
                        if (val.length > 20) {
                            $('#PassportInfo_PassportNumber').addClass('input-validation-error');
                            $('#PassportInfo_PassportNumber_Message').text('Слишком длинное значение');
                            $('#PassportInfo_PassportNumber_Message').show();
                        }
                        else {
                            $('#PassportInfo_PassportNumber').removeClass('input-validation-error');
                            $('#PassportInfo_PassportNumber_Message').hide();
                            $('#PassportInfo_PassportNumber_Message').text(PassportInfo_PassportNumber_Message);
                        }
                    }
                }
                return ret;
            }
            function CheckDate() {
                var ret = true;
                if ($('#PassportInfo_PassportDate').val() == '') {
                    ret = false;
                    $('#PassportInfo_PassportDate').addClass('input-validation-error');
                    $('#PassportInfo_PassportDate_Message').show();
                }
                else {
                    $('#PassportInfo_PassportDate').removeClass('input-validation-error');
                    $('#PassportInfo_PassportDate_Message').hide();
                }
                return ret;
            }
            function CheckAuthor() {
                var ret = true;
                if ($('#PassportInfo_PassportType').val() == '1' && $('#PassportInfo_PassportAuthor').val() == '') {
                    ret = false;
                    $('#PassportInfo_PassportAuthor').addClass('input-validation-error');
                    $('#PassportInfo_PassportAuthor_Message').show();
                }
                else {
                    $('#PassportInfo_PassportAuthor').removeClass('input-validation-error');
                    $('#PassportInfo_PassportAuthor_Message').hide();
                }
                return ret;
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
                        <h4><%= GetGlobalResourceObject("PassportInfo", "HeaderPassport").ToString()%></h4>
                        <hr />
                        <%= Html.ValidationSummary(GetGlobalResourceObject("PersonInfo", "ValidationSummaryHeader").ToString())%>
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <input name="Enabled" type="hidden" value="<%= Model.Enabled %>" />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportType, GetGlobalResourceObject("PassportInfo", "PassportType").ToString())%>
                            <%= Html.DropDownListFor(x => x.PassportInfo.PassportType, Model.PassportInfo.PassportTypeList) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportSeries, GetGlobalResourceObject("PassportInfo", "PassportSeries").ToString())%>
                            <%= Html.TextBoxFor(x => x.PassportInfo.PassportSeries)%>
                            <br />
                            <span id="PassportInfo_PassportSeries_Message" class="Red" style="display:none">
                                <%= GetGlobalResourceObject("PassportInfo", "PassportInfo_PassportSeries_Message").ToString()%>
                            </span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportNumber, GetGlobalResourceObject("PassportInfo", "PassportNumber").ToString())%>
                            <%= Html.TextBoxFor(x => x.PassportInfo.PassportNumber)%>
                            <br />
                            <span id="PassportInfo_PassportNumber_Message" class="Red" style="display:none">
                                <%= GetGlobalResourceObject("PassportInfo", "PassportInfo_PassportNumber_Message").ToString()%>
                            </span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportAuthor, GetGlobalResourceObject("PassportInfo", "PassportAuthor").ToString())%>
                            <%= Html.TextBoxFor(x => x.PassportInfo.PassportAuthor)%>
                            <br />
                            <span id="PassportInfo_PassportAuthor_Message" class="Red" style="display:none">
                                <%= GetGlobalResourceObject("PassportInfo", "PassportInfo_PassportAuthor_Message").ToString()%>
                            </span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportDate, GetGlobalResourceObject("PassportInfo", "PassportDate").ToString())%>
                            <%= Html.TextBoxFor(x => x.PassportInfo.PassportDate)%>
                            <br />
                            <span id="PassportInfo_PassportDate_Message" class="Red" style="display:none">
                                <%= GetGlobalResourceObject("PassportInfo", "PassportInfo_PassportDate_Message").ToString()%>
                            </span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportCode, GetGlobalResourceObject("PassportInfo", "PassportCode").ToString())%>
                            <%= Html.TextBoxFor(x => x.PassportInfo.PassportCode)%>
                        </div>
                        <hr />
                        <div class="clearfix">
                            <input id="Submit1" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
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
<%
    }
    if (Model.Stage == 3)
    {
%>
        <script type="text/javascript">
            $(function () {
                $('#form').submit(function () {
                    return CheckForm();
                })
                <% if (!Model.Enabled)
                   { %>
                $('input').attr('readonly', 'readonly');
                $('select').attr('disabled', 'disabled');
                <% }
                   else
                   { %>
                $('#ContactsInfo_CountryId').change(function () { setTimeout(ValidateCountry); });
                function ValidateCountry() {
                    var countryid = $('#ContactsInfo_CountryId').val();
                    if (countryid == '193') {
                        $('#Region').show();
                    }
                    else {
                        $('#Region').hide();
                    }
                }
                ValidateCountry();
                <% } %>
                
                $('#ContactsInfo_MainPhone').keyup(function() { setTimeout(CheckPhone); } );
                //$('#ContactsInfo_PostIndex').keyup(function() { setTimeout(CheckIndex); } );
                $('#ContactsInfo_City').keyup(function() { setTimeout(CheckCity); } );
                //$('#ContactsInfo_Street').keyup(function() { setTimeout(CheckStreet); } );
                $('#ContactsInfo_House').keyup(function() { setTimeout(CheckHouse); } );
                
                $('#ContactsInfo_MainPhone').blur(function() { setTimeout(CheckPhone); } );
                //$('#ContactsInfo_PostIndex').blur(function() { setTimeout(CheckIndex); } );
                $('#ContactsInfo_City').blur(function() { setTimeout(CheckCity); } );
                //$('#ContactsInfo_Street').blur(function() { setTimeout(CheckStreet); } );
                $('#ContactsInfo_House').blur(function() { setTimeout(CheckHouse); } );

            });
        </script>
        <script type="text/javascript">
            function CheckPhone() {
                var ret = true;
                if ($('#ContactsInfo_MainPhone').val() == '') {
                    ret = false;
                    $('#ContactsInfo_MainPhone').addClass('input-validation-error');
                    $('#ContactsInfo_MainPhone_Message').show();
                }
                else {
                    $('#ContactsInfo_MainPhone').removeClass('input-validation-error');
                    $('#ContactsInfo_MainPhone_Message').hide();
                }
                return ret;
            }
            function CheckIndex() {
                var ret = true;
                if ($('#ContactsInfo_PostIndex').val() == '') {
                    ret = false;
                    $('#ContactsInfo_PostIndex').addClass('input-validation-error');
                    $('#ContactsInfo_PostIndex_Message').show();
                }
                else {
                    $('#ContactsInfo_PostIndex').removeClass('input-validation-error');
                    $('#ContactsInfo_PostIndex_Message').hide();
                }
                return ret;
            }
            function CheckCity() {
                var ret = true;
                if ($('#ContactsInfo_City').val() == '') {
                    ret = false;
                    $('#ContactsInfo_City').addClass('input-validation-error');
                    $('#ContactsInfo_City_Message').show();
                }
                else {
                    $('#ContactsInfo_City').removeClass('input-validation-error');
                    $('#ContactsInfo_City_Message').hide();
                }
                return ret;
            }
            function CheckStreet() {
                var ret = true;
                if ($('#ContactsInfo_Street').val() == '') {
                    ret = false;
                    $('#ContactsInfo_Street').addClass('input-validation-error');
                    $('#ContactsInfo_Street_Message').show();
                }
                else {
                    $('#ContactsInfo_Street').removeClass('input-validation-error');
                    $('#ContactsInfo_Street_Message').hide();
                }
                return ret;
            }
            function CheckHouse() {
                var ret = true;
                if ($('#ContactsInfo_House').val() == '') {
                    ret = false;
                    $('#ContactsInfo_House').addClass('input-validation-error');
                    $('#ContactsInfo_House_Message').show();
                }
                else {
                    $('#ContactsInfo_House').removeClass('input-validation-error');
                    $('#ContactsInfo_House_Message').hide();
                }
                return ret;
            }
            
            function CheckForm() {
                var res = true;
                if (!CheckPhone()) { res = false; }
                //if (!CheckIndex) { res = false; }
                if (!CheckCity()) { res = false; }
                if (!CheckStreet()) { res = false; }
                if (!CheckHouse()) { res = false; }
                return res;
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
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <h3>Контактные телефоны:</h3>
                        <hr />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.MainPhone, GetGlobalResourceObject("ContactsInfo", "MainPhone").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.MainPhone) %>
                            <br />
                            <span id="ContactsInfo_MainPhone_Message" class="Red" style="display:none">Введите основной номер</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.SecondPhone, GetGlobalResourceObject("ContactsInfo", "SecondPhone").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.SecondPhone)%>
                        </div>
                        <h3>Адрес регистрации:</h3>
                        <hr />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.CountryId, GetGlobalResourceObject("ContactsInfo", "CountryId").ToString())%>
                            <%= Html.DropDownListFor(x => x.ContactsInfo.CountryId, Model.ContactsInfo.CountryList) %>
                        </div>
                        <div class="clearfix" id="Region">
                            <%= Html.LabelFor(x => x.ContactsInfo.RegionId, GetGlobalResourceObject("ContactsInfo", "RegionId").ToString())%>
                            <%= Html.DropDownListFor(x => x.ContactsInfo.RegionId, Model.ContactsInfo.RegionList) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.PostIndex, GetGlobalResourceObject("ContactsInfo", "PostIndex").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.PostIndex) %>
                            <br />
                            <span id="Span1" class="Red" style="display:none">Введите дату выдачи паспорта</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.City, GetGlobalResourceObject("ContactsInfo", "City").ToString()) %>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.City) %>
                            <br />
                            <span id="ContactsInfo_City_Message" class="Red" style="display:none">Введите город</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.Street, GetGlobalResourceObject("ContactsInfo", "Street").ToString()) %>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.Street)%>
                            <br />
                            <span id="ContactsInfo_Street_Message" class="Red" style="display:none">Введите улицу</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.House, GetGlobalResourceObject("ContactsInfo", "House").ToString()) %>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.House) %>
                            <br />
                            <span id="ContactsInfo_House_Message" class="Red" style="display:none">Введите дом</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.Korpus, GetGlobalResourceObject("ContactsInfo", "Korpus").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.Korpus) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.Flat, GetGlobalResourceObject("ContactsInfo", "Flat").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.Flat) %>
                        </div>
                        <h3>Адрес проживания (если отличается):</h3>
                        <hr />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.PostIndexReal, GetGlobalResourceObject("ContactsInfo", "PostIndex").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.PostIndexReal)%>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.CityReal, GetGlobalResourceObject("ContactsInfo", "City").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.CityReal)%>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.StreetReal, GetGlobalResourceObject("ContactsInfo", "Street").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.StreetReal)%>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.HouseReal, GetGlobalResourceObject("ContactsInfo", "House").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.HouseReal) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.KorpusReal, GetGlobalResourceObject("ContactsInfo", "Korpus").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.KorpusReal) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.FlatReal, GetGlobalResourceObject("ContactsInfo", "Flat").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.FlatReal)%>
                        </div>
                        <hr />
                        <div class="clearfix">
                            <input id="Submit2" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
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
<%
    }
    if (Model.Stage == 4)
    {
%>
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
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolTypeId, GetGlobalResourceObject("EducationInfo", "SchoolTypeId").ToString())%>
                            <%= Html.DropDownListFor(x => x.EducationInfo.SchoolTypeId, Model.EducationInfo.SchoolTypeList) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolName, GetGlobalResourceObject("EducationInfo", "SchoolName").ToString())%>
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
                            <%= Html.LabelFor(x => x.EducationInfo.AvgMark, GetGlobalResourceObject("EducationInfo", "AvgMark").ToString()) %>
                            <%= Html.TextBoxFor(x => x.EducationInfo.AvgMark) %>
                        </div>
                        <div id="_IsExcellent" class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.IsExcellent, "Медалист (красный диплом)") %>
                            <%= Html.CheckBoxFor(x => x.EducationInfo.IsExcellent)%>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.CountryEducId, GetGlobalResourceObject("EducationInfo", "CountryEducId").ToString()) %>
                            <%= Html.DropDownListFor(x => x.EducationInfo.CountryEducId, Model.EducationInfo.CountryList) %>
                        </div>
                        <div id="CountryMessage" class="message info" style="display:none; border-collapse:collapse;">
                            Пожалуйста, укажите в названии ВУЗа страну, где Вы обучались (например, "Oxford, UK", "Oberwolfach, Germany")
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.LanguageId, GetGlobalResourceObject("EducationInfo", "LanguageId").ToString())%>
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
<%
    }
    if (Model.Stage == 5)//работа и научная деятельность
    {
%>
        <script type="text/javascript">
            $(function () {
                $('#WorkPlace').keyup(function () {
                    var str = $('#WorkPlace').val();
                    if (str != "") {
                        $('#validationMsgPersonWorksPlace').text('');
                    }
                    else {
                        $('#validationMsgPersonWorksPlace').text('Введите место работы');
                    }
                });
                $('#WorkProf').keyup(function () {
                    var str = $('#WorkProf').val();
                    if (str != "") {
                        $('#validationMsgPersonWorksLevel').text('');
                    }
                    else {
                        $('#validationMsgPersonWorksLevel').text('Введите должность');
                    }
                });
                $('#WorkSpec').keyup(function () {
                    var str = $('#WorkSpec').val();
                    if (str != "") {
                        $('#validationMsgPersonWorksDuties').text('');
                    }
                    else {
                        $('#validationMsgPersonWorksDuties').text('Введите должностные обязанности');
                    }
                });
            });
            function UpdScWorks() {
                if ($('#ScWorkInfo').val() == '') {
                    return false;
                }
                var params = new Object();
                params['ScWorkInfo'] = $('#ScWorkInfo').val();
                params['ScWorkType'] = $('#WorkInfo_ScWorkId').val();
                $.post('Abiturient/UpdateScienceWorks', params, function (res) {
                    if (res.IsOk) {
                        var output = '';
                        output += '<tr id=\'' + res.Data.Id + '\'><td>';
                        output += res.Data.Type + '</td>';
                        output += '<td>' + res.Data.Info + '</td>';
                        output += '<td><span class="link" onclick="DeleteScWork(\'' + res.Data.Id + '\')" ><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить оценку" /><span></td>';
                        output += '</tr>';
                        $('#ScWorks tbody').append(output);
                    }
                    else {
                        alert(res.ErrorMsg);
                    }
                }, 'json');
            }
            function DeleteScWork(id) {
                var param = new Object();
                param['id'] = id;
                $.post('Abiturient/DeleteScienceWorks', param, function (res) {
                    if (res.IsOk) {
                        $("#" + id).hide(250).html("");
                    }
                    else {
                        alert(res.ErrorMessage);
                    }
                }, 'json');
            }
            function AddWorkPlace() {
                var params = new Object();
                params['WorkStag'] = $('#WorkStag').val();
                params['WorkPlace'] = $('#WorkPlace').val();
                params['WorkProf'] = $('#WorkProf').val();
                params['WorkSpec'] = $('#WorkSpec').val();
                var Ok = true;
                if (params['WorkPlace'] == "") {
                    $('#validationMsgPersonWorksPlace').text('Введите место работы');
                    Ok = false;
                }
                if (params['WorkPlace'] == "") {
                    $('#validationMsgPersonWorksLevel').text('Введите должность');
                    Ok = false;
                }
                if (params['WorkSpec'] == "") {
                    $('#validationMsgPersonWorksDuties').text('Введите должностные обязанности');
                    Ok = false;
                }
                if (Ok) {
                    $.post('Abiturient/AddWorkPlace', params, function (res) {
                        if (res.IsOk) {
                            $('#NoWorks').hide();
                            var info = '<tr id="' + res.Data.Id + '">';
                            info += '<td>' + res.Data.Place + '</td>';
                            info += '<td>' + res.Data.Stag + '</td>';
                            info += '<td>' + res.Data.Level + '</td>';
                            info += '<td>' + res.Data.Duties + '</td>';
                            info += '<td><span class="link" onclick="DeleteWorkPlace(\'' + res.Data.Id + '\')"><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить" /></span></td>';
                            $('#PersonWorks tbody').append(info);
                        }
                        else {
                            alert(res.ErrorMessage);
                        }
                    }, 'json');
                }
            }
            function DeleteWorkPlace(id) {
                var parm = new Object();
                parm["wrkId"] = id;
                $.post('Abiturient/DeleteWorkPlace', parm, function (res) {
                    if (res.IsOk) {
                        $('#' + id).hide(250).html('');
                    }
                    else {
                        alert(res.ErrorMessage);
                    }
                }, 'json');
            }
            function CheckRegExp() {
                var val = $('#WorkStag').val();
                var regex = /^([0-9])+$/i;
                if (!regex.test(val)) {
                    $('#btnAddProfs').hide();
                    $('#validationMsgPersonWorksExperience').text('Введите целое число').show();
                }
                else {
                    $('#btnAddProfs').show();
                    $('#validationMsgPersonWorksExperience').hide();
                }
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
                    <h3>Участие в научно-исследовательской работе:</h3>
                    <hr />
                    <p>Участие в научных конференциях (укажите тему конференции, дату и место ее проведения; тему доклада/выступления), опубликованные научные статьи, работа в научных лабораториях, работа в проектных группах.</p>
                    <p>Напоминаем, что Вам необходимо предоставить документы, <b>подтверждающие:</b> </p>
                    <ul>
                        <li>Ваше участие в конференциях, семинарах, круглых столах и прочих научных и научно-практических мероприятиях. В качестве таковых могут выступать опубликованные тезисы доклада и программа  мероприятия.</li>
                        <li>Ваше  участие в <b>исследовательских проектах, поддержанных грантами,</b> а также подтверждающие полученные Вами результаты.</li>
                        <li>Вашу работу (в том числе и эффективность  деятельности).</li>
                    </ul>
                    <div class="form">
                        <div class="clearfix">
                            <%= Html.DropDownListFor(x => x.WorkInfo.ScWorkId, Model.WorkInfo.ScWorks)%>
                        </div>
                        <div class="clearfix">
                            <textarea class="noresize" id="ScWorkInfo" rows="5" cols="80"></textarea>
                        </div>
                        <br />
                        <div class="clearfix">
                            <button id="btnAddScWork" onclick="UpdScWorks()" class="button button-blue">Добавить</button>
                        </div>
                        <br /><br />
                        <table id="ScWorks" class="paginate" style="width:100%;">
                            <thead>
                                <tr>
                                    <th>Тип</th>
                                    <th>Текст</th>
                                    <th>Удалить</th>
                                </tr>
                            </thead>
                            <tbody>
                            <% foreach (var scWork in Model.WorkInfo.pScWorks)
                                {
                            %>
                                <tr>
                                <%= Html.Raw(string.Format(@"<tr id=""{0}"">", scWork.Id)) %>
                                    <td><%= Html.Encode(scWork.ScienceWorkType) %></td>
                                    <td><%= Html.Encode(scWork.ScienceWorkInfo) %></td>
                                    <td><%= Html.Raw("<span class=\"link\" onclick=\"DeleteScWork('" + scWork.Id.ToString() + "')\" ><img src=\"../../Content/themes/base/images/delete-icon.png\" alt=\"Удалить\" /></span>") %></td>
                                </tr>
                            <% } %>
                            </tbody>
                        </table>
                    </div>
                    <br /><br />
                    <h3>Опыт работы (практики):</h3>
                    <hr />
                    <div class="form">
                        <div class="clearfix">
                            <label for="WorkStag">Стаж (полных лет):</label>
                            <input id="WorkStag" onkeyup="CheckRegExp()" type="text" class="text ui-widget-content ui-corner-all"/><br /><span id="validationMsgPersonWorksExperience" class="Red"></span>
                        </div>
                        <div class="clearfix">
                            <label for="WorkPlace">Место работы(практики):</label>
                            <input id="WorkPlace" type="text" /><br /><span id="validationMsgPersonWorksPlace" class="Red"></span>
                        </div>
                        <div class="clearfix">
                            <label for="WorkProf">Должность:</label>
                            <input id="WorkProf" type="text" /><br /><span id="validationMsgPersonWorksLevel" class="Red"></span>
                        </div>
                        <div class="clearfix">
                            <label for="WorkSpec">Должностные обязанности:</label>
                            <textarea id="WorkSpec" cols="80" rows="4" ></textarea><br /><span id="validationMsgPersonWorksDuties" class="Red"></span>
                        </div>
                    </div>
                    <div class="clearfix">
                        <button id="btnAddProfs" onclick="AddWorkPlace()" class="button button-blue">Добавить</button>
                    </div>
                    <br /><br />
                    <table id="PersonWorks" class="paginate" style="width:100%;">
                        <thead>
                            <tr>
                                <th>Место работы</th>
                                <th>Стаж</th>
                                <th>Должность</th>
                                <th>Должностные обязанности</th>
                                <th>Удалить</th>
                            </tr>
                        </thead>
                        <tbody>
                        <% foreach (var wrk in Model.WorkInfo.pWorks)
                           {
                        %>
                            <tr>
                            <%= Html.Raw(string.Format(@"<tr id=""{0}"">", wrk.Id.ToString())) %>
                                <td><%= Html.Encode(wrk.Place) %></td>
                                <td><%= Html.Encode(wrk.Stag) %></td>
                                <td><%= Html.Encode(wrk.Level) %></td>
                                <td><%= Html.Encode(wrk.Duties) %></td>
                                <td><%= Html.Raw(string.Format(@"<span class=""link"" onclick=""DeleteWorkPlace('{0}')""><img src=""../../Content/themes/base/images/delete-icon.png"" alt=""Удалить"" /></span>", wrk.Id.ToString()))%></td>
                            </tr>
                        <% } %>
                        </tbody>
                    </table>
                    <% if (Model.WorkInfo.pWorks.Count == 0)
                       {
                    %>
                        <h5 id="NoWorks">Нет</h5>
                    <% } %>

                    <br /><hr style="color:#A6C9E2;" />

                    <% using (Html.BeginForm("NextStep", "Abiturient", FormMethod.Post))
                       {
                    %>
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <input id="Submit4" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                    <% } %>
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
<%
    }
    if (Model.Stage == 6)
    {
%>
    <script type="text/javascript">
        $(function () {
            $('form').submit(function () {
                var FZAgree = $('#AddInfo_FZ_152Agree').is(':checked');
                if (FZAgree) {
                    $('#FZ').hide();
                    return true;
                }
                else {
                    $('#FZ').show();
                    return false;
                }
            });
        });
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
                <form class="panel form" action="Abiturient/NextStep" method="post">
                    <%= Html.ValidationSummary() %>
                    <%= Html.HiddenFor(x => x.Stage) %>
                    <div class="clearfix">
                        <h4>Общежитие</h4>
                        <%= Html.CheckBoxFor(x => x.AddInfo.HostelAbit)%>
                        <span>Нуждаюсь в общежитии на время поступления</span>
                    </div>
                    <div class="clearfix">
                        <h4>Право на льготы:</h4>
                        <%= Html.CheckBoxFor(x => x.AddInfo.HasPrivileges) %>
                        <span>Претендую на льготы (инвалид I,II ст., участник боевых действий, сирота, чернобылец...)</span>
                    </div>
                    <div class="clearfix">
                        <h4>Лицо, с которым можно связаться в экстренных случаях:</h4>
                        <span>(указать Ф.И.О., степень родства, телефон, моб.телефон, эл.почта)</span><br />
                        <!-- <textarea id="AddPerson_ContactPerson" name="AddPerson.ContactPerson" cols="40" rows="4" class="ui-widget-content ui-corner-all"></textarea> -->
                        <%= Html.TextAreaFor(x => x.AddInfo.ContactPerson, 5, 70, new SortedList<string, object>() { { "class", "noresize" } }) %>
                    </div>
                    <div class="clearfix">
                        <h4>О себе дополнительно сообщаю:</h4>
                        <!-- <textarea id="AddPerson_ExtraInfo" name="AddPerson.ExtraInfo" cols="40" rows="4" class="ui-widget-content ui-corner-all"></textarea> -->
                        <%= Html.TextAreaFor(x => x.AddInfo.ExtraInfo, 5, 70, new SortedList<string, object>() { { "class", "noresize" } })%>
                    </div>
                    <div class="clearfix">
                        <h4>Я подтверждаю, что предоставленная мной информация корректна и достоверна. Даю согласие на обработку предоставленных персональных данных в порядке, установленном Федеральным законом от 27 июля 2006 года № 152-ФЗ «О персональных данных».</h4>
                        <%= Html.CheckBoxFor(x => x.AddInfo.FZ_152Agree) %>
                        <span>Подтверждаю и согласен</span>    
                    </div>
                    <span id="FZ" class="Red" style="display:none;">Вы должны принять условия</span>
                    <hr />
                    <div class="clearfix">
                        <input id="Submit5" class="button button-green" type="submit" value="Закончить регистрацию" />
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
<%
    }
%>
</asp:Content>
