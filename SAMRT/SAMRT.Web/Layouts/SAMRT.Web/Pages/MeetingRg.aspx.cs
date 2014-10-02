using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SAMRT.Common;
using System;
using System.Linq;
using System.Web.Services;
using BL = SAMRT.Common.BL;

namespace SAMRT.Web
{
    public partial class MeetingRg : LayoutsPageBase
    {
        private bool _isInited;
        private bool _isQuestionCommentEnabled;

        protected bool IsQuestionCommentEnabled
        {
            get
            {
                if (!_isInited)
                {
                    try
                    {
                        _isQuestionCommentEnabled = SPContext.Current.Web.IsQuestionCommentAvailable();
                    }
                    catch (Exception e)
                    {
                        Log.Unexpected(e, "Непредвиденная ошибка при определении возможности добавления комментариев к вопросу");
                    }
                    _isInited = true;
                }
                return _isQuestionCommentEnabled;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static string AddOrdersRg(string meetingRgId, string[] orderRgIdList)
        {
            string returnMessage = string.Format("Веб-метод AddOrdersRg(meetingRgId = {0}, orderRgIdList = [{1}]): ", meetingRgId, string.Join(",", orderRgIdList));
            try
            {
                int meetingRgIdInt = Convert.ToInt32(meetingRgId);
                BL.MeetingRg.CreateIssuesRgFromOrdersRg(SPContext.Current.Web, meetingRgIdInt, orderRgIdList.Select(s => Convert.ToInt32(s)));
            }
            catch (Exception e)
            {
                return Log.Unexpected(e, returnMessage + "ошибка");
            }
            return returnMessage + "успешно выполнен";
        }
    }
}
