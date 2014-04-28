<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AgendaQuestionTitleUserControl.ascx.cs" Inherits="GSWeb.WebParts.AgendaQuestionTitle.AgendaQuestionTitleUserControl" %>

<div class="container" id="header">
    <h4 class="text-center">Рассмотрение вопроса на Градостроительном Совете</h4>
    <h5 class="text-center">
      <strong>
        <span id="labelMeetingAddress" style="color: blue" runat="server"><%= Address %></span>
        <span id="labelCadastreNumber" runat="server">кад.№ <%= CadastreNumber %></span>
      </strong>
    </h5>
    <h5 class="text-center" id="customText"><strong><%= Category %></strong></h5>
    <div class="container" style="margin-bottom: 25px;">
        <div class="col-lg-4 text-left">
            <a id="meetingLink" href="/_layouts/15/gradsovetpages/pages/meeting.aspx?ID=<%= MeetingId %>&mode=display" target="_blank"><%= MeetingDescription %></a>
        </div>
        <div class="col-lg-offset-6 col-lg-2 text-right">
          <asp:HyperLink ID="protocolLink" Text="Протокол заседания" runat="server" />
        </div>
    </div>
</div>