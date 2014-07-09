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

    [Guid("1f0bb1f9-129c-4d48-bae6-75c11bfa103e")]
    public class AssignmentListsEventReceiver : SPFeatureReceiver
    {
        #region Constants

        private readonly string fieldContentTypeName        = "AssignmentMVK";
        private readonly string fieldGuid                   = "{4D2A9A7F-4305-4CBD-996C-CBD6BF99A1D7}";
        private readonly string fieldName                   = "AssignmentDependentAssignmentMVK";

        private readonly string fieldParentFeatureId        = "10ced598-c9fa-4bfe-bf35-120844f63239";
        private readonly string fieldGroupNameRes           = "MVKColumnsGroup";
        private readonly string fieldDisplayNameRes         = "AssignmentDependentAssignmentMVKDisplayName";
        private readonly string fieldDescriptionRes         = "AssignmentDependentAssignmentMVKDescription";

        private readonly string targetShowFieldName         = "AssignmentNumberMVK";
        private readonly string targetLookupListRelativeUrl = "/Lists/AssignmentMVKList";
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
