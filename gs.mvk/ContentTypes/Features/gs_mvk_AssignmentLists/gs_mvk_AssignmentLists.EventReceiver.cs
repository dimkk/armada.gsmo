using System;
using System.Web;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using gs.mvk.BusinessLogic.Utils;

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

        private const string resFileRelative        = @"gs.mvk\AssignmentListsStrings";
        private const string resDispNameToken       = "$Resources:AssignmentDependentAssignmentDispName";
        private const string resDescriptionToken    = "$Resources:AssignmentDependentAssignmentDescr";
        private const string resColGroupToken       = "$Resources:MVKColumnsGroup";
        private const string resFmtListNotFound     = "$Resources:ListNotFoundFmtErr";
        private const string resFmtColumnAddFailed  = "$Resources:SiteColumnAddFmtErr";
        private const string resFmtCTNotFound       = "$Resources:ContentTypeNotFoundFmtErr";

        private const string AssignListRelativeUrl  = "/Lists/AssignmentMVKList";

        #endregion

        private SPList GetLookupList(SPWeb web)
        {
            SPList retVal = null;

            try
            {
                retVal = web.GetList(AssignListRelativeUrl);
            }
            catch (FileNotFoundException)
            {
                var msgFmt = SPUtility.GetLocalizedString(resFmtListNotFound, resFileRelative, web.Language);
                System.Diagnostics.Debug.WriteLine(String.Format(msgFmt, AssignListRelativeUrl, web.Title));
            }

            return retVal;
        }

        private SPField AddAssignmentLinkColumn(SPWeb web, SPList lookupList, string groupName, string displayName, string description)
        {
            string fieldXml = "<Field " +
                                        "Group=\"" + groupName + "\" " +
                                        "DisplayName=\"" + displayName + "\" " +
                                        "ID=\"{4D2A9A7F-4305-4CBD-996C-CBD6BF99A1D7}\" " +
                                        "SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" " +
                                        "StaticName=\"AssignmentDependentAssignment\" " +
                                        "Name=\"AssignmentDependentAssignment\" " +
                                        "Type=\"Lookup\" " +
                                        "List=\"{" + lookupList.ID + "}\" " +
                                        "ShowField=\"AsignmentNumberMVK\" " +
                                        "Mult=\"False\" " +
                                        "UnlimitedLengthInDocumentLibrary=\"False\" " +
                                        "Description=\"" + description + "\" " +
                                        "Overwrite=\"TRUE\" " +
                                        "AllowDeletion=\"FALSE\"></Field>";
            SPField retVal = Utility.EnsureSiteColumnXml(web, displayName, fieldXml);
            if (retVal == null)
            {
                var msgFmt = SPUtility.GetLocalizedString(resFmtColumnAddFailed, resFileRelative, web.Language);
                System.Diagnostics.Debug.WriteLine(String.Format(msgFmt, "AssignmentDependentAssignment"));
            }

            return retVal;
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite site = (SPSite)properties.Feature.Parent;
            if (site == null)
                throw new Exception("Feature must be activated at site collection level");

            foreach (SPWeb web in site.AllWebs)
            {
                var gName = SPUtility.GetLocalizedString(resColGroupToken, resFileRelative, web.Language);
                var cName = SPUtility.GetLocalizedString(resDispNameToken, resFileRelative, web.Language);
                var dName = SPUtility.GetLocalizedString(resDescriptionToken, resFileRelative, web.Language);

                SPList lookupList = GetLookupList(web);
                if (lookupList != null)
                {
                    SPField field = AddAssignmentLinkColumn(web, lookupList, gName, cName, dName);
                    if (field != null)
                    {
                        SPContentType ct = web.ContentTypes["AssignmentMVK"];
                        if (ct != null)
                        {
                            Utility.LinkFieldToContentType(web, ct, field);
                        }
                        else
                        {
                            var msgFmt = SPUtility.GetLocalizedString(resFmtCTNotFound, resFileRelative, web.Language);
                            System.Diagnostics.Debug.WriteLine(String.Format(msgFmt, "AssignmentMVK"));
                        }
                    }
                }
            }
        }


        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite site = (SPSite)properties.Feature.Parent;
            if (site == null)
                throw new Exception("Feature must be activated at site collection level");

            foreach (SPWeb web in site.AllWebs)
            {
                var cName = SPUtility.GetLocalizedString(resDispNameToken, resFileRelative, web.Language);
                Utility.RemoveSiteColumn(web, cName);
            }
        }
    }
}
