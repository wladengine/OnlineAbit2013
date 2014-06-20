<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="OnlineAbit2013.Models" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/AbiturientNew/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<Mag_ApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("NewApplication", "PageTitle")%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
   <h2> <%= GetGlobalResourceObject("NewApplication", "PageSubheader")%></h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
  
</script>

     <p class="message info"> <a href="/Application/Index/<%=Model.CommitId%>"><b>Удалите</b></a> заявление и измените необходимые сведения в <a href="/AbiturientNew?step=4"><b>Анкете</b></a>
     </p>
</asp:Content>
