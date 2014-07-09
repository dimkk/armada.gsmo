using GS.MVK.Utils;
using Microsoft.SharePoint;
using System;
using System.Runtime.InteropServices;

namespace GS.MVK.ContentTypes.Features
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("86f66461-2c8c-40b6-bb1b-f7c5e21f16cf")]
    public class ReportListsEventReceiver : SPFeatureReceiver
    {
        #region Constants

        private readonly string fieldContentTypeName        = "AssignmentMVK";
        private readonly string fieldGuid                   = "{0EC1DABE-3F49-4FBA-BA0C-6C11779C758B}";
        private readonly string fieldName                   = "AssignmentLastReportMVK";

        private readonly string fieldParentFeatureId        = "10ced598-c9fa-4bfe-bf35-120844f63239";
        private readonly string fieldGroupNameRes           = "MVKColumnsGroup";
        private readonly string fieldDisplayNameRes         = "AssignmentLastReportMVKDisplayName";
        private readonly string fieldDescriptionRes         = "AssignmentLastReportMVKDescription";

        private readonly string targetShowFieldName         = "ID";
        private readonly string targetLookupListRelativeUrl = "/Lists/ReportMVKList";

        #endregion

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            using (SPSite site = (SPSite)properties.Feature.Parent)
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.AddLookupField(fieldParentFeatureId, fieldContentTypeName, fieldGuid, fieldName, fieldGroupNameRes, fieldDisplayNameRes, fieldDescriptionRes, targetShowFieldName, targetLookupListRelativeUrl);
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            using (SPSite site = (SPSite)properties.Feature.Parent)
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.DeleteField(fieldName);
            }
        }
    }
}
