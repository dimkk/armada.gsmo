<%@ Page language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.10.3.min.js"></script>
    <script type="text/javascript" src="../Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="../Scripts/knockout-2.3.0.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <script type="text/javascript" src="/_layouts/15/SP.RequestExecutor.js"></script>
    <script type="text/javascript" src="../Scripts/searchQuestionModel.js"></script>
    
    <link rel="stylesheet" type="text/css" href="../Content/bootstrap/bootstrap.min.css" />
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#s4-ribbonrow").css({ 'display': 'none' });
            
            SearchQuestionPage.init();
        });
    </script>
    <div class="container">
        <table class="table">
            <tbody>
                <tr>
                    <td>
                        <input data-bind="value: searchTheme, valueUpdate: 'afterkeydown'" type="text" class="form-control data-input" placeholder="Поиск по теме вопроса" />
                    </td>
                    <td>
                        <button type="button" class="btn btn-default" data-bind="click: $root.search" id="search-btn"><span class="glyphicon glyphicon-search"></span></button>
                    </td>
                </tr>
            </tbody>
        </table>
        <table id="searchResultsTable" class="table table-striped">
            <tbody data-bind="foreach: searchResults">
                <tr>
                    <td data-bind="text: AgendaQuestionNumber"></td>
                    <td data-bind="text: AgendaQuestionTheme"></td>
                    <td>
                        <button type="button" class="btn btn-default" data-bind="click: $root.CommitPopUp"><span class="glyphicon glyphicon-plus"></span></button>
                    </td>
                </tr>
            </tbody>            
        </table>
    </div>
</asp:Content>
