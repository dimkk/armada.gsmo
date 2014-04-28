<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MarksListWebPartUserControl.ascx.cs" Inherits="gs.meeting.Components.MarksListWebPartUserControl, gs.meeting.Components, Version=1.0.0.0, Culture=neutral, PublicKeyToken=98b8bbe69daf0ccc" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<style type="text/css">
    table td {
        border-top: none !important;
    }

    .font-larger {
        font-size: larger !important;
    }

    .line-double {
        line-height: 200% !important;
    }
</style>

<div class="panel panel-default">
    <div class="panel-heading">
        <h3 class="panel-title">
            <asp:Label ID="PanelTitleLabel" runat="server"></asp:Label>
        </h3>    
    </div>
    <div class="panel-body">
        <div class="table-responsive">
            <table class="table table-striped">
                <tbody data-bind="foreach: MarkRecordList">
                    <tr>
                        <td><label class="label label-danger font-larger" data-bind="text: CountValue"></label></td>
                        <td class="line-double" data-bind="html: getFormattedTitle()"></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</div>


<SharePoint:ScriptLink ID="ScriptLink1" runat="server" Name="Components/Scripts/knockout-2.3.0.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink2" runat="server" Name="Components/Scripts/camljs.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink3" runat="server" Name="Components/Scripts/gsCore.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink4" runat="server" Name="Components/Scripts/marksModel.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>

<script type="text/javascript">
    $(document).ready(function () {
        gsCore.Utils.SureSPContext(function (ctx) {
            var params = gsCore.Utils.getWebPartCustomProperties("WebPart<%=this.Parent.ClientID%>");
            if (params && params.DataSourceListName) {
                var model = new MarksModule.Model(ctx).LoadData(params.DataSourceListName);
                ko.applyBindings(model, $("[id=WebPart<%=this.Parent.ClientID%>]")[0]);
            } else {
                /*$("#NewsPanelContentID").empty().append(
                    "<p class='text-danger'>There was an error while processing web part parameters. Be sure to point list name out. </p>");*/
            }
        });
    });
</script>
