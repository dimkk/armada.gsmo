using ITB.SP.Tools;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using System.Collections;
using System.Linq;

namespace SAMRT.Common.BL
{
    public class IssueAk
    {
        protected static readonly string AttachmentListName = "AttachmentList";
        protected static readonly string AttachmentNameTemplate = "Вопрос №{0}";

        protected static readonly string AttachmentIssueRgFieldName = "AttachmentIssueRg";
        protected static readonly string AttachmentIssueAkFieldName = "AttachmentIssueAk";

        protected static readonly string IssueAkIssueRgFieldName = "AgendaQuestionIssue";
        protected static readonly string IssueNumberAkFieldName = "AgendaQuestionNumber";
        protected static readonly string IssueAkMeetingAkFieldName = "MeetingLink";

        public static SPListItem CreateAttachment(SPWeb web, SPListItem issueAk)
        {
            SPList attachmentList = web.GetListByUrl(AttachmentListName);

            string issueNumber = issueAk.GetFieldValue(IssueNumberAkFieldName);
            string attachmentName = string.Format(AttachmentNameTemplate, issueNumber);

            SPContentTypeId contentTypeId =
                attachmentList.ContentTypes.Cast<SPContentType>()
                    .First(s => s.Id.ToString().StartsWith(SPBuiltInContentTypeId.DocumentSet.ToString()))
                    .Id;

            var props = new Hashtable
                {
                    { "DocumentSetDescription", "Набор документов для вопроса Архитектурной комиссии" },
                    { AttachmentIssueAkFieldName, issueAk}
                };

            SPFolder folder = MeetingAk.EnsureAttachmentSubFolder(web, issueAk.GetFieldLookup(IssueAkMeetingAkFieldName).LookupId);

            DocumentSet documentSet = null;
            SPSecurity.RunWithElevatedPrivileges(
                () => documentSet = DocumentSet.Create(folder, attachmentName, contentTypeId, props));

            return attachmentList.GetItemById(documentSet.Item.ID);
        }

        public static void CreateOrLinkAttachment(SPWeb web, SPListItem issueAk)
        {
            int issueRgId = issueAk.GetFieldLookup(IssueAkIssueRgFieldName).LookupId;

            SPListItem attachment = null;
            if (issueRgId > 0)
            {
                //Если есть связь с вопросом РГ, то пытаемся получить вложение этого вопроса
                SPList attachmentList = web.GetListByUrl(AttachmentListName);
                attachment = attachmentList.GetItemsByLookupId(AttachmentIssueRgFieldName, issueRgId).SingleOrDefault();
                if (attachment == null)
                    Log.Error("Не найден набор документов из списка {0} для вопроса АК (ID = {1}), унаследованный от вопроса РГ (ID = {2}). Будет создан новый набор документов.", AttachmentListName, issueAk.ID, issueRgId);
            }

            if (issueRgId <= 0 || attachment == null)
                //Если связи с вопросом РГ нет или набор документов у связанного вопроса не обнаружен, то создаем новый набор документов для вопроса АК
                attachment = CreateAttachment(web, issueAk);

            attachment[AttachmentIssueAkFieldName] = new SPFieldLookupValue(issueAk.ID, issueAk.ID.ToString());

            web.AllowUnsafeUpdates = true;
            attachment.Update();
            web.AllowUnsafeUpdates = false;
        }
    }
}
