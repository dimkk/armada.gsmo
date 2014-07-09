using GS.MVK.Utils;
using Microsoft.SharePoint;
using System;
using System.Runtime.InteropServices;

namespace GS.MVK.ContentTypes.Features.GS.MVK.MeetingLists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("da0601d1-0631-4d07-860b-f1a6d3a7dc2b")]
    public class MeetingListsEventReceiver : SPFeatureReceiver
    {
        #region Constants
        private readonly string fieldContentTypeName    = "MeetingMVK";
        private readonly string fieldName               = "MeetingDateNumberMVK";
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
