<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Assembly Name="Microsoft.SharePoint.IdentityModel, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="Microsoft.SharePoint.WebControls" %>
<%@ Register Tagprefix="SharePoint" 
    Namespace="Microsoft.SharePoint.WebControls" 
    Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" 
    Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" 
    Inherits="gs.meeting.Components.Login" MasterPageFile="~/_layouts/simple.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link href="../Content/login-page.css?rev=23" rel="stylesheet" type="text/css"/>
    <script type="text/javascript" src="/Style Library/ResponsiveServer/Bootstrap3/js/jquery-1.9.1.min.js"></script>

    <script>
        $(document).ready(function () {
            $('#s4-simple-card').attr("id", "s4-simple-card2");
            $('#s4-simple-header').hide();
            $('#s4-simple-card-top').hide();
            $('#s4-simple-card-content').css("margin", "0px");
            $('.s4-simple-iconcont').hide();
            $('#s4-simple-content').css("margin", "0px");
            $('#s4-simple-content h1:first').hide();
            $('.s4-die').hide();

            $("[id$=signInControl] tr:first").append("<td></td>");
            var td1 = $("[id$=signInControl] tr:first td:first");
            var td2 = $("[id$=signInControl] tr:first td:last");

            $(td1).css("width", "49.95%");
            $(td2).css("width", "49.95%");

            $(td1).find("table tr:first td:first").attr("align", "left");
            $(td1).find("table").css("width", "100%").attr("cellpadding", 3);
            $(td1).find("input.ms-input").css("width", "100%");
            $(td1).css("padding-right", "25px");
            $("[id$=signInControl_LoginButton]").css("font-size", "x-large").closest("td").attr("align", "left");

            $(td2).css("vertical-align", "initial");
            $(td2).css("padding-top", "25px");
            $(td2).css("padding-left", "25px");
            $(td2).css("border-left", "silver");
            $(td2).css("border-left-width", "1px");
            $(td2).css("border-left-style", "solid");
            $(td2).css("position", "relative");
            $(td2).css("text-align", "left");
            $(td2).append($("[id=SignInInfo]"));

            $("[id$=signInControl]").css("margin-bottom", "25px");
        });
    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <header>
        <div class="header">
            <div class="logo">
                <img class="logo-img" src="../Content/logo.png?rev=23" />
            </div>
            <div class="title">
                <label>
                    АИС Градостроительного Совета
                </label>
            </div>
        </div>
        <div class="subheader"></div>
        <div id="SignInInfo" style="width:375px;font-size:medium; font-style:italic">
            <div style="position:absolute; top: 25px;">
                Для получения доступа к системе необходимо обратиться в службу поддержки по адресу gradsovet@armd.ru или по телефону +7(499)372-16-92
            </div>
            <div style="position:absolute; bottom: 25px;">
                <label>Полезные ссылки:</label>
                <br>
                <a href="/files/instruction-ipad.docx" target="_blank">Инструкция по настройке АИС ГС на iPad</a>
                <br>
                <a href="/files/requirements.docx" target="_blank">Требования для работы АИС ГС</a>
            </div>
        </div>
    </header>

    <div class="content">
        <center>
            <div id="content">
                <asp:Login ID="signInControl" style="width: 750px" FailureText="<%$Resources:wss,login_pageFailureText%>" 
                    MembershipProvider="FBAMembershipProvider" runat="server" DisplayRememberMe="true" 
                    TextBoxStyle-Width="250px" RememberMeSet="true" RememberMeText="Запомнить меня" 
                    LoginButtonStyle-CssClass="ms-buttonheightwidth" 
                    UserNameLabelText="Введите имя пользователя" TextLayout="TextOnTop" PasswordLabelText="Введите пароль" 
                    LabelStyle-Font-Bold="false" LabelStyle-Font-Size="Large" LabelStyle-ForeColor="Black" 
                    LabelStyle-Font-Names="Calibri" LabelStyle-CssClass="ms-standardheader ms-inputformheader" 
                    TextBoxStyle-CssClass="ms-input" CheckBoxStyle-Font-Bold="false" CheckBoxStyle-Font-Names="Calibri" 
                    CheckBoxStyle-ForeColor="Black" CheckBoxStyle-CssClass="ms-standardheader ms-inputformheader"
                    CheckBoxStyle-Font-Size="Large" FailureTextStyle-Wrap="true" FailureTextStyle-Font-Names="Calibri" 
                    FailureTextStyle-Font-Size="Small" LoginButtonStyle-Font-Names="Calibri" LoginButtonStyle-Font-Size="Large" 
                    TitleText="Войдите в систему" TitleTextStyle-ForeColor="Black" TitleTextStyle-Font-Bold="true" 
                    TitleTextStyle-Wrap="true" TitleTextStyle-Font-Names="Calibri" TitleTextStyle-Font-Size="X-Large" 
                    LoginButtonText="Вход" TextBoxStyle-Height="30px" TextBoxStyle-Font-Size="14pt" LoginButtonType="Button" />
                <asp:LinkButton ID="ButtonInternalUsers" Text="Вход с помощью доменной аутентификации" runat="server" 
                    Font-Names="Calibri" Font-Size="Large" CssClass="ms-standardheader ms-inputformheader" 
                    Font-Bold="true" ForeColor="#A8A8A8" />
                <asp:Label ID="lblError" runat="server" Font-Bold="true" ForeColor="Red" EnableViewState="false"></asp:Label>
            </div>
        </center>
    
        <SharePoint:EncodedLiteral runat="server" EncodeMethod="HtmlEncode" ID="ClaimsFormsPageMessage" Visible="false">
        </SharePoint:EncodedLiteral>
    </div>

    <footer>
        <div class="subheader"></div>
        <div class="header">
            <div class="footer-title">
                Автоматизированная информационная система Градостроительного Совета Московской области
            </div>
            <div class="logo">
                <img class="logo-footer-img" src="../Content/tver-logo.png?rev=23" />
            </div>
        </div>
    </footer>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:EncodedLiteral runat="server" EncodeMethod="HtmlEncode" ID="ClaimsFormsPageTitle" Visible="false">
    </SharePoint:EncodedLiteral>
    Вход в систему
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    <SharePoint:EncodedLiteral runat="server" EncodeMethod="HtmlEncode" ID="ClaimsFormsPageTitleInTitleArea" Visible="false">
    </SharePoint:EncodedLiteral>
    Вход в систему
</asp:Content>

<asp:Content ID="SiteName" ContentPlaceHolderID="PlaceHolderSiteName" runat="server">

</asp:Content>
