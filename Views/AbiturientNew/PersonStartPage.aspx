<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/Site.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.OpenPersonalAccountModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("PersonStartPage", "Header") %>
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
<h2><%= GetGlobalResourceObject("PersonStartPage", "Header") %></h2>
<hr /><br />
<form id="form" method="post" action="/Abiturient/OpenPersonalAccount">
    <input name="val_h" id="val_h" type="hidden" value="1" />
    <input type="button" class="button button-blue" name="Val" onclick="Submit1()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType1") %>" /><br /><br />
    <input type="button" class="button button-blue" name="Val" onclick="Submit2()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType2") %>" /><br /><br />
    <input type="button" class="button button-blue" name="Val" onclick="Submit3()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType3") %>" /><br /><br />
    <input type="button" class="button button-blue" name="Val" onclick="Submit4()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType4") %>" /><br /><br />
    <input type="button" class="button button-blue" name="Val" onclick="Submit5()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType5") %>" /><br /><br />
    <input type="button" class="button button-blue" name="Val" onclick="Submit6()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType6") %>" /><br /><br />
    <input type="button" class="button button-blue" name="Val" onclick="Submit7()" style="width:45em;" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType7") %>" /><br /><br />
    
    <input type="button" class="button button-blue" name="Val" onclick="Submit8()" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType8") %>" /><br /><br />
    <input type="button" class="button button-blue" name="Val" onclick="Submit9()" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType9") %>" /><br /><br />
    <input type="button" class="button button-blue" name="Val" onclick="Submit10()" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType10") %>" /><br /><br />
    <input type="button" class="button button-blue" name="Val" onclick="Submit11()" value="<%= GetGlobalResourceObject("PersonStartPage", "AbiturientType11") %>" /><br /><br />
    
</form>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="NavigationList" runat="server">
    <ul class="clearfix">
        <li class="active"><a href="../../Abiturient/Main"><%= GetGlobalResourceObject("Common", "StartPageNav").ToString()%></a></li>
    </ul>
</asp:Content>
