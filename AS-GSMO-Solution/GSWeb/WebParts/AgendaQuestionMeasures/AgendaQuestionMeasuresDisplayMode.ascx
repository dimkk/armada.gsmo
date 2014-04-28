<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AgendaQuestionMeasuresDisplayMode.ascx.cs" Inherits="GSWeb.WebParts.AgendaQuestionMeasures.AgendaQuestionMeasuresDisplayMode" %>

<div class="container" style="margin-top: 25px;">
  <div class="form-horizontal" role="form">
    <div class="form-group">
      <label class="col-lg-2">Категория вопроса</label>
      <div class="col-lg-5"><SharePoint:FieldValue FieldName="QuestionCategoryLink" runat="server" /></div>
      <label class="col-lg-2">Тип решения</label>
      <div class="col-lg-3">
				<SharePoint:FormField runat="server" FieldName="Тип решения" />
      </div>
    </div>
    <asp:Repeater ID="repeaterMeasures" runat="server">
      <ItemTemplate>
        <div class="form-group">
          <label class="col-lg-2"><%# Eval("Measure") %></label>
          <div class="col-lg-10">
            <%# Eval("Value") %> <%# Eval("MeasuringUnit") %>
          </div>
        </div>
      </ItemTemplate>
    </asp:Repeater>
  </div>
</div>

