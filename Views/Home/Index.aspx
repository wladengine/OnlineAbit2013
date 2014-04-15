<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.HomeModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Добро пожаловать
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Добро пожаловать / Welcome</h2>
    <table class="StartTable">
    <tr>
        <th align="center">Российским абитуриентам</th>
        <th align="center">Inernational / Иностранцам</th>
    </tr>
    <tr>
        <td>Я — гражданин России или <abbr title="граждане Белоруссии, Казахстана, Киргизии, Таждикистана">имеею равные с гражданами Российской Федерации права</abbr> при поступлении на основные образовательные программы</td>
        <td>Я — гражданин иностранного государства и подаю заявление для поступления на основные образовательные программы.
<br /><br />Dear <b style="font-size: 1.2em;">International Applicants</b>, in order to create your application, please, click the button below </td>
    </tr>
    <tr>
        <td align="center"><a style="font-size:1.5em;" href="../../Account/LogOn">Войти</a></td>
        <td align="center"><a style="font-size:1.5em;" href="../../Account/LogOnFor">Enter</a></td>
    </tr>
    </table>
</asp:Content>
