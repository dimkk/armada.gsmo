<%@ Assembly Name="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%> 
<%@ Page Language="C#" Inherits="Microsoft.SharePoint.WebPartPages.WikiEditPage" MasterPageFile="~masterurl/default.master"      MainContentID="PlaceHolderMain" %> 
<%@ Import Namespace="Microsoft.SharePoint.WebPartPages" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Src="~/_controltemplates/15/GradSovetMeetingCalendar/CalendarUserControl.ascx" TagPrefix="myControls" TagName="CalendarUserControl" %>
<%@ Register Src="~/_controltemplates/15/GradSovetMeetingCalendar/LastProtocolsUserControl.ascx" TagPrefix="myControls" TagName="LastProtocolsUserControl" %>
<%@ Register Src="~/_controltemplates/15/GradSovetMeetingCalendar/NextMeetingUserControl.ascx" TagPrefix="myControls" TagName="NextMeetingUserControl" %>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server"></asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageImage" runat="server"></asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server"></asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderMiniConsole" runat="server"></asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderLeftActions" runat="server"></asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <h1>Календарь заседаний</h1>
    <table>
       <tr>
           <td rowspan="2" style="width: 100%; min-width: 750px;"><myControls:CalendarUserControl runat="server" /></td>
           <td style="vertical-align:top; width: 230px;"><myControls:NextMeetingUserControl runat="server" /></td>
       </tr>
       <tr style="width: 230px;">
           <td style="vertical-align: top; min-height: 300px; width: 230px;"><myControls:LastProtocolsUserControl runat="server" /></td>
       </tr>
   </table>
</asp:Content>