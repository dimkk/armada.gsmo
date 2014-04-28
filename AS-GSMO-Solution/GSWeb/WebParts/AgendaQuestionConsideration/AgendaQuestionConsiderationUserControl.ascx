<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AgendaQuestionConsiderationUserControl.ascx.cs" Inherits="GSWeb.WebParts.AgendaQuestionConsideration.AgendaQuestionConsiderationUserControl" %>

<SharePoint:SPDataSource ID="DataSource1" runat="server">
  <SelectParameters>
    <asp:Parameter Name="ListName" DefaultValue="Вопросы повестки заседания" />
  </SelectParameters>
</SharePoint:SPDataSource>

<SharePoint:SPGridView DataSourceID="DataSource1" AutoGenerateColumns="false" runat="server">
  <Columns>
    <SharePoint:SPBoundField HeaderText="Номер Заседания" DataField="Заседание" />
    <asp:TemplateField HeaderText="Дата Заседания"> 
      <ItemTemplate> 
        <%# Convert.ToDateTime(new SPFieldLookupValue(Eval("Заседание:Дата проведения").ToString()).LookupValue).ToShortDateString() %> 
      </ItemTemplate> 
    </asp:TemplateField>
    <SharePoint:SPBoundField HeaderText="Инвестор" DataField="Инвестор" />
    <asp:TemplateField HeaderText="Рассмотрение вопроса"> 
      <ItemTemplate> 
        <%# Convert.ToString(Eval("Адрес")) %> (кадастровый номер <%# Eval("Кадастровый номер") %>). <%# Eval("Тема вопроса") %>
      </ItemTemplate> 
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Решение"> 
      <ItemTemplate> 
        <%# Trim(Convert.ToString(Eval("Решение")), 200) %> 
      </ItemTemplate> 
    </asp:TemplateField>
  </Columns>
</SharePoint:SPGridView>
