using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using gs.mvk.BusinessLogic.Utils;

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

        private const string resFileRelative        = @"gs.mvk\AssignmentListsStrings";
        private const string resDispNameToken       = "$Resources:AssignmentDependentAssignmentDispName";
        private const string resReportDispNameToken = "$Resources:AssignmentLastReportMVKDispName";
        private const string resDescriptionToken    = "$Resources:AssignmentDependentAssignmentDescr";
        private const string resReportDescrToken    = "$Resources:AssignmentLastReportMVKDescr";
        private const string resColGroupToken       = "$Resources:MVKColumnsGroup";
        private const string resFmtListNotFound     = "$Resources:ListNotFoundFmtErr";
        private const string resFmtColumnAddFailed  = "$Resources:SiteColumnAddFmtErr";
        private const string resFmtCTNotFound       = "$Resources:ContentTypeNotFoundFmtErr";

        private const string ReportListRelativeUrl = "/Lists/ReportMVKList";

        #endregion

        private SPList GetLookupList(SPWeb web)
        {
            SPList retVal = null;

            try
            {
                retVal = web.GetList(ReportListRelativeUrl);
            }
            catch (FileNotFoundException)
            {
                var msgFmt = SPUtility.GetLocalizedString(resFmtListNotFound, resFileRelative, web.Language);
                System.Diagnostics.Debug.WriteLine(String.Format(msgFmt, ReportListRelativeUrl, web.Title));
            }

            return retVal;
        }

        private SPField AddReportLinkColumn(SPWeb web, SPList lookupList, string groupName, string displayName, string description)
        {
            string fieldXml = "<Field " +
                                        "Group=\"" + groupName + "\" " +
                                        "DisplayName=\"" + displayName + "\" " +
                                        "ID=\"{0EC1DABE-3F49-4FBA-BA0C-6C11779C758B}\" " +
                                        "SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" " +
                                        "StaticName=\"AssignmentLastReportMVK\" " +
                                        "Name=\"AssignmentLastReportMVK\" " +
                                        "Type=\"Lookup\" " +
                                        "List=\"{" + lookupList.ID + "}\" " +
                                        "ShowField=\"ID\" " +
                                        "Mult=\"False\" " +
                                        "UnlimitedLengthInDocumentLibrary=\"False\" " +
                                        "Description=\"" + description + "\" " +
                                        "Overwrite=\"TRUE\" " +
                                        "AllowDeletion=\"FALSE\"></Field>";
            SPField retVal = Utility.EnsureSiteColumnXml(web, displayName, fieldXml);
            if (retVal == null)
            {
                var msgFmt = SPUtility.GetLocalizedString(resFmtColumnAddFailed, resFileRelative, web.Language);
                System.Diagnostics.Debug.WriteLine(String.Format(msgFmt, "AssignmentLastReportMVK"));
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
                var cName = SPUtility.GetLocalizedString(resReportDispNameToken, resFileRelative, web.Language);
                var dName = SPUtility.GetLocalizedString(resReportDescrToken, resFileRelative, web.Language);

                SPList lookupList = GetLookupList(web);
                if (lookupList != null)
                {
                    SPField field = AddReportLinkColumn(web, lookupList, gName, cName, dName);
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

                web.Dispose();
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite site = (SPSite)properties.Feature.Parent;
            if (site == null)
                throw new Exception("Feature must be activated at site collection level");

            foreach (SPWeb web in site.AllWebs)
            {
                var cName = SPUtility.GetLocalizedString(resReportDispNameToken, resFileRelative, web.Language);
                Utility.RemoveSiteColumn(web, cName);
                web.Dispose();
            }
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
