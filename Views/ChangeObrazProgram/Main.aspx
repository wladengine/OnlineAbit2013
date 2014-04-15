<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Abiturient/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OnlineAbit2013.Models.SimplePerson>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Главная страница
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Главная страница</h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    $('#UILink').hide();
    function DeleteMsg(id) {
        var p = new Object();
        p["id"] = id;
        $.post('/Abiturient/DeleteMsg', p, function (res) {
            if (res.IsOk) {
                $('#' + id).hide(250).html("");
            }
            else {
                if (res != undefined) {
                    alert(res.ErrorMessage);
                }
            }
        }, 'json');
    }  
</script>
<h2><%= Html.Encode(Model.Surname + " " + Model.Name + " " + Model.SecondName) %></h2>
<% foreach (var msg in Model.Messages)
   { %>
    <div id="<%= msg.Id %>" class="message info" style="padding:5px">
        <span class="ui-icon ui-icon-alert"></span><%= msg.Text %>
        <div style="float:right;"><span class="link" onclick="DeleteMsg('<%= msg.Id %>')"><img src="../../Content/themes/base/images/delete-icon.png" alt="Удалить" /></span></div>
    </div>
<% } %>
<p>
    Вы находитесь на главной странице Вашего <b>Личного Кабинета</b>. Здесь Вы можете подать заявление, выставить приоритеты, добавить сканы документов, 
    создать мотивационное письмо, отредактировать свои персональные данные.
</p>
    <h4>Поданные заявления</h4>
    <hr />
    <% if (Model.Applications.Where(x => x.Enabled == true).Count() > 0)
       { %>
        <table class="paginate full">
            <thead>
                <tr>
                    <th>Направление</th>
                    <th>Образовательная программа</th>
                    <th>Профиль</th>
                    <th>Форма обучения</th>
                    <th>Основа обучения</th>
                    <th>Просмотр заявления</th>
                </tr>
            </thead>
    <% foreach (OnlineAbit2013.Models.SimpleApplication app in Model.Applications.Where(x => x.Enabled == true).OrderBy(x => x.Priority).ToList())
        { %>
         <tr>
            <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.Profession) %></td>
            <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.ObrazProgram) %></td>
            <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.Specialization) %></td>
            <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.StudyForm) %></td>
            <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.StudyBasis) %></td>
            <td style="vertical-align:middle; text-align:center;"><a href="<%= string.Format("../../Application/Index/{0}", app.Id) %>">Просмотр</a></td>
         </tr>
     <% } %>
     </table>
    <% }
       else
       { 
    %>
        <h5>Нет поданных заявлений</h5>
    <% } %>
    <br />
    <h4>Отозванные заявления</h4>
    <hr />
    <% if (Model.Applications.Where(x => x.Enabled == false).Count() > 0)
       { %>
        <table class="paginate">
            <thead>
                <tr>
                    <th>Направление</th>
                    <th>Образовательная программа</th>
                    <th>Профиль</th>
                    <th>Форма обучения</th>
                    <th>Основа обучения</th>
                    <th>Просмотр заявления</th>
                </tr>
            </thead>
    <% foreach (OnlineAbit2013.Models.SimpleApplication app in Model.Applications.Where(x => x.Enabled == false).OrderBy(x => x.Priority).ToList())
       { %>
            <tbody>
                <tr>
                    <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.Profession)%></td>
                    <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.ObrazProgram)%></td>
                    <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.Specialization)%></td>
                    <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.StudyForm)%></td>
                    <td style="vertical-align:middle; text-align:center;"><%= Html.Encode(app.StudyBasis)%></td>
                    <td style="vertical-align:middle; text-align:center;"><a href="<%= string.Format("../../Application/Index/{0}", app.Id) %>">Просмотр</a></td>
                </tr>
            </tbody>
     <% } %>
     </table>
     <% }
       else
       { %>
        <h5>Нет отозванных заявлений</h5>
    <% } %>
    <hr />

</asp:Content>
