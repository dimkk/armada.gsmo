using ITB.SP.Tools;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using System.Collections;
using System.Linq;

namespace SAMRT.Common.BL
{
    public class OrderRg
    {
        protected static readonly string OrderRgListName = "OrderMVKList";

        protected static readonly string AttachmentListName = "AttachmentList";
        protected static readonly string AttachmentNameTemplate = "Заявка №{0}";
        protected static readonly string AttachmentOrderRgFieldName = "AttachmentOrder";

        public static void CreateAttachment(SPWeb web, SPListItem orderRg)
        {
            SPList attachmentList = web.GetListByUrl(AttachmentListName);
            string attachmentName = string.Format(AttachmentNameTemplate, orderRg.ID);
            SPContentTypeId contentTypeId =
                attachmentList.ContentTypes.Cast<SPContentType>()
                    .First(s => s.Id.ToString().StartsWith(SPBuiltInContentTypeId.DocumentSet.ToString()))
                    .Id;

            var props = new Hashtable
                {
                    { "DocumentSetDescription", "Набор документов для заявки Рабочей Группы" },
                    { AttachmentOrderRgFieldName, orderRg}
                };

            SPSecurity.RunWithElevatedPrivileges(
                () => DocumentSet.Create(attachmentList.RootFolder, attachmentName, contentTypeId, props));
        }

        public static void DeleteAttachment(SPWeb web, SPListItem orderRg)
        {
            SPList attachmentList = web.GetListByUrl(AttachmentListName);
            SPListItem attachment = attachmentList.GetItemsByLookupId(AttachmentOrderRgFieldName, orderRg.ID).SingleOrDefault();
            if (attachment == null)
                throw new HandledException("У заявки РГ (ID = {0}) не найден набор документов из списка {1}", orderRg.ID, AttachmentListName);

            SPSecurity.RunWithElevatedPrivileges(attachment.Delete);
        }
    }
}
