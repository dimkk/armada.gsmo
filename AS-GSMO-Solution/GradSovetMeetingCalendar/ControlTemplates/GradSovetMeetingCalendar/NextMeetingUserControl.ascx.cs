using System;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace GradSovetMeetingCalendar.ControlTemplates.GradSovetMeetingCalendar
{
    public partial class NextMeetingUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected string MeetingLoad()
        {
            string dataString = "<tr><td><font size=\"4\">Следующее заседание</font></td></tr><tr><td><font size=\"5\">";
            SPWeb web = SPContext.Current.Web.Site.RootWeb;

            try
            {
                var meetingList = web.Lists["Заседания"];
                var query = new SPQuery
                    {
                        Query = string.Concat(
                            "<Where><Eq>",
                                "<FieldRef Name='MeetingStatus' /><Value Type='Choice'>Планируемое</Value>",
                            "</Eq></Where><OrderBy><FieldRef Name='MeetingDate' /></OrderBy>"
                            ),
                        ViewFields = string.Concat("<FieldRef Name='ID' />", "<FieldRef Name='MeetingDate' />"),
                        ViewFieldsOnly = true,
                        RowLimit = 1
                    };
                var items = meetingList.GetItems(query);
                if (items != null && items.Count > 0)
                {
                    var meetingDateAndTime = Convert.ToDateTime(items[0]["MeetingDate"].ToString());
                    dataString += string.Format(
                            "{0}</font></td></tr><tr><td><font size=\"2\">начало в</font></td></tr><tr><td><font size=\"6\">{1}</font></td></tr><tr><td><font size=\"2\"><a href=\"/_layouts/15/gradsovetpages/Pages/Meeting.aspx?ID={2}\">Перейти к повестке</a></font></td></tr>",
                            meetingDateAndTime.ToString("d MMMM yyyy"), meetingDateAndTime.ToString("HH:mm"), items[0]["ID"]);
                }
                else
                {
                    dataString = "<tr><td>Дата проведения следующего Заседания не назначена</td></tr>";
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("GrdSov Meeting Calendar", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return dataString;
        }
    }
}
