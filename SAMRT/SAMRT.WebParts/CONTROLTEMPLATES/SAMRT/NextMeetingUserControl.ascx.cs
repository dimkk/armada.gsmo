using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;

namespace SAMRT.ControlTemplates
{
    public partial class NextMeetingUserControl : UserControl
    {
        public string Title { get; set; }

        public string MeetingsListName { get; set; }

        public string MeetingStatusFieldName { get; set; }

        public string MeetingDateFieldName { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string MeetingLoad()
        {
            StringBuilder dataString = new StringBuilder();
            
            SPWeb web = SPContext.Current.Web.Site.RootWeb;
            try
            {
                SPList meetingList = web.Lists[MeetingsListName];
                var query = new SPQuery
                    {
                        Query = string.Format("<Where><Neq><FieldRef Name='{0}' /><Value Type='Choice'>Завершенное</Value></Neq></Where><OrderBy><FieldRef Name='{1}' /></OrderBy>", MeetingStatusFieldName, MeetingDateFieldName),
                        ViewFields = string.Format("<FieldRef Name='ID' /><FieldRef Name='{0}' />", MeetingDateFieldName),
                        ViewFieldsOnly = true,
                        RowLimit = 1
                    };
                SPListItemCollection items = meetingList.GetItems(query);
                if (items != null && items.Count > 0)
                {
                    dataString.Append("<tr><td><font size=\"4\">Следующее заседание</font></td></tr><tr><td><font size=\"5\">");

                    DateTime meetingDateAndTime = Convert.ToDateTime(items[0][MeetingDateFieldName]);
                    string meetingUrl = AddUrlParam(meetingList.DefaultDisplayFormUrl, "ID", items[0]["ID"].ToString());
                    dataString.AppendFormat(
                            "{0}</font></td></tr><tr><td><font size=\"2\">начало в</font></td></tr><tr><td><font size=\"6\">{1}</font></td></tr><tr><td><font size=\"2\"><a href=\"{2}\">Перейти к повестке</a></font></td></tr>",
                            meetingDateAndTime.ToString("d MMMM yyyy"), meetingDateAndTime.ToString("HH:mm"), meetingUrl);
                }
                else
                {
                    dataString.Append("<tr><td>Дата проведения следующего Заседания не назначена</td></tr>");
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SAMRT.WebParts.MeetingCalendar", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return dataString.ToString();
        }

        private string AddUrlParam(string url, string name, string value)
        {
            string[] parts = url.Split('?');

            string root = parts[0];
            NameValueCollection query = HttpUtility.ParseQueryString(parts.Length > 1 ? parts[1] : string.Empty);
            query.Add(name, value);

            return root + "?" + query.ToString();
        }
    }
}
