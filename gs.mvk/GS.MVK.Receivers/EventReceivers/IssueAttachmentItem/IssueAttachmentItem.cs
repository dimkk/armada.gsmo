using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System;
using System.Security.Permissions;

namespace GS.MVK.Receivers
{
    /// <summary>
    /// TODO: Add comment for IssueAttachmentItem
    /// </summary>
    public class IssueAttachmentItem : SPItemEventReceiver
    {
        private static bool CanPerformAction(SPItemEventProperties properties, out string errorMessage)
        {
            errorMessage = String.Empty;

            if (properties.Web.DoesUserHavePermissions(SPBasePermissions.FullMask)) return true;
            // default to file because of it doesn't influence on result
            var itemType = SPFileSystemObjectType.File;

            switch (properties.EventType)
            {
                case SPEventReceiverType.ItemAdding:
                case SPEventReceiverType.ItemUpdating:
                    {
                        itemType = properties.GetItemType();
                        break;
                    }
                case SPEventReceiverType.ItemDeleting:
                    {
                        itemType = properties.ListItem.FileSystemObjectType;
                        break;
                    }
            }

            if (itemType != SPFileSystemObjectType.Folder) return true;

            errorMessage = "Библиотека является служебной. Действия с папками и наборами документов запрещены";
            return false;
        }
        /// <summary>
        /// TODO: Add comment for event ItemAdding in erAttachDocumentSetLib 
        /// </summary>
        /// <param name="properties">Contains list event properties</param>   
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdding(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;

            try
            {
                string errorMessage;
                if (CanPerformAction(properties, out errorMessage)) return;

                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = errorMessage;
            }
            finally
            {
                EventFiringEnabled = true;
            }
        }

        /// <summary>
        /// TODO: Add comment for event ItemDeleting in erAttachDocumentSetLib 
        /// </summary>
        /// <param name="properties">Contains list event properties</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            string errorMessage;
            if (CanPerformAction(properties, out errorMessage)) return;

            properties.Status = SPEventReceiverStatus.CancelWithError;
            properties.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// TODO: Add comment for event ItemUpdating in erAttachDocumentSetLib 
        /// </summary>
        /// <param name="properties">Contains list event properties</param>   
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;

            try
            {
                string errorMessage;
                if (CanPerformAction(properties, out errorMessage)) return;

                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = errorMessage;
            }
            finally
            {
                EventFiringEnabled = true;
            }
        }
    }

    public static class SPItemEventPropertiesExtensions
    {
        public static SPFileSystemObjectType GetItemType(this SPItemEventProperties value)
        {
            var fileSizeObj = value.AfterProperties["vti_filesize"];
            // when adding via webdav file size always 0
            return fileSizeObj == null ? SPFileSystemObjectType.Folder : SPFileSystemObjectType.File;
        }
    }
}

