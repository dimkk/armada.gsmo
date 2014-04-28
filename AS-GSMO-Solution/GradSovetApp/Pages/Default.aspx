<%@ Page language="C#" MasterPageFile="~site/_catalogs/masterpage/gs.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <link rel="Stylesheet" type="text/css" href="../Content/default-page.css" />
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.10.3.min.js"></script>
    <SharePoint:ScriptLink name="sp.js" runat="server" OnDemand="true" LoadAfterUI="true" Localizable="false" />
    <script type="text/javascript">
        $(document).ready(function () {

            ExecuteOrDelayUntilScriptLoaded(function () {
                
                //update master page for internal pages        
                var strIntMasterPageUrl = "/GradSovetApp/_catalogs/masterpage/gs-internal.master";
                var ctx = SP.ClientContext.get_current();
                var web = ctx.get_web();

                ctx.load(web);
                ctx.executeQueryAsync(function() {
                    if (web.get_masterUrl() !== strIntMasterPageUrl) {

                        web.set_masterUrl(strIntMasterPageUrl);
                        web.set_customMasterUrl(strIntMasterPageUrl);
                        web.update();

                        ctx.executeQueryAsync(
                            null,
                            function(sender, args) {
                                alert("Произошла ошибка при установке master page: " + args.get_message());
                            }
                        );
                    }
                });

                // delete redundant webpart after upgrade (sp bug)
                var oFile = web.getFileByServerRelativeUrl("/GradSovetApp/Pages/Default.aspx");
                var wpManager = oFile.getLimitedWebPartManager(SP.WebParts.PersonalizationScope.shared);
                var wpList = wpManager.get_webParts();
                
                ctx.load(wpList);
                ctx.executeQueryAsync(function () {
                    var wpDefinition;
                    if (wpList.get_count() > 1) {
                        wpDefinition = wpList.get_item(0);
                        wpDefinition.deleteWebPart();

                        ctx.load(wpDefinition);
                        ctx.executeQueryAsync(function() {
                            location.reload();
                        }, function(sender, args) {
                            alert("Произошла ошибка при удалении веб части: " + args.get_message());
                        })
                    } else {
                        $('#divMenuTiles').css({ 'display': 'block' });
                        $('#divWaitBlock').css({ 'display': 'none' });
                    }
                }, function(sender, args) {
                    alert("Произошла ошибка при получении LimitedWebPartManager: " + args.get_message());
                });


            }, "sp.js");
            

            // Update this value to the number of links you want to show per row
            var numberOfLinksPerRow = 4;

            // local variables
            var pre = "<tr><td><div class='ms-promlink-body' id='promlink_row_";
            var post = "'></div></td></tr>";
            var numberOfLinksInCurrentRow = numberOfLinksPerRow;
            var currentRow = 1;
            // find the number of promoted links we're displaying
            var numberOfPromotedLinks = $('.ms-promlink-body > .ms-tileview-tile-root').length;
            // if we have more links then we want in a row, let's continue
            if (numberOfPromotedLinks > numberOfLinksPerRow) {
                // we don't need the header anymore, no cycling through links
                $('.ms-promlink-root > .ms-promlink-header').empty();
                // let's iterate through all the links after the maximum displayed link
                for (i = numberOfLinksPerRow + 1; i <= numberOfPromotedLinks; i++) {
                    // if we're reached the maximum number of links to show per row, add a new row
                    // this happens the first time, with the values set initially
                    if (numberOfLinksInCurrentRow == numberOfLinksPerRow) {
                        // i just want the 2nd row to
                        currentRow++;

                        // create a new row of links
                        $('.ms-promlink-root > table > tbody:last').append(pre + currentRow + post);
                        // reset the number of links for the current row
                        numberOfLinksInCurrentRow = 0
                    }

                    // move the Nth (numberOfLinksPerRow + 1) div to the current table row
                    $('.ms-promlink-body > .ms-tileview-tile-root:nth-child(' + (numberOfLinksPerRow + 1) + ')').appendTo($('#promlink_row_' + currentRow));
                    // increment the number of links in the current row
                    numberOfLinksInCurrentRow++;

                }
            }

            // update width of the 7's tile
            //$("div[id^='Tile_WPQ1_7']").each(function () { $(this).width(310) });
        });
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <WebPartPages:WebPartZone runat="server" FrameType="TitleBarOnly" ID="full" Title="loc:full" />
    <div id="divWaitBlock" class="ms-dlgLoadingTextDiv ms-alignCenter">
	    <span style="padding-top: 6px; padding-right: 10px;">
	        <img src="/_layouts/15/images/gears_anv4.gif?rev=23" title="Эта анимация обозначает выполняющуюся операцию" />
	    </span>
	    <span class="ms-core-pageTitle ms-accentText">Пожалуйста, подождите</span>
    </div>
    <div id="divMenuTiles" class="centered" style="display: none">
        <WebPartPages:WebPartZone runat="server" FrameType="TitleBarOnly" ID="MenuTiles" Title="loc:full">
            <WebPartPages:XsltListViewWebPart runat="server" 
                ListUrl="Lists/MainMenuList" IsIncluded="True" NoDefaultStyle="TRUE" 
                Title=" " PageType="PAGE_NORMALVIEW" Default="False" ViewContentTypeId="0x">
            </WebPartPages:XsltListViewWebPart>
        </WebPartPages:WebPartZone>
    </div>
</asp:Content>
