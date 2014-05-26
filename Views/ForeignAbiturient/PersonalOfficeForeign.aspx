<%@ Page Title="" Language="C#" MasterPageFile="~/Views/ForeignAbiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.ForeignPersonModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderPersonalOffice").ToString()%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderPersonalOffice").ToString()%></h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% if (1 == 0)
   { %>
   <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
<% } %>
    <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
    
<% if (Model.IsLocked)
   { %>
    <script type="text/javascript">
        $(function () {
            $('input').attr('readonly', 'readonly');
            $('textarea').attr('readonly', 'readonly');
            $('select').attr('disabled', 'disabled');
        });
        function AddLang() { }
        function DeleteLang(id) { }
    </script>
<% }
   else { %>
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
       $(function () {
           GetPersonLanguages();
           GetLanguages();
           $('#BirthDate').datepicker({
               changeMonth: true,
               changeYear: true,
               showOn: "focus",
               yearRange: '1920:2000',
               maxDate: "+1D",
               defaultDate: '-18y'
           });
           $('#PassportDate').datepicker({
               changeMonth: true,
               changeYear: true,
               showOn: "focus",
               yearRange: '2000:2012',
               maxDate: "+1D",
               defaultDate: '-2y'
           });
           $('#PassportExpire').datepicker({
               changeMonth: true,
               changeYear: true,
               showOn: "focus",
               yearRange: '2012:2020',
               minDate: "+1D",
               defaultDate: '+1y'
           });
           $('#StudyStart').datepicker({
               changeMonth: true,
               changeYear: true,
               showOn: "focus",
               yearRange: '1920:2011',
               maxDate: "+1D",
               defaultDate: '-3y'
           });
           $('#StudyFinish').datepicker({
               changeMonth: true,
               changeYear: true,
               showOn: "focus",
               yearRange: '1920:2012',
               maxDate: "+1D",
               defaultDate: '-1D'
           });


           $('form').submit(function () { return CheckForm(); });

           $('#Surname').keyup(function () { setTimeout(CheckSurname); });
           $('#Name').keyup(function () { setTimeout(CheckName); });
           $('#BirthDate').keyup(function () { setTimeout(CheckBirthDate); });
           $('#BirthPlace').keyup(function () { setTimeout(CheckBirthPlace); });
           $('#PassportNumber').keyup(function () { setTimeout(CheckPassportNumber); });
           $('#PassportDate').keyup(function () { setTimeout(CheckPassportDate); });
           //$('#PassportExpire').keyup(function () { setTimeout(CheckPassportExpire); });
           //$('#VisaCountryName').keyup(function () { setTimeout(CheckVisaCountryName); });
           //$('#VisaTownName').keyup(function () { setTimeout(CheckVisaTownName); });
           //$('#VisaPostAddress').keyup(function () { setTimeout(CheckVisaPostAddress); });
           $('#Address').keyup(function () { setTimeout(CheckAddress); });
           $('#Phone').keyup(function () { setTimeout(CheckPhone); });
           $('#StudyPlace').keyup(function () { setTimeout(CheckStudyPlace); });
           $('#StudyStart').keyup(function () { setTimeout(CheckStudyStart); });
           //$('#StudyFinish').keyup(function () { setTimeout(CheckStudyFinish); });

           $('#Surname').blur(function () { setTimeout(CheckSurname); });
           $('#Name').blur(function () { setTimeout(CheckName); });
           $('#BirthDate').blur(function () { setTimeout(CheckBirthDate); });
           $('#BirthPlace').blur(function () { setTimeout(CheckBirthPlace); });
           $('#PassportNumber').blur(function () { setTimeout(CheckPassportNumber); });
           $('#PassportDate').blur(function () { setTimeout(CheckPassportDate); });
           //$('#PassportExpire').blur(function () { setTimeout(CheckPassportExpire); });
           //$('#VisaCountryName').blur(function () { setTimeout(CheckVisaCountryName); });
           //$('#VisaTownName').blur(function () { setTimeout(CheckVisaTownName); });
           //$('#VisaPostAddress').blur(function () { setTimeout(CheckVisaPostAddress); });
           $('#Address').blur(function () { setTimeout(CheckAddress); });
           $('#Phone').blur(function () { setTimeout(CheckPhone); });
           $('#StudyPlace').blur(function () { setTimeout(CheckStudyPlace); });
           $('#StudyStart').blur(function () { setTimeout(CheckStudyStart); });
           //$('#StudyFinish').blur(function () { setTimeout(CheckStudyFinish); });
       });



       function CheckForm() {
           var res = true;
           if (!CheckSurname()) { res = false; }
           if (!CheckName()) { res = false; }
           if (!CheckBirthDate()) { res = false; }
           if (!CheckBirthPlace()) { res = false; }
           if (!CheckPassportNumber()) { res = false; }
           if (!CheckPassportDate()) { res = false; }
           //if (!CheckPassportExpire()) { res = false; }
           if (!CheckVisaCountryName()) { res = false; }
           if (!CheckVisaTownName()) { res = false; }
           if (!CheckVisaPostAddress()) { res = false; }
           if (!CheckAddress()) { res = false; }
           if (!CheckPhone()) { res = false; }
           if (!CheckStudyPlace()) { res = false; }
           if (!CheckStudyStart()) { res = false; }
           if (!CheckAgree()) { res = false; }

           //if (!CheckStudyFinish()) { res = false; }
           return res;
       }
       function CheckSurname() {
           var val = $('#Surname').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#Surname')._addClass('input-validation-error');
               $('#SurnameMessage').show();
           }
           else {
               res = true;
               $('#Surname')._removeClass('input-validation-error');
               $('#SurnameMessage').hide();
           }
           return res;
       }
       function CheckName() {
           var val = $('#Name').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#Name')._addClass('input-validation-error');
               $('#NameMessage').show();
           }
           else {
               res = true;
               $('#Name')._removeClass('input-validation-error');
               $('#NameMessage').hide();
           }
           return res;
       }
       function CheckBirthDate() {
           var val = $('#BirthDate').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#BirthDate')._addClass('input-validation-error');
               $('#BirthDateMessage').show();
           }
           else {
               res = true;
               $('#BirthDate')._removeClass('input-validation-error');
               $('#BirthDateMessage').hide();
           }
           return res;
       }
       function CheckBirthPlace() {
           var val = $('#BirthPlace').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#BirthPlace')._addClass('input-validation-error');
               $('#BirthPlaceMessage').show();
           }
           else {
               res = true;
               $('#BirthPlace')._removeClass('input-validation-error');
               $('#BirthPlaceMessage').hide();
           }
           return res;
       }
       function CheckPassportNumber() {
           var val = $('#PassportNumber').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#PassportNumber')._addClass('input-validation-error');
               $('#PassportNumberMessage').show();
           }
           else {
               res = true;
               $('#PassportNumber')._removeClass('input-validation-error');
               $('#PassportNumberMessage').hide();
           }
           return res;
       }
       function CheckPassportDate() {
           var val = $('#PassportDate').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#PassportDate')._addClass('input-validation-error');
               $('#PassportDateMessage').show();
           }
           else {
               res = true;
               $('#PassportDate')._removeClass('input-validation-error');
               $('#PassportDateMessage').hide();
           }
           return res;
       }
       function CheckPassportExpire() {
           var val = $('#PassportExpire').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#PassportExpire')._addClass('input-validation-error');
               $('#PassportExpireMessage').show();
           }
           else {
               res = true;
               $('#PassportExpire')._removeClass('input-validation-error');
               $('#PassportExpireMessage').hide();
           }
           return res;
       }
       function CheckVisaCountryName() {
           var val = $('#VisaCountryName').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#VisaCountryName')._addClass('input-validation-error');
               $('#VisaCountryNameMessage').show();
           }
           else {
               res = true;
               $('#VisaCountryName')._removeClass('input-validation-error');
               $('#VisaCountryNameMessage').hide();
           }
           return res;
       }
       function CheckVisaTownName() {
           var val = $('#VisaTownName').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#VisaTownName')._addClass('input-validation-error');
               $('#VisaTownNameMessage').show();
           }
           else {
               res = true;
               $('#VisaTownName')._removeClass('input-validation-error');
               $('#VisaTownNameMessage').hide();
           }
           return res;
       }
       function CheckVisaPostAddress() {
           var val = $('#VisaPostAddress').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#VisaPostAddress')._addClass('input-validation-error');
               $('#VisaPostAddressMessage').show();
           }
           else {
               res = true;
               $('#VisaPostAddress')._removeClass('input-validation-error');
               $('#VisaPostAddressMessage').hide();
           }
           return res;
       }
       function CheckAddress() {
           var val = $('#Address').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#Address')._addClass('input-validation-error');
               $('#AddressMessage').show();
           }
           else {
               res = true;
               $('#Address')._removeClass('input-validation-error');
               $('#AddressMessage').hide();
           }
           return res;
       }
       function CheckPhone() {
           var val = $('#Phone').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#Phone')._addClass('input-validation-error');
               $('#PhoneMessage').show();
           }
           else {
               res = true;
               $('#Phone')._removeClass('input-validation-error');
               $('#PhoneMessage').hide();
           }
           return res;
       }
       function CheckStudyPlace() {
           var val = $('#StudyPlace').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#StudyPlace')._addClass('input-validation-error');
               $('#StudyPlaceMessage').show();
           }
           else {
               res = true;
               $('#StudyPlace')._removeClass('input-validation-error');
               $('#StudyPlaceMessage').hide();
           }
           return res;
       }
       function CheckStudyStart() {
           var val = $('#StudyStart').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#StudyStart')._addClass('input-validation-error');
               $('#StudyStartMessage').show();
           }
           else {
               res = true;
               $('#StudyStart')._removeClass('input-validation-error');
               $('#StudyStartMessage').hide();
           }
           return res;
       }
       function CheckStudyFinish() {
           var val = $('#Surname').val();
           var res = true;
           if (val == '') {
               res = false;
               $('#StudyFinish')._addClass('input-validation-error');
               $('#StudyFinishMessage').show();
           }
           else {
               res = true;
               $('#StudyFinish')._removeClass('input-validation-error');
               $('#StudyFinishMessage').hide();
           }
           return res;
       }
       function CheckAgree() {
           if ($('#Agree').is(':checked')) {
               $('AgreeMsg').hide();
               return true;
           }
           else {
               $('#AgreeMsg').show();
               return false;
           }
       }
    </script>
<% } %>
<h2><%= GetGlobalResourceObject("PersonalOfficeForeign", "FormMainHeader").ToString()%></h2>
<form action="/ForeignAbiturient/PersonalOffice" method="post" class="form">
    <% if (Model.IsLocked)
       { %>
        <div id="Message" class="message warning">
            <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
        </div>
    <% } %>
    <%= Html.ValidationSummary(GetGlobalResourceObject("PersonInfo", "ValidationSummaryHeader").ToString())%>
    <fieldset>
        <h4><%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderPersonalInformation").ToString()%></h4>
        <hr />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.Surname, GetGlobalResourceObject("PersonalOfficeForeign", "Surname").ToString())%>
            <%= Html.TextBoxFor(x => x.Surname) %>
            <br />
            <span id="SurnameMessage" class="Red" style="display:none;"><%= GetGlobalResourceObject("PersonalOfficeForeign", "SurnameMessage").ToString()%></span>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.Name, GetGlobalResourceObject("PersonalOfficeForeign", "Name").ToString())%>
            <%= Html.TextBoxFor(x => x.Name)%>
            <br />
            <span id="NameMessage" class="Red" style="display:none;"><%= GetGlobalResourceObject("PersonalOfficeForeign", "NameMessage").ToString()%></span>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.NationalityId, GetGlobalResourceObject("PersonalOfficeForeign", "NationalityId").ToString())%>
            <%= Html.DropDownListFor(m => m.NationalityId, Model.lCountries)%>
        </div><br />
        <div class="clearfix">
            <label><%= GetGlobalResourceObject("PersonalOfficeForeign", "Sex").ToString()%></label>
            <span class="radio-input"><%= Html.RadioButtonFor(x => x.Sex, Model.Sex) %>
            <%--<input type="radio" name="Sex" value="True" <%= Model.Sex ? checked="checked" : "" %>  />--%>
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "SexMale").ToString()%></span>
            <span class="radio-input">
            <%= Html.RadioButtonFor(x => x.Sex, !Model.Sex) %>
            <%--<input type="radio" name="Sex" value="False" />--%>
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "SexFemale").ToString()%></span>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.BirthDate, GetGlobalResourceObject("PersonalOfficeForeign", "BirthDate").ToString())%>
            <%= Html.TextBoxFor(x => x.BirthDate) %>
            <br />
            <span id="BirthDateMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "BirthDateMessage").ToString()%>
            </span>
        </div><br /> 
        <div class="clearfix">
            <%= Html.LabelFor(x => x.BirthPlace, GetGlobalResourceObject("PersonalOfficeForeign", "BirthPlace").ToString())%>
            <%= Html.TextBoxFor(x => x.BirthPlace)%>
            <br />
            <span id="BirthPlaceMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "BirthPlaceMessage").ToString()%>
            </span>
        </div>
        <br />
        <h4><%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderPassport").ToString()%></h4>
        <hr />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.PassportSeries, GetGlobalResourceObject("PersonalOfficeForeign", "PassportSeries").ToString())%>
            <%= Html.TextBoxFor(x => x.PassportSeries)%>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.PassportNumber, GetGlobalResourceObject("PersonalOfficeForeign", "PassportNumber").ToString())%>
            <%= Html.TextBoxFor(x => x.PassportNumber)%>
            <br />
            <span id="PassportNumberMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "PassportNumberMessage").ToString()%>
            </span>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.PassportDate, GetGlobalResourceObject("PersonalOfficeForeign", "PassportDate").ToString())%>
            <%= Html.TextBoxFor(x => x.PassportDate)%>
            <br />
            <span id="PassportDateMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "PassportDateMessage").ToString()%>
            </span>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.PassportExpire, GetGlobalResourceObject("PersonalOfficeForeign", "PassportExpire").ToString())%>
            <%= Html.TextBoxFor(x => x.PassportExpire)%>
            <br />
            <span id="PassportExpireMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "PassportExpireMessage").ToString()%>
            </span>
        </div><br />
        <h4><%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderVisa").ToString()%></h4>
        <hr />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.VisaCountryName, GetGlobalResourceObject("PersonalOfficeForeign", "VisaCountryName").ToString())%>
            <%= Html.TextBoxFor(x => x.VisaCountryName) %>
            <br />
            <span id="VisaCountryNameMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "VisaCountryNameMessage").ToString()%>
            </span>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.VisaTownName, GetGlobalResourceObject("PersonalOfficeForeign", "VisaTownName").ToString())%>
            <%= Html.TextBoxFor(x => x.VisaTownName) %>
            <br />
            <span id="VisaTownNameMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "VisaTownNameMessage").ToString()%>
            </span>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.VisaPostAddress, GetGlobalResourceObject("PersonalOfficeForeign", "VisaPostAddress").ToString())%>
            <%= Html.TextBoxFor(x => x.VisaPostAddress)%>
            <br />
            <span id="VisaPostAddressMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "VisaPostAddressMessage").ToString()%>
            </span>
        </div><br />
        <h4><%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderAddress").ToString()%></h4>
        <hr />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.Address, GetGlobalResourceObject("PersonalOfficeForeign", "Address").ToString())%>
            <%= Html.TextAreaFor(x => x.Address, 5, 70, new SortedList<string, object>() { { "class", "noresize" } })%>
            <br />
            <span id="AddressMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "AddressMessage").ToString()%>
            </span>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.Phone, GetGlobalResourceObject("PersonalOfficeForeign", "Phone").ToString())%>
            <%= Html.TextBoxFor(x => x.Phone) %>
            <br />
            <span id="PhoneMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "PhoneMessage").ToString()%>
            </span>
        </div><br />
        <h4><%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderEducation").ToString()%></h4>
        <hr />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.StudyLevelId, GetGlobalResourceObject("PersonalOfficeForeign", "StudyLevelId").ToString())%>
            <%= Html.DropDownListFor(x => x.StudyLevelId, Model.lStudyLevels) %>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.StudyPlace, GetGlobalResourceObject("PersonalOfficeForeign", "StudyPlace").ToString())%>
            <%= Html.TextBoxFor(x => x.StudyPlace) %>
            <br />
            <span id="StudyPlaceMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "StudyPlaceMessage").ToString()%>
            </span>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.StudyStart, GetGlobalResourceObject("PersonalOfficeForeign", "StudyStart").ToString())%>
            <%= Html.TextBoxFor(x => x.StudyStart)%>
            <br />
            <span id="StudyStartMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "StudyStartMessage").ToString()%>
            </span>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.StudyFinish, GetGlobalResourceObject("PersonalOfficeForeign", "StudyFinish").ToString())%>
            <%= Html.TextBoxFor(x => x.StudyFinish)%>
            <br />
            <span id="StudyFinishMessage" class="Red" style="display:none;">
            <%= GetGlobalResourceObject("PersonalOfficeForeign", "StudyFinishMessage").ToString()%>
            </span>
        </div><br />
        <div class="clearfix">
            <%= Html.LabelFor(x => x.ObtainingLevelId, GetGlobalResourceObject("PersonalOfficeForeign", "ObtainingLevelId").ToString())%>
            <%= Html.DropDownListFor(x => x.ObtainingLevelId, Model.lStudyLevels) %>
        </div><br />
    </fieldset>
    <h4><%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderForeignLanguages").ToString()%></h4>
    <hr />
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
            <select id="Languages">
            <% foreach (var lang in Model.dicLanguages) { %>
                <option value="<%= lang.Key.ToString() %>"><%= lang.Value %></option>
            <% } %>
            </select>
        </div><br />
        <div class="clearfix">
            <span style="font-size:1.1em;"><%= GetGlobalResourceObject("PersonalOfficeForeign", "Level").ToString()%></span>
            <%= Html.DropDownList("Levels", Model.lLanguageLevels) %>
        </div><br />
        <div class="clearfix">
            <button onclick="AddLang()" type="button" class="button button-blue">Add</button>
        </div><br />
    </fieldset>
    <br />
    <h4><%= GetGlobalResourceObject("PersonalOfficeForeign", "HeaderJob").ToString()%></h4>
    <hr />
    <div class="clearfix">
        <%= Html.TextAreaFor(x => x.Works, 8, 100, new SortedList<string, object>() { { "class", "noresize" } })%>
    </div>
    <hr />
    <div class="clearfix" style=" font-size: 1.3em;">
        <asp:Label Text="<%$Resources:PersonalOfficeForeign, HeaderPrivacyTerms%>" runat="server"></asp:Label>
    </div>
    <br /><br />
    <fieldset>
        <div class="clearfix">
            <%= Html.CheckBoxFor(x => x.Agree) %>
            <span style="font-size: 1.1em;"><%= GetGlobalResourceObject("PersonalOfficeForeign", "Agree").ToString()%></span>
        </div>
        <div id="AgreeMsg" class="message error" style="display:none;">
            <span class="Red"><%= GetGlobalResourceObject("PersonalOfficeForeign", "AgreeError").ToString()%></span>
        </div>
    </fieldset>
    <hr />
    <input type="submit" class="button button-green" value="<%= GetGlobalResourceObject("PersonalOfficeForeign", "btnRegister").ToString()%>" />
</form>

</asp:Content>
