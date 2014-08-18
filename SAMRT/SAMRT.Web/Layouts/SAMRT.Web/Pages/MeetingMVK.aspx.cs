using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SAMRT.Web
{
    public partial class MeetingMvk : LayoutsPageBase
    {
        private bool isInited;
        private bool isQuestionCommentEnabled;

        protected bool IsQuestionCommentEnabled
        {
            get
            {
                if (!isInited)
                {
                    SPList configList = SPContext.Current.Web.GetList("Lists/ConfigurationList");
                    SPListItem config = configList.GetItemById(1);
                    isQuestionCommentEnabled = SPContext.Current.Web.IsCurrentUserMemberOfGroup(new SPFieldLookupValue(config["QuestionCommentGroup"].ToString()).LookupId);
                    isInited = true;
                }
                return isQuestionCommentEnabled;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
