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
    <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
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

            $('#ContactsInfo_MainPhone').change(function () { setTimeout(CheckPhone); });
            $('#ContactsInfo_City').change(function () { setTimeout(CheckCity); });
            $('#ContactsInfo_House').change(function () { setTimeout(CheckHouse); });
            //get list of names
            $('#ContactsInfo_RegionId').change(function () { setTimeout(GetCities); });
            $('#ContactsInfo_City').blur(function () { setTimeout(GetStreets); });
            $('#ContactsInfo_Street').blur(function () { setTimeout(GetHouses); });
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
    <script type ="text/javascript">
        function GetCities() {
            $.post('../../Abiturient/GetCityNames', { regionId: $('#ContactsInfo_RegionId').val() }, function (data) {
                if (data.IsOk) {
                    $('#ContactsInfo_City').autocomplete({
                        source: data.List
                    });
                }
            }, 'json');
        }
        function GetStreets() {
            $.post('../../Abiturient/GetStreetNames', { regionId: $('#ContactsInfo_RegionId').val(), cityName: $('#ContactsInfo_City').val() }, function (data) {
                if (data.IsOk) {
                    $('#ContactsInfo_Street').autocomplete({
                        source: data.List
                    });
                }
            }, 'json');
        }
        function GetHouses() {
            $.post('../../Abiturient/GetHouseNames', { regionId: $('#ContactsInfo_RegionId').val(), cityName: $('#ContactsInfo_City').val(), streetName: $('#ContactsInfo_Street').val() }, function (data) {
                if (data.IsOk) {
                    $('#ContactsInfo_House').autocomplete({
                        source: data.List
                    });
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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderScriptsContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Анкета</h2>
</asp:Content>
