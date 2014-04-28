using System;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace GSWeb.WebParts.AgendaQuestionTitle
{
    public partial class AgendaQuestionTitleUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            protocolLink.NavigateUrl = ProtocolUrl;
            if (string.IsNullOrEmpty(protocolLink.NavigateUrl))
                protocolLink.Visible = false;

            if (SPContext.Current != null && SPContext.Current.Item != null)
            {
                labelMeetingAddress.Visible = !string.IsNullOrEmpty(Convert.ToString(SPContext.Current.Item["AgendaQuestionAddress"]));
                labelCadastreNumber.Visible = !string.IsNullOrEmpty(Convert.ToString(SPContext.Current.Item["CadastreNumber"]));
            }
        }

        protected string Address
        {
            get { return Convert.ToString(SPContext.Current.Item["AgendaQuestionAddress"]); }
        }

        protected string CadastreNumber
        {
            get { return Convert.ToString(SPContext.Current.Item["CadastreNumber"]); }
        }

        protected string Category
        {
            get { return new SPFieldLookupValue(Convert.ToString(SPContext.Current.Item["QuestionCategoryLink"])).LookupValue; }
        }
        
        protected int MeetingId
        {
            get { return new SPFieldLookupValue(Convert.ToString(SPContext.Current.Item["MeetingLink"])).LookupId; }
        }

        protected string ProtocolUrl
        {
            get
            {
                var query = new SPQuery();
                query.Query = "<Where><And><Eq><FieldRef Name='MeetingLink' LookupId='True' /><Value Type='Integer'>" + MeetingId + "</Value></Eq><Eq><FieldRef Name='AttachmentProtocolScanCopy' /><Value Type='Integer'>1</Value></Eq></And></Where>";

                var attachments = SPContext.Current.Web.Lists["Вложения заседаний"];
                var items = attachments.GetItems(query);

                return items.Count > 0 && items[0].Attachments.Count > 0
                    ? SPUrlUtility.CombineUrl(items[0].Attachments.UrlPrefix, items[0].Attachments[0])
                    : "";
            }
        }

        protected string MeetingDescription
        {
            get
            {
                var query = new SPQuery();
                query.Query = "<Where><Eq><FieldRef Name='ID'/><Value Type='Integer'>" + MeetingId + "</Value></Eq></Where>";

                var meetings = SPContext.Current.Web.Lists["Заседания"];
                var meeting = meetings.GetItems(query);

                return meeting.Count > 0
                    ? string.Format("Заседание {0} №{1}, п.№{2}", Convert.ToDateTime(meeting[0]["MeetingDate"]).ToShortDateString(), meeting[0]["MeetingNumber"], SPContext.Current.Item["AgendaQuestionNumber"])
                    : "";
            }
        }
    }
}
