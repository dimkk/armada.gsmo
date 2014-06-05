using gs.mvk.BusinessLogic.Utils;
using Microsoft.SharePoint;
using System;
using System.Runtime.InteropServices;

namespace gs.mvk.ContentTypes.Features.gs_mvk_AssignmentLists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("1f0bb1f9-129c-4d48-bae6-75c11bfa103e")]
    public class gs_mvk_AssignmentListsEventReceiver : SPFeatureReceiver
    {
        #region Constants

        private readonly string fieldContentTypeName        = "AssignmentMVK";
        private readonly string fieldGuid                   = "{4D2A9A7F-4305-4CBD-996C-CBD6BF99A1D7}";
        private readonly string fieldName                   = "AssignmentDependentAssignment";

        private readonly string fieldGroupNameRes           = "$Resources:MVKColumnsGroup";
        private readonly string fieldDisplayNameRes         = "$Resources:AssignmentDependentAssignmentDispName";
        private readonly string fieldDescriptionRes         = "$Resources:AssignmentDependentAssignmentDescr";

        private readonly string targetShowFieldName         = "AsignmentNumberMVK";
        private readonly string targetLookupListRelativeUrl = "/Lists/AssignmentMVKList";

        #endregion

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
<<<<<<< HEAD
            using (SPSite site = (SPSite)properties.Feature.Parent)
=======
            SPSite site = (SPSite)properties.Feature.Parent;
            if (site == null)
                throw new Exception("Feature must be activated at site collection level");

            SPWeb web = site.RootWeb;
            if (web != null)
>>>>>>> 6734b84e4faa4a6e1cddc9e4e0ff25a57c05869f
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.AddLookupField(fieldContentTypeName, fieldGuid, fieldName, fieldGroupNameRes, fieldDisplayNameRes, fieldDescriptionRes, targetShowFieldName, targetLookupListRelativeUrl);
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
<<<<<<< HEAD
            using (SPSite site = (SPSite)properties.Feature.Parent)
=======
            SPSite site = (SPSite)properties.Feature.Parent;
            if (site == null)
                throw new Exception("Feature must be activated at site collection level");

            SPWeb web = site.RootWeb;
            if (web != null)
>>>>>>> 6734b84e4faa4a6e1cddc9e4e0ff25a57c05869f
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.RemoveSiteColumn(fieldDisplayNameRes);
            }
        }
    }
}
