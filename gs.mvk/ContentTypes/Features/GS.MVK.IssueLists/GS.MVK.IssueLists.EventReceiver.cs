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

    [Guid("d2a14231-0902-426e-82b1-86d462646639")]
    public class IssueListsEventReceiver : SPFeatureReceiver
    {
        #region Constants
        private readonly string fieldContentTypeName    = "IssueMVK";
        private readonly string fieldName               = "IssueNumberTextMVK";
        #endregion

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //Добавлять вычисляемое поле в тип содержимого можно только после создания экземляра списка
            using (SPSite site = (SPSite)properties.Feature.Parent)
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.AddExistField(fieldContentTypeName, fieldName);
            }
        }
    }
}
