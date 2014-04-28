<%@ Page language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.10.3.min.js"></script>
    <script type="text/javascript" src="../Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="../Scripts/knockout-2.3.0.js"></script>
    <script type="text/javascript" src="../Scripts/moment-with-langs.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <script type="text/javascript" src="/_layouts/15/SP.RequestExecutor.js"></script>
    <script type="text/javascript" src="../Scripts/participantModel.js"></script>
    <script type="text/javascript" src="../Scripts/App.js"></script>

    <link rel="stylesheet" type="text/css" href="../Content/bootstrap/bootstrap.min.css" />
    <link rel="Stylesheet" type="text/css" href="../Content/participantsDialog.css" />
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <script type="text/javascript">
        $(function () {
            $(".data-input").keyup(function (event) {
                if (event.keyCode == 13) {
                    $("#search-btn").click();
                }
            });
            
            $("#s4-ribbonrow").css({ 'display': 'none' });

            ParticipantPage.init(decodeURIComponent($.getUrlVar("Multi")));
        });
    </script>
    <div class="container">
        <table class="table">
            <tbody>
                <tr>
                    <td>
                        <input data-bind="value: searchFIO, valueUpdate: 'afterkeydown'" type="text" class="form-control data-input" placeholder="Имя" />
                    </td> 
                    <td>
                        <input data-bind="value: searchPosition, valueUpdate: 'afterkeydown'" type="text" class="form-control data-input" placeholder="Должность" />
                    </td>
                    <td>
                        <input data-bind="value: searchOrganization, valueUpdate: 'afterkeydown'" type="text" class="form-control data-input" placeholder="Организация" /> 
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
                    <td data-bind="text: ParticipantFullName"></td>
                    <td data-bind="text: ParticipantPosition"></td>
                    <td data-bind="text: ParticipantOrg"></td>
                    <td><button type="button" class="btn btn-default" data-bind="click: $root.select"><span class="glyphicon glyphicon-plus"></span></button></td>
                    </tr>
                </tbody>
            </table>
        <label data-bind="visible: isMulti">Выбранные участники</label>
        <div id="selectedParticipantsContainer" data-bind="visible: isMulti">
            <table id="selectedParticipantsTable" class="table">
                <tbody data-bind="foreach: selectedParticipants">
                    <tr>
                        <td data-bind="text: ParticipantFullName"></td>
                        <td data-bind="text: ParticipantPosition"></td>
                        <td data-bind="text: ParticipantOrg"></td>
                        <td data-bind="text: ParticipantRole"></td>
                        <td>
                            <button type="button" class="btn btn-default" data-bind="click: $root.remove"><span class="glyphicon glyphicon-trash"></span></button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="modal-footer" data-bind="visible: isMulti">
            <button type="button" class="btn btn-default" data-bind="click: $root.ClosePopUp">Отмена</button>
            <button type="button" class="btn btn-primary" data-bind="click: $root.CommitPopUp">Ок</button>
        </div>
    </div>
</asp:Content>
