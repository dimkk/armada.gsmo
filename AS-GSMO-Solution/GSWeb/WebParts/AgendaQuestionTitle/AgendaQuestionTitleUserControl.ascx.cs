using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Text;
using System.Web.UI;

namespace GSWeb.WebParts.AgendaQuestionTitle
{
    public partial class AgendaQuestionTitleUserControl : UserControl
    {
        #region Common
        public string Header { get; set; }
        #endregion

        #region Question
        public string QuestionAddressFieldName { get; set; }
        public string QuestionCadastreNumberFieldName { get; set; }
        public string QuestionNumberFieldName { get; set; }
        public string QuestionCategoryLinkFieldName { get; set; }
        public string QuestionMeetingLinkFieldName { get; set; }
        #endregion

        #region Meeting
        public string MeetingListName { get; set; }
        public string MeetingDateFieldName { get; set; }
        public string MeetingNumberFieldName { get; set; }
        #endregion

        #region MeetingAttachment
        public string MeetingAttachmentsListName { get; set; }
        public string MeetingAttachmentMeetingLinkFieldName { get; set; }
        public string MeetingAttachmentExistsFieldName { get; set; }
        #endregion

        #region Errors
        private StringBuilder _errors = new StringBuilder();
        public bool IsErrors = false;
        public string Errors { get { return _errors.ToString().Replace(Environment.NewLine, "<br/>"); } }
        protected void AddErrorDescription(Exception ex)
        {
            IsErrors = true;
            _errors.AppendLine(ex.ToString());
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private string _address;
        protected string Address
        {
            get
            {
                if (_address == null)
                    _address = ErrorHandler(() =>
                        {
                            return GetFieldValue(QuestionAddressFieldName);
                        });
                return _address;
            }
        }

        private string _cadastreNumber;
        protected string CadastreNumber
        {
            get
            {
                if (_cadastreNumber == null)
                    _cadastreNumber = ErrorHandler(() =>
                        {
                            return GetFieldValue(QuestionCadastreNumberFieldName);
                        });
                return _cadastreNumber;
            }
        }

        private string _category;
        protected string Category
        {
            get
            {
                if (_category == null)
                    _category = ErrorHandler(() =>
                        {
                            return new SPFieldLookupValue(GetFieldValue(QuestionCategoryLinkFieldName)).LookupValue;
                        });
                return _category;
            }
        }

        private string _meetingId;
        protected string MeetingId
        {
            get
            {
                if (_meetingId == null)
                    _meetingId = ErrorHandler(() =>
                        {
                            return new SPFieldLookupValue(GetFieldValue(QuestionMeetingLinkFieldName)).LookupId.ToString();
                        });
                return _meetingId;
            }
        }

        private string _protocolUrl;
        protected string ProtocolUrl
        {
            get
            {
                if (_protocolUrl == null)
                    _protocolUrl = ErrorHandler(() =>
                        {
                            var query = new SPQuery();
                            query.Query = string.Format("<Where><And><Eq><FieldRef Name='{0}' LookupId='True' /><Value Type='Integer'>{1}</Value></Eq><Eq><FieldRef Name='{2}' /><Value Type='Integer'>1</Value></Eq></And></Where>",
                                MeetingAttachmentMeetingLinkFieldName, MeetingId, MeetingAttachmentExistsFieldName);

                            SPList attachments = SPContext.Current.Web.GetListByUrl(MeetingAttachmentsListName);
                            SPListItemCollection items = attachments.GetItems(query);

                            return items.Count > 0 && items[0].Attachments.Count > 0
                                ? SPUrlUtility.CombineUrl(items[0].Attachments.UrlPrefix, items[0].Attachments[0])
                                : string.Empty;
                        });
                return _protocolUrl;
            }
        }

        private bool _isMeetingListLoaded;
        private SPList _meetingList;
        protected SPList MeetingList
        {
            get
            {
                if (!_isMeetingListLoaded)
                {
                    _isMeetingListLoaded = true;
                    _meetingList = SPContext.Current.Web.GetListByUrl(MeetingListName);
                }
                return _meetingList;
            }
        }

        private string _meetingUrl;
        protected string MeetingUrl
        {
            get
            {
                if (_meetingUrl == null)
                    _meetingUrl = ErrorHandler(() =>
                        {
                            return MeetingList.DefaultDisplayFormUrl.AddUrlParam("ID", MeetingId.ToString());
                        });
                return _meetingUrl;
            }
        }

        private string _meetingDescription;
        protected string MeetingDescription
        {
            get
            {
                if (_meetingDescription == null)
                    _meetingDescription = ErrorHandler(() =>
                        {
                            var query = new SPQuery();
                            query.Query = "<Where><Eq><FieldRef Name='ID'/><Value Type='Integer'>" + MeetingId + "</Value></Eq></Where>";

                            SPListItemCollection meeting = MeetingList.GetItems(query);

                            return meeting.Count > 0
                                ? string.Format("Заседание {0} №{1}, п.№{2}", Convert.ToDateTime(meeting[0][MeetingDateFieldName]).ToShortDateString(), meeting[0][MeetingNumberFieldName], GetFieldValue(QuestionNumberFieldName))
                                : "";
                        });
                return _meetingDescription;
            }
        }

        protected string ErrorHandler(Func<string> worker)
        {
            return worker.ErrorHandler(AddErrorDescription) ?? string.Empty;
        }

        protected string GetFieldValue(string fieldName)
        {
            return SPContext.Current.GetFieldValue(fieldName) ?? string.Empty;
        }
    }
}
