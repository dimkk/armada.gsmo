using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace GSWeb.WebParts.AgendaQuestionTitle
{
    [ToolboxItemAttribute(false)]
    public class AgendaQuestionTitle : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/GSWeb.WebParts/AgendaQuestionTitle/AgendaQuestionTitleUserControl.ascx";

        #region Common
        [Category("Common"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string Header { get; set; }
        #endregion

        #region Question
        [Category("Question"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string QuestionAddressFieldName { get; set; }
        [Category("Question"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string QuestionCadastreNumberFieldName { get; set; }
        [Category("Question"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string QuestionNumberFieldName { get; set; }
        [Category("Question"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string QuestionCategoryLinkFieldName { get; set; }
        [Category("Question"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string QuestionMeetingLinkFieldName { get; set; }
        #endregion

        #region Meeting
        [Category("Meeting"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string MeetingsListName { get; set; }
        [Category("Meeting"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string MeetingDateFieldName { get; set; }
        [Category("Meeting"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string MeetingNumberFieldName { get; set; }
        #endregion

        #region MeetingAttachment
        [Category("MeetingAttachment"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string MeetingAttachmentsListName { get; set; }
        [Category("MeetingAttachment"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string MeetingAttachmentMeetingLinkFieldName { get; set; }
        [Category("MeetingAttachment"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string MeetingAttachmentExistsFieldName { get; set; }
        #endregion

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            InitControl(control);
            Controls.Add(control);
        }

        protected void InitControl(Control control)
        {
            AgendaQuestionTitleUserControl uc = (AgendaQuestionTitleUserControl)control;
            
            uc.Header = Header;

            uc.QuestionAddressFieldName = QuestionAddressFieldName;
            uc.QuestionCadastreNumberFieldName = QuestionCadastreNumberFieldName;
            uc.QuestionCategoryLinkFieldName = QuestionCategoryLinkFieldName;
            uc.QuestionMeetingLinkFieldName = QuestionMeetingLinkFieldName;
            uc.QuestionNumberFieldName = QuestionNumberFieldName;

            uc.MeetingListName = MeetingsListName;
            uc.MeetingNumberFieldName = MeetingNumberFieldName;
            uc.MeetingDateFieldName = MeetingDateFieldName;
        
            uc.MeetingAttachmentsListName = MeetingAttachmentsListName;
            uc.MeetingAttachmentMeetingLinkFieldName = MeetingAttachmentMeetingLinkFieldName;
            uc.MeetingAttachmentExistsFieldName = MeetingAttachmentExistsFieldName;
        }
    }
}
