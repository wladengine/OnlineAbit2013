﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.PriorityChangerProfileModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    PriemChangerProfile
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<style>
	#sortable { list-style-type: decimal; margin: 10px; padding: 10px; width: 90%; cursor: move; }
	#sortable li { margin: 0 5px 5px 5px; padding: 5px; font-size: 1.2em; /*height: 1.5em; */}
	html>body #sortable li { /*height: 1.5em; */line-height: 1.2em; }
	.ui-state-highlight { /*height: 1.5em; */line-height: 1.2em; }
</style>
<script>
    $(function () {
        $("#sortable").sortable({
            placeholder: "message warning"
        });
        $("#sortable").disableSelection();
    });
</script>
<script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
    <h2>PriemChangerProfile</h2>
    <p class="message info">
        Расставьте приоритеты профилей внутри отдельно заданной образовательной программы.
    </p>
    <form action="/AbiturientNew/PriorityChangeProfile" method="post">
        <ul id="sortable">
    <% for (int i = 0; i < Model.lstProfiles.Count; i++) { %>
            <li class="message success">
                <table style="font-size:0.75em;" class="nopadding" cellspacing="0" cellpadding="0">
                    <tr>
                        <td style="width:12em"><%= GetGlobalResourceObject("PriorityChangerForeign", "Profile").ToString()%></td>
                        <td><%= Model.lstProfiles[i].Value %></td>
                    </tr>
                </table>
                <input type="hidden" name="<%= Model.lstProfiles[i].Key.ToString("N") %>" />
                <%--<a href="PersonStartPage.aspx">some path...</a>--%>
            </li>
    <% } %>
        </ul>
        <button id="btnSave" type="submit" class="button button-green">Сохранить</button><br />
    </form>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
</asp:Content>