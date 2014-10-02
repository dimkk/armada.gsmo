using ITB.SP.Tools;
using Microsoft.SharePoint;

namespace SAMRT.Common
{
    public static class Config
    {
        const string ConfigListName = "ConfigurationList";

        private static SPListItem GetConfigItem(this SPWeb web)
        {
            SPList configList = web.GetListByUrl(ConfigListName);
            return configList.GetItemById(1);
        }

        public static bool IsQuestionCommentAvailable(this SPWeb web)
        {
            return web.IsCurrentUserMemberOfGroup(web.GetConfigItem().GetFieldLookup("QuestionCommentGroup").LookupId);
        }

        public static int? GetNextMeetingNumberRg(this SPWeb web)
        {
            return web.GetConfigItem().GetFieldValueInt("CurrentMeetingNumberRg");
        }

        public static void SetNextMeetingNumberRg(this SPWeb web, int number)
        {
            SPListItem config = web.GetConfigItem();
            config["CurrentMeetingNumberRg"] = number;
            web.AllowUnsafeUpdates = true;
            config.Update();
            web.AllowUnsafeUpdates = false;
        }

        public static int? GetNextMeetingNumberAk(this SPWeb web)
        {
            return web.GetConfigItem().GetFieldValueInt("CurrentMeetingNumber");
        }

        public static void SetNextMeetingNumberAk(this SPWeb web, int number)
        {
            SPListItem config = web.GetConfigItem();
            config["CurrentMeetingNumber"] = number;
            web.AllowUnsafeUpdates = true;
            config.Update();
            web.AllowUnsafeUpdates = false;
        }
    }
}
