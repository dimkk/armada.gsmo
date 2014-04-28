<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Import Namespace="Microsoft.SharePoint" %> <%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AgendaQuestionMeasuresEditMode.ascx.cs" Inherits="GSWeb.WebParts.AgendaQuestionMeasures.AgendaQuestionMeasuresEditMode" %>

<asp:UpdatePanel runat="server">
  <ContentTemplate>
    <div class="container" style="margin-top: 25px;">
      <div class="form-horizontal" role="form">
        <div class="form-group">
          <label class="col-lg-2">Категория вопроса</label>
          <div class="col-lg-5">
            <asp:DropDownList ID="lookupQuestionType" DataValueField="Id" DataTextField="Name" AppendDataBoundItems="True"
                              OnSelectedIndexChanged="OnQuestionTypeChanged" AutoPostBack="true" CssClass="form-control" runat="server">
              <Items>
                <asp:ListItem Text="" Value="0"></asp:ListItem>
              </Items>
            </asp:DropDownList>
          </div>
          <label class="col-lg-2">Тип решения</label>
          <div class="col-lg-3">
						<SharePoint:FormField runat="server" ControlMode="Edit" CssClass="form-control" FieldName="Тип решения" />
          </div>
        </div>
        <asp:Repeater ID="repeaterMeasures" OnItemDataBound="OnItemDataBound" runat="server">
          <ItemTemplate>
            <asp:HiddenField ID="hiddenMeasure" runat="server" />
            <div class="form-group">
              <label class="col-lg-2"><%# Eval("Measure.LookupValue") %></label>
              <div class="col-lg-5">
                <asp:TextBox ID="textValue" CssClass="form-control" runat="server" />
              </div>
              <div class="col-lg-5">
                <asp:PlaceHolder ID="placeMeasuringUnit" runat="server" />
                <asp:DropDownList ID="lookupUnit" DataValueField="LookupId" DataTextField="LookupValue" CssClass="form-control" Visible="false" runat="server" />
              </div>
            </div>
          </ItemTemplate>
        </asp:Repeater>
      </div>
    </div>
  </ContentTemplate>
</asp:UpdatePanel>
