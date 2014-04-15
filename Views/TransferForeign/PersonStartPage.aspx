<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.OpenPersonalAccountModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    PersonStartPage
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Выберите действие, которое вы хотите совершить в Личном Кабинете</h2>
<br /><br />
<form method="post" action="/Abiturient/OpenPersonalAccount">
    <input type="submit" class="button button-green" name="Val" value="Поступление на 1 курс гражданам РФ" /><br /><br />
    <input type="submit" class="button button-green" name="Val" value="Поступление на 1 курс иностранным гражданам" /><br /><br />
    <input type="submit" class="button button-green" name="Val" value="Перевод из российского университета в СПбГУ" /><br /><br />
    <input type="submit" class="button button-green" name="Val" value="Перевод из иностранного университета в СПбГУ" /><br /><br />
    <input type="submit" class="button button-green" name="Val" value="Восстановление в СПбГУ" /><br /><br />
</form>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="NavigationList" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
</asp:Content>
