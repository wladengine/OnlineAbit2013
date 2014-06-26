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
        $(function () {

            $('#WorkPlace').keyup(function () {
                var str = $('#WorkPlace').val();
                if (str != "") {
                    $('#WorkPlace').removeClass('input-validation-error');
                    $('#validationMsgPersonWorksPlace').hide(); ;
                }
                else {
                    $('#WorkPlace').addClass('input-validation-error');
                    $('#validationMsgPersonWorksPlace').show();
                }
            });
            $('#WorkProf').keyup(function () {
                var str = $('#WorkProf').val();
                if (str != "") {
                    $('#WorkProf').removeClass('input-validation-error');
                    $('#validationMsgPersonWorksLevel').hide(); ;
                }
                else {
                    $('#WorkProf').addClass('input-validation-error');
                    $('#validationMsgPersonWorksLevel').show() ;
                }
            });
            $('#WorkSpec').keyup(function () {
                var str = $('#WorkSpec').val();
                if (str != "") {
                    $('#WorkSpec').removeClass('input-validation-error');
                    $('#validationMsgPersonWorksDuties').hide();
                }
                else {
                    $('#WorkSpec').addClass('input-validation-error');
                    $('#validationMsgPersonWorksDuties').show();
                }
            });
        });
        function UpdScWorks() {
            if ($('#ScWorkInfo').val() == '') {
                $('#ScWorkInfo').addClass('input-validation-error');
                $('#ScWorkInfo_Message').show();
                return false;
            }
            else {
                $('#ScWorkInfo').removeClass('input-validation-error');
                $('#ScWorkInfo_Message').hide();
            }
            var params = new Object();
            params['ScWorkInfo'] = $('#ScWorkInfo').val();
            params['ScWorkType'] = $('#WorkInfo_ScWorkId').val();
            $.post('AbiturientNew/UpdateScienceWorks', params, function (res) {
                if (res.IsOk) {
                    $('#ScWorks').show();
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
            $.post('AbiturientNew/DeleteScienceWorks', param, function (res) {
                if (res.IsOk) {
                    $("#" + id).hide(250).html(""); 
                    if (res.Count == 0) {
                        $('#ScWorks').hide();
                    }
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
            var Ok = CheckRegExp();
             
            if (params['WorkPlace'] == "") {
                $('#WorkPlace').addClass('input-validation-error');
                $('#validationMsgPersonWorksPlace').show();
                Ok = false;
            }
            if (params['WorkProf'] == "") {
                $('#WorkProf').addClass('input-validation-error');
                $('#validationMsgPersonWorksLevel').show() ;
                Ok = false;
            }
            if (params['WorkSpec'] == "") {
                $('#WorkSpec').addClass('input-validation-error');
                $('#validationMsgPersonWorksDuties').show();
                Ok = false;
            }
            if (Ok) {
                $.post('AbiturientNew/AddWorkPlace', params, function (res) {
                    if (res.IsOk) {
                        $('#BlockPersonWorks').show();
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
            $.post('AbiturientNew/DeleteWorkPlace', parm, function (res) {
                if (res.IsOk) {
                    $('#' + id).hide(250).html('');
                    if (res.Count == 0) {
                        $('#BlockPersonWorks').hide();
                    }
                }
                else {
                    alert(res.ErrorMessage);
                }
            }, 'json');
        }
        function CheckRegExp() {
            var val = $('#WorkStag').val();
            var regex = /^([0-9])+$/i;
            if (val != '') {
                if (!regex.test(val)) {
                    $('#WorkStag').addClass('input-validation-error');
                    $('#validationMsgPersonWorksExperience').show();
                    return false;
                }
                else {
                    $('#WorkStag').removeClass('input-validation-error');
                    $('#validationMsgPersonWorksExperience').hide();
                }
            }
            return true;
        }
    </script>
    <script type="text/javascript">
        $(function () {
            $('#OlDate').datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: "focus"
            });
            $('#OlympType').change(function () { setTimeout(UpdateAfterOlympType); });
            $('#OlympName').change(function () { setTimeout(UpdateAfterOlympName); });
            $('#OlympSubject').change(function () { setTimeout(UpdateAfterOlympSubject); });
            $('#OlympValue').change(function () { setTimeout(UpdateAfterOlympValue) });
        });

        $.datepicker.regional["ru"];
        function UpdateAfterOlympType() {
            $('#btnAddOlympiad').hide();
            $('#_OlympSubject').hide();
            $('#_OlympValue').hide();
            var param = new Object();
            param['OlympTypeId'] = $('#OlympType').val();
            $.post('AbiturientNew/GetOlympNameList', param, function (json_data) {
                if (json_data.IsOk) {
                    var output = '';
                    for (var i = 0; i < json_data.List.length; i++) {
                        output += '<option value="' + json_data.List[i].Id + '">' + json_data.List[i].Name + '</option>';
                    }
                    $('#OlympName').html(output);
                    $('#_OlympName').show();
                }
            }, 'json');
        }
        function UpdateAfterOlympName() {
            $('#btnAddOlympiad').hide();
            $('#_OlympValue').hide();
            var param = new Object();
            param['OlympTypeId'] = $('#OlympType').val();
            param['OlympNameId'] = $('#OlympName').val();
            $.post('AbiturientNew/GetOlympSubjectList', param, function (json_data) {
                if (json_data.IsOk) {
                    var output = '';
                    for (var i = 0; i < json_data.List.length; i++) {
                        output += '<option value="' + json_data.List[i].Id + '">' + json_data.List[i].Name + '</option>';
                    }
                    $('#OlympSubject').html(output);
                    $('#_OlympSubject').show();
                }
            }, 'json');
        }
        function UpdateAfterOlympSubject() {
            $('#btnAddOlympiad').hide();
            $('#OlympValue').val('0');
            $('#_OlympValue').show();
        }
        function UpdateAfterOlympValue() {
            $('#btnAddOlympiad').show();
        }
        
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
        function AddOlympiad() {
            var ret = true;
            if (!CheckOlSeries()) { ret = false; }
            if (!CheckOlNumber()) { ret = false; }
            //if (!CheckOlDate()) { ret = false; }
            if (!ret)
                return false;
            var param = new Object();
            param['OlympTypeId'] = $('#OlympType').val();
            param['OlympNameId'] = $('#OlympName').val();
            param['OlympSubjectId'] = $('#OlympSubject').val();
            param['OlympValueId'] = $('#OlympValue').val();
            param['Series'] = $('#OlSeries').val();
            param['Number'] = $('#OlNumber').val();
            param['Date'] = $('#OlDate').val();
            $.post('AbiturientNew/AddOlympiad', param, function (res) {
                if (res.IsOk) {
                    $('#OlympBlock').show();
                    var output = '';
                    output += '<tr id=\'' + res.Id + '\'><td style="vertical-align: middle;">';
                    output += res.Type + '</td>';
                    output += '<td style="vertical-align: middle;">' + res.Name + '</td>';
                    output += '<td style="vertical-align: middle;">' + res.Subject + '</td>';
                    output += '<td style="vertical-align: middle;">' + res.Value + '</td>';
                    output += '<td style="width:35%; vertical-align: middle;">' + res.Doc + '</td>';
                    output += '<td style="width:10%; vertical-align: middle;"><span class="link" onclick="DeleteOlympiad(\'' + res.Id + '\')" ><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить оценку" /><span></td>';
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
            $.post('AbiturientNew/DeleteOlympiad', param, function (res) {
                if (res.IsOk) {
                    $('#' + id).hide();
                    if (res.Count == 0) {
                        $('#OlympBlock').hide();
                    }
                }
                else {
                    alert(res.ErrorMessage);
                }
            }, 'json');
        }
    </script>
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
                <div class="form panel">
                <h3><%= GetGlobalResourceObject("PersonalOffice_Step5", "ResearchWorkHeader").ToString()%></h3>
                <hr />
                <asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step5, ResearchWorkMessage %>"></asp:Literal>
                    <div class="clearfix">
                        <%= Html.DropDownListFor(x => x.WorkInfo.ScWorkId, Model.WorkInfo.ScWorks)%>
                    </div>
                    <div class="clearfix">
                        <textarea class="noresize" id="ScWorkInfo" rows="5" cols="80"></textarea>
                    </div>
                    <div class = "clearfix">
                        <span id="ScWorkInfo_Message" style="display:none; color:Red;"><asp:Literal runat="server" Text="<%$Resources:PersonalOffice_Step5, ScWork_Message %>"></asp:Literal></span>
                    </div>
                    <br />
                    <div class="clearfix">
                        <button id="btnAddScWork" onclick="UpdScWorks()" class="button button-blue"><%= GetGlobalResourceObject("PersonalOffice_Step5", "btnAdd").ToString()%></button>
                    </div>
                    <br /><br />
                    <% if (Model.WorkInfo.pScWorks.Count == 0)
                       { %> <table id="ScWorks" class="paginate" style="display: none; width:100%; text-align: center;"> <%   }
                       else
                       {%>  <table id="ScWorks" class="paginate" style="width:100%; text-align: center;">  <%} %>
                        <thead>
                            <tr>
                                <th><%= GetGlobalResourceObject("PersonalOffice_Step5", "ScWorksType").ToString()%></th>
                                <th><%= GetGlobalResourceObject("PersonalOffice_Step5", "ScWorksText").ToString()%></th>
                                <th><%= GetGlobalResourceObject("PersonalOffice_Step5", "btnDelete").ToString()%></th>
                            </tr>
                        </thead>
                        <tbody>
                        <% foreach (var scWork in Model.WorkInfo.pScWorks)
                            {
                        %>
                            <tr>
                            <%= Html.Raw(string.Format(@"<tr id=""{0}"">", scWork.Id)) %>
                                <td ><%= Html.Encode(scWork.ScienceWorkType) %></td>
                                <td><%= Html.Encode(scWork.ScienceWorkInfo) %></td>
                                <td><%= Html.Raw("<span class=\"link\" onclick=\"DeleteScWork('" + scWork.Id.ToString() + "')\" ><img src=\"../../Content/themes/base/images/delete-icon.png\" alt=\"Удалить\" /></span>") %></td>
                            </tr>
                        <% } %>
                        </tbody>
                    </table>
                </div>
                <br /><br /> 

                <div class="form panel">
                <h3><%= GetGlobalResourceObject("PersonalOffice_Step5", "WorkExperienceHeader").ToString()%></h3>
                <hr /> 
                    <div class="clearfix">
                        <label for="WorkStag"><%= GetGlobalResourceObject("PersonalOffice_Step5", "JobExperience").ToString()%></label>
                        <input id="WorkStag" onkeyup="CheckRegExp()" type="text"/>
                        <br /><p></p><span id="validationMsgPersonWorksExperience" style="display:none; color:Red;"><%= GetGlobalResourceObject("PersonalOffice_Step5", "validationMsgPersonWorksExperience").ToString()%></span>
                    </div>
                    <div class="clearfix">
                        <label for="WorkPlace"><%= GetGlobalResourceObject("PersonalOffice_Step5", "JobLocation").ToString()%></label>
                        <input id="WorkPlace" type="text" /><br /><p></p><span id="validationMsgPersonWorksPlace" class="Red"  style="display:none;"><%= GetGlobalResourceObject("PersonalOffice_Step5", "validationMsgPersonWorksPlace").ToString()%></span>
                    </div>
                    <div class="clearfix">
                        <label for="WorkProf"><%= GetGlobalResourceObject("PersonalOffice_Step5", "JobPosition").ToString()%></label>
                        <input id="WorkProf" type="text" /><br /><p></p><span id="validationMsgPersonWorksLevel" class="Red" style="display:none;"><%= GetGlobalResourceObject("PersonalOffice_Step5", "validationMsgPersonWorksLevel").ToString()%></span>
                    </div>
                    <div class="clearfix">
                        <label for="WorkSpec"><%= GetGlobalResourceObject("PersonalOffice_Step5", "JobFunctions").ToString()%></label>
                        <textarea id="WorkSpec" cols="80" rows="4" ></textarea>
                    </div>
                    <div>
                        <span id="validationMsgPersonWorksDuties" class="Red" style="display:none;"><%= GetGlobalResourceObject("PersonalOffice_Step5", "validationMsgPersonWorksDuties").ToString()%></span>
                    </div> 
                    <div class="clearfix">
                        <button id="btnAddProfs" onclick="AddWorkPlace()" class="button button-blue"><%= GetGlobalResourceObject("PersonalOffice_Step5", "btnAdd").ToString()%></button>
                    </div>
                <br /><br />
                <% if (Model.WorkInfo.pWorks.Count == 0)
                   { %>  <div id = "BlockPersonWorks" style="width: 464px; overflow-x: scroll; display: none;"> <% }
                   else
                   { %> <div id = "BlockPersonWorks" style="width: 464px; overflow-x: scroll; "> <%} %>
                        <table id="PersonWorks" class="paginate" style="width:100%; text-align: center; vertical-align:middle; " >
                            <thead>
                                <tr>
                                    <th><%= GetGlobalResourceObject("PersonalOffice_Step5", "JobLocation").ToString()%></th>
                                    <th><%= GetGlobalResourceObject("PersonalOffice_Step5", "JobExperience").ToString()%></th>
                                    <th><%= GetGlobalResourceObject("PersonalOffice_Step5", "JobPosition").ToString()%></th>
                                    <th><%= GetGlobalResourceObject("PersonalOffice_Step5", "JobFunctions").ToString()%></th>
                                    <th><%= GetGlobalResourceObject("PersonalOffice_Step5", "btnDelete").ToString()%></th>
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
                    </div>
                </div>
                <br /> 
                
                <div class="form panel">
                <h3><%= GetGlobalResourceObject("PersonalOffice_Step5", "OlympiadsHeader").ToString()%></h3>
                    <div class="clearfix">
                        <%= Html.DropDownList("OlympType", Model.PrivelegeInfo.OlympTypeList, 
                        new SortedList<string, object>() { {"style", "width:460px;"} , {"size", "4"} }) %>
                    </div>
                    <div class="clearfix" id="_OlympName" style="display:none">
                        <%= Html.DropDownList("OlympName", Model.PrivelegeInfo.OlympNameList, 
                        new SortedList<string, object>() { {"style", "width:460px;"} , {"size", "6"} }) %>
                    </div>
                    <div class="clearfix" id="_OlympSubject" style="display:none">
                        <%= Html.DropDownList("OlympSubject", Model.PrivelegeInfo.OlympSubjectList, 
                        new SortedList<string, object>() { {"style", "width:460px;"} , {"size", "6"} }) %>
                    </div>
                    <div class="clearfix" id="_OlympValue" style="display:none">
                        <%= Html.DropDownList("OlympValue", Model.PrivelegeInfo.OlympValueList,
                        new SortedList<string, object>() { {"style", "width:460px;"} , {"size", "4"} }) %>
                    </div>
                    <div>
                        <h4><%= GetGlobalResourceObject("PersonalOffice_Step5", "DocumentHeader").ToString()%></h4>
                        <hr />
                        <div class="clearfix">
                            <label for="OlSeries"><%= GetGlobalResourceObject("PersonalOffice_Step5", "DocumentSeries").ToString()%></label>
                            <input id="OlSeries" type="text" /><br/><p></p>
                            <span id="OlSeries_Message" style="display:none; color:Red;"><%= GetGlobalResourceObject("PersonalOffice_Step5", "DocumentSeriesMessage").ToString()%></span>
                        </div>
                        <div class="clearfix">
                            <label for="OlNumber"><%= GetGlobalResourceObject("PersonalOffice_Step5", "DocumentNumber").ToString()%></label>
                            <input id="OlNumber" type="text" /><br/><p></p>
                            <span id="OlNumber_Message" style="display:none; color:Red;"><%= GetGlobalResourceObject("PersonalOffice_Step5", "DocumentNumberMessage").ToString()%></span>
                        </div>
                        <div class="clearfix">
                            <label for="OlDate"><%= GetGlobalResourceObject("PersonalOffice_Step5", "DocumentDate").ToString()%></label>
                            <input id="OlDate" type="text" /><br/><p></p>
                            <span id="OlDate_Message" style="display:none; color:Red;"><%= GetGlobalResourceObject("PersonalOffice_Step5", "DocumentDateMessage").ToString()%></span>
                        </div>
                        <hr />
                        <div class="message info">
                            <%= GetGlobalResourceObject("PersonalOffice_Step5", "OlympiadsMessage").ToString()%>
                        </div>
                        <div class="clearfix" id="btnAddOlympiad" >
                            <button onclick="AddOlympiad()" class="button button-blue"> <%= GetGlobalResourceObject("PersonalOffice_Step5", "btnAdd").ToString()%></button>
                        </div>
                        <br />
                       
                            <% if (Model.PrivelegeInfo.pOlympiads.Count==0) { %> 
                            <div id = "OlympBlock" style="width: 464px; overflow-x: scroll; display: none;"><% } else { %> 
                            <div id = "OlympBlock" style="width: 464px; overflow-x: scroll; "><% } %>
                            <h4 > <%= GetGlobalResourceObject("PersonalOffice_Step5", "OlympiadsAdded").ToString()%></h4>
                            <table id="tblOlympiads" class="paginate" style="width:100%; text-align: center; vertical-align:middle; ">
                                <thead>
                                    <tr>
                                        <th style="text-align:center;"> <%= GetGlobalResourceObject("PersonalOffice_Step5", "OlympiadsType").ToString()%></th>
                                        <th style="text-align:center;"> <%= GetGlobalResourceObject("PersonalOffice_Step5", "OlympiadsName").ToString()%></th>
                                        <th style="text-align:center;"> <%= GetGlobalResourceObject("PersonalOffice_Step5", "OlympiadsSubject").ToString()%></th>
                                        <th style="text-align:center;"> <%= GetGlobalResourceObject("PersonalOffice_Step5", "OlympiadsStatus").ToString()%></th>
                                        <th style="width:35%;text-align:center;"> <%= GetGlobalResourceObject("PersonalOffice_Step5", "DocumentHeader").ToString()%></th>
                                        <th style="width:10%;text-align:center;"> <%= GetGlobalResourceObject("PersonalOffice_Step5", "btnDelete").ToString()%></th>
                                    </tr>
                                </thead>
                                <tbody>
                                <% foreach (var olympiad in Model.PrivelegeInfo.pOlympiads)
                                    {
                                %>
                                    <tr id='<%= olympiad.Id.ToString("N") %>'>
                                        <td style="vertical-align: middle;"><%= Html.Encode(olympiad.OlympType) %></td>
                                        <td style="vertical-align: middle;"><%= Html.Encode(olympiad.OlympName) %></td>
                                        <td style="vertical-align: middle;"><%= Html.Encode(olympiad.OlympSubject) %></td>
                                        <td style="vertical-align: middle;"><%= Html.Encode(olympiad.OlympValue) %></td>
                                        <td style="width:35%; vertical-align: middle;">
                                            <%= Html.Encode(olympiad.DocumentSeries + " " + olympiad.DocumentNumber + " от " + olympiad.DocumentDate.ToShortDateString())%>
                                        </td>
                                        <td style="width:10%; vertical-align: middle;"><%= Html.Raw("<span class=\"link\" onclick=\"DeleteOlympiad('" + olympiad.Id.ToString("N") + "')\" ><img src=\"../../Content/themes/base/images/delete-icon.png\" alt=\"Удалить\" /></span>")%></td>
                                    </tr>
                                <% } %>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div> 
                <hr style="color:#A6C9E2;" />

                <% using (Html.BeginForm("NextStep", "AbiturientNew", FormMethod.Post))
                   {
                %>
                    <h3><%= GetGlobalResourceObject("PersonalOffice_Step5", "SportValue").ToString()%></h3>
                    <hr />
                    <div class="form">
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PrivelegeInfo.SportQualificationId, GetGlobalResourceObject("PersonalOffice_Step5", "SportQualification").ToString())%>
                            <%= Html.DropDownListFor(x => x.PrivelegeInfo.SportQualificationId, Model.PrivelegeInfo.SportQualificationList) %>
                        </div>
                        <div id="dSportQualificationLevel" class="clearfix">
                            <%= Html.LabelFor(x => x.PrivelegeInfo.SportQualificationLevel, GetGlobalResourceObject("PersonalOffice_Step5", "SportCategory").ToString()) %>
                            <%= Html.TextBoxFor(x => x.PrivelegeInfo.SportQualificationLevel) %>
                        </div>
                        <div id="dOtherSport" class="clearfix" style=" display:none; border-collapse:collapse;">
                            <%= Html.LabelFor(x => x.PrivelegeInfo.OtherSportQualification, GetGlobalResourceObject("PersonalOffice_Step5", "SportQualificationCategory").ToString())%>
                            <%= Html.TextBoxFor(x => x.PrivelegeInfo.OtherSportQualification) %>
                        </div>
                    </div>
                    <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                    <input id="Submit4" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                <% } %>
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
