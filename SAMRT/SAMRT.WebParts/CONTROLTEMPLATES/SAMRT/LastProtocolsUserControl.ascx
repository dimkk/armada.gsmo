<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LastProtocolsUserControl.ascx.cs" Inherits="SAMRT.ControlTemplates.LastProtocolsUserControl" %>

<style type="text/css">
    #lastProtocolsDiv {
        background: white; /* обязательно для ie */
        -webkit-box-shadow: 5px 5px 5px #444;
        -moz-box-shadow: 5px 5px 5px #444;
        box-shadow: 5px 5px 5px #444;
        vertical-align:top;
    }
    #lastProtocolsDiv table {
        width: 230px;
        border: solid;
        border-color: #F3F3F3;
        align-content:center;
    }
    #lastProtocolsDiv tr {
        text-align: center;
        height: 30px;
    }
</style>

<!--[if IE]>
<style type="text/css">
    #lastProtocolsDiv {
        filter:
        progid:DXImageTransform.Microsoft.Shadow(color='#042b47', Direction=135, Strength=6);
        position: relative;
        top: -5px;
        left: -5px;
        zoom: 1;
        vertical-align: top;
    }
</style>
<![endif]-->

<div id="lastProtocolsDiv">
    <table>
        <tr>
            <td style="background-color: #EFEFEF;">
                <label>Последние протоколы</label>
            </td>
        </tr>
        <%=ProtocolsLoad() %>
        <tr>
            <td>
                <a href="/Lists/MeetingList/finished.aspx">Все протоколы</a>
            </td>
        </tr>
    </table>
</div>