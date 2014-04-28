using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace GradSovetMeetingCalendar.ControlTemplates.GradSovetMeetingCalendar
{
    public partial class CalendarUserControl : UserControl
    {
        string[,] meetings = new String[13, 32];

        protected void Page_Load(object sender, EventArgs e)
        {
            var buttonModel = new Calendar();
            this.MonthButton_Click(buttonModel, new MonthChangedEventArgs(DateTime.Today, DateTime.Today));
        }

        protected void MonthButton_Click(object sender, MonthChangedEventArgs e)
        {
            DateTime newMonth = e.NewDate;
            //meetingCalendar.TodaysDate = newMonth;
            try
            {
                SPWeb web = SPContext.Current.Web.Site.RootWeb;

                var query = new SPQuery
                    {
                        Query = string.Format(string.Concat(
                            "<Where><And>",
                            "<Geq><FieldRef Name='MeetingDate' /><Value Type='DateTime'>{0}-21T12:00:00Z</Value></Geq>",
                            "<Leq><FieldRef Name='MeetingDate' /><Value Type='DateTime'>{1}-11T12:00:00Z</Value></Leq>",
                            "</And></Where>"), newMonth.AddMonths(-1).ToString("yyyy-MM"), newMonth.AddMonths(1).ToString("yyyy-MM")),
                        ViewFields = string.Concat("<FieldRef Name='ID' />", "<FieldRef Name='MeetingDate' />"),
                        ViewFieldsOnly = true
                    };
                foreach (SPListItem item in web.Lists["Заседания"].GetItems(query))
                {
                    DateTime theDate = Convert.ToDateTime(item["MeetingDate"].ToString());
                    meetingCalendar.SelectedDates.Add(theDate);
                    meetings[theDate.Month, theDate.Day] = item["ID"].ToString();
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("GrdSov Meeting Calendar", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void meetingCalendar_DayRender(object sender, DayRenderEventArgs e)
        {
            string meetingNumber;
            DateTime theDate = e.Day.Date;
            meetingNumber = meetings[theDate.Month, theDate.Day];
            if (meetingNumber != null)
            {
                e.Cell.Text = string.Format("<a href=\"/_layouts/15/gradsovetpages/Pages/Meeting.aspx?ID={0}\">{1}<br /><font size=\"4\">{2}</font></a>", meetingNumber, theDate.Day, theDate.ToString("MMMM"));
                e.Day.IsSelectable = true;
            }
        }
    }
}
