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
    <script type="text/javascript" src="../../Scripts/jquery.ui.datepicker-ru.js"></script>
    <script type="text/javascript">
        $(function () { setTimeout(ChangeNationality, 50) });
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
            
            $('#PersonInfo_Nationality').change(function () { setTimeout(ChangeNationality) });
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
        function ChangeNationality(){
            if ($('#PersonInfo_Nationality').val() != '193') {
                        $('#_HasRussianNationality').show();
                    }
                    else {
                        $('#_HasRussianNationality').hide();
                    }
        }

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

        var regexp = /^[A-Za-z\А-Яа-яё\-\'\s]+$/i;
        function CheckSurname() {
            var ret = true;
            var val = $('#PersonInfo_Surname').val().trim();
            var PersonInfo_Surname_Message = $('#PersonInfo_Surname_Message_Hidden').text();
            if (val == '') {
                ret = false;
                $('#PersonInfo_Surname').addClass('input-validation-error');
                $('#PersonInfo_Surname_Message').text(PersonInfo_Surname_Message);
                $('#PersonInfo_Surname_Message').show();
            }
            else {
                $('#PersonInfo_Surname').removeClass('input-validation-error');
                $('#PersonInfo_Surname_Message').hide();
                if (!regexp.test(val)) {
                    ret = false;
                    var text = $('#MessageLatinSymbols').text();
                    $('#PersonInfo_Surname_Message').text(text);
                    $('#PersonInfo_Surname_Message').show();
                    $('#PersonInfo_Surname').addClass('input-validation-error');
                }
                else {
                    if (val.length > <%=Model.ConstInfo.Surname %>) {
                        var text = $('#MessageMaxLength').text();
                        $('#PersonInfo_Surname_Message').text(text);
                        $('#PersonInfo_Surname_Message').show();
                        $('#PersonInfo_Surname').addClass('input-validation-error');
                        ret=false;
                    }
                    else {
                        $('#PersonInfo_Surname_Message').hide();
                        $('#PersonInfo_Surname').removeClass('input-validation-error');
                    }
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
                var PersonInfo_Name_Message = $('#PersonInfo_Name_Message_Hidden').text(); 
                $('#PersonInfo_Name_Message').text(PersonInfo_Name_Message);
                $('#PersonInfo_Name_Message').show();
            }
            else {
                $('#PersonInfo_Name').removeClass('input-validation-error');
                $('#PersonInfo_Name_Message').hide();
                if (!regexp.test(val)) {
                    var text = $('#MessageLatinSymbols').text();
                    $('#PersonInfo_Name_Message').text(text);
                    $('#PersonInfo_Name_Message').show();
                    $('#PersonInfo_Name').addClass('input-validation-error');
                    ret = false;
                }
                else {
                    if (val.length > <%=Model.ConstInfo.Name %>) { 
                        var text = $('#MessageMaxLength').text();
                        $('#PersonInfo_Name_Message').text(text);
                        $('#PersonInfo_Name_Message').show();
                        $('#PersonInfo_Name').addClass('input-validation-error');
                        ret = false;
                    }
                    else {
                        $('#PersonInfo_Name_Message').hide();
                        $('#PersonInfo_Name').removeClass('input-validation-error');
                    }
                }
            }
            return ret;
        }
        function CheckSecondName() {
            var val = $('#PersonInfo_SecondName').val();
            if (val != '') {
                if (!regexp.test(val)) {
                    var text = $('#MessageLatinSymbols').text();
                    $('#PersonInfo_SecondName_Message').text(text);
                    $('#PersonInfo_SecondName_Message').show();
                    $('#PersonInfo_SecondName').addClass('input-validation-error');
                    ret = false;
                }
                else {
                    if (val.length > <%=Model.ConstInfo.SecondName %>) { 
                        var text = $('#MessageMaxLength').text();
                        $('#PersonInfo_SecondName_Message').text(text);
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
            else{
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
                var PersonInfo_BirthPlace_Message =$('#PersonInfo_BirthPlace_Message_Hidden').text();
                $('#PersonInfo_BirthPlace_Message').text(PersonInfo_BirthPlace_Message);                
                $('#PersonInfo_BirthPlace_Message').show();
                $('#PersonInfo_BirthPlace').addClass('input-validation-error');
            }
            else {
                 if ($('#PersonInfo_BirthPlace').val().length > <%=Model.ConstInfo.BirthPlace %>) { 
                    ret= false;
                    var text = $('#MessageMaxLength').text();
                    $('#PersonInfo_BirthPlace_Message').text(text);
                    $('#PersonInfo_BirthPlace').addClass('input-validation-error');
                    $('#PersonInfo_BirthPlace_Message').show();
                 }
                 else
                 {
                    $('#PersonInfo_BirthPlace_Message').text(PersonInfo_BirthPlace_Message);
                    $('#PersonInfo_BirthPlace').removeClass('input-validation-error');
                    $('#PersonInfo_BirthPlace_Message').hide();
                 }
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
            <span id = "MessageLatinSymbols" style="display: none;"><%= GetGlobalResourceObject("PersonInfo", "MessageLatinSymbols").ToString()%></span>
            <span id = "MessageMaxLength" style="display: none;"><%= GetGlobalResourceObject("PersonInfo", "MessageMaxLength").ToString()%></span>



            <form id="form" class="form panel" action="AbiturientNew/NextStep" method="post" onsubmit="return CheckForm();">
                <h3><%= GetGlobalResourceObject("PersonalOffice_Step1", "HeaderPersonalInfo").ToString()%></h3>
                <hr />
                <%= Html.ValidationSummary(GetGlobalResourceObject("PersonInfo", "ValidationSummaryHeader").ToString())%>
                <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                <input name="Enabled" type="hidden" value="<%= Model.Enabled %>" />
                <fieldset> 
                    <div class="clearfix">
                        <label for="PersonInfo_Surname" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step1, Surname %>"></asp:Literal><asp:Literal  runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.PersonInfo.Surname)%>
                        <br /><p></p>
                        <span id="PersonInfo_Surname_Message_Hidden"  style="display:none">
                            <%= GetGlobalResourceObject("PersonalOffice_Step1", "PersonInfo_Surname_Message").ToString()%> 
                        </span>
                        <span id="PersonInfo_Surname_Message" class="Red" style="display:none"> 
                        </span>
                    </div>
                    <div class="clearfix">
                        <label for="PersonInfo_Name" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step1, Name %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.PersonInfo.Name)%>
                        <br /><p></p>
                        <span id="PersonInfo_Name_Message_Hidden" style="display:none">
                            <%= GetGlobalResourceObject("PersonalOffice_Step1", "PersonInfo_Name_Message").ToString()%>
                        </span>
                        <span id="PersonInfo_Name_Message" class="Red" style="display:none"> 
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.SecondName, GetGlobalResourceObject("PersonalOffice_Step1", "SecondName").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.SecondName)%>
                        <br /><p></p>
                        <span id="PersonInfo_SecondName_Message" class="Red" style="display:none"> 
                        </span>
                    </div>
                    <div class="clearfix">
                        <label for="PersonInfo_Sex" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step1, Sex %>"></asp:Literal><asp:Literal  runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.DropDownListFor(x => x.PersonInfo.Sex, Model.PersonInfo.SexList)%>
                    </div>
                    <div class="clearfix">
                        <label for="PersonInfo_BirthDate" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step1, BirthDate %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.PersonInfo.BirthDate)%>
                        <br /><p></p>
                        <span id="PersonInfo_BirthDate_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonalOffice_Step1", "PersonInfo_BirthDate_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <label for="PersonInfo_BirthPlace" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal  runat="server" Text="<%$Resources:PersonalOffice_Step1, BirthPlace %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.TextBoxFor(x => x.PersonInfo.BirthPlace)%>
                        <br /><p></p>
                        <span id="PersonInfo_BirthPlace_Message_Hidden" style="display:none">
                            <%= GetGlobalResourceObject("PersonalOffice_Step1", "PersonInfo_BirthPlace_Message").ToString()%>
                        </span><br />
                        <span id="PersonInfo_BirthPlace_Message" class="Red" style="display:none"> 
                        </span>
                    </div>
                    <div class="clearfix">
                        <label for="PersonInfo_Nationality" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step1, Nationality %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.DropDownListFor(x => x.PersonInfo.Nationality, Model.PersonInfo.NationalityList)%>
                    </div>
                    <div id="_HasRussianNationality" style="display:none;">
                         <label for="PersonInfo_HasRussianNationality" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step1, HasRussianNationality %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.CheckBoxFor(x => x.PersonInfo.HasRussianNationality)%>
                    </div>
                    <div class="clearfix">
                        <label for="ContactsInfo_CountryId" title='<asp:Literal runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>'> 
                        <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step1, Country %>"></asp:Literal><asp:Literal runat="server" Text="<%$Resources:PersonInfo, Star %>"></asp:Literal>
                        </label>
                        <%= Html.DropDownListFor(x => x.ContactsInfo.CountryId, Model.ContactsInfo.CountryList)%>
                   </div>
                </fieldset>
                <hr />
                <div class="clearfix">
                    <input id="btnSubmit" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                </div>
                <div> 
                <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step1, Star %>"></asp:Literal> - <asp:Literal  runat="server" Text="<%$ Resources:PersonInfo, RequiredField%>"></asp:Literal>  
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
