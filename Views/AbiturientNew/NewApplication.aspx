<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="OnlineAbit2013.Models" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<ApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Создание нового заявления
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
   <h2>Новое заявление</h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    //$(document).ready(function () {
    function Submit1() {
        $('#val_h').val("1");
        document.forms['form'].submit();
    }
    function Submit2() {
        $('#val_h').val("2");
        document.forms['form'].submit();
    }
    function Submit3() {
        $('#val_h').val("3");
        document.forms['form'].submit();
    }
    function Submit4() {
        $('#val_h').val("4");
        document.forms['form'].submit();
    }
    function Submit5() {
        $('#val_h').val("5");
        document.forms['form'].submit();
    }
    function Submit6() {
        $('#val_h').val("6");
        document.forms['form'].submit();
    }
    function Submit7() {
        $('#val_h').val("7");
        document.forms['form'].submit();
    }
    function Submit8() {
        $('#val_h').val("8");
        document.forms['form'].submit();
    }
    function Submit9() {
        $('#val_h').val("9");
        document.forms['form'].submit();
    }
    function Submit10() {
        $('#val_h').val("10");
        document.forms['form'].submit();
    }
    function Submit11() {
        $('#val_h').val("11");
        document.forms['form'].submit();
    }
    //});
    
</script>
 
    <%= Html.ValidationSummary() %>
    <% if (Model.EntryType == 1 && DateTime.Now < new DateTime(2012, 6, 20, 0, 0, 0))
       { %>
       <div class="message warning">Внимание! Подача заявлений на <strong style="font-size:10pt">первый курс</strong> начнётся с <strong style="font-size:11pt">20 июня 2012 года</strong></div>
    <% } %>
    
    <% if (Model.EntryType != 1)
       { %>
        <%= Html.HiddenFor(x => x.EntryType) %>
        <select id="Entry" name="Entry" onchange="ChangeEType()">
            <option value="2">Магистратура</option>
            <option value="3">Первый курс</option>
        </select>
    <% }
       else
       { %>
        <div class="message info">Согласно данным анкеты, вы поступаете на <strong>первый курс</strong></div>
        <input type="hidden" id="hEntryType" name="EntryType" value="1" />
        <input type="hidden" id="EntryType" name="Entry" value="1" />
    <% } %>
     <form id="form" method="post" action="/AbiturientNew/NewApplicationSelect"> 
      <input name="val_h" id="val_h" type="hidden" value="1" />
    <!--ag-->
    <input type="button" class="button button-blue" name="Val" onclick="Submit8()" style="width:45em; "value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType8") %>" /><br /><br />
    //////////////////////
    <!-- 1 курс -->
    <input type="button" class="button button-blue" name="Val" onclick="Submit1()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType1") %>" /><br /><br />
    <!-- 1 курс иностр граждане -->
    <input type="button" class="button button-blue" name="Val" onclick="Submit2()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType2") %>" /><br /><br />
    <!-- Перевод РФ-спбгу -->
    <input type="button" class="button button-blue" name="Val" onclick="Submit3()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType3") %>" /><br /><br />
    <!-- Перевод иностр-спбгу -->
    <input type="button" class="button button-blue" name="Val" onclick="Submit4()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType4") %>" /><br /><br />
    <!-- восстановление -->
    <input type="button" class="button button-blue" name="Val" onclick="Submit5()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType5") %>" /><br /><br />
    <!-- Перевод с платной на бюджет -->
    <input type="button" class="button button-blue" name="Val" onclick="Submit6()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType6") %>" /><br /><br />
    <!-- смена образ программы -->
    <input type="button" class="button button-blue" name="Val" onclick="Submit7()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType7") %>" /><br /><br />
    <!-- СПО --> 
    <input type="button" class="button button-blue" name="Val" onclick="Submit9()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType9") %>" /><br /><br />
    <!-- аспирантура рф -->
    <input type="button" class="button button-blue" name="Val" onclick="Submit10()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType10") %>" /><br /><br />
    <!-- аспирантура иностр -->
    <input type="button" class="button button-blue" name="Val" onclick="Submit11()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType11") %>" /><br /><br />
</form>
 
</asp:Content>
