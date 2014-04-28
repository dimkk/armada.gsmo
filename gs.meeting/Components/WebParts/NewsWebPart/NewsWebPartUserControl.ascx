<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsWebPartUserControl.ascx.cs" Inherits="gs.meeting.Components.NewsWebPartUserControl, gs.meeting.Components, Version=1.0.0.0, Culture=neutral, PublicKeyToken=98b8bbe69daf0ccc" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<style type="text/css">
    .carousel-inner > .item {
        height: 400px;
    }    

    .carousel-inner > .item > a > img,
    .carousel-inner > .item > img {
        max-width: 100%;
        max-height: 300px;
        margin: auto;
    }

    #NewsPanelContentID {
        padding: 10px;
    }
</style>

<div class="panel panel-default" id="NewsWebPartId">
    <div class="panel-heading">
        <h3 class="panel-title">
            <asp:Label ID="PanelTitleLabel" runat="server"></asp:Label>
        </h3>    
    </div>
    <div class="panel-body" id="NewsPanelContentID">
        <div id="CarouselNews" class="carousel slide" data-ride="carousel">
            <div class="carousel-inner" data-bind="foreach: News">
                <div class="item" data-bind="css: { 'active': $index() == 0 }">
                  <img data-bind="attr: {'src': ImgUrl, 'alt': Caption}">
                  <div>
                    <h5 data-bind="text: Caption" style="padding: 5px; margin: 0px;"></h5>  
                    <p style="padding: 5px; margin: 0px;"><a data-bind="attr: { 'href': ContentUrl, 'target': '_blank' }, text: $root.DetailsMsg"></a></p>
                  </div>
                </div>
            </div>
            
            <a class="left carousel-control" href="#CarouselNews" data-slide="prev" style="background-image: none">
                <span class="glyphicon glyphicon-chevron-left"></span>
            </a>
            <a class="right carousel-control" href="#CarouselNews" data-slide="next" style="background-image: none">
                <span class="glyphicon glyphicon-chevron-right"></span>
            </a>
        </div>
    </div>
</div>

<SharePoint:ScriptLink ID="ScriptLink1" runat="server" Name="Components/Scripts/knockout-2.3.0.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink2" runat="server" Name="Components/Scripts/camljs.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink4" runat="server" Name="Components/Scripts/spin.min.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="ScriptLink3" runat="server" Name="Components/Scripts/newsModel.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>

    <script type="text/javascript">
        $(document).ready(function () {
            SP.SOD.executeFunc("sp.js", "SP.ClientContext", function() {
                SP.SOD.executeOrDelayUntilScriptLoaded(function () {
                    var ctx = SP.ClientContext.get_current();
                    //read webpart properties
                    var webpart = $("#NewsWebPartId").parent();
                    var field = $(webpart).find("[id*='NewsWebPartPropsFieldID']")[0];
                    var params = $.parseJSON($(field).val());
                    if (params.DataSourceListName && params.LastDayCount) {
                        ko.applyBindings(new NewsModule.Model(ctx).LoadData(
                                params.DataSourceListName,
                                params.LastDayCount),
                            document.getElementById("NewsWebPartId"));
                    } else {
                        $("#NewsPanelContentID").empty().append(
                            "<p class='text-danger'>There was an error while processing web part parameters. Be sure to point list name out. </p>");
                    }
                }, "sp.js");
            });
        });
    </script>
