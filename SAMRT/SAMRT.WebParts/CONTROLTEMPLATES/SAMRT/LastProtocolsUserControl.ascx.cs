using System;
using System.Linq;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SAMRT.ControlTemplates
{
    public partial class LastProtocolsUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string ProtocolsLoad()
        {
            string dataString = string.Empty;
            SPWeb web = SPContext.Current.Web.Site.RootWeb;
            
            SPList meetingList = web.Lists["АК - Заседания"];
            SPListItemCollection items = null;
            try
            {
                var query = new SPQuery
                    {
                        Query = string.Concat("<Where><Eq>",
                                "<FieldRef Name='MeetingStatus' /><Value Type='Choice'>Завершенное</Value>",
                            "</Eq></Where><OrderBy><FieldRef Name='MeetingDate' Ascending='FALSE' /></OrderBy>"
                            ),
                        ViewFields = string.Concat("<FieldRef Name='ID' />", "<FieldRef Name='MeetingDate' />", "<FieldRef Name='MeetingNumber' />"),
                        ViewFieldsOnly = true,
                        RowLimit = 5
                    };

                items = meetingList.GetItems(query);
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SAMRT.WebParts.MeetingCalendar", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            if (items != null)
            {
                dataString = items.Cast<SPListItem>().Aggregate(dataString, (current, item) =>
                                                current + string.Format("<tr align=\"center\"><td><a href=\"/sites/gca/_layouts/15/SAMRT.Web/Pages/Meeting.aspx?ID={0}\">№ {1} от {2}</a></td></tr>",
                                                    item["ID"],
                                                    item["MeetingNumber"],
                                                    Convert.ToDateTime(item["MeetingDate"].ToString()).ToString("d.MM.yyyy")));
            }

            return dataString;
        }
    }
}
