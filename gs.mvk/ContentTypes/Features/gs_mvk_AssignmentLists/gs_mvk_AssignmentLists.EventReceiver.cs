using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

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
        // Uncomment the method below to handle the event raised after a feature has been activated.
        public static SPField EnsureSiteColumnXml(SPWeb web, string fieldName, string fieldXml)
        {
            SPField addField = null;
            SPField existField = null;

            if (web.Fields.ContainsField(fieldName))
                existField = web.Fields[fieldName];

            if (existField == null)
            {
                string fName = web.Fields.AddFieldAsXml(fieldXml, false, SPAddFieldOptions.AddFieldInternalNameHint);
                addField = web.Fields.GetFieldByInternalName(fName);
                addField.Update();
            }
            else
            {
                addField = existField;
            }

            return addField;
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite site = (SPSite)properties.Feature.Parent;
            if (site != null)
            {
                SPWeb web = site.RootWeb;
                if (web != null)
                {
                    
                    string fieldName = "AssignmentDependentAssignment";
                    string fieldXml = "<Field " +
                                            "Group=\"$Resources:MVKColumnsGroup\" " +
                                            "DisplayName=\"$Resources:AssignmentDependentAssignmentDispName\" " +
                                            "ID=\"{4D2A9A7F-4305-4CBD-996C-CBD6BF99A1D7}\" " +
                                            "SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" " +
                                            "StaticName=\"AssignmentDependentAssignment\" " +
                                            "Name=\"AssignmentDependentAssignment\" " +
                                            "Type=\"Lookup\" " +
                                            "List=\"Lists/AssignmentMVKList\" " +
                                            "ShowField=\"AsignmentNumberMVK\" " +
                                            "Mult=\"False\" " +
                                            "UnlimitedLengthInDocumentLibrary=\"False\" " +
                                            "Description=\"$Resources:AssignmentDependentAssignmentDescr\" " +
                                            "Overwrite=\"TRUE\" " +
                                            "AllowDeletion=\"FALSE\"></Field>";
                    SPField DepAssignmentField = EnsureSiteColumnXml(web, fieldName, fieldXml);
                    if (DepAssignmentField == null)
                        throw new Exception(String.Format("Unable to add site column {0}", fieldName));
                }
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


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
