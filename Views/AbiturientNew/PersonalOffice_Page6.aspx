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
                <form class="panel form" action="AbiturientNew/NextStep" method="post">
                    <%= Html.ValidationSummary() %>
                    <%= Html.HiddenFor(x => x.Stage) %>
                    <div class="clearfix">
                        <h4><%= GetGlobalResourceObject("PersonalOffice_Step6", "HostelHeader").ToString()%></h4>
                        <%= Html.CheckBoxFor(x => x.AddInfo.HostelAbit)%>
                        <span><%= GetGlobalResourceObject("PersonalOffice_Step6", "HostelAbit").ToString()%></span>
                    </div>
                    <div class="clearfix">
                        <h4>Право на льготы:</h4>
                        <%= Html.CheckBoxFor(x => x.AddInfo.HasPrivileges) %>
                        <span>Претендую на льготы (инвалид I,II ст., участник боевых действий, сирота, чернобылец...)</span>
                    </div>
                    <div class="clearfix">
                        <h4><%= GetGlobalResourceObject("PersonalOffice_Step6", "ContactPerson").ToString()%></h4>
                        <span><%= GetGlobalResourceObject("PersonalOffice_Step6", "ContactPerson_SubHeader").ToString()%></span><br />
                        <!-- <textarea id="AddPerson_ContactPerson" name="AddPerson.ContactPerson" cols="40" rows="4" class="ui-widget-content ui-corner-all"></textarea> -->
                        <%= Html.TextAreaFor(x => x.AddInfo.ContactPerson, 5, 70, new Dictionary<string, object>() { { "class", "noresize" } }) %>
                    </div>
                    <div class="clearfix">
                        <h4><%= GetGlobalResourceObject("PersonalOffice_Step6", "ExtraInfo").ToString()%></h4>
                        <!-- <textarea id="AddPerson_ExtraInfo" name="AddPerson.ExtraInfo" cols="40" rows="4" class="ui-widget-content ui-corner-all"></textarea> -->
                        <%= Html.TextAreaFor(x => x.AddInfo.ExtraInfo, 5, 70, new Dictionary<string, object>() { { "class", "noresize" } })%>
                    </div>
                    <h4><%= GetGlobalResourceObject("PersonalOffice_Step6", "DocumentsReturn").ToString()%></h4>
                    <div class="clearfix">
                    <%
                        foreach (SelectListItem SLI in Model.AddInfo.ReturnDocumentTypeList)
                        { 
                            %><input type="radio" name="AddInfo.ReturnDocumentTypeId" value= <%="\""+SLI.Value+"\"" %>
                            <% if (SLI.Value==Model.AddInfo.ReturnDocumentTypeId) { %> checked="checked" <% } %>
                            /> <%=SLI.Text%> <br /><p></p> <%
                        }
                    %>
                    </div>
                    <div class="clearfix">
                        <h4><%= GetGlobalResourceObject("PersonalOffice_Step6", "FZ152_Header").ToString()%></h4>
                        <%= Html.CheckBoxFor(x => x.AddInfo.FZ_152Agree) %>
                        <span><%= GetGlobalResourceObject("PersonalOffice_Step6", "FZ_152Agree").ToString()%></span>    
                    </div>
                    <span id="FZ" class="Red" style="display:none;"><%= GetGlobalResourceObject("PersonalOffice_Step6", "FZ_152_Message").ToString()%></span>
                    <hr />
                    <div class="clearfix">
                        <input id="Submit5" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonalOffice_Step6", "btnValue_EndRegisration").ToString()%>" />
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
