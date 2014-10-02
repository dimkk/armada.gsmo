using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using SAMRT.Common.BL;
using System;
using System.Security.Permissions;

namespace SAMRT.Receivers
{
    public class MeetingAkItem : SPItemEventReceiver
    {
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            EnsureAttachmentSubFolder(properties);
            CreateIssuesAkFromUnusedIssuesRg(properties);
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdding(SPItemEventProperties properties)
        {
            SetMeetingNumberAndTitle(properties);
        }

        private void CreateIssuesAkFromUnusedIssuesRg(SPItemEventProperties properties)
        {
            try
            {
                MeetingAk.CreateIssuesAkFromUnusedIssuesRg(properties.Web, properties.ListItemId, 25);
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "��� �������� �������� �� �� �������� �� � ��������� �� (ID = {0}) ��������� ����������� ����������", properties.ListItemId);
            }
        }

        private void EnsureAttachmentSubFolder(SPItemEventProperties properties)
        {
            try
            {
                MeetingAk.EnsureAttachmentSubFolder(properties.Web, properties.ListItem);
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "��� �������� �������� ��� �������� ��������� �� (ID = {0}) ��������� ����������� ����������", properties.ListItemId);
            }
        }

        private void SetMeetingNumberAndTitle(SPItemEventProperties properties)
        {
            try
            {
                int number = MeetingAk.GetNextMeetingNumber(properties.Web);
                DateTime date = DateTime.Parse(properties.AfterProperties["MeetingDate"].ToString());
                properties.AfterProperties["MeetingNumber"] = number;
                properties.AfterProperties["Title"] = string.Format("��������� ������������� �������� �{0} �� {1}", number, date.ToShortDateString());
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "��� ��������� ������ ��� �������� ��������� �� ��������� ����������� ����������");
            }
        }
    }
}

