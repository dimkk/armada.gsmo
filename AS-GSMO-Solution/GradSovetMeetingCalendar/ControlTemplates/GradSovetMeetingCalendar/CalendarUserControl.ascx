<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarUserControl.ascx.cs" Inherits="GradSovetMeetingCalendar.ControlTemplates.GradSovetMeetingCalendar.CalendarUserControl" %>
<style type="text/css">
    .calendarStyle {
        width: 100%;
    }
</style>

<table style="width: 100%;">
    <tr>
        <td>
            <asp:Calendar CssClass="calendarStyle" ID="meetingCalendar" SelectionMode="None" FirstDayOfWeek="Monday" runat="server" BackColor="White" BorderColor="#F0F0F0" BorderWidth="2px" Font-Names="Verdana" Font-Size="9pt" ForeColor="Black" Height="470px" Width="100%" NextPrevFormat="FullMonth" OnDayRender="meetingCalendar_DayRender" OnVisibleMonthChanged="MonthButton_Click">
                <DayHeaderStyle Height="1px" HorizontalAlign="Left" Font-Bold="True" Font-Size="8pt" />
                <NextPrevStyle Font-Bold="True" Font-Size="10pt" ForeColor="#333333" VerticalAlign="Bottom"  />
                <OtherMonthDayStyle ForeColor="#999999" />
                <SelectedDayStyle CssClass="selectedDay" HorizontalAlign="Center" VerticalAlign="Middle"  BackColor="#F5F5FF" ForeColor="#0071C6" Font-Size="20pt" />
                <TitleStyle Height="1px" BackColor="White" Font-Bold="True" Font-Size="12pt" />
                <TodayDayStyle BackColor="#CCCCCC" />
                <DayStyle HorizontalAlign="Left" VerticalAlign="Top" BorderColor="#F0F0F0" BorderWidth="2px" Height="64px" Width="14%"  />
            </asp:Calendar>
        </td>
    </tr>
</table>