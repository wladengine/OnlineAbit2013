<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.RuslangExamModelPersonList>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    RusLangExam_ufms
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Поиск сертификата по номеру</h2>
<script type="text/javascript">
    function GetList() {
        var Fio = '#fio';
        var Level = '#level';
        var Sex = '#sex';
        var birthdate = '#birthdate';
        var Date = '#date';
        var Nationality = '#nationality';
        var Document = '#passport';
        var Number = '#number';
        $.post('/AbiturientNew/GetUFMS', { sertId: $('#findlist').val()
        }, function (json_data) {
            if (json_data.NoFree) {

            }
            else {
                $(Fio).text(json_data.fio);
                $(Level).text(json_data.level);
                $(Sex).text(json_data.sex);
                $(Date).text(json_data.date);
                $(Nationality).text(json_data.nationality);
                $(birthdate).text(json_data.birthdate);
                $(Document).text(json_data.document);
                $(Number).text(json_data.number);  
                
            }
        }, 'json');
    }
</script>

<table style= "width : 100%; border-spacing: 7px 11px; ">
<tr >
    <td style=" vertical-align: top; " >
        <form class="form" method="post" action="../../AbiturientNew/RuslangExam_ufms" >
        <b>Введите номер сертификата:</b><br /><p></p>
        <input type = "text" name="findstring" value="<%=Model.findstring %>"/><br /><p></p>
        <input id="submitBtn" class="button button-green" type="submit" value="Найти" />
        </form>
    </td>
    <td style="width: 10px;">
    </td>
    <td style="vertical-align: top; width: 300px; border-spacing: 7px 11px;">
   Результаты поиска:<br />
        <select id = "findlist" size = "4" style="width: 300px;" onchange="GetList()">
            <% foreach (var x in Model.PersonList)
               { %>
               <option value=<%=x.Id %>> <%=x.Name %></option>
               <% } %>
        </select>
    </td>  
</tr>
<tr>
    <td colspan="3">
        <br />
        <p></p>
        <hr /> 
        <p><b>ФИО: </b><span id="fio"></span></p>
        <p><b>Дата рождения: </b><span id="birthdate"></span></p>
        <p><b>Гражданство: </b><span id="nationality"></span></p>
        <p><b>Пол: </b><span id="sex"></span></p> 
        <p><b>Документ: </b><span id="passport"></span></p>
        <br />
        <p><b>Уровень тестирования: </b><span id="level"></span></p>
        <p><b>Полный номер сертификата: </b><span id="number"></span></p>
        <p><b>Дата выдачи: </b><span id="date"></span></p>
    </td>
</tr>
</table>



</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="NavigationList" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Subheader" runat="server">
</asp:Content>
