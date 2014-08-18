<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchQuestion.aspx.cs" Inherits="SAMRT.Web.SearchQuestion" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/knockout-2.3.0.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <script type="text/javascript" src="/_layouts/15/SP.RequestExecutor.js"></script>
    <script type="text/javascript" src="../Scripts/searchQuestionModel.js"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
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

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Страница приложения
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Моя страница приложения
</asp:Content>
