<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="gs" Namespace="gs.meeting.Components" Assembly="gs.meeting.Components, Version=1.0.0.0, Culture=neutral, PublicKeyToken=98b8bbe69daf0ccc"%>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Assembly Name="Microsoft.Office.DocumentManagement, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AgendaQuestionAttach.aspx.cs" Inherits="SAMRT.Web.AgendaQuestionAttach" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBar" Src="~/_controltemplates/15/ToolBar.ascx" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <WebPartPages:WebPartZone runat="server" ID="MainWPZone" Title="Зона веб частей" WebPart="true">
        <ZoneTemplate>
            <gs:AgendaQuestionDocumentSetWebPart runat="server" __MarkupType="xmlmarkup" WebPart="true" __WebPartId="{710654AA-507A-4A63-93D9-E1F6A6B0487B}">
                <WebPart xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.microsoft.com/WebPart/v2">
                  <Title />
                  <FrameType>None</FrameType>
                  <Description>Веб часть отображает вложения вопроса повестки в виде набора документов. Размещается на странице вопроса повестки</Description>
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
                  <ID>g_710654AA_507A_4A63_93D9_E1F6A6B0487B</ID>
                </WebPart>                   
            </gs:AgendaQuestionDocumentSetWebPart>
        </ZoneTemplate>
    </WebPartPages:WebPartZone>
	<SharePoint:GoBackButton ID="GoBackButton" ControlMode="Display" runat="server"/>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Вложения вопроса повестки
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Вложения вопроса повестки
</asp:Content>
