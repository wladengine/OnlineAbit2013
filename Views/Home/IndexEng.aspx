<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SiteForeign.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Welcome
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Welcome</h2>
    <table class="StartTable">
    <tr>
        <th align="center">Russia</th>
        <th align="center">Inernational</th>
    </tr>
    <tr>
        <td>I am a Russian citizen, or have equal rights with citizens of the Russian Federation <abbr title="Citizens of Belarus, Kazakhstan, Kyrgyzstan, Tajikistan">have equal rights with citizens of the Russian Federation law </abbr> on admission to basic education programs.</td>
        <td>I am a citizen of a foreign country, and am applying for admission to basic education programs.
<br /><br />Dear <b style="font-size: 1.2em;">International Applicants</b>, in order to create your application, please, click the button below </td>
    </tr>
    <tr>
        <td align="center"><a style="font-size:1.5em;" href="../../Account/LogOn">Enter</a></td>
        <td align="center"><a style="font-size:1.5em;" href="../../Account/LogOnFor">Enter</a></td>
    </tr>
    </table>

</asp:Content>
