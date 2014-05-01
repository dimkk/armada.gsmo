<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObjectPhotoLibraryWebPartUserControl.ascx.cs" Inherits="gs.meeting.Components.ObjectPhotoLibraryWebPartUserControl, gs.meeting.Components, Version=1.0.0.0, Culture=neutral, PublicKeyToken=98b8bbe69daf0ccc" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<style>
    .galleria {
        height:420px
    }

    .galleria-container {
        background: white !important;
        border: 1px solid black !important;
    }
</style>

<asp:Label ID="labelMessage" runat="server" Text="Необходимо настроить соединение с веб частью объекта строительства"></asp:Label>

<div class="content">
    <div data-bind="foreach: GalleryRecordList" class="galleria">
        <a data-bind="attr: { href: Url }">
            <img data-bind="attr: { src: Url }">
        </a>
    </div>
    <div class="error">
        <p class="text-danger" data-bind="text: ErrorMsg"></p>
    </div>
</div>

<SharePoint:ScriptLink ID="ScriptLink1" runat="server" Name="Components/Scripts/knockout-2.3.0.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink2" runat="server" Name="Components/Scripts/camljs.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink3" runat="server" Name="Components/Scripts/gsCore.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink4" runat="server" Name="Components/Scripts/objectGalleryModel.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink5" runat="server" Name="Components/Scripts/galleria/galleria-1.3.5.min.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>

<script type="text/javascript">
    $(function () {
        gsCore.Utils.SureSPContext(function (ctx) {
            var webpartId = 'WebPart<%=this.Parent.ClientID%>';
            var params = gsCore.Utils.getWebPartCustomProperties(webpartId);
            if (!params ||
                !params.DataSourceListName ||
                !params.ObjectID ||
                !params.LinkFieldName ||
                !params.GalleriaConf ||
                !params.GalleriaTheme) return;
            
            var model = new ObjectGalleryModule.Model(ctx).LoadData({
                listName:       params.DataSourceListName,
                filterField:    params.LinkFieldName,
                filterValue:    params.ObjectID
            }, function(success) {
                if (!success) return;
                
                ko.applyBindings(model, document.getElementById(webpartId));
                
                Galleria.loadTheme('~site/_layouts/15/components/scripts/galleria/themes/' + params.GalleriaTheme);
                Galleria.run('#' + webpartId + ' .galleria', params.GalleriaConf);
            });
        });
    });
</script>

