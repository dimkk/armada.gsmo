using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

namespace GSWeb.Features.GSWeb_EReceivers
{
    /// <summary>
    /// Этот класс обрабатывает события, возникающие в ходе активации, деактивации, установки, удаления и обновления компонентов.
    /// </summary>
    /// <remarks>
    /// GUID, присоединенный к этому классу, может использоваться при создании пакета и не должен изменяться.
    /// </remarks>

    [Guid("a054d84c-e089-470c-948c-11e632bbf47b")]
    public class GSWeb_EReceiversEventReceiver : SPFeatureReceiver
    {
        private SPContentType getContentType(SPContentTypeCollection contentTypeCollection, string ID)
        {
            SPContentType publContentType = null;
            foreach (SPContentType contentType in contentTypeCollection)
            {
                if (string.Equals(contentType.Id.ToString(), ID, StringComparison.InvariantCultureIgnoreCase))
                {
                    publContentType = contentType;
                    break;
                }
            }
            return publContentType;
        }

        // Раскомментируйте ниже метод для обработки события, возникающего после активации компонента.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWeb myweb = properties.Feature.Parent as SPWeb;
            try
            {
                SPContentTypeCollection contentTypeCollection = myweb.ContentTypes;

                SPContentType publPageContentType = getContentType(contentTypeCollection, "0x01");

                if (publPageContentType != null)
                {
                    publPageContentType.EventReceivers.Add(SPEventReceiverType.ItemAdding,
                           "GSWeb, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9a3cb80ac0d0c704",
                           "GSWeb.erTitle.erTitle");

                    publPageContentType.Update(true, false);
                    myweb.Update();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                myweb.Dispose();
            }
        }


        // Раскомментируйте ниже метод для обработки события, возникающего перед деактивацией компонента.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // Раскомментируйте ниже метод для обработки события, возникающего после установки компонента.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Раскомментируйте ниже метод для обработки события, возникающего перед удалением компонента.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Раскомментируйте ниже метод для обработки события, возникающего при обновлении компонента.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
