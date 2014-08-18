<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="MVK" Namespace="GS.MVK.Web" Assembly="GS.MVK.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e6d7b98455b283b9"%>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Office.DocumentManagement, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IssueAttachmentMVK.aspx.cs" Inherits="SAMRT.Web.IssueAttachmentMVK" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <style type="text/css">
        .ms-menutoolbar { display: none }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <WebPartPages:webpartzone runat="server" id="MainWPZone" title="Зона веб частей" webpart="true">
        <ZoneTemplate>
            <MVK:DocumentSet runat="server" DocumentLibraryUrl="IssueAttachmentMVKList" QuestionLinkFieldName="IssueAttachmentIssueMVK" __MarkupType="xmlmarkup" WebPart="true" __WebPartId="{6A9AEC59-E2C0-4DDA-9F20-7DB5F74F80AC}">
                <WebPart xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.microsoft.com/WebPart/v2">
                  <Title>Набор документов</Title>
                  <FrameType>None</FrameType>
                  <Description>Веб-часть для работы с наборами документов</Description>
                  <IsIncluded>true</IsIncluded>
                  <ZoneID>MainWPZone</ZoneID>
                  <PartOrder>1</PartOrder>
                  <FrameState>Normal</FrameState>
                  <Height />
                  <Width />
                  <AllowRemove>false</AllowRemove>
                  <AllowZoneChange>false</AllowZoneChange>
                  <AllowMinimize>false</AllowMinimize>
                  <AllowConnect>true</AllowConnect>
                  <AllowEdit>true</AllowEdit>
                  <AllowHide>false</AllowHide>
                  <IsVisible>true</IsVisible>
                  <DetailLink />
                  <HelpLink />
                  <HelpMode>Modeless</HelpMode>
                  <Dir>Default</Dir>
                  <PartImageSmall />
                  <MissingAssembly>Невозможно импортировать эту веб-часть.</MissingAssembly>
                  <PartImageLarge />
                  <IsIncludedFilter />
                  <ExportControlledProperties>true</ExportControlledProperties>
                  <ConnectionID>00000000-0000-0000-0000-000000000000</ConnectionID>
                  <ID>g_6a9aec59_e2c0_4dda_9f20_7db5f74f80ac</ID>
                </WebPart>                   
            </MVK:DocumentSet>
        </ZoneTemplate>
    </WebPartPages:webpartzone>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Вложения вопроса повестки МВК
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
Вложения вопроса повестки МВК
</asp:Content>
