<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignmentOverduedExecutorsUserControl.ascx.cs" Inherits="gs.meeting.Components.AssignmentOverduedExecutorsUserControl, gs.meeting.Components, Version=1.0.0.0, Culture=neutral, PublicKeyToken=98b8bbe69daf0ccc" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<div class="panel panel-default" id="OverduedExecutorsWebPartID">
    <div class="panel-heading">
        <h3 class="panel-title">
            <asp:Label ID="PanelTitleLabel" runat="server"></asp:Label>
        </h3>    
    </div>
    <div class="panel-body" id="PanelContent">
        <div>
            <table class="table">
                <thead>
                    <tr>
                        <th><asp:Label ID="OrgLabel" runat="server"></asp:Label></th>
                        <th><asp:Label ID="AmountLabel" runat="server"></asp:Label></th>
                    </tr>
                </thead>
                <tbody data-bind="foreach: Executors">
                    <tr>
                        <td data-bind="text: Name, style: {textAlign: 'left'}"></td>
                        <td data-bind="style: {textAlign: 'center'}">
                            <a data-bind="text: OverCount, click: showOverduedAssignments, style: {cursor: 'pointer'}"></a>
                        </td>
                    </tr>
                </tbody>
            </table>
            <div data-bind="style: {display: MoreMsg == '' ? 'none' : 'block', float: 'left'}">
                <a data-bind="text: MoreMsg, attr: {href: _spPageContextInfo.webAbsoluteUrl + '/_layouts/15/components/Pages/OverduedExecutors.aspx'}" target="_blank"></a>
            </div>
        </div>
    </div>
</div>

<SharePoint:ScriptLink ID="ScriptLink1" runat="server" Name="Components/Scripts/knockout-2.3.0.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink2" runat="server" Name="Components/Scripts/camljs.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink4" runat="server" Name="Components/Scripts/spin.min.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink3" runat="server" Name="Components/Scripts/overduedExecutorModel.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>

<script type="text/javascript">
    $(document).ready(function () {
        SP.SOD.executeFunc("sp.js", "SP.ClientContext", function() {
            SP.SOD.executeOrDelayUntilScriptLoaded(function () {
                var ctx = SP.ClientContext.get_current();
                ko.applyBindings(new OverduedExecutor.Model(ctx).loadData(5), document.getElementById("OverduedExecutorsWebPartID"));
            }, "sp.js");
        });
    });
</script>


