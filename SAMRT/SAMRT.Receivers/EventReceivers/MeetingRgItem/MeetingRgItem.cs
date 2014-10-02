using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using SAMRT.Common;
using SAMRT.Common.BL;
using System;
using System.Security.Permissions;

namespace SAMRT.Receivers
{
    public class MeetingRgItem : SPItemEventReceiver
    {
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            EnsureAttachmentSubFolder(properties);
            CreateIssuesRgFromUnusedOrdersRg(properties);
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdding(SPItemEventProperties properties)
        {
            SetMeetingNumberAndTitle(properties);
        }

        private void CreateIssuesRgFromUnusedOrdersRg(SPItemEventProperties properties)
        {
            try
            {
                MeetingRg.CreateIssuesRgFromUnusedOrdersRg(properties.Web, properties.ListItemId);
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "При создании вопросов РГ из заявок РГ в заседании РГ (ID = {0}) произошло неожиданное исключение", properties.ListItemId);
            }
        }

        private void EnsureAttachmentSubFolder(SPItemEventProperties properties)
        {
            try
            {
                MeetingRg.EnsureAttachmentSubFolder(properties.Web, properties.ListItem);
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "При создании подпапки для вложений заседания РГ (ID = {0}) произошло неожиданное исключение", properties.ListItemId);
            }
        }

        private void SetMeetingNumberAndTitle(SPItemEventProperties properties)
        {
            try
            {
                int number = MeetingRg.GetNextMeetingNumber(properties.Web);
                DateTime date = DateTime.Parse(properties.AfterProperties["MeetingDateMVK"].ToString());
                properties.AfterProperties["MeetingNumberMVK"] = number;
                properties.AfterProperties["Title"] = string.Format("Заседание Рабочей группы №{0} от {1}", number, date.ToShortDateString());
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "При установке номера или названия заседания РГ произошло неожиданное исключение");
            }
        }
    }
}

