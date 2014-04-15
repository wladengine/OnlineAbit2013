<%@ Import Namespace="OnlineAbit2013" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/SPO/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.PersonalOffice_SPO>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Личный кабинет
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Анкета</h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script>
    $('#UILink').hide();
</script>
<% if (Model.Stage == int.MaxValue)/* типа затычка, чтобы VS видела скрипты */
   {
%>
    <script language="javascript" type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script language="javascript" type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
<% } %>
<% if (Model.Stage == 1)
   {
%>
    <script language="javascript" type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
    <script language="javascript" type="text/javascript" src="../../Scripts/jquery.ui.datepicker-ru.js"></script>
    <script language="javascript" type="text/javascript">
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
                defaultDate: '-15y',
            });
            $.datepicker.regional["ru"];
            <% } %>

            $('#PersonInfo_Surname').keyup( function() { setTimeout(CheckSurname) });
            $('#PersonInfo_Name').keyup( function() { setTimeout(CheckName) });
            $('#PersonInfo_BirthDate').keyup( function() { setTimeout(CheckBirthDate) });
            $('#PersonInfo_BirthPlace').keyup( function() { setTimeout(CheckBirthPlace) });
            $('#PersonInfo_Surname').blur( function() { setTimeout(CheckSurname) });
            $('#PersonInfo_Name').blur( function() { setTimeout(CheckName) });
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
    <script language="javascript" type="text/javascript">
        function CheckSurname() {
            var ret = true;
            if ($('#PersonInfo_Surname').val() == '') {
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
            if ($('#PersonInfo_Name').val() == '') {
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
                    <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString() %>
                </div>
            <% } %>
            <form id="form" class="form panel" action="SPO/NextStep" method="post" onsubmit="return CheckForm();">
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
                    <li><a href="../../SPO?step=1">Личные данные</a></li>
                    <li><a href="../../SPO?step=2">Паспорт</a></li>
                    <li><a href="../../SPO?step=3">Контактная информация</a></li>
                    <li><a href="../../SPO?step=4">Образование</a></li>
                    <li><a href="../../SPO?step=5">Опыт работы</a></li>
                    <li><a href="../../SPO?step=6">Дополнительные сведения</a></li>
                </ol>
            </div>
        </div>
    </div>
<%  }
    if (Model.Stage == 2)
    {
%>
        <script language="javascript" type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
        <script language="javascript" type="text/javascript" src="../../Scripts/jquery.ui.datepicker-ru.js"></script>
        <script language="javascript" type="text/javascript">
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
            function CheckSeries() {
                var ret = true;
                if ($('#PassportInfo_PassportType').val() == '1' && $('#PassportInfo_PassportSeries').val() == '') {
                    ret = false;
                    $('#PassportInfo_PassportSeries').addClass('input-validation-error');
                    $('#PassportInfo_PassportSeries_Message').show();
                }
                else {
                    $('#PassportInfo_PassportSeries').removeClass('input-validation-error');
                    $('#PassportInfo_PassportSeries_Message').hide();
                }
                return ret;
            }
            function CheckNumber() {
                var ret = true;
                if ($('#PassportInfo_PassportNumber').val() == '') {
                    ret = false;
                    $('#PassportInfo_PassportNumber').addClass('input-validation-error');
                    $('#PassportInfo_PassportNumber_Message').show();
                }
                else {
                    $('#PassportInfo_PassportNumber').removeClass('input-validation-error');
                    $('#PassportInfo_PassportNumber_Message').hide();
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
                    <form id="form" class="form panel" action="SPO/NextStep" method="post" onsubmit="return CheckForm();">
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
                        <li><a href="../../SPO?step=1">Личные данные</a></li>
                        <li><a href="../../SPO?step=2">Паспорт</a></li>
                        <li><a href="../../SPO?step=3">Контактная информация</a></li>
                        <li><a href="../../SPO?step=4">Образование</a></li>
                        <li><a href="../../SPO?step=5">Опыт работы</a></li>
                        <li><a href="../../SPO?step=6">Дополнительные сведения</a></li>
                    </ol>
                </div>
            </div>
        </div>
<%
    }
    if (Model.Stage == 3)
    {
%>
        <script type="text/javascript" language="javascript">
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
                    if (countryid == '3') {
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
        <script type="text/javascript" language="javascript">
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
                    <form id="form" class="form panel" action="SPO/NextStep" method="post" onsubmit="return CheckForm();">
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <h3>Контактные телефоны:</h3>
                        <hr />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.MainPhone, GetGlobalResourceObject("ContactsInfo", "MainPhone").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.MainPhone, new Dictionary<string, object>() {{"maxlength", "50"}})%>
                            <br />
                            <span id="ContactsInfo_MainPhone_Message" class="Red" style="display:none">Введите основной номер</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.SecondPhone, GetGlobalResourceObject("ContactsInfo", "SecondPhone").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.SecondPhone, new Dictionary<string, object>() { { "maxlength", "50" } })%>
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
                        <li><a href="../../SPO?step=1">Личные данные</a></li>
                        <li><a href="../../SPO?step=2">Паспорт</a></li>
                        <li><a href="../../SPO?step=3">Контактная информация</a></li>
                        <li><a href="../../SPO?step=4">Образование</a></li>
                        <li><a href="../../SPO?step=5">Опыт работы</a></li>
                        <li><a href="../../SPO?step=6">Дополнительные сведения</a></li>
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
                if (!CheckSchoolAddress()) { ret = false; }
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
            function CheckSchoolAddress() {
                var ret = true;
                if ($('#EducationInfo_SchoolAddress').val() == '') {
                    ret = false;
                    $('#EducationInfo_SchoolAddress').addClass('input-validation-error');
                    $('#EducationInfo_SchoolAddress_Message').show();
                }
                else {
                    $('#EducationInfo_SchoolAddress').removeClass('input-validation-error');
                    $('#EducationInfo_SchoolAddress_Message').hide();
                }
                return ret;
            }

            function CheckAttestatRegion() {
                var ret = true;
                var regex = /^\d{2}$/i;
                if ($('#EducationInfo_AttestatRegion').val() == '') {
                    ret = true;
                }
                else {
                    if (!regex.test($('#EducationInfo_AttestatRegion').val())) {
                        ret = false;
                        $('#EducationInfo_AttestatRegion').addClass('input-validation-error');
                        $('#EducationInfo_AttestatRegion_Message').show();
                    }
                    else {
                        $('#EducationInfo_AttestatRegion').removeClass('input-validation-error');
                        $('#EducationInfo_AttestatRegion_Message').hide();
                    }
                }
                return ret;
            }

            function CheckSchoolYear() {
                var ret = true;
                if ($('#EducationInfo_SchoolYear').val() == '') {
                    ret = false;
                    $('#EducationInfo_SchoolYear').addClass('input-validation-error');
                    $('#EducationInfo_SchoolYear_Message').show();
                }
                else {
                    $('#EducationInfo_SchoolYear').removeClass('input-validation-error');
                    $('#EducationInfo_SchoolYear_Message').hide();
                }
                return ret;
            }
            function OnSchoolTypeChange()
            {
                var val = $('#EducationInfo_SchoolTypeId').val();
                if (val == 1) {//School
                    $('#Attestat').show(100);
                    $('#Diplom').hide(100);
                    $('#SchoolExitClass').show(100);
                }
                else {//others
                    $('#Attestat').hide(100);
                    $('#Diplom').show(100);
                    $('#SchoolExitClass').hide(100);
                }
            }

            var certificateNumber = $("#EgeCert");
			var examName = $("#EgeExam");
			var examMark = $("#EgeMark");
			var allFields = $([]).add($("#EgeCert")).add($("#EgeExam")).add($("#EgeMark"));

            function updateTips(t) {
                $("#validation_info").text(t);
                $("#validation_info").addClass("ui-state-highlight");
                setTimeout(function () {
                    $("#validation_info").removeClass("ui-state-highlight", 1500);
                }, 500);
            }

            function checkLength() {
//                if ($("#EgeCert").val().length > 15 || $("#EgeCert").val().length < 15) {
//                    $("#EgeCert").addClass("ui-state-error");
//                    updateTips("Номер сертификата должен быть 15-значным в формате РР-ХХХХХХХХ-ГГ");
//                    return false;
//                } else {
                    return true;
//                }
            }

            function checkVal() {
                var val = $("#EgeMark").val();
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

            function fIsGIAClick() {
                var val = $('#IsGIA').is(':checked');
                if (val) {
                    $('#EgeCert').val('ГИА-' + <%= Model.EducationInfo.Barcode %>).attr('readonly', 'readonly');
                }
                else {
                    $('#EgeCert').val('').removeAttr('readonly', 'readonly');
                }
            }

            $(function () {
                OnSchoolTypeChange();
                $('#EducationInfo_SchoolTypeId').change(OnSchoolTypeChange);
                $('#IsGIA').change(fIsGIAClick);
                $('form').submit(function () {
                    return CheckForm();
                });
                $("#dialog:ui-dialog").dialog("destroy");
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
                $("#dialog-form").dialog(
                {
                    autoOpen: false,
                    height: 400,
                    width: 350,
                    modal: true,
                    buttons: {
                        "Добавить": function () {
                            var bValid = true;
                            $("#EgeCert").removeClass("ui-state-error");
                            $("#EgeMark").removeClass("ui-state-error");
                            $("#EgeExam").removeClass("ui-state-error");

                            bValid = bValid && checkLength();
                            bValid = bValid && checkVal();
                            if (!$('#IsGIA').is(':checked')){
                                bValid = bValid && checkRegexp($("#EgeCert"), /^\d{2}\-\d{9}\-(09|10|11|12)$/i, "Номер сертификата должен быть 15-значным в формате РР-ХХХХХХХХХ-ГГ");
                            }

                            if (bValid) {
                                //add to DB
                                var parm = new Object();
                                parm["certNumber"] = $("#EgeCert").val();
                                parm["examName"] = $("#EgeExam").val();
                                parm["examValue"] = $("#EgeMark").val();
                                parm["isGia"] = $("#IsGIA").is(':checked');
                                $.post("Abiturient/AddMark", parm, function (res) {
                                    //add to table if ok
                                    if (res.IsOk) {
                                        $("#tblEGEData tbody").append('<tr id="' + res.Data.Id + '">' +
							            '<td>' + res.Data.CertificateNumber + '</td>' +
							            '<td>' + res.Data.ExamName + '</td>' +
							            '<td>' + res.Data.ExamMark + '</td>' +
                                        '<td><span class="link" onclick="DeleteMrk(\'' + res.Data.Id + '\')"><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить оценку" /></span></td>' +
						                '</tr>');
                                        $("#tblEGEData").show();
                                        $("#noMarks").hide();
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
                <% if (!Model.Enabled)
                   { %>
                $('input').attr('readonly', 'readonly');
                $('select').attr('disabled', 'disabled');
                <% } %>
            });
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
                    <form id="form" class="form panel" action="SPO/NextStep" method="post" onsubmit="return CheckForm();">
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
                            <span id="EducationInfo_SchoolName_Message" class="Red" style="display:none">Укажите название</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolCity, GetGlobalResourceObject("EducationInfo", "SchoolAddress").ToString())%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolCity)%>
                            <br />
                            <span id="EducationInfo_SchoolAddress_Message" class="Red" style="display:none">Укажите город</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.CountryEducId, GetGlobalResourceObject("EducationInfo", "CountryEducId").ToString())%>
                            <%= Html.DropDownListFor(x => x.EducationInfo.CountryEducId, Model.EducationInfo.CountryList)%>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolExitYear, "Год окончания")%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolExitYear)%>
                            <br />
                            <span id="EducationInfo_SchoolYear_Message" class="Red" style="display:none">Укажите год окончания</span>
                        </div>
                        <div class="clearfix" id="SchoolExitClass">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolExitClassId, "Оконченный класс")%>
                            <%= Html.DropDownListFor(x => x.EducationInfo.SchoolExitClassId, Model.EducationInfo.SchoolExitClassList)%>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.AvgMark, "Средний балл")%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.AvgMark)%>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.IsExcellent, "Отличник")%>
                            <%= Html.CheckBoxFor(x => x.EducationInfo.IsExcellent)%>
                        </div>
                        
                        <div id="Diplom">
                            <h4>Данные диплома</h4>
                            <hr />
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.DiplomSeries, GetGlobalResourceObject("EducationInfo", "DiplomSeries").ToString()) %>
                                <%= Html.TextBoxFor(x => x.EducationInfo.DiplomSeries)%>
                                <br />
                                <span id="EducationInfo_DiplomSeries_Message" class="Red" style="display:none">Укажите серию документа</span>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.DiplomNumber, GetGlobalResourceObject("EducationInfo", "DiplomNumber").ToString())%>
                                <%= Html.TextBoxFor(x => x.EducationInfo.DiplomNumber)%>
                                <br />
                                <span id="EducationInfo_DiplomNumber_Message" class="Red" style="display:none">Укажите номер документа</span>
                            </div>
                        </div>
                        <div id="Attestat">
                            <h4>Данные аттестата</h4>
                            <hr />
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.AttestatRegion, "Регион") %>
                                <%= Html.TextBoxFor(x => x.EducationInfo.AttestatRegion)%>
                                <br />
                                <span id="EducationInfo_AttestatRegion_Message" class="Red" style="display:none">Регион аттестата должен состоять из двух цифр</span>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.AttestatSeries, GetGlobalResourceObject("EducationInfo", "DiplomSeries").ToString()) %>
                                <%= Html.TextBoxFor(x => x.EducationInfo.AttestatSeries)%>
                                <br />
                                <span id="EducationInfo_AttestatSeries_Message" class="Red" style="display:none">Укажите серию документа</span>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.EducationInfo.AttestatNumber, GetGlobalResourceObject("EducationInfo", "DiplomNumber").ToString())%>
                                <%= Html.TextBoxFor(x => x.EducationInfo.AttestatNumber)%>
                                <br />
                                <span id="EducationInfo_AttestatNumber_Message" class="Red" style="display:none">Укажите номер документа</span>
                            </div>
                        </div>
                        <div id="EGEData" class="clearfix">
                            <h4>Баллы ЕГЭ/ГИА</h4>
                            <hr />
                            <% if (Model.EducationInfo.EgeMarks.Count == 0)
                               { 
                            %>
                                <h6 id="noMarks">Нет баллов</h6>
                                <table id="tblEGEData" class="paginate full" style="display:none;">
                                <thead>
                                <tr>
                                    <th>Номер сертификата</th>
                                    <th>Предмет</th>
                                    <th>Балл</th>
                                    <th></th>
                                </tr>
                                </thead>
                                <tbody>
                                </tbody>
                                </table>
                            <%
                               }
                               else
                               {
                            %>
                            <table id="tblEGEData" class="paginate full">
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
                                <p id="validation_info">Если у ваш оценка по ГИА, поставьте галочку "ГИА"</p>
	                            <hr />
                                <fieldset>
                                    <div id="Cert" class="clearfix">
                                        <label for="EgeCert">Номер сертификата </label><br />
		                                <input type="text" id="EgeCert" /><br />
                                    </div>
                                    <div class="clearfix">
                                        <label for="IsGIA">ГИА</label><br />
		                                <input type="checkbox" id="IsGIA" ><br />
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
                        <li><a href="../../SPO?step=1">Личные данные</a></li>
                        <li><a href="../../SPO?step=2">Паспорт</a></li>
                        <li><a href="../../SPO?step=3">Контактная информация</a></li>
                        <li><a href="../../SPO?step=4">Образование</a></li>
                        <li><a href="../../SPO?step=5">Опыт работы</a></li>
                        <li><a href="../../SPO?step=6">Дополнительные сведения</a></li>
                    </ol>
                </div>
            </div>
        </div>
<%
    }
    if (Model.Stage == 5)//льготы и олимпиады
    {
%>
        <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
        <script type="text/javascript" src="../../Scripts/jquery.ui.datepicker-ru.js"></script>
        <script type="text/javascript">
            $(function () {
                $('#PrivelegeInfo_SportQualificationId').change(function () { setTimeout(fChangeSportQualification); });
            });
            function fChangeSportQualification() {
                var val = $('#PrivelegeInfo_SportQualificationId').val();
                if (val == 44) {
                    $('#dSportQualificationLevel').hide();
                    $('#dOtherSport').show();
                }
                else {
                    $('#dSportQualificationLevel').show();
                    $('#dOtherSport').hide();
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
                <% using (Html.BeginForm("NextStep", "SPO", FormMethod.Post))
                   {
                %>
                    <h3>Опыт работы (практики):</h3>
                    <hr />
                    <div class="form">
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PrivelegeInfo.Stag, "Стаж:") %>
                            <%= Html.TextBoxFor(x => x.PrivelegeInfo.Stag) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PrivelegeInfo.WorkPlace, "Место работы, должность:") %>
                            <%= Html.TextAreaFor(x => x.PrivelegeInfo.WorkPlace, 2, 40, null) %>
                        </div>
                    </div>
                    <h3>Спортивные достижения</h3>
                    <hr />
                    <div class="form">
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PrivelegeInfo.SportQualificationId, "Спорт. квалификация:") %>
                            <%= Html.DropDownListFor(x => x.PrivelegeInfo.SportQualificationId, Model.PrivelegeInfo.SportQualificationList) %>
                        </div>
                        <div id="dSportQualificationLevel" class="clearfix">
                            <%= Html.LabelFor(x => x.PrivelegeInfo.SportQualificationLevel, "Pазряд:") %>
                            <%= Html.TextBoxFor(x => x.PrivelegeInfo.SportQualificationLevel) %>
                        </div>
                        <div id="dOtherSport" class="clearfix" style=" display:none; border-collapse:collapse;">
                            <%= Html.LabelFor(x => x.PrivelegeInfo.SportQualification, "Спорт. квалификация, разряд:") %>
                            <%= Html.TextBoxFor(x => x.PrivelegeInfo.SportQualification) %>
                        </div>
                    </div>
                    <hr style="color:#A6C9E2;" />
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <input id="Submit4" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                    <% } %>
                </div>
                <div class="grid_2">
                    <ol>
                        <li><a href="../../SPO?step=1">Личные данные</a></li>
                        <li><a href="../../SPO?step=2">Паспорт</a></li>
                        <li><a href="../../SPO?step=3">Контактная информация</a></li>
                        <li><a href="../../SPO?step=4">Образование</a></li>
                        <li><a href="../../SPO?step=5">Опыт работы</a></li>
                        <li><a href="../../SPO?step=6">Дополнительные сведения</a></li>
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
                <form class="panel form" action="SPO/NextStep" method="post">
                    <%= Html.ValidationSummary() %>
                    <%= Html.HiddenFor(x => x.Stage) %>
                    <div class="clearfix">
                        <h4>Общежитие</h4>
                        <%= Html.CheckBoxFor(x => x.AddInfo.HostelAbit)%>
                        <span>Нуждаюсь в общежитии на время поступления</span>
                    </div>
                    <div class="clearfix">
                        <h4>Льготы</h4>
                        <%= Html.CheckBoxFor(x => x.AddInfo.HasPrivileges)%>
                        <span>Претендую на льготы(сирота, инвалид, ветеран боевых действий, военнослужащий, лицо, пострадавшее в результате аварии на ЧАЭС и(или) других радиационных катастроф)</span>
                    </div>
                    <div class="clearfix">
                        <h4>Родители (законные представители)</h4>
                        <span>(указать Ф.И.О., степень родства, телефон, моб.телефон, эл.почта)</span><br />
                        <!-- <textarea id="AddPerson_ContactPerson" name="AddPerson.ContactPerson" cols="40" rows="4" class="ui-widget-content ui-corner-all"></textarea> -->
                        <%= Html.TextAreaFor(x => x.AddInfo.ContactPerson, 5, 70, new Dictionary<string, object>() { { "class", "noresize" } })%>
                    </div>
                    <div class="clearfix">
                        <h4>Дополнительное образование:</h4>
                        <!-- <textarea id="AddPerson_ExtraInfo" name="AddPerson.ExtraInfo" cols="40" rows="4"></textarea> -->
                        <%= Html.TextAreaFor(x => x.AddInfo.ExtraInfo, 5, 70, new Dictionary<string, object>(){ { "class", "noresize" } })%>
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
                    <li><a href="../../SPO?step=1">Личные данные</a></li>
                    <li><a href="../../SPO?step=2">Паспорт</a></li>
                    <li><a href="../../SPO?step=3">Контактная информация</a></li>
                    <li><a href="../../SPO?step=4">Образование</a></li>
                    <li><a href="../../SPO?step=5">Опыт работы</a></li>
                    <li><a href="../../SPO?step=6">Дополнительные сведения</a></li>
                </ol>
            </div>
        </div>
    </div>
<%
    }
%>
</asp:Content>
