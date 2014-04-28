<%@ Page language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <label>Список справочников системы</label>
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
    <SharePoint:ScriptLink name="sp.js" runat="server" OnDemand="true" LoadAfterUI="true" Localizable="false" />
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <WebPartPages:WebPartZone runat="server" FrameType="TitleBarOnly" ID="full" Title="loc:full" />
    <div class="container" style="margin-top: 25px;">
        <ul>
            <li><a href="/GradSovetApp/Lists/PositionBookList">Должности</a></li>
            <li><a href="/GradSovetApp/Lists/OrganizationBookList">Организации</a></li>
            <li><a href="/GradSovetApp/Lists/DocTypeBookList">Типы документов</a></li>
            <li><a href="/GradSovetApp/Lists/AssignmentTypeList">Типы поручений</a></li>
            <li><a href="/GradSovetApp/Lists/ParticipantBookList">Участники</a></li>
            <li><a href="/GradSovetApp/Lists/MeasuringUnitBookList">Единицы измерения</a></li>
            <li><a href="/GradSovetApp/Lists/AgendaQuestionCategoryBookList">Категории вопросов</a></li>
            <li><a href="/GradSovetApp/Lists/MeasureTypeBookList">Список показателей</a></li>
            <li><a href="/GradSovetApp/Lists/MeasureList">Градостроительные показатели</a></li>
        </ul>
    </div>
</asp:Content>
