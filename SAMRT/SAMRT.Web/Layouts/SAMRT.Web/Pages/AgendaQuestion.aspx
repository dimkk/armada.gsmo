<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WpNs0" Namespace="Microsoft.SharePoint.Portal.WebControls" Assembly="Microsoft.Office.Server.FilterControls, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>
<%@ Register TagPrefix="wssuc" TagName="ToolBar" src="~/_controltemplates/15/ToolBar.ascx" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AgendaQuestion.aspx.cs" Inherits="SAMRT.Web.AgendaQuestion" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <WebPartPages:SPProxyWebPartManager runat="server" ID="WPManager">
        <SPWebPartConnections>
            <WebPartPages:SPWebPartConnection ID="AssignmentListConnection"
                ConsumerConnectionPointID="DFWP Filter Consumer ID"
                ConsumerID="WPAssignmentList"
                ProviderConnectionPointID="ListFormRowProvider_WPQ_"
                ProviderID="g_e6a39e37_e8d8_4988_b730_d38b53c99d8b">
                <WebPartPages:SPRowToParametersTransformer
                    ConsumerFieldNames="AgendaQuestionLink"
                    ProviderFieldNames="ID" />
            </WebPartPages:SPWebPartConnection>
            <%--<WebPartPages:SPWebPartConnection 
                ID="AttachListConnection"
                ConsumerConnectionPointID="DFWP Filter Consumer ID" 
                ConsumerID="g_f4192783_6b05_4ba0_b705_756dd400e24c" 
                ProviderConnectionPointID="ListFormRowProvider_WPQ_" 
                ProviderID="g_e6a39e37_e8d8_4988_b730_d38b53c99d8b">
                <WebPartPages:SPRowToParametersTransformer 
                    ConsumerFieldNames="AgendaQuestionLink" 
                    ProviderFieldNames="ID">
                </WebPartPages:SPRowToParametersTransformer>
            </WebPartPages:SPWebPartConnection>--%>

            <%--<WebPartPages:SPWebPartConnection 
                ConsumerConnectionPointID="DFWP Filter Consumer ID" 
                ConsumerID="WPAttachList" 
                ID="c48669568" 
                ProviderConnectionPointID="ListFormRowProvider_WPQ_" 
                ProviderID="g_ce401811_ec8c_46e8_8691_b1b1761351bc">
                <WebPartPages:SPRowToParametersTransformer 
                    ConsumerFieldNames="AgendaQuestionLink" 
                    ProviderFieldNames="ID">
                </WebPartPages:SPRowToParametersTransformer>
            </WebPartPages:SPWebPartConnection>--%>

            <%-- Соединение для выборки связанных вложений --%>
            <%--<WebPartPages:SPWebPartConnection 
                ID="AttachListConnection"
                ConsumerConnectionPointID="DFWP Filter Consumer ID"
                ConsumerID="WPAttachList"
                ProviderConnectionPointID="ListFormRowProvider_WPQ_"
                ProviderID="g_516D4BAC_C760_40BE_BF11_3FA3DAD11430">
                <WebPartPages:SPRowToParametersTransformer 
                    ConsumerFieldNames="AgendaQuestionLink" 
                    ProviderFieldNames="ID">
                </WebPartPages:SPRowToParametersTransformer>
            </WebPartPages:SPWebPartConnection>--%>

            <%-- Соединение для выборки связанных поручений --%>
            <%--<WebPartPages:SPWebPartConnection
                ID="AssignmentListConnection"
                ConsumerConnectionPointID="DFWP Filter Consumer ID"
                ConsumerID="WPAssignmentList"
                ProviderConnectionPointID="ITransformableFilterValues"
                ProviderID="g_7028bc94_2259_4afd_94af_b6bf60e72759">
                <WebPartPages:TransformableFilterValuesToParametersTransformer
                    ConsumerFieldNames="AgendaQuestionLink"
                    ProviderFieldNames="QuestionId">
                </WebPartPages:TransformableFilterValuesToParametersTransformer>
            </WebPartPages:SPWebPartConnection>--%>
        </SPWebPartConnections>
    </WebPartPages:SPProxyWebPartManager>

    <WebPartPages:WebPartZone runat="server" ID="WebPartZoneService">
        <ZoneTemplate>
            <%--Веб часть фильтрации параметра ID строки запроса--%>
            <%--<WpNs0:QueryStringFilterWebPart runat="server" QueryStringParameterName="ID" FilterName="QuestionId"
                IsIncluded="True" ZoneID="WebPartZoneMain" FrameState="Normal" AllowConnect="True"
                MissingAssembly="Не удается импортировать эту веб-часть."
                ImportErrorMessage="Не удается импортировать эту веб-часть."
                ExportControlledProperties="True"
                ID="g_7028bc94_2259_4afd_94af_b6bf60e72759" ChromeType="None" ExportMode="All"
                __MarkupType="vsattributemarkup" __WebPartId="{7028BC94-2259-4AFD-94AF-B6BF60E72759}"
                WebPart="true" Height="" Width=""></WpNs0:QueryStringFilterWebPart>--%>
        </ZoneTemplate>
    </WebPartPages:WebPartZone>

    <div class="container" id="header">
        <h4 class="text-center">Рассмотрение вопроса на Градостроительном Совете</h4>
        <h5 class="text-center" id="meetingAddress"></h5>
        <h5 class="text-center" id="customText"></h5>
        <div class="container" style="margin-bottom: 25px;">
            <div class="col-lg-4 text-left">
                <a id="meetingLink" target="_blank"></a>
            </div>
            <div class="col-lg-offset-6 col-lg-2 text-right">
                <a id="protocolLink" target="_blank"></a>
            </div>
        </div>
    </div>

    <%-- Контейнер для вкладок --%>
    <div class="container" id="body">
        <%-- Вкладки --%>
        <ul id="tabs" class="nav nav-tabs">
            <li class="active"><a href="#tab-common" data-toggle="tab">Карточка вопроса</a></li>
            <li class="">
                <a href="#tab-assignment" data-toggle="tab">Поручения</a>
            </li>
            <li class="">
                <a href="#tab-attach" data-toggle="tab">Материалы к заседанию</a>
            </li>
            <li class="">
                <a href="#tab-search" data-toggle="tab">История рассмотрений</a>
            </li>
        </ul>
        <%-- Содержимое вкладок --%>
        <div id="tabContent" class="tab-content">
            <div id="tab-common" class="tab-pane fade active in">
                <WebPartPages:WebPartZone runat="server" FrameType="None" ID="WebPartZone1" Title="loc:Main">
                    <ZoneTemplate>
                        <WebPartPages:XsltListViewWebPart runat="server" ViewFlag=""
                            ViewSelectorFetchAsync="False" InplaceSearchEnabled="False" ServerRender="False"
                            ClientRender="True" InitialAsyncDataFetch="False"
                            WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False"
                            GhostedXslLink="main.xsl" NoDefaultStyle=""
                            ViewGuid="{F4192783-6B05-4BA0-B705-756DD400E24C}" EnableOriginalValue="False"
                            ViewContentTypeId="" ListUrl="" ListDisplayName=""
                            ListName="{AC20AD79-FE45-46E6-BE14-FA4E7A145011}"
                            ListId="ac20ad79-fe45-46e6-be14-fa4e7a145011"
                            PageSize="-1" UseSQLDataSourcePaging="True" DataSourceID=""
                            ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False"
                            AutoRefresh="False" AutoRefreshInterval="60"
                            Title="Вложения вопроса повестки" FrameType="Default" SuppressWebPartChrome="False"
                            Description="" IsIncluded="True"
                            FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                            AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True"
                            IsVisible="True" CatalogIconImageUrl="/_layouts/15/images/itgen.png"
                            TitleUrl="/Lists/AgendaAttachmentList" DetailLink="/Lists/AgendaAttachmentList"
                            HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall=""
                            MissingAssembly="Невозможно импортировать эту веб-часть."
                            ImportErrorMessage="Невозможно импортировать эту веб-часть."
                            PartImageLarge="/_layouts/15/images/itgen.png" IsIncludedFilter=""
                            ExportControlledProperties="False"
                            ConnectionID="00000000-0000-0000-0000-000000000000"
                            ID="g_f4192783_6b05_4ba0_b705_756dd400e24c" __MarkupType="vsattributemarkup"
                            __WebPartId="{F4192783-6B05-4BA0-B705-756DD400E24C}"
                            __AllowXSLTEditing="true"
                            __designer:CustomXsl="fldtypes_Ratings.xsl" WebPart="true" Height="" Width="">
                            <ParameterBindings>
                                <ParameterBinding Name="dvt_sortdir" Location="Postback;Connection"/>
                                <ParameterBinding Name="dvt_sortfield" Location="Postback;Connection"/>
                                <ParameterBinding Name="dvt_startposition" Location="Postback" DefaultValue=""/>
                                <ParameterBinding Name="dvt_firstrow" Location="Postback;Connection"/>
                                <ParameterBinding Name="OpenMenuKeyAccessible" Location="Resource(wss,OpenMenuKeyAccessible)" />
                                <ParameterBinding Name="open_menu" Location="Resource(wss,open_menu)" />
                                <ParameterBinding Name="select_deselect_all" Location="Resource(wss,select_deselect_all)" />
                                <ParameterBinding Name="idPresEnabled" Location="Resource(wss,idPresEnabled)" />
                                <ParameterBinding Name="NoAnnouncements" Location="Resource(wss,noXinviewofY_LIST)" />
                                <ParameterBinding Name="NoAnnouncementsHowTo" Location="Resource(wss,noXinviewofY_DEFAULT)" />
                            </ParameterBindings>
                            <DataFields>
                            </DataFields>
                            <XmlDefinition>
                                <View Name="{F4192783-6B05-4BA0-B705-756DD400E24C}" MobileView="TRUE" 
                                    Type="HTML" Hidden="TRUE" DisplayName="" 
                                    Url="/Lists/AgendaQuestionList/EditForm.aspx" 
                                    Level="1" BaseViewID="1" ContentTypeID="0x" 
                                    ImageUrl="/_layouts/15/images/generic.png?rev=23" >
                                    <Query>
                                        <OrderBy>
                                            <FieldRef Name="ID"/>
                                        </OrderBy>
                                    </Query>
                                    <ViewFields>
                                        <FieldRef Name="AgendaQuestionLink"/>
                                        <FieldRef Name="AttachmentDescription"/>
                                        <FieldRef Name="AttachmentDocumentTypeLink"/>
                                    </ViewFields>
                                    <RowLimit Paged="TRUE">30</RowLimit>
                                    <JSLink>clienttemplates.js</JSLink>
                                    <XslLink Default="TRUE">main.xsl</XslLink>
                                    <Toolbar Type="Standard"/></View>
                            </XmlDefinition>
                        </WebPartPages:XsltListViewWebPart>


                        <WebPartPages:XsltListViewWebPart ID="WPAssignmentList" runat="server"
                            ListUrl="Lists/AssignmentList" PageType="PAGE_NORMALVIEW" Title="Поручения"
                            FrameType="None">
                        </WebPartPages:XsltListViewWebPart>


                        <WebPartPages:ListFormWebPart ID="ListFormWebPart1" runat="server" __MarkupType="xmlmarkup"
                            WebPart="true" __WebPartId="{E6A39E37-E8D8-4988-B730-D38B53C99D8B}"
                            OnInit="WPAgendaQuestionForm_Init">
                            <WebPart xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
                                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
                                xmlns="http://schemas.microsoft.com/WebPart/v2">
                              <Title>Вопрос повестки заседания</Title>
                              <FrameType>Default</FrameType>
                              <Description />
                              <IsIncluded>true</IsIncluded>
                              <PartOrder>4</PartOrder>
                              <FrameState>Normal</FrameState>
                              <Height />
                              <Width />
                              <AllowRemove>true</AllowRemove>
                              <AllowZoneChange>true</AllowZoneChange>
                              <AllowMinimize>true</AllowMinimize>
                              <AllowConnect>true</AllowConnect>
                              <AllowEdit>true</AllowEdit>
                              <AllowHide>true</AllowHide>
                              <IsVisible>true</IsVisible>
                              <DetailLink />
                              <HelpLink />
                              <HelpMode>Modeless</HelpMode>
                              <Dir>Default</Dir>
                              <PartImageSmall />
                              <MissingAssembly>Невозможно импортировать эту веб-часть.</MissingAssembly>
                              <PartImageLarge />
                              <IsIncludedFilter />
                              <ExportControlledProperties>true</ExportControlledProperties>
                              <ConnectionID>00000000-0000-0000-0000-000000000000</ConnectionID>
                              <ID>g_e6a39e37_e8d8_4988_b730_d38b53c99d8b</ID>
                              <ListName xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">{1A9B16CD-3205-407B-BA65-E42AE5138ABB}</ListName>
                              <ListId xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">1a9b16cd-3205-407b-ba65-e42ae5138abb</ListId>
                              <PageType xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">PAGE_DISPLAYFORM</PageType>
                              <ListItemId xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">0</ListItemId>
                              <TemplateName xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">ListForm</TemplateName>
                            </WebPart>
                        </WebPartPages:ListFormWebPart>
                    </ZoneTemplate>
                </WebPartPages:WebPartZone>


                <WebPartPages:WebPartZone runat="server" ID="WebPartZoneMain">
                    <ZoneTemplate>
                        <%--<WebPartPages:ListFormWebPart ID="ListFormWebPart1" runat="server" __MarkupType="xmlmarkup" WebPart="true" __WebPartId="{CE401811-EC8C-46E8-8691-B1B1761351BC}" >
	                        <WebPart xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.microsoft.com/WebPart/v2">
		                        <Title>Вопрос повестки</Title>
		                        <FrameType>Default</FrameType>
		                        <Description />
		                        <IsIncluded>true</IsIncluded>
		                        <FrameState>Normal</FrameState>
		                        <Height />
		                        <Width />
		                        <AllowRemove>true</AllowRemove>
		                        <AllowZoneChange>true</AllowZoneChange>
		                        <AllowMinimize>true</AllowMinimize>
		                        <AllowConnect>true</AllowConnect>
		                        <AllowEdit>true</AllowEdit>
		                        <AllowHide>true</AllowHide>
		                        <IsVisible>true</IsVisible>
		                        <DetailLink />
		                        <HelpLink />
		                        <HelpMode>Modeless</HelpMode>
		                        <Dir>Default</Dir>
		                        <PartImageSmall />
		                        <MissingAssembly>Невозможно импортировать эту веб-часть.</MissingAssembly>
		                        <PartImageLarge />
		                        <IsIncludedFilter />
		                        <ExportControlledProperties>true</ExportControlledProperties>
		                        <ConnectionID>00000000-0000-0000-0000-000000000000</ConnectionID>
		                        <ID>g_ce401811_ec8c_46e8_8691_b1b1761351bc</ID>
		                        <ListName xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">{1A9B16CD-3205-407B-BA65-E42AE5138ABB}</ListName>
		                        <ListId xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">1A9B16CD-3205-407B-BA65-E42AE5138ABB</ListId>
		                        <PageType xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">PAGE_DISPLAYFORM</PageType>
		                        <ViewFlag xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">1048576</ViewFlag>
		                        <ViewFlags xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">Default</ViewFlags>
		                        <ListItemId xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">0</ListItemId>
		                        <TemplateName xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">ListForm</TemplateName>
	                        </WebPart>
                        </WebPartPages:ListFormWebPart>--%>


                        <%-- Веб часть отображения атрибутов вопроса, закладка "Карточка вопроса" --%>
                        <%--<WebPartPages:ListFormWebPart ID="WPAgendaQuestionForm" runat="server"
                            WebPart="true"
                            __WebPartId="{516D4BAC-C760-40BE-BF11-3FA3DAD11430}"
                            IsIncluded="True" 
                            FrameState="Normal" 
                            AllowRemove="True" 
                            AllowZoneChange="True" 
                            AllowMinimize="True" 
                            AllowConnect="True" 
                            AllowEdit="True" 
                            AllowHide="True" 
                            IsVisible="True">
                            <WebPart xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
                                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
                                xmlns="http://schemas.microsoft.com/WebPart/v2">
                              <Title>Карточка вопроса</Title>
                              <FrameType>None</FrameType>
                              <ListName xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">{1A9B16CD-3205-407B-BA65-E42AE5138ABB}</ListName>
                              <ListItemId xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">0</ListItemId>
                              <PageType xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">PAGE_DISPLAYFORM</PageType>
                              <ID>g_516D4BAC_C760_40BE_BF11_3FA3DAD11430</ID>
                              <TemplateName xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">CSRListForm</TemplateName>
                              <JSLink xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">~site/_layouts/15/SAMRT.Web/Scripts/csr/renderAgendaQuestion.js</JSLink>
                            </WebPart>
                        </WebPartPages:ListFormWebPart>--%>
                    </ZoneTemplate>
                </WebPartPages:WebPartZone>
            </div>
            <div id="tab-assignment" class="tab-pane fade">
                <%--<WebPartPages:WebPartZone runat="server" ID="WebPartZoneAssignments">
                    <ZoneTemplate>--%>
                        <%-- Связанные с вопросом повестки поручения, закладка "Поручения" --%>
                        <%--<WebPartPages:XsltListViewWebPart ID="WPAssignmentList" runat="server"
                            ListUrl="Lists/AssignmentList" PageType="PAGE_NORMALVIEW" Title="Поручения"
                            FrameType="None" OnInit="WPAssignmentList_Init">
                        </WebPartPages:XsltListViewWebPart>
                    </ZoneTemplate>
                </WebPartPages:WebPartZone>--%>
            </div>
            <div id="tab-attach" class="tab-pane fade">
                <%--<WebPartPages:WebPartZone runat="server" ID="WebPartZoneAttachs">
                    <ZoneTemplate>--%>
                        <%-- Вложения вопроса повестки, закладка "Материалы к заседанию" --%>
                        <%--<WebPartPages:XsltListViewWebPart ID="WPAttachList" runat="server"
                            ListUrl="Lists/AgendaAttachmentList" PageType="PAGE_NORMALVIEW" Title="Материалы к заседанию"
                            FrameType="None">
                        </WebPartPages:XsltListViewWebPart>
                    </ZoneTemplate>
                </WebPartPages:WebPartZone>--%>
            </div>
            <div id="tab-search" class="tab-pane fade">
               
            </div>
        </div>
    </div>
    <div class="container" id="buttons">
        <wssuc:ToolBar CssClass="ms-formtoolbar" id="toolBarTbl" RightButtonSeparator="&amp;#160;" runat="server">
            <template_buttons>
                <SharePoint:CreatedModifiedInfo ID="CreatedModifiedInfo" runat="server"/>
            </template_buttons>
            <template_rightbuttons>
		        <SharePoint:SaveButton ID="SaveButton" runat="server"/>
			    <SharePoint:GoBackButton ID="GoBackButton" runat="server"/>
		    </template_rightbuttons>
        </wssuc:ToolBar>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Вопрос повестки
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Вопрос повестки
</asp:Content>
