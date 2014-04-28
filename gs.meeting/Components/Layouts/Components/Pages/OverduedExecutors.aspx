<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OverduedExecutors.aspx.cs" Inherits="gs.meeting.Components.OverduedExecutors" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="/_controltemplates/15/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="/_controltemplates/15/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" Src="/_controltemplates/15/ButtonSection.ascx" %>

<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
	<SharePoint:EncodedLiteral runat="server" text="Должники по поручениям" EncodeMethod='HtmlEncode'/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
	<SharePoint:EncodedLiteral runat="server" text="Должники по поручениям" EncodeMethod='HtmlEncode'/>
</asp:Content>
<asp:Content ID="PageDescription" ContentPlaceHolderID="PlaceHolderPageDescription"  runat="server">
    <SharePoint:EncodedLiteral runat="server" text="Список должников по поручениям с количеством просроченных поручений" EncodeMethod='HtmlEncode'/>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    
    <style type="text/css">
        /*Table even odd row colors*/
        table.evenodd tr:nth-child(odd) {color: #000; background: #FFF}
        table.evenodd tr:nth-child(even) {color: #000; background: #F7F7F7}
    </style>

    <SharePoint:ScriptLink ID="ScriptLink1" runat="server" Name="Components/Scripts/knockout-2.3.0.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
    <SharePoint:ScriptLink ID="ScriptLink2" runat="server" Name="Components/Scripts/camljs.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
    <SharePoint:ScriptLink ID="ScriptLink4" runat="server" Name="Components/Scripts/spin.min.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
    <SharePoint:ScriptLink ID="ScriptLink3" runat="server" Name="Components/Scripts/overduedExecutorModel.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>

    <script type="text/javascript">
        function sharepointReady() {
            SP.SOD.executeOrDelayUntilScriptLoaded(function () {
                var ctx = SP.ClientContext.get_current();
                ko.applyBindings(new OverduedExecutor.Model(ctx).loadData(0));
            }, "sp.js");

        }

        $(document).ready(function () {
            SP.SOD.executeFunc("sp.js", "SP.ClientContext", sharepointReady);
        });
    </script>

</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
        
    <div class="container">
        <table class="table evenodd">
            <tbody data-bind="foreach: Executors">
                <tr>
                    <td data-bind="text: Name, style: { textAlign: 'left' }"></td>
                    <td data-bind="style: { textAlign: 'center' }">
                        <a data-bind="text: OverCount, click: showOverduedAssignments, style: { cursor: 'pointer' }"></a>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <SharePoint:FormDigest ID="FormDigest1" runat="server" />
</asp:Content>
