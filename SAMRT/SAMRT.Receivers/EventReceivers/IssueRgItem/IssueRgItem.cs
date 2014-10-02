using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using SAMRT.Common.BL;
using System;
using System.Security.Permissions;

namespace SAMRT.Receivers
{
    public class IssueRgItem : SPItemEventReceiver
    {
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            CreateOrLinkIssueRgAttachment(properties);
        }

        private void CreateOrLinkIssueRgAttachment(SPItemEventProperties properties)
        {
            try
            {
                IssueRg.CreateOrLinkAttachment(properties.Web, properties.ListItem);
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "��� �������� ��� �������� ������ ���������� ��� ������� �� (ID = {0}) ��������� ����������� ����������", properties.ListItemId);
            }
        }
    }
}