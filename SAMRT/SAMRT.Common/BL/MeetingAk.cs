using ITB.SP.Tools;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAMRT.Common.BL
{
    public class MeetingAk
    {
        protected static readonly string MeetingAkListName = "MeetingList";
        protected static readonly string IssueRgListName = "IssueMVKList";
        protected static readonly string IssueAkListName = "AgendaQuestionList";
        protected static readonly string AttachmentListName = "AttachmentList";

        protected static readonly string IssueMeetingAkFieldName = "MeetingLink";
        protected static readonly string IssueNumberAkFieldName = "AgendaQuestionNumber";
        protected static readonly string IssueAkIssueRgFieldName = "AgendaQuestionIssue";

        protected static readonly string IssueDecisionTypeRgFieldName = "IssueDecisionTypeMVK";
        protected static readonly string IssueRgIssueAkCountFieldName = "IssueRgIssueAkCount";

        protected static readonly string AttachmentIssueRgFieldName = "AttachmentIssueRg";
        protected static readonly string AttachmentIssueAkFieldName = "AttachmentIssueAk";

        protected static readonly string AttachmentFolderNameTemplate = "Заседание АК №{0} от {1}";

        protected static readonly string MeetingNumberAkFieldName = "MeetingNumber";
        protected static readonly string MeetingDateAkFieldName = "MeetingDate";

        protected static readonly Dictionary<string, string> IssueRgToIssueAkFieldMapping = new Dictionary<string, string>()
        {
            { "IssueAddressMVK", "AgendaQuestionAddress" },
            { "IssueCadastreIdMVK", "CadastreNumber" },
            { "IssueCategoryMVKLink", "QuestionCategoryLink" },
            { "IssueMunicipalDistrictMVK", "_x041c__x0443__x043d__x0438__x04" },
            { "IssueDescriptionMVK", "AgendaQuestionDescription" },
            { "IssueSettlementMVK", "_x041f__x043e__x0441__x0435__x04" },
            { "IssueThemeMVK", "AgendaQuestionTheme" },
            { "IssueProjectOrgRg", "IssueProjectOrgAk" },
            { "IssueReporterRg", "IssueReporterAk" }
        };

        public static void CreateIssuesAkFromUnusedIssuesRg(SPWeb web, int meetingAkId, int decisionTypeIssueRgId)
        {
            SPList issueRgList = web.GetListByUrl(IssueRgListName);

            var query = new SPQuery()
            {
                Query = string.Format(@"<Where>" +
                                            "<And>" +
                                                "<Eq>" +
                                                    "<FieldRef Name='{0}' LookupId='TRUE' />" +
                                                    "<Value Type='Lookup'>{1}</Value>" +
                                                "</Eq>" +
                                                "<Eq>" +
                                                    "<FieldRef Name='{2}' />" +
                                                    "<Value Type='Text'>0</Value>" +
                                                "</Eq>" +
                                            "</And>" +
                                        "</Where>" +
                                        "<OrderBy>" +
                                            "<FieldRef Name='Created' Ascending='TRUE'/>" +
                                        "</OrderBy>", IssueDecisionTypeRgFieldName, decisionTypeIssueRgId, IssueRgIssueAkCountFieldName)
            };

            SPListItemCollection unusedIssueRgList = issueRgList.GetItems(query);

            CreateIssuesAkFromIssuesRg(web, meetingAkId, unusedIssueRgList.Cast<SPListItem>());
        }

        public static int GetIssueAkNextNumber(SPWeb web, int meetingAkId)
        {
            SPList issueAkList = web.GetListByUrl(IssueAkListName);

            var query = new SPQuery()
            {
                Query = string.Format(@"<Where>" +
                                            "<Eq>" +
                                                "<FieldRef Name='{0}' LookupId='TRUE' />" +
                                                "<Value Type='Lookup'>{1}</Value>" +
                                            "</Eq>" +
                                        "</Where>" +
                                        "<OrderBy>" +
                                            "<FieldRef Name='{2}' Ascending='FALSE'/>" +
                                        "</OrderBy>", IssueMeetingAkFieldName, meetingAkId, IssueNumberAkFieldName),
                RowLimit = 1
            };

            SPListItemCollection issuesAk = issueAkList.GetItems(query);
            return issuesAk.Count == 0 ? 1 : Convert.ToInt32(issuesAk[0][IssueNumberAkFieldName]) + 1;
        }

        public static void CreateIssuesAkFromIssuesRg(SPWeb web, int meetingAkId, IEnumerable<int> issuesRgId)
        {
            SPList issueRgList = web.GetListByUrl(IssueRgListName);

            var query = new SPQuery()
            {
                Query = string.Format(@"<Where>" +
                                            "<In>" +
                                                "<FieldRef Name='ID' LookupId='TRUE' />" +
                                                "<Values>{0}</Values>" +
                                            "</In>" +
                                        "</Where>", string.Join(string.Empty, issuesRgId.Select(s => string.Format("<Value Type='Lookup'>{0}</Value>", s))))
            };

            CreateIssuesAkFromIssuesRg(web, meetingAkId, issueRgList.GetItems(query).Cast<SPListItem>());
        }

        public static void CreateIssuesAkFromIssuesRg(SPWeb web, int meetingAkId, IEnumerable<SPListItem> issuesRg)
        {
            var exceptions = new List<Exception>();

            int nextIssueAkNumber = GetIssueAkNextNumber(web, meetingAkId);

            foreach (SPListItem issueRg in issuesRg)
            {
                try
                {
                    CreateIssueAkFromIssueRg(web, meetingAkId, issueRg, nextIssueAkNumber++);
                }
                catch (Exception e)
                {
                    exceptions.Add(new Exception(string.Format("При обработке элемента (ID = {0}) списка {1} произошло неожиданное исключение", issueRg.ID, IssueRgListName), e));
                }
            }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        public static void CreateIssueAkFromIssueRg(SPWeb web, int meetingAkId, SPListItem issueRg, int issueAkNumber)
        {
            SPList issueAkList = web.GetListByUrl(IssueAkListName);

            SPListItem newIssueAk = issueAkList.AddItem();
            newIssueAk[IssueMeetingAkFieldName] = new SPFieldLookupValue(meetingAkId, meetingAkId.ToString());
            newIssueAk[IssueAkIssueRgFieldName] = new SPFieldLookupValue(issueRg.ID, issueRg.ID.ToString());
            newIssueAk[IssueNumberAkFieldName] = issueAkNumber;

            foreach (string issueRgFieldName in IssueRgToIssueAkFieldMapping.Keys)
                newIssueAk[IssueRgToIssueAkFieldMapping[issueRgFieldName]] = issueRg[issueRgFieldName];

            web.AllowUnsafeUpdates = true;
            newIssueAk.Update();
            web.AllowUnsafeUpdates = false;
        }

        public static SPFolder EnsureAttachmentSubFolder(SPWeb web, int meetingAkId)
        {
            SPList meetingAkList = web.GetListByUrl(MeetingAkListName);
            SPListItem meetingAk = meetingAkList.GetItemById(meetingAkId);
            return EnsureAttachmentSubFolder(web, meetingAk);
        }

        public static SPFolder EnsureAttachmentSubFolder(SPWeb web, SPListItem meetingAk)
        {
            SPList attachmentList = web.GetListByUrl(AttachmentListName);

            string folderName = string.Format(AttachmentFolderNameTemplate,
                meetingAk.GetFieldValue(MeetingNumberAkFieldName),
                meetingAk.GetFieldValueDateTime(MeetingDateAkFieldName).ToShortDateString());

            SPFolder folder = null;
            SPSecurity.RunWithElevatedPrivileges(
                () => folder = SPFolderHierarchy.GetSubFolder(attachmentList.RootFolder, folderName, true));

            return folder;
        }

        public static int GetNextMeetingNumber(SPWeb web)
        {
            int? number = web.GetNextMeetingNumberAk();
            if (!number.HasValue || number.Value <= 0)
                number = 1;
            web.SetNextMeetingNumberAk(number.Value + 1);
            return number.Value;
        }
    }
}
