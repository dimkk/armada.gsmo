<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OverduedExecutorAssignments.aspx.cs" Inherits="gs.meeting.Components.OverduedExecutorAssignments" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" runat="server" ContentPlaceHolderID="PlaceHolderAdditionalPageHead">
    <SharePoint:ScriptLink runat="server" ID="ScriptLink1" Name="Components/Scripts/adjustModal.js" Localizable="False" OnDemand="False" ></SharePoint:ScriptLink>
    <link rel="stylesheet" type="text/css" href="<%= SPUtility.MakeBrowserCacheSafeLayoutsUrl("Components/Content/ExecutorModal.css", false) %>" />
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat  ="server">
	<SharePoint:EncodedLiteral runat="server" text="Просроченные поручения" EncodeMethod='HtmlEncode'/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
	<SharePoint:EncodedLiteral runat="server" text="Просроченные поручения" EncodeMethod='HtmlEncode'/>
</asp:Content>
<asp:Content ID="PageDescription" ContentPlaceHolderID="PlaceHolderPageDescription"  runat="server">
    <SharePoint:EncodedLiteral runat="server" text="Просроченные поручения" EncodeMethod='HtmlEncode'/>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    
    <div class="container col-lg-10" id="ListViewByQuery1">
        <SharePoint:ListViewByQuery ID="MainListView" runat="server"/>  
    </div>
    <SharePoint:FormDigest ID="FormDigest1" runat="server" />

</asp:Content>

