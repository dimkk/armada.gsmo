<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AgendaQuestionMeasuresEditMode.ascx.cs" Inherits="GradSovetWeb.WebParts.AgendaQuestionMeasures.AgendaQuestionMeasuresEditMode" %>

<asp:UpdatePanel runat="server">
  <ContentTemplate>
    <div class="container" style="margin-top: 25px;">
      <div class="form-horizontal" role="form">
        <div class="form-group">
          <label class="col-lg-2">Категория вопроса</label>
          <div class="col-lg-10">
            <asp:DropDownList ID="lookupQuestionType" DataValueField="Id" DataTextField="Name" AppendDataBoundItems="True"
                              OnSelectedIndexChanged="OnQuestionTypeChanged" AutoPostBack="true" runat="server">
              <Items>
                <asp:ListItem Text="" Value="0"></asp:ListItem>
              </Items>
            </asp:DropDownList>
          </div>
        </div>
        <asp:Repeater ID="repeaterMeasures" OnItemDataBound="OnItemDataBound" runat="server">
          <ItemTemplate>
            <asp:HiddenField ID="hiddenMeasure" runat="server" />
            <div class="form-group">
              <label class="col-lg-2"><%# Eval("Measure.LookupValue") %></label>
              <div class="col-lg-5">
                <asp:TextBox ID="textValue" runat="server" />
              </div>
              <div class="col-lg-5">
                <asp:PlaceHolder ID="placeMeasuringUnit" runat="server" />
                <asp:DropDownList ID="lookupUnit" DataValueField="LookupId" DataTextField="LookupValue" Visible="false" runat="server" />
              </div>
            </div>
          </ItemTemplate>
        </asp:Repeater>
      </div>
    </div>
  </ContentTemplate>
</asp:UpdatePanel>
