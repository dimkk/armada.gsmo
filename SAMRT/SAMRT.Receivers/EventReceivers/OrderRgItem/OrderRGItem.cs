using System;
using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System.Security.Permissions;
using SAMRT.Common.BL;

namespace SAMRT.Receivers
{
    public class OrderRgItem : SPItemEventReceiver
    {
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            CreateAttachment(properties);
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            try
            {
                OrderRg.DeleteAttachment(properties.Web, properties.ListItem);
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "��� �������� ������ ���������� ��� ������ �� (ID = {0}) ��������� ����������� ����������", properties.ListItemId);
            }
        }

        private void CreateAttachment(SPItemEventProperties properties)
        {
            try
            {
                OrderRg.CreateAttachment(properties.Web, properties.ListItem);
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "��� �������� ������ ���������� ��� ������ �� (ID = {0}) ��������� ����������� ����������", properties.ListItemId);
            }
        }
    }
}

