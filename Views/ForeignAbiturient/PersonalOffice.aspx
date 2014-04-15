<%@ Import Namespace="OnlineAbit2013" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/ForeignAbiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.PersonalOfficeForeign>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("Common", "ForeignAbiturient_Title").ToString()%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("PersonalOffice_Step1", "FormHeader").ToString()%></h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
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

            $('#PersonInfo_Nationality').change( function() { setTimeout(CheckNationality) });
            CheckNationality();
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
        function CheckNationality() {
            if ($('#PersonInfo_Nationality').val() != '193') {
                $('#EqualWithRussian').show();
            }
            else {
                $('#EqualWithRussian').hide();
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
            <form id="form" class="form panel" action="ForeignAbiturient/NextStep" method="post" onsubmit="return CheckForm();">
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
                    <%--<div id="EqualWithRussian" class="clearfix" style="display:none">
                        <%= Html.LabelFor(x => x.PersonInfo.IsEqualWithRussian, GetGlobalResourceObject("PersonInfo", "IsEqualWithRussian").ToString())%>
                        <%= Html.CheckBoxFor(x => x.PersonInfo.IsEqualWithRussian)%>
                    </div>--%>
                </fieldset>
                <hr />
                <div class="clearfix">
                    <input id="btnSubmit" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                </div>
            </form>
            </div>
            <div class="grid_2">
                <ol>
                    <li><a href="../../ForeignAbiturient?step=1"><%= GetGlobalResourceObject("PersonInfo", "Step1")%></a></li>
                    <li><a href="../../ForeignAbiturient?step=2"><%= GetGlobalResourceObject("PersonInfo", "Step2")%></a></li>
                    <li><a href="../../ForeignAbiturient?step=3"><%= GetGlobalResourceObject("PersonInfo", "Step3")%></a></li>
                    <li><a href="../../ForeignAbiturient?step=4"><%= GetGlobalResourceObject("PersonInfo", "Step4")%></a></li>
                    <li><a href="../../ForeignAbiturient?step=5"><%= GetGlobalResourceObject("PersonInfo", "Step6")%></a></li>
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
                    <form id="form" class="form panel" action="ForeignAbiturient/NextStep" method="post" onsubmit="return CheckForm();">
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
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportValid, GetGlobalResourceObject("PersonalOfficeForeign", "PassportExpire").ToString())%>
                            <%= Html.TextBoxFor(x => x.PassportInfo.PassportValid)%>
                            <br />
                            <span id="PassportExpireMessage" class="Red" style="display:none;"><%= GetGlobalResourceObject("PersonalOfficeForeign", "PassportExpireMessage").ToString()%></span>
                        </div><br />
                        
                        <h4><%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderVisa").ToString()%></h4>
                        <hr />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.VisaInfo.CountryId, GetGlobalResourceObject("PersonalOfficeForeign", "VisaCountryName").ToString())%>
                            <%= Html.DropDownListFor(x => x.VisaInfo.CountryId, Model.VisaInfo.CountryList)%>
                            <br />
                            <span id="VisaCountryNameMessage" class="Red" style="display:none;">
                            <%= GetGlobalResourceObject("PersonalOfficeForeign", "VisaCountryNameMessage").ToString()%>
                            </span>
                        </div><br />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.VisaInfo.Town, GetGlobalResourceObject("PersonalOfficeForeign", "VisaTownName").ToString())%>
                            <%= Html.TextBoxFor(x => x.VisaInfo.Town)%>
                            <br />
                            <span id="VisaTownNameMessage" class="Red" style="display:none;">
                            <%= GetGlobalResourceObject("PersonalOfficeForeign", "VisaTownNameMessage").ToString()%>
                            </span>
                        </div><br />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.VisaInfo.PostAddress, GetGlobalResourceObject("PersonalOfficeForeign", "VisaPostAddress").ToString())%>
                            <%= Html.TextBoxFor(x => x.VisaInfo.PostAddress)%>
                            <br />
                            <span id="VisaPostAddressMessage" class="Red" style="display:none;">
                            <%= GetGlobalResourceObject("PersonalOfficeForeign", "VisaPostAddressMessage").ToString()%>
                            </span>
                        </div><br />
                        <hr />
                        <div class="clearfix">
                            <input id="Submit1" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                        </div>
                    </form>
                </div>
                <div class="grid_2">
                    <ol>
                        <li><a href="../../ForeignAbiturient?step=1"><%= GetGlobalResourceObject("PersonInfo", "Step1")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=2"><%= GetGlobalResourceObject("PersonInfo", "Step2")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=3"><%= GetGlobalResourceObject("PersonInfo", "Step3")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=4"><%= GetGlobalResourceObject("PersonInfo", "Step4")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=5"><%= GetGlobalResourceObject("PersonInfo", "Step6")%></a></li>
                        
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
                <% } %>
                
                $('#ContactsInfo_MainPhone').keyup(function() { setTimeout(CheckPhone); } );
                $('#ContactsInfo_MainPhone').blur(function() { setTimeout(CheckPhone); } );
                
                $('#ContactsInfo_AddressData').keyup(function() { setTimeout(CheckAddress); } );
                $('#ContactsInfo_AddressData').blur(function() { setTimeout(CheckAddress); } );
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
            
            function CheckAddress() {
                var ret = true;
                if ($('#ContactsInfo_AddressData').val() == '') {
                    ret = false;
                    $('#ContactsInfo_AddressData').addClass('input-validation-error');
                    $('#ContactsInfo_AddressData_Message').show();
                }
                else {
                    $('#ContactsInfo_AddressData').removeClass('input-validation-error');
                    $('#ContactsInfo_AddressData_Message').hide();
                }
                return ret;
            }
            
            function CheckForm() {
                var res = true;
                if (!CheckPhone()) { res = false; }
                if (!CheckAddress()) { res = false; }
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
                    <form id="form" class="form panel" action="ForeignAbiturient/NextStep" method="post" onsubmit="return CheckForm();">
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <h3><%= GetGlobalResourceObject("ContactsInfo", "PhonesHeader").ToString()%></h3>
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
                        <h3><%= GetGlobalResourceObject("ContactsInfo", "RegistrationHeader").ToString()%></h3>
                        <hr />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.CountryId, GetGlobalResourceObject("ContactsInfo", "CountryId").ToString())%>
                            <%= Html.DropDownListFor(x => x.ContactsInfo.CountryId, Model.ContactsInfo.CountryList) %>
                        </div>
                        
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.AddressData, GetGlobalResourceObject("PersonalOfficeForeign", "Address").ToString())%>
                            <%= Html.TextAreaFor(x => x.ContactsInfo.AddressData, 5, 70, new Dictionary<string, object>() { { "class", "noresize" } })%>
                            <br />
                            <span id="ContactsInfo_AddressData_Message" class="Red" style="display:none">Введите адрес</span>
                        </div>
                        <hr />
                        <div class="clearfix">
                            <input id="Submit2" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                        </div>
                    </form>
                </div>
                <div class="grid_2">
                    <ol>
                        <li><a href="../../ForeignAbiturient?step=1"><%= GetGlobalResourceObject("PersonInfo", "Step1")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=2"><%= GetGlobalResourceObject("PersonInfo", "Step2")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=3"><%= GetGlobalResourceObject("PersonInfo", "Step3")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=4"><%= GetGlobalResourceObject("PersonInfo", "Step4")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=5"><%= GetGlobalResourceObject("PersonInfo", "Step6")%></a></li>
                        
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
            
            $(function () {
                fStartOne();
                fStartTwo();

                $('#EducationInfo_SchoolExitYear').keyup(function () { setTimeout(CheckSchoolExitYear); });
                $('#EducationInfo_SchoolExitYear').blur(function () { setTimeout(CheckSchoolExitYear); });
                $('#EducationInfo_SchoolName').keyup(function () { setTimeout(CheckSchoolName); });
                $('#EducationInfo_SchoolName').blur(function () { setTimeout(CheckSchoolName); });
            });

            function fStartOne() {
                LoadAutoCompleteValues();
                if ($('#EducationInfo_SchoolTypeId').val() != 4) {
                    $('#HEData').hide();
                }
                else {
                    $('#HEData').show();
                }

                $('#EducationInfo_SchoolTypeId').change(function changeTbls() {
                    if ($('#EducationInfo_SchoolTypeId').val() != 4) {
                        $('#HEData').hide();
                    }
                    else {
                        $('#HEData').show();
                    }
                });

                var cachedVuzNames = false;
                var VuzNamesCache;
                var EmptySource = [];
                function LoadAutoCompleteValues() {
                    var vals = new Object();
                    vals["schoolType"] = 4//$('#EducationInfo_SchoolTypeId').val();
                    if (!cachedVuzNames) {
                        $.post('/ForeignAbiturient/LoadVuzNames', vals, function (res) {
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
            function AddLang() {
                var data = new Object();
                data["langid"] = $('#Languages').val();
                data["levelid"] = $('#Levels').val();
                $.post('/ForeignAbiturient/AddLang', data, function (res) {
                    if (res == 'OK') {
                        GetPersonLanguages();
                        GetLanguages();
                    }
                }, 'json');
            }
            function DeleteLang(id) {
                var data = new Object();
                data["id"] = id;
                $.post('/ForeignAbiturient/DeleteLang', data, function (res) {
                    if (res.IsOk) {
                        $('#' + id).html('');
                        GetPersonLanguages();
                        GetLanguages();
                    }
                }, 'json');
            }
            function GetPersonLanguages() {
                $.post('/ForeignAbiturient/GetPersonLanguages', null, function (res) {
                    if (res.Data.length > 0) {
                        var htmldata = '';
                        for (var i = 0; i < res.Data.length; i++) {
                            htmldata += '<tr id="' + res.Data[i].Id + '">';
                            htmldata += '<td>' + res.Data[i].Language + '</td>';
                            htmldata += '<td>' + res.Data[i].Level + '</td>';
                            htmldata += '<td><a class="action-button"><span class="delete" style="cursor:pointer;" onclick="DeleteLang(\'' + res.Data[i].Id + '\')"></span></a></td>';
                            htmldata += '</tr>';
                        }
                        $('#tblLanguages tbody').html(htmldata);
                    }
                }, 'json');
            }
            function GetLanguages() {
                $.post('/ForeignAbiturient/GetLanguages', null, function (res) {
                    var opts = '';
                    for (var i = 0; i < res.length; i++) {
                        opts += '<option value="' + res[i].Id + '">' + res[i].Name + '</option>';
                    }
                    $('#Languages').html(opts);
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
                    <form id="form" class="form panel" action="ForeignAbiturient/NextStep" method="post" onsubmit="return CheckForm();">
                        <h3><%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderEducation").ToString()%></h3>
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
                            <span id="EducationInfo_SchoolName_Message" class="Red" style="display:none"><%= GetGlobalResourceObject("EducationInfo", "EducationInfo_SchoolName_Message").ToString()%></span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolExitYear, GetGlobalResourceObject("EducationInfo", "SchoolExitYear").ToString())%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolExitYear)%>
                            <br />
                            <span id="EducationInfo_SchoolExitYear_Message" class="Red" style="display:none; border-collapse:collapse;"><%= GetGlobalResourceObject("PersonalOfficeForeign", "StudyFinishMessage").ToString()%></span>
                            <span id="EducationInfo_SchoolExitYear_MessageFormat" class="Red" style="display:none; border-collapse:collapse;"><%= GetGlobalResourceObject("EducationInfo", "EducationInfo_SchoolExitYear_MessageFormat").ToString()%></span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.CountryEducId, GetGlobalResourceObject("EducationInfo", "CountryEducId").ToString()) %>
                            <%= Html.DropDownListFor(x => x.EducationInfo.CountryEducId, Model.EducationInfo.CountryList) %>
                        </div>
                        <h4><%= GetGlobalResourceObject("EducationInfo", "EducationDocumentHeader").ToString()%></h4>
                        <hr />
                        <div id="_AttRegion" class="clearfix" style="display:none">
                            <%= Html.LabelFor(x => x.EducationInfo.AttestatRegion, GetGlobalResourceObject("EducationInfo", "AttestatRegion").ToString())%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.AttestatRegion) %>
                            <span id="EducationInfo_AttestatRegion_Message" class="Red" style="display:none; border-collapse:collapse;"><%= GetGlobalResourceObject("EducationInfo", "AttestatRegion_Message").ToString()%></span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.DiplomSeries, GetGlobalResourceObject("EducationInfo", "DiplomSeries").ToString()) %>
                            <%= Html.TextBoxFor(x => x.EducationInfo.DiplomSeries) %>
                            <br />
                            <span id="EducationInfo_DiplomSeries_Message" class="Red" style="display:none"><%= GetGlobalResourceObject("EducationInfo", "DiplomSeries_Message").ToString()%></span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.DiplomNumber, GetGlobalResourceObject("EducationInfo", "DiplomNumber").ToString())%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.DiplomNumber)%>
                            <br />
                            <span id="EducationInfo_DiplomNumber_Message" class="Red" style="display:none"><%= GetGlobalResourceObject("EducationInfo", "DiplomNumber_Message").ToString()%></span>
                        </div>
                        <div id="HEData">
                            <h4><%= GetGlobalResourceObject("EducationInfo", "HEDataHeader").ToString()%></h4>
                            <hr />
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.ProgramName, GetGlobalResourceObject("EducationInfo", "PersonSpecialization").ToString())%>
                                <%= Html.TextBoxFor(x => x.EducationInfo.ProgramName)%>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.PersonQualification, GetGlobalResourceObject("EducationInfo", "PersonQualification").ToString()) %>
                                <%= Html.DropDownListFor(x => x.EducationInfo.PersonQualification, Model.EducationInfo.QualificationList) %>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.HEEntryYear, GetGlobalResourceObject("EducationInfo", "HEEntryYear").ToString()) %>
                                <%= Html.TextBoxFor(x => x.EducationInfo.HEEntryYear) %>
                            </div>
                        </div>
                        
                        </fieldset>
                        <h4><%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderForeignLanguages").ToString()%></h4>
                        <hr />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.HasTRKI, GetGlobalResourceObject("EducationInfo", "HasTRKI").ToString())%>
                            <%= Html.CheckBoxFor(x => x.EducationInfo.HasTRKI)%>
                        </div>
                        <div class="clearfix" id="TRKI">
                            <%= Html.LabelFor(x => x.EducationInfo.TRKICertificateNumber, GetGlobalResourceObject("EducationInfo", "TRKICertificateNumber").ToString())%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.TRKICertificateNumber) %>
                        </div><br /><br />
                        <table id="tblLanguages" class="paginate" style="width: 70%">
                            <thead>
                                <tr>
                                    <th><%= GetGlobalResourceObject("PersonalOfficeForeign", "Language").ToString()%></th>
                                    <th><%= GetGlobalResourceObject("PersonalOfficeForeign", "Level").ToString()%></th>
                                    <th style="width:10%;"><%= GetGlobalResourceObject("Common", "Delete").ToString()%></th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                        <br />
                        <fieldset>
                            <div class="clearfix">
                                <span style="font-size:1.1em;"><%= GetGlobalResourceObject("PersonalOfficeForeign", "Language").ToString()%></span>
                                <%= Html.DropDownList("Languages", Model.EducationInfo.LanguageList) %>
                            </div><br />
                            <div class="clearfix">
                                <span style="font-size:1.1em;"><%= GetGlobalResourceObject("PersonalOfficeForeign", "Level").ToString()%></span>
                                <%= Html.DropDownList("Levels", Model.EducationInfo.LanguageLevelList) %>
                            </div><br />
                            <div class="clearfix">
                                <button onclick="AddLang()" type="button" class="button button-blue">Add</button>
                            </div><br />
                        </fieldset>
                        <hr />
                        <div class="clearfix">
                            <input id="Submit3" type="submit" class="button button-green" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                        </div>
                    </form>
                </div>
                <div class="grid_2">
                    <ol>
                        <li><a href="../../ForeignAbiturient?step=1"><%= GetGlobalResourceObject("PersonInfo", "Step1")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=2"><%= GetGlobalResourceObject("PersonInfo", "Step2")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=3"><%= GetGlobalResourceObject("PersonInfo", "Step3")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=4"><%= GetGlobalResourceObject("PersonInfo", "Step4")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=5"><%= GetGlobalResourceObject("PersonInfo", "Step6")%></a></li>
                        
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
                    <div id="Div1" class="message warning">
                        <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                    </div>
                <% } %>
                    <form class="panel form" action="ForeignAbiturient/NextStep" method="post">
                        <%= Html.ValidationSummary() %>
                        <%= Html.HiddenFor(x => x.Stage) %>
                        <div class="clearfix">
                            <h4><%= GetGlobalResourceObject("PersonalOffice_Step6", "HostelHeader").ToString()%></h4>
                            <%= Html.CheckBoxFor(x => x.AddInfo.HostelAbit)%>
                            <span><%= GetGlobalResourceObject("PersonalOffice_Step6", "HostelAbit").ToString()%></span>
                        </div>
                        <div class="clearfix">
                            <h4><%= GetGlobalResourceObject("PersonalOffice_Step6", "ContactPerson").ToString()%></h4>
                            <span><%= GetGlobalResourceObject("PersonalOffice_Step6", "ContactPerson_SubHeader").ToString()%></span><br />
                            <%= Html.TextAreaFor(x => x.AddInfo.ContactPerson, 5, 70, new Dictionary<string, object>() { { "class", "noresize" } }) %>
                        </div>
                        <div class="clearfix">
                            <h4><%= GetGlobalResourceObject("PersonalOffice_Step6", "ExtraInfo").ToString()%></h4>
                            <%= Html.TextAreaFor(x => x.AddInfo.ExtraInfo, 5, 70, new Dictionary<string, object>() { { "class", "noresize" } })%>
                        </div>
                        <div class="clearfix">
                            <h4><%= GetGlobalResourceObject("PersonalOffice_Step6", "FZ152_Header").ToString()%></h4>
                            <%= Html.CheckBoxFor(x => x.AddInfo.FZ_152Agree) %>
                            <span><%= GetGlobalResourceObject("PersonalOffice_Step6", "FZ_152Agree").ToString()%></span>
                        </div>
                        <span id="FZ" class="Red" style="display:none;"><%= GetGlobalResourceObject("PersonalOffice_Step6", "FZ_152_Message").ToString()%></span>
                        <hr />
                        <div class="clearfix">
                            <input id="Submit4" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonalOffice_Step6", "btnValue_EndRegisration").ToString()%>" />
                        </div>
                    </form>
                </div>
                <div class="grid_2">
                    <ol>
                        <li><a href="../../ForeignAbiturient?step=1"><%= GetGlobalResourceObject("PersonInfo", "Step1")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=2"><%= GetGlobalResourceObject("PersonInfo", "Step2")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=3"><%= GetGlobalResourceObject("PersonInfo", "Step3")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=4"><%= GetGlobalResourceObject("PersonInfo", "Step4")%></a></li>
                        <li><a href="../../ForeignAbiturient?step=5"><%= GetGlobalResourceObject("PersonInfo", "Step6")%></a></li>
                    </ol>
                </div>
            </div>
        </div>
<%
    }
%>
</asp:Content>
