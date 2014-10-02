using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using SAMRT.Common.BL;
using System;
using System.Security.Permissions;

namespace SAMRT.Receivers
{
    public class IssueAkItem : SPItemEventReceiver
    {
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            CreateOrLinkIssueAkAttachment(properties);
        }

        private void CreateOrLinkIssueAkAttachment(SPItemEventProperties properties)
        {
            try
            {
                IssueAk.CreateOrLinkAttachment(properties.Web, properties.ListItem);
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "При создании или привязке набора документов для вопроса АК (ID = {0}) произошло неожиданное исключение", properties.ListItemId);
            }
        }
    }
}

