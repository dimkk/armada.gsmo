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

    <link rel="stylesheet" type="text/css" href="../Content/bootstrap/bootstrap.min.css" />
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />
    <link rel="Stylesheet" type="text/css" href="../Content/participantsDialog.css" />

    <script type="text/javascript" src="../Scripts/executorModel.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#s4-ribbonrow").css({ 'display': 'none' });
            
            ExecutorPage.init();
        });
    </script>
    <div class="container">
        <table class="table" style="margin-bottom: 0px">
            <tbody>
                <tr>
                    <td>
                        <div class="radio">
                            <label>
                                <input type="radio" value="FIO" data-bind="checked: radioSelectedOptionValue, click: $root.search" />
                                По ФИО
                            </label>
                        </div>
                    </td>            
                    <td>
                        <div class="radio">
                            <label>
                                <input type="radio" value="Position" data-bind="checked: radioSelectedOptionValue, click: $root.search" />
                                По должности
                            </label>
                        </div>    
                    </td>
                    <td>
                        <div class="radio">
                            <label>
                                <input type="radio" value="Organization" data-bind="checked: radioSelectedOptionValue, click: $root.search" />
                                По организации
                            </label>
                        </div>    
                    </td>
                </tr>
            </tbody>
        </table>
        <div style="float: left; width: 94%; margin-bottom: 20px">
            <input data-bind="value: searchInput, valueUpdate: 'afterkeydown'" type="text" class="form-control" />
        </div>
        <div style="float: left; width: 5%; margin-left: 1%">
            <button type="button" class="btn btn-default" data-bind="click: $root.search"><span class="glyphicon glyphicon-search"></span></button>    
        </div>
        
        <table class="table">
            <thead>
                <tr>
                    <th data-bind="style: { display: radioSelectedOptionValue() == 'FIO' ? 'table-cell' : 'none' }">ФИО</th>
                    <th data-bind="style: { display: radioSelectedOptionValue() == 'FIO' || radioSelectedOptionValue() == 'Position' ? 'table-cell' : 'none' }">Должность</th>
                    <th data-bind="style: { display: radioSelectedOptionValue() == 'FIO' || radioSelectedOptionValue() == 'Organization' ? 'table-cell' : 'none' }">Организация</th>
                    <th></th>
                </tr>
            </thead>
            <tbody data-bind="foreach: searchResults">
                <tr>
                    <td data-bind="text: FullName, style: { display: $root.radioSelectedOptionValue() == 'FIO' ? 'table-cell' : 'none' }"></td>
                    <td data-bind="text: Position, style: { display: $root.radioSelectedOptionValue() == 'FIO' || $root.radioSelectedOptionValue() == 'Position' ? 'table-cell' : 'none' }"></td>
                    <td data-bind="text: Org, style: { display: $root.radioSelectedOptionValue() == 'FIO' || $root.radioSelectedOptionValue() == 'Organization' ? 'table-cell' : 'none' }"></td>
                    <td><button type="button" class="btn btn-default" data-bind="click: $root.CommitPopUp"><span class="glyphicon glyphicon-plus"></span></button></td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
