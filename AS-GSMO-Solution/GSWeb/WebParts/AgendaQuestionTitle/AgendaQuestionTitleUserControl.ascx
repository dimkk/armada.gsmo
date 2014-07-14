<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AgendaQuestionTitleUserControl.ascx.cs" Inherits="GSWeb.WebParts.AgendaQuestionTitle.AgendaQuestionTitleUserControl" %>

<div class="container" id="header">
    <h4 class="text-center"><%= Header %></h4>
    <h5 class="text-center">
      <strong>
        <% if (!string.IsNullOrEmpty(Address)) { %>
        <span id="labelMeetingAddress" style="color: blue"><%= Address %></span>
        <% } %>
        <% if (!string.IsNullOrEmpty(CadastreNumber)) { %>
        <span id="labelCadastreNumber">кад.№ <%= CadastreNumber %></span>
        <% } %>
      </strong>
    </h5>
    <h5 class="text-center" id="customText"><strong><%= Category %></strong></h5>
    <div class="container" style="margin-bottom: 25px;">
        <div class="col-lg-4 text-left">
            <a id="meetingLink" href="<%= MeetingUrl %>" target="_blank"><%= MeetingDescription %></a>
        </div>
        <% if (!string.IsNullOrEmpty(ProtocolUrl)) { %>
        <div class="col-lg-offset-6 col-lg-2 text-right">
            <a id="protocolLink" href="<%= ProtocolUrl %>" target="_blank">Протокол заседания</a>
        </div>
        <% } %>
    </div>
    <% if (IsErrors) { %>
    <span id="lErrors" style="color: red"><%= Errors %></span>
    <% } %>
</div>