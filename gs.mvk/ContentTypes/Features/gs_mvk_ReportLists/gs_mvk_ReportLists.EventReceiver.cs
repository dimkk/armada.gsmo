using gs.mvk.BusinessLogic.Utils;
using Microsoft.SharePoint;
using System;
using System.Runtime.InteropServices;

namespace gs.mvk.ContentTypes.Features.gs_mvk_ReportLists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("d04a7f88-10cd-4b5a-81f4-312c59c08b90")]
    public class gs_mvk_ReportListsEventReceiver : SPFeatureReceiver
    {
        #region Constants

        private readonly string fieldContentTypeName        = "AssignmentMVK";
        private readonly string fieldGuid                   = "{0EC1DABE-3F49-4FBA-BA0C-6C11779C758B}";
        private readonly string fieldName                   = "AssignmentLastReportMVK";

        private readonly string fieldGroupNameRes           = "$Resources:MVKColumnsGroup";
        private readonly string fieldDisplayNameRes         = "$Resources:AssignmentLastReportMVKDispName";
        private readonly string fieldDescriptionRes         = "$Resources:AssignmentLastReportMVKDescr";

        private readonly string targetShowFieldName         = "ID";
        private readonly string targetLookupListRelativeUrl = "/Lists/ReportMVKList";

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
