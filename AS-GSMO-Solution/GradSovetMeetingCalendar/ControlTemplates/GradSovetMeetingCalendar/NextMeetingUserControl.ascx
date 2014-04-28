<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NextMeetingUserControl.ascx.cs" Inherits="GradSovetMeetingCalendar.ControlTemplates.GradSovetMeetingCalendar.NextMeetingUserControl" %>

<style type="text/css">
    #NextMeetingDiv {
        background: white; /* обязательно для ie */
        -webkit-box-shadow: 5px 5px 5px #444;
        -moz-box-shadow: 5px 5px 5px #444;
        box-shadow: 5px 5px 5px #444;
        vertical-align: top;
    }    
    .mainTable {
        width: 230px; 
        border-color: #F3F3F3;
        text-align:center;
    }
    .content-table {
        width: 100%;
    }
    .td-content {
        height: 150px;
    }
    .td-head {
        background-color: #235D8D;
        border-color: #235D8D;
        height: 30px;
    }
</style>

<!--[if IE]>
<style type="text/css">
    #NextMeetingDiv {
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

<div id="NextMeetingDiv">
    <table class="mainTable">
        <tr>
            <td class="td-head">
                <font color="white">Объявления</font>
            </td>
        </tr>
        <tr>
            <td class="td-content">
                
                <table class="content-table">
                    <tbody>
                        
                        <% = MeetingLoad() %>
                    </tbody>
                </table>
            </td>
        </tr>
    </table>
</div>