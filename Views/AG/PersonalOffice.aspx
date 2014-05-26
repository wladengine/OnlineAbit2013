<%@ Import Namespace="OnlineAbit2013" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AG/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.PersonalOfficeAG>" %>

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
            <% if (DateTime.Now >= new DateTime(2013, 6, 23, 0, 0, 0))
               { %>
               <div class="message error" style="width:450px;">
                <strong style="font-size:10pt">Внимание! Приём документов в АГ СПбГУ закрыт.</strong>
               </div>
            <% } %>
            <% if (!Model.Enabled)
               { %>
                <div id="Message" class="message warning">
                    <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString() %>
                </div>
            <% } %>
            <form id="form" class="form panel" action="AG/NextStep" method="post" onsubmit="return CheckForm();">
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
                    <li><a href="../../AG?step=1">Личные данные</a></li>
                    <li><a href="../../AG?step=2">Паспорт</a></li>
                    <li><a href="../../AG?step=3">Контактная информация</a></li>
                    <li><a href="../../AG?step=4">Образование</a></li>
                    <li><a href="../../AG?step=5">Льготы</a></li>
                    <li><a href="../../AG?step=6">Дополнительные сведения</a></li>
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
                    <% if (DateTime.Now >= new DateTime(2013, 6, 23, 0, 0, 0))
                       { %>
                       <div class="message error" style="width:450px;">
                        <strong style="font-size:10pt">Внимание! Приём документов в АГ СПбГУ закрыт.</strong>
                       </div>
                    <% } %>
                    <% if (!Model.Enabled)
                       { %>
                        <div id="Message" class="message warning">
                            <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                        </div>
                    <% } %>
                    <form id="form" class="form panel" action="AG/NextStep" method="post" onsubmit="return CheckForm();">
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
                        <li><a href="../../AG?step=1">Личные данные</a></li>
                        <li><a href="../../AG?step=2">Паспорт</a></li>
                        <li><a href="../../AG?step=3">Контактная информация</a></li>
                        <li><a href="../../AG?step=4">Образование</a></li>
                        <li><a href="../../AG?step=5">Льготы</a></li>
                        <li><a href="../../AG?step=6">Дополнительные сведения</a></li>
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
                <% if (DateTime.Now >= new DateTime(2013, 6, 23, 0, 0, 0))
                   { %>
                   <div class="message error" style="width:450px;">
                    <strong style="font-size:10pt">Внимание! Приём документов в АГ СПбГУ закрыт.</strong>
                   </div>
                <% } %>
                <% if (!Model.Enabled)
                   { %>
                    <div id="Message" class="message warning">
                        <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                    </div>
                <% } %>
                    <form id="form" class="form panel" action="AG/NextStep" method="post" onsubmit="return CheckForm();">
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
                        <li><a href="../../AG?step=1">Личные данные</a></li>
                        <li><a href="../../AG?step=2">Паспорт</a></li>
                        <li><a href="../../AG?step=3">Контактная информация</a></li>
                        <li><a href="../../AG?step=4">Образование</a></li>
                        <li><a href="../../AG?step=5">Льготы</a></li>
                        <li><a href="../../AG?step=6">Дополнительные сведения</a></li>
                    </ol>
                </div>
            </div>
        </div>
<%
    }
    if (Model.Stage == 4)
    {
%>
        <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
        <script type="text/javascript">
            function CheckForm() {
                var ret = true;
                if (!CheckSchoolName()) { ret = false; }
                if (!CheckSchoolAddress()) { ret = false; }
                return ret;
            }
            function CheckSchoolName() {
                var ret = true;
                if ($('#PersonSchoolInfo_SchoolName').val() == '') {
                    ret = false;
                    $('#PersonSchoolInfo_SchoolName').addClass('input-validation-error');
                    $('#PersonSchoolInfo_SchoolName_Message').show();
                }
                else {
                    $('#PersonSchoolInfo_SchoolName').removeClass('input-validation-error');
                    $('#PersonSchoolInfo_SchoolName_Message').hide();
                }
                return ret;
            }
            function CheckSchoolAddress() {
                var ret = true;
                if ($('#PersonSchoolInfo_SchoolAddress').val() == '') {
                    ret = false;
                    $('#PersonSchoolInfo_SchoolAddress').addClass('input-validation-error');
                    $('#PersonSchoolInfo_SchoolAddress_Message').show();
                }
                else {
                    $('#PersonSchoolInfo_SchoolAddress').removeClass('input-validation-error');
                    $('#PersonSchoolInfo_SchoolAddress_Message').hide();
                }
                return ret;
            }
            function OnExitClassChange()
            {
                var val = $('#PersonSchoolInfo_SchoolExitClassId').val();
                if (val == 2) {//10 class 
                    $('#Attestat').show(100);
                }
                else {
                    $('#Attestat').hide(100);
                }

            }
            $(function () {
                OnExitClassChange();
                $('#PersonSchoolInfo_SchoolExitClassId').change(OnExitClassChange);
                $('form').submit(function () {
                    return CheckForm();
                });
                <% if (!Model.Enabled)
                   { %>
                $('input').attr('readonly', 'readonly');
                $('select').attr('disabled', 'disabled');
                <% } %>
            });
	</script>
        <div class="grid">
            <div class="wrapper">
                <div class="grid_4 first">
                   <% if (DateTime.Now >= new DateTime(2013, 6, 23, 0, 0, 0))
                       { %>
                       <div class="message error" style="width:450px;">
                        <strong style="font-size:10pt">Внимание! Приём документов в АГ СПбГУ закрыт.</strong>
                       </div>
                    <% } %>
                    <% if (!Model.Enabled)
                       { %>
                        <div id="Message" class="message warning">
                            <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                        </div>
                    <% } %>
                    <form id="form" class="form panel" action="AG/NextStep" method="post" onsubmit="return CheckForm();">
                        <h3>Данные об образовании</h3>
                        <hr />
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <fieldset><br />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PersonSchoolInfo.SchoolTypeId, GetGlobalResourceObject("EducationInfo", "SchoolTypeId").ToString())%>
                            <%= Html.DropDownListFor(x => x.PersonSchoolInfo.SchoolTypeId, Model.PersonSchoolInfo.SchoolTypeList)%>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PersonSchoolInfo.SchoolName, GetGlobalResourceObject("EducationInfo", "SchoolName").ToString())%>
                            <%= Html.TextBoxFor(x => x.PersonSchoolInfo.SchoolName)%>
                            <br />
                            <span id="EducationInfo_SchoolName_Message" class="Red" style="display:none">Укажите название</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PersonSchoolInfo.SchoolAddress, GetGlobalResourceObject("EducationInfo", "SchoolAddress").ToString())%>
                            <%= Html.TextBoxFor(x => x.PersonSchoolInfo.SchoolAddress)%>
                            <br />
                            <span id="EducationInfo_SchoolAddress_Message" class="Red" style="display:none">Укажите адрес</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PersonSchoolInfo.SchoolExitClassId, GetGlobalResourceObject("EducationInfo", "SchoolExitClass").ToString())%>
                            <%= Html.DropDownListFor(x => x.PersonSchoolInfo.SchoolExitClassId, Model.PersonSchoolInfo.SchoolExitClassList)%>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PersonSchoolInfo.CountryEducId, GetGlobalResourceObject("EducationInfo", "CountryEducId").ToString())%>
                            <%= Html.DropDownListFor(x => x.PersonSchoolInfo.CountryEducId, Model.PersonSchoolInfo.CountryList)%>
                        </div>
                        <div id="Attestat">
                            <h4>Документ об образовании (для поступающих в 10 класс)</h4>
                            <hr />
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.PersonSchoolInfo.DiplomSeries, GetGlobalResourceObject("EducationInfo", "DiplomSeries").ToString())%>
                                <%= Html.TextBoxFor(x => x.PersonSchoolInfo.DiplomSeries)%>
                                <br />
                                <span id="EducationInfo_DiplomSeries_Message" class="Red" style="display:none">Укажите серию документа</span>
                            </div>
                            <div class="clearfix">
                                <%= Html.LabelFor(x => x.PersonSchoolInfo.DiplomNumber, GetGlobalResourceObject("EducationInfo", "DiplomNumber").ToString())%>
                                <%= Html.TextBoxFor(x => x.PersonSchoolInfo.DiplomNumber)%>
                                <br />
                                <span id="EducationInfo_DiplomNumber_Message" class="Red" style="display:none">Укажите номер документа</span>
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
                        <li><a href="../../AG?step=1">Личные данные</a></li>
                        <li><a href="../../AG?step=2">Паспорт</a></li>
                        <li><a href="../../AG?step=3">Контактная информация</a></li>
                        <li><a href="../../AG?step=4">Образование</a></li>
                        <li><a href="../../AG?step=5">Льготы</a></li>
                        <li><a href="../../AG?step=6">Дополнительные сведения</a></li>
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
                $('#OlDate').datepicker({
                    changeMonth: true,
                    changeYear: true,
                    showOn: "focus"
                });
            });
            $.datepicker.regional["ru"];
            function CheckOlSeries() {
                if ($('#OlSeries').val() == '') {
                    $('#OlSeries_Message').show();
                    $('#OlSeries').addClass('input-validation-error');
                    return false;
                }
                else {
                    $('#OlSeries_Message').hide();
                    $('#OlSeries').removeClass('input-validation-error');
                    return true;
                }
            }
            function CheckOlNumber() {
                if ($('#OlNumber').val() == '') {
                    $('#OlNumber_Message').show();
                    $('#OlNumber').addClass('input-validation-error');
                    return false;
                }
                else {
                    $('#OlNumber_Message').hide();
                    $('#OlNumber').removeClass('input-validation-error');
                    return true;
                }
            }
            function CheckOlDate() {
                if ($('#OlDate').val() == '') {
                    $('#OlDate_Message').show();
                    $('#OlDate').addClass('input-validation-error');
                    return false;
                }
                else {
                    $('#OlDate_Message').hide();
                    $('#OlDate').removeClass('input-validation-error');
                    return true;
                }
            }
            function AddPrivelege() {
                if ($('#Number').val() == '') {
                    alert('Введите номер документа, подтверждающего льготу');
                    return false;
                }
                var params = new Object();
                var privId = $(":radio[name=Privilege]").filter(":checked").val();
                params['id'] = privId;
                params['docNum'] = $('#Number').val();
                $.post('AG/AddPrivelege', params, function (res) {
                    if (res.IsOk) {
                        var output = '';
                        output += '<tr id=\'' + res.Id + '\'><td style="text-align:center; vertical-align:middle;">';
                        output += res.Type + '</td>';
                        output += '<td style="width:35%;text-align:center; vertical-align:middle;">' + res.Doc + '</td>';
                        output += '<td style="width:10%;text-align:center; vertical-align:middle;"><span class="link" onclick="DeletePrivelege(\'' + res.Id + '\')" ><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить оценку" /><span></td>';
                        output += '</tr>';
                        $('#ScWorks tbody').append(output);
                        $('#Priv' + privId).hide();
                    }
                    else {
                        alert(res.ErrorMessage);
                    }
                }, 'json');
            }
            function DeletePrivelege(id) {
                var param = new Object();
                param['id'] = id;
                $.post('AG/DeletePrivilege', param, function (res) {
                    if (res.IsOk) {
                        $("#" + id).hide(250).html("");
                    }
                    else {
                        alert(res.ErrorMessage);
                    }
                }, 'json');
            }
            function AddOlympiad() {
                var ret = true;
                if (!CheckOlSeries()) { ret = false; }
                if (!CheckOlNumber()) { ret = false; }
                if (!CheckOlDate()) { ret = false; }
                if (!ret)
                    return false;
                var param = new Object();
                param['id'] = $('#PrivelegeInfo_OlympiadId').val();
                param['Series'] = $('#OlSeries').val();
                param['Number'] = $('#OlNumber').val();
                param['Date'] = $('#OlDate').val();
                $.post('AG/AddOlympiad', param, function (res) {
                    if (res.IsOk) {
                        var output = '';
                        output += '<tr id=\'' + res.Id + '\'><td style="text-align:center; vertical-align:middle;">';
                        output += res.Type + '</td>';
                        output += '<td style="width:35%;text-align:center; vertical-align:middle;">' + res.Doc + '</td>';
                        output += '<td style="width:10%;text-align:center; vertical-align:middle;"><span class="link" onclick="DeleteOlympiad(\'' + res.Id + '\')" ><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить оценку" /><span></td>';
                        output += '</tr>';
                        $('#tblOlympiads tbody').append(output);
                    }
                    else {
                        alert(res.ErrorMessage);
                    }
                }, 'json');
            }
            function DeleteOlympiad(id) {
                var param = new Object();
                param['id'] = id;
                $.post('AG/DeleteOlympiad', param, function (res) {
                    if (res.IsOk) {
                        $('#' + id).hide();
                    }
                    else {
                        alert(res.ErrorMessage);
                    }
                }, 'json');
            }
        </script>
        <div class="grid">
            <div class="wrapper">
                <div class="grid_4 first">
                <% if (DateTime.Now >= new DateTime(2013, 6, 23, 0, 0, 0))
                    { %>
                    <div class="message error" style="width:450px;">
                    <strong style="font-size:10pt">Внимание! Приём документов в АГ СПбГУ закрыт.</strong>
                    </div>
                <% } %>
                <% if (!Model.Enabled)
                   { %>
                    <div id="Message" class="message warning">
                        <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                    </div>
                <% } %>
                    <h2>Льготы</h2>
                    <div class="form panel">
                        <h4>Тип льготы</h4>
                        <div id="Privs" class="clearfix">
                            <div id="Priv15" class="clearfix">
                                <input id="Privelege15" type="radio" name="Privilege" value="15" />
                                <span>Дети-сироты и дети, оставшиеся без попечения родителей</span>
                            </div>
                            <div id="Priv16" class="clearfix">
                                <input id="Privelege16" type="radio" name="Privilege" value="16" />
                                <span>Дети-инвалиды I-ой и II-ой группы</span>
                            </div>
                            <div id="Priv17" class="clearfix">
                                <input id="Privelege17" type="radio" name="Privilege" value="17" />
                                <span>Граждане, имеющие только одного родителя - инвалида I группы при среднедушевом доходе семьи ниже прожиточного минимума</span>
                            </div>
                        </div>
                        <br />
                        <h4>Документ</h4>
                        <hr />
                        <div id="pNumber" class="clearfix">
                            <label for="Number">Номер документа</label>
                            <input type="text" id="Number" />
                        </div>
                        <div class="message info">
                            Пожалуйста, не забудьте приложить копию(скан) документа, подтверждающего льготу, в разделе "Общие документы"
                        </div>
                        <hr />
                        <div class="clearfix">
                            <button id="btnAddPrivelege" onclick="AddPrivelege()" class="button button-blue">Добавить</button>
                        </div>
                        <br /><br />
                        <h4>Добавленные льготы</h4>
                        <table id="ScWorks" class="paginate" style="width:100%;">
                            <thead>
                                <tr>
                                    <th style="text-align:center;">Тип</th>
                                    <th style="width:35%;text-align:center;">Документ</th>
                                    <th style="width:10%;text-align:center;">Удалить</th>
                                </tr>
                            </thead>
                            <tbody>
                            <% foreach (var privelege in Model.PrivelegeInfo.pPrivileges)
                                {
                            %>
                                <tr id="<%= privelege.Id.ToString("N") %>">
                                    <td style="text-align:center; vertical-align:middle;"><%= Html.Encode(privelege.PrivilegeType)%></td>
                                    <td style="width:35%;text-align:center; vertical-align:middle;"><%= Html.Encode(privelege.PrivilegeInfo)%></td>
                                    <td style="width:10%; text-align:center; vertical-align:middle;"><%= Html.Raw("<span class=\"link\" onclick=\"DeletePrivelege('" + privelege.Id.ToString("N") + "')\" ><img src=\"../../Content/themes/base/images/delete-icon.png\" alt=\"Удалить\" /></span>")%></td>
                                </tr>
                            <% } %>
                            </tbody>
                        </table>
                    </div>
                    <h2>Олимпиады</h2>
                    <div class="form panel">
                        <div class="clearfix">
                            <%= Html.DropDownListFor(x => x.PrivelegeInfo.OlympiadId, Model.PrivelegeInfo.OlympiadsList, 
                            new SortedList<string, object>() { {"style", "width:460px;"} , {"size", "6"} })%>
                        </div>
                        <div>
                            <h4>Документ</h4>
                            <hr />
                            <div class="clearfix">
                                <label for="OlSeries">Серия</label>
                                <input id="OlSeries" type="text" />
                                <span id="OlSeries_Message" style="display:none; color:Red;">Введите серию</span>
                            </div>
                            <div class="clearfix">
                                <label for="OlNumber">Номер</label>
                                <input id="OlNumber" type="text" />
                                <span id="OlNumber_Message" style="display:none; color:Red;">Введите номер</span>
                            </div>
                            <div class="clearfix">
                                <label for="OlDate">Дата</label>
                                <input id="OlDate" type="text" />
                                <span id="OlDate_Message" style="display:none; color:Red;">Введите дату</span>
                            </div>
                            <hr />
                            <div class="message info">
                                Пожалуйста, не забудьте приложить копию(скан) документа, в разделе "Общие документы"
                            </div>
                            <div class="clearfix">
                                <button id="btnAddOlympiad" onclick="AddOlympiad()" class="button button-blue">Добавить</button>
                            </div>
                            <h4>Добавленные олимпиады</h4>
                            <table id="tblOlympiads" class="paginate" style="width:100%;">
                                <thead>
                                    <tr>
                                        <th style="text-align:center;">Тип</th>
                                        <th style="width:35%;text-align:center;">Документ</th>
                                        <th style="width:10%;text-align:center;">Удалить</th>
                                    </tr>
                                </thead>
                                <tbody>
                                <% foreach (var olympiad in Model.PrivelegeInfo.pOlympiads)
                                    {
                                %>
                                    <tr id="<%= olympiad.Id.ToString("N") %>">
                                        <td style="text-align:center; vertical-align:middle;"><%= Html.Encode(olympiad.OlympType)%></td>
                                        <td style="width:35%;text-align:center; vertical-align:middle;">
                                            <%= Html.Encode(olympiad.DocumentSeries + " " + olympiad.DocumentNumber + " от " + olympiad.DocumentDate.ToShortDateString())%>
                                        </td>
                                        <td style="width:10%; text-align:center; vertical-align:middle;"><%= Html.Raw("<span class=\"link\" onclick=\"DeleteOlympiad('" + olympiad.Id.ToString("N") + "')\" ><img src=\"../../Content/themes/base/images/delete-icon.png\" alt=\"Удалить\" /></span>")%></td>
                                    </tr>
                                <% } %>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <hr style="color:#A6C9E2;" />
                    <% using (Html.BeginForm("NextStep", "Abiturient", FormMethod.Post))
                       {
                    %>
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <input id="Submit4" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                    <% } %>
                </div>
                <div class="grid_2">
                    <ol>
                        <li><a href="../../AG?step=1">Личные данные</a></li>
                        <li><a href="../../AG?step=2">Паспорт</a></li>
                        <li><a href="../../AG?step=3">Контактная информация</a></li>
                        <li><a href="../../AG?step=4">Образование</a></li>
                        <li><a href="../../AG?step=5">Льготы</a></li>
                        <li><a href="../../AG?step=6">Дополнительные сведения</a></li>
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
                <form class="panel form" action="AG/NextStep" method="post">
                    <%= Html.ValidationSummary() %>
                    <%= Html.HiddenFor(x => x.Stage) %>
                    <div class="clearfix">
                        <h4>Общежитие</h4>
                        <%= Html.CheckBoxFor(x => x.AddInfo.HostelAbit)%>
                        <span>Нуждаюсь в общежитии на время поступления</span>
                    </div>
                    <div class="clearfix">
                        <h4>Родители (законные представители)</h4>
                        <span>(указать Ф.И.О., степень родства, телефон, моб.телефон, эл.почта)</span><br />
                        <!-- <textarea id="AddPerson_ContactPerson" name="AddPerson.ContactPerson" cols="40" rows="4" class="ui-widget-content ui-corner-all"></textarea> -->
                        <%= Html.TextAreaFor(x => x.AddInfo.ContactPerson, 5, 70, new SortedList<string, object>() { { "class", "noresize" } }) %>
                    </div>
                    <div class="clearfix">
                        <h4>Дополнительное образование:</h4>
                        <!-- <textarea id="AddPerson_ExtraInfo" name="AddPerson.ExtraInfo" cols="40" rows="4"></textarea> -->
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
                    <li><a href="../../AG?step=1">Личные данные</a></li>
                    <li><a href="../../AG?step=2">Паспорт</a></li>
                    <li><a href="../../AG?step=3">Контактная информация</a></li>
                    <li><a href="../../AG?step=4">Образование</a></li>
                    <li><a href="../../AG?step=5">Льготы</a></li>
                    <li><a href="../../AG?step=6">Дополнительные сведения</a></li>
                </ol>
            </div>
        </div>
    </div>
<%
    }
%>
</asp:Content>
