<%@ Import Namespace="OnlineAbit2013" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.PersonalOffice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Личный кабинет
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% if (1 == 0)/* типа затычка, чтобы VS видела скрипты */
   {
%>
    <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
<% } %>
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
                yearRange: '1920:2001',
                defaultDate: '-17y',
            });
            $.datepicker.regional["ru"];
            <% } %>

            $('#PersonInfo_Surname').keyup(function () { setTimeout(CheckSurname) });
            $('#PersonInfo_Name').keyup(function () { setTimeout(CheckName) });
            $('#PersonInfo_SecondName').keyup(function () { setTimeout(CheckSecondName) });
            $('#PersonInfo_BirthDate').keyup(function () { setTimeout(CheckBirthDate) });
            $('#PersonInfo_BirthPlace').keyup(function () { setTimeout(CheckBirthPlace) });
            $('#PersonInfo_Surname').blur(function () { setTimeout(CheckSurname) });
            $('#PersonInfo_Name').blur(function () { setTimeout(CheckName) });
            $('#PersonInfo_SecondName').blur(function () { setTimeout(CheckSecondName) });
            $('#PersonInfo_BirthDate').blur(function () { setTimeout(CheckBirthDate) });
            $('#PersonInfo_BirthPlace').blur(function () { setTimeout(CheckBirthPlace) });
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
        var PersonInfo_Surname_Message = $('#PersonInfo_Surname_Message').text(); // введите фамилию
        var PersonInfo_Name_Message = $('#PersonInfo_Name_Message').text();       // введите имя
        var regexp = /^[А-Яа-яё\-\'\s]+$/i;
        function CheckSurname() {
            var ret = true;
            var val = $('#PersonInfo_Surname').val().trim();
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
            var val = $('#PersonInfo_Name').val().trim();
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
            if (val != '') {
                if (!regexp.test(val)) {
                    $('#PersonInfo_SecondName_Message').text('Использование латинских символов не допускается');
                    $('#PersonInfo_SecondName_Message').show();
                    $('#PersonInfo_SecondName').addClass('input-validation-error');
                    ret = false;
                }
                else {
                    $('#PersonInfo_SecondName_Message').hide();
                    $('#PersonInfo_SecondName').removeClass('input-validation-error');
                }
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
            <form id="form" class="form panel" action="AbiturientNew/NextStep" method="post" onsubmit="return CheckForm();">
                <h4><%= GetGlobalResourceObject("PersonInfo", "HeaderPersonalInfo").ToString()%></h4>
                <hr />
                <%= Html.ValidationSummary(GetGlobalResourceObject("PersonInfo", "ValidationSummaryHeader").ToString())%>
                <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                <input name="Enabled" type="hidden" value="<%= Model.Enabled %>" />
                <fieldset> 
                    <div class="clearfix">
                        <label for="PersonInfo_Surname" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:PersonInfo, Surname %>"></asp:Literal><asp:Literal ID="Literal2" runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.PersonInfo.Surname)%>
                        <br /><p></p>
                        <span id="PersonInfo_Surname_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_Surname_Message").ToString()%> 
                        </span>
                    </div>
                    <div class="clearfix">
                        <label for="PersonInfo_Name" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal ID="Literal3" runat="server" Text="<%$Resources:PersonInfo, Name %>"></asp:Literal><asp:Literal ID="Literal4" runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.PersonInfo.Name)%>
                        <br /><p></p>
                        <span id="PersonInfo_Name_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_Name_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.SecondName, GetGlobalResourceObject("PersonInfo", "SecondName").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.SecondName)%>
                        <br /><p></p>
                        <span id="PersonInfo_SecondName_Message" class="Red" style="display:none"> 
                        </span>
                    </div>
                    <div class="clearfix">
                        <label for="PersonInfo_Sex" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal ID="Literal5" runat="server" Text="<%$Resources:PersonInfo, Sex %>"></asp:Literal><asp:Literal ID="Literal6" runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.DropDownListFor(x => x.PersonInfo.Sex, Model.PersonInfo.SexList)%>
                    </div>
                    <div class="clearfix">
                        <label for="PersonInfo_BirthDate" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal ID="Literal7" runat="server" Text="<%$Resources:PersonInfo, BirthDate %>"></asp:Literal><asp:Literal ID="Literal8" runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.PersonInfo.BirthDate)%>
                        <br /><p></p>
                        <span id="PersonInfo_BirthDate_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_BirthDate_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <label for="PersonInfo_BirthPlace" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal ID="Literal9" runat="server" Text="<%$Resources:PersonInfo, BirthPlace %>"></asp:Literal><asp:Literal ID="Literal10" runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.PersonInfo.BirthPlace)%>
                        <br /><p></p>
                        <span id="PersonInfo_BirthPlace_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_BirthPlace_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <label for="PersonInfo_Nationality" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal ID="Literal11" runat="server" Text="<%$Resources:PersonInfo, Nationality %>"></asp:Literal><asp:Literal ID="Literal12" runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.DropDownListFor(x => x.PersonInfo.Nationality, Model.PersonInfo.NationalityList)%>
                    </div>
                     <div class="clearfix">
                        <label for="ContactsInfo_CountryId" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal ID="Literal13" runat="server" Text="<%$Resources:PersonInfo, Country %>"></asp:Literal><asp:Literal ID="Literal14" runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.DropDownListFor(x => x.ContactsInfo.CountryId, Model.ContactsInfo.CountryList)%>
                    </div>
                </fieldset>
                <hr />
                <div class="clearfix">
                    <input id="btnSubmit" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                </div>
                <div> 
                <asp:Literal ID="Literal15" runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal> - <asp:Literal ID="Literal16" runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>  
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
