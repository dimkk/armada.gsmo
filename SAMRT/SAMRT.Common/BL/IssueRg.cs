using ITB.SP.Tools;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using System.Collections;
using System.Linq;

namespace SAMRT.Common.BL
{
    public class IssueRg
    {
        protected static readonly string AttachmentListName = "AttachmentList";
        protected static readonly string AttachmentNameTemplate = "Вопрос №{0}";

        protected static readonly string AttachmentOrderRgFieldName = "AttachmentOrder";
        protected static readonly string AttachmentIssueRgFieldName = "AttachmentIssueRg";
        
        protected static readonly string IssueRgOrderRgFieldName = "IssueOrderMVK";
        protected static readonly string IssueNumberRgFieldName = "IssueNumberMVK";

        protected static readonly string IssueRgMeetingRgFieldName = "IssueMeetingMVK";

        public static SPListItem CreateAttachment(SPWeb web, SPListItem issueRg)
        {
            SPList attachmentList = web.GetListByUrl(AttachmentListName);

            string issueNumber = issueRg.GetFieldValue(IssueNumberRgFieldName);
            string attachmentName = string.Format(AttachmentNameTemplate, issueNumber);

            SPContentTypeId contentTypeId =
                attachmentList.ContentTypes.Cast<SPContentType>()
                    .First(s => s.Id.ToString().StartsWith(SPBuiltInContentTypeId.DocumentSet.ToString()))
                    .Id;

            var props = new Hashtable
                {
                    { "DocumentSetDescription", "Набор документов для вопроса Рабочей Группы" },
                    { AttachmentIssueRgFieldName, issueRg}
                };

            SPFolder folder = MeetingRg.EnsureAttachmentSubFolder(web, issueRg.GetFieldLookup(IssueRgMeetingRgFieldName).LookupId);

            DocumentSet documentSet = null;
            SPSecurity.RunWithElevatedPrivileges(
                () => documentSet = DocumentSet.Create(folder, attachmentName, contentTypeId, props));

            return attachmentList.GetItemById(documentSet.Item.ID);
        }

        public static void CreateOrLinkAttachment(SPWeb web, SPListItem issueRg)
        {
            int orderRgId = issueRg.GetFieldLookup(IssueRgOrderRgFieldName).LookupId;

            SPListItem attachment = null;
            if (orderRgId > 0)
            {
                //Если есть связь с заявкой, то пытаемся получить вложение этой заявки
                SPList attachmentList = web.GetListByUrl(AttachmentListName);
                attachment = attachmentList.GetItemsByLookupId(AttachmentOrderRgFieldName, orderRgId).SingleOrDefault();
                if (attachment == null)
                    Log.Error("Не найден набор документов из списка {0} для вопроса РГ (ID = {1}), унаследованный от заявки РГ (ID = {2}). Будет создан новый набор документов.", AttachmentListName, issueRg.ID, orderRgId);
            }

            if (orderRgId <= 0 || attachment == null)
                //Если связи с заявкой нет или набор документов у связанной заявки не обнаружен, то создаем новый набор документов для вопроса
                attachment = CreateAttachment(web, issueRg);

            attachment[AttachmentIssueRgFieldName] = new SPFieldLookupValue(issueRg.ID, issueRg.ID.ToString());

            web.AllowUnsafeUpdates = true;
            attachment.Update();
            web.AllowUnsafeUpdates = false;
        }
    }
}
