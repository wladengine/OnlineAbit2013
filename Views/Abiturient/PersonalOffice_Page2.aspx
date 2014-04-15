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
                $('#PassportInfo_PassportSeries').change(function () { setTimeout(CheckSeries); });
                $('#PassportInfo_PassportNumber').change(function () { setTimeout(CheckNumber); });
                $('#PassportInfo_PassportAuthor').change(function () { setTimeout(CheckAuthor); });
                $('#PassportInfo_PassportDate').change(function () { setTimeout(CheckDate); });
                //$('#PassportInfo_PassportSeries').blur(function () { setTimeout(CheckSeries); });
                //$('#PassportInfo_PassportNumber').blur(function () { setTimeout(CheckNumber); });
                //$('#PassportInfo_PassportAuthor').blur(function () { setTimeout(CheckAuthor); });
                //$('#PassportInfo_PassportDate').blur(function () { setTimeout(CheckDate); });
            });
            function CheckForm() {
                //var res = true;
                //if (!CheckSeries()) { res = false; }
                //if (!CheckNumber()) { res = false; }
                //if (!CheckAuthor()) { res = false; }
                //if (!CheckDate()) { res = false; }
                //res = CheckSeries() && CheckNumber() && CheckAuthor() && CheckDate();
                //return res;
                return CheckSeries() && CheckNumber() && CheckAuthor() && CheckDate();
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
                    if ($('#PassportInfo_PassportType').val() == '1' && !ruPassportRegex.test(val)) {
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
                    if ($('#PassportInfo_PassportType').val() == '1' && !ruPassportRegex.test(val)) {
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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderScriptsContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Анкета</h2>
</asp:Content>
