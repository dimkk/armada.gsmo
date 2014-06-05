using gs.mvk.BusinessLogic.Utils;
using Microsoft.SharePoint;
using System;
using System.Runtime.InteropServices;

namespace gs.mvk.ContentTypes.Features.gs_mvk_OrderLists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("62809e42-2eb4-43cf-9d18-420e6b99abce")]
    public class gs_mvk_OrderListsEventReceiver : SPFeatureReceiver
    {
        #region Constants

        private readonly string fieldContentTypeName        = "IssueMVK";
        private readonly string fieldGuid                   = "{A507DB3A-FEDE-43DB-A276-FC636F612316}";
        private readonly string fieldName                   = "IssueOrderMVKLink";

        private readonly string fieldGroupNameRes           = "$Resources:MVKColumnsGroup";
        private readonly string fieldDisplayNameRes         = "$Resources:IssueOrderMVKLinkDisplayName";
        private readonly string fieldDescriptionRes         = "$Resources:IssueOrderMVKLinkDescr";
        
        private readonly string targetShowFieldName         = "Title";
        private readonly string targetLookupListRelativeUrl = "/Lists/OrderMVKList";

        #endregion
        
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            using (SPSite site = (SPSite)properties.Feature.Parent)
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.AddLookupField(fieldContentTypeName, fieldGuid, fieldName, fieldGroupNameRes, fieldDisplayNameRes, fieldDescriptionRes, targetShowFieldName, targetLookupListRelativeUrl);
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            using (SPSite site = (SPSite)properties.Feature.Parent)
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.RemoveSiteColumn(fieldDisplayNameRes);
            }
        }
    }
}
