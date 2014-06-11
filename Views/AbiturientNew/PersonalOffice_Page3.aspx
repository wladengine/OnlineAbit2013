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
                $(function () { setTimeout(ValidateCountry, 50) });
                function ValidateCountry() {
                    var countryid = $('#ContactsInfo_CountryId').val();
                    if (countryid == '193') {
                        $('#Region').show();
                    }
                    else {
                        $('#Region').hide();
                    }
                }
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
            $('#ContactsInfo_PostIndex').change(function () { setTimeout(CheckIndex); }); 
            $('#ContactsInfo_City').change(function () { setTimeout(CheckCity); });
            $('#ContactsInfo_Street').change(function () { setTimeout(CheckStreet); });
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
            if (!CheckIndex()) { res = false; }
            if (!CheckCity()) { res = false; }
            if (!CheckStreet()) { res = false; }
            if (!CheckHouse()) { res = false; }
            return res;
        }
    </script>
    <script type ="text/javascript">
        $(function () { setTimeout(GetCities, 50) });
        function GetCities() {
            if ($('#ContactsInfo_CountryId').val() == '193') {
                $.post('../../AbiturientNew/GetCityNames', { regionId: $('#ContactsInfo_RegionId').val() }, function (data) {
                    if (data.IsOk) {
                        $('#ContactsInfo_City').autocomplete({
                            source: data.List
                        });
                    }
                }, 'json');
            }
        }
        function GetStreets() {
            $.post('../../AbiturientNew/GetStreetNames', { regionId: $('#ContactsInfo_RegionId').val(), cityName: $('#ContactsInfo_City').val() }, function (data) {
                if (data.IsOk) {
                    $('#ContactsInfo_Street').autocomplete({
                        source: data.List
                    });
                }
            }, 'json');
        }
        function GetHouses() {
            $.post('../../AbiturientNew/GetHouseNames', { regionId: $('#ContactsInfo_RegionId').val(), cityName: $('#ContactsInfo_City').val(), streetName: $('#ContactsInfo_Street').val() }, function (data) {
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
                <form id="form" class="form panel" action="AbiturientNew/NextStep" method="post" onsubmit="return CheckForm();">
                    <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                    <h3><%= GetGlobalResourceObject("PersonalOffice_Step3", "PhonesHeader").ToString()%></h3>
                    <hr />
                    <div class="clearfix">
                        <label for="ContactsInfo_MainPhone" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step3, MainPhone%>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.MainPhone) %>
                         <br /><p></p>
                         <span id="ContactsInfo_MainPhone_Message" class="Red" style="display:none"><%= GetGlobalResourceObject("PersonalOffice_Step3", "MainPhone_Message")%></span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.ContactsInfo.SecondPhone, GetGlobalResourceObject("PersonalOffice_Step3", "SecondPhone").ToString())%>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.SecondPhone)%>
                    </div>
                    <h3><%= GetGlobalResourceObject("PersonalOffice_Step3", "RegistrationHeader").ToString()%></h3>
                    <hr />
                    <div class="clearfix">
                        <input type="hidden" name="CountryId" value=" <%=Model.ContactsInfo.CountryId %> "/>
                        <label for="ContactsInfo_CountryId" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step3, CountryId %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.DropDownListFor(x => x.ContactsInfo.CountryId, Model.ContactsInfo.CountryList, new { disabled = "disabled", title = GetGlobalResourceObject("PersonalOffice_Step3", "ChangeCountry_Title").ToString() })%>
                    </div>
                    <div class="clearfix" id="Region">
                        <label for="ContactsInfo_RegionId" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step3, RegionId %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.DropDownListFor(x => x.ContactsInfo.RegionId, Model.ContactsInfo.RegionList) %>
                    </div>
                    <div class="clearfix">
                        <label for="ContactsInfo_PostIndex" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step3, PostIndex %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.PostIndex) %>
                         <br /><p></p>
                        <span id="ContactsInfo_PostIndex_Message" class="Red" style="display:none"> <%= GetGlobalResourceObject("PersonalOffice_Step3", "PostIndex_Message").ToString()%> </span> 
                    </div>
                    <div class="clearfix">
                        <label for="ContactsInfo_City" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step3, City %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.City) %> 
                        <br /><p></p>
                        <span id="ContactsInfo_City_Message" class="Red" style="display:none"><%= GetGlobalResourceObject("PersonalOffice_Step3", "City_Message").ToString()%> </span> 
                    </div>
                    <div class="clearfix">
                        <label for="ContactsInfo_Street" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal  runat="server" Text="<%$Resources:PersonalOffice_Step3, Street %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal></asp:Literal><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:PersonalOffice_Step3, Street_1 %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.Street)%>
                        <br /><p></p>
                        <span id="ContactsInfo_Street_Message" class="Red" style="display:none"><%= GetGlobalResourceObject("PersonalOffice_Step3", "Street_Message").ToString()%> </span>
                    </div>
                    <div class="clearfix">
                        <label for="ContactsInfo_House" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step3, House %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.House) %> 
                        <br /><p></p>
                        <span id="ContactsInfo_House_Message" class="Red" style="display:none"><%= GetGlobalResourceObject("PersonalOffice_Step3", "House_Message").ToString()%> </span> 
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.ContactsInfo.Korpus, GetGlobalResourceObject("PersonalOffice_Step3", "Korpus").ToString())%>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.Korpus) %>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.ContactsInfo.Flat, GetGlobalResourceObject("PersonalOffice_Step3", "Flat").ToString())%>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.Flat) %>
                    </div>
                    <%--<% if ((Model.res == 1) || (Model.res == 3))
                       { %> --%>
                    <h4><%= GetGlobalResourceObject("PersonalOffice_Step3", "AdditionalAddress_Header").ToString()%> </h4>
                    <hr />
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.ContactsInfo.PostIndexReal, GetGlobalResourceObject("PersonalOffice_Step3", "PostIndex").ToString())%>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.PostIndexReal)%>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.ContactsInfo.CityReal, GetGlobalResourceObject("PersonalOffice_Step3", "City").ToString())%>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.CityReal)%>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.ContactsInfo.StreetReal, GetGlobalResourceObject("PersonalOffice_Step3", "Street").ToString())%>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.StreetReal)%>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.ContactsInfo.HouseReal, GetGlobalResourceObject("PersonalOffice_Step3", "House").ToString())%>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.HouseReal)%>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.ContactsInfo.KorpusReal, GetGlobalResourceObject("PersonalOffice_Step3", "Korpus").ToString())%>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.KorpusReal)%>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.ContactsInfo.FlatReal, GetGlobalResourceObject("PersonalOffice_Step3", "Flat").ToString())%>
                        <%= Html.TextBoxFor(x => x.ContactsInfo.FlatReal)%>
                    </div>
                    <%--<% } %>--%>
                    <hr />
                    <div class="clearfix">
                        <input id="Submit2" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                    </div>
                    <div> 
                    <asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal> - <asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>  
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
