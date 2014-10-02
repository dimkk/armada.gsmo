using ITB.SP.Tools;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAMRT.Common.BL
{
    public class MeetingRg
    {
        protected static readonly string OrderRgListName = "OrderMVKList";
        protected static readonly string MeetingRgListName = "MeetingMVKList";
        protected static readonly string IssueRgListName = "IssueMVKList";
        protected static readonly string AttachmentListName = "AttachmentList";
        protected static readonly string AttachmentFolderNameTemplate = "Заседание РГ №{0} от {1}";

        protected static readonly string OrderRgIssueRgCountFieldName = "OrderIssueRgCount";

        protected static readonly string IssueMeetingRgFieldName = "IssueMeetingMVK";
        protected static readonly string IssueOrderRgFieldName = "IssueOrderMVK";
        protected static readonly string IssueNumberRgFieldName = "IssueNumberMVK";

        protected static readonly string AttachmentOrderRgFieldName = "AttachmentOrder";
        protected static readonly string AttachmentIssueRgFieldName = "AttachmentIssueRg";

        protected static readonly string MeetingNumberRgFieldName = "MeetingNumberMVK";
        protected static readonly string MeetingDateRgFieldName = "MeetingDateMVK";

        protected static readonly Dictionary<string, string> OrderToIssueRgCopiedFieldNames = new Dictionary<string, string>()
        {
            { "OrderBuilderMVK",    "OrderBuilderMVK" },
            { "adress",             "IssueAddressMVK" },
            { "kadnomber",          "IssueCadastreIdMVK" },
            { "QuaestionCategory",  "IssueCategoryMVKLink" },
            { "municipality",       "IssueMunicipalDistrictMVK" },
            { "OrderTextMVK",       "IssueDescriptionMVK" },
            { "settlement",         "IssueSettlementMVK" },
            { "SubjectQuestion",    "IssueThemeMVK" },
            { "OrderProjectOrgRg",  "IssueProjectOrgRg" },
            { "OrderReporterRg",    "IssueReporterRg" }
        };

        public static void CreateIssuesRgFromUnusedOrdersRg(SPWeb web, int meetingRgId)
        {
            SPList orderRgList = web.GetListByUrl(OrderRgListName);

            var query = new SPQuery()
            {
                Query = string.Format(@"<Where>" +
                                            "<Eq>" +
                                                "<FieldRef Name='{0}' />" +
                                                "<Value Type='Text'>0</Value>" +
                                            "</Eq>" +
                                        "</Where>" +
                                        "<OrderBy>" +
                                            "<FieldRef Name='Created' Ascending='TRUE'/>" +
                                        "</OrderBy>", OrderRgIssueRgCountFieldName),
                RowLimit = 10
            };

            SPListItemCollection unusedOrdersRg = orderRgList.GetItems(query);
            CreateIssuesRgFromOrdersRg(web, meetingRgId, unusedOrdersRg.Cast<SPListItem>());
        }

        public static int GetIssueRgNextNumber(SPWeb web, int meetingRgId)
        {
            SPList issueRgList = web.GetListByUrl(IssueRgListName);

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
                                        "</OrderBy>", IssueMeetingRgFieldName, meetingRgId, IssueNumberRgFieldName),
                RowLimit = 1
            };

            SPListItemCollection issuesRg = issueRgList.GetItems(query);
            return issuesRg.Count == 0 ? 1 : Convert.ToInt32(issuesRg[0][IssueNumberRgFieldName]) + 1;
        }

        public static void CreateIssuesRgFromOrdersRg(SPWeb web, int meetingRgId, IEnumerable<int> orderRgIdList)
        {
            SPList orderRgList = web.GetListByUrl(OrderRgListName);

            var query = new SPQuery()
            {
                Query = string.Format(@"<Where>" +
                                            "<In>" +
                                                "<FieldRef Name='ID' LookupId='TRUE' />" +
                                                "<Values>{0}</Values>" +
                                            "</In>" +
                                        "</Where>", string.Join(string.Empty, orderRgIdList.Select(s => string.Format("<Value Type='Lookup'>{0}</Value>", s))))
            };

            CreateIssuesRgFromOrdersRg(web, meetingRgId, orderRgList.GetItems(query).Cast<SPListItem>());
        }

        public static void CreateIssuesRgFromOrdersRg(SPWeb web, int meetingRgId, IEnumerable<SPListItem> ordersRg)
        {
            var exceptions = new List<Exception>();

            int nextIssueRgNumber = GetIssueRgNextNumber(web, meetingRgId);

            foreach (SPListItem orderRg in ordersRg)
            {
                try
                {
                    CreateIssueRgFromOrderRg(web, meetingRgId, orderRg, nextIssueRgNumber++);
                }
                catch (Exception e)
                {
                    exceptions.Add(new Exception(string.Format("При обработке элемента (ID = {0}) списка {1} произошло неожиданное исключение", orderRg.ID, OrderRgListName), e));
                }
            }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        public static void CreateIssueRgFromOrderRg(SPWeb web, int meetingRgId, SPListItem orderRg, int issueRgNumber)
        {
            SPList issueRgList = web.GetListByUrl(IssueRgListName);

            SPListItem newIssueRg = issueRgList.AddItem();
            newIssueRg[IssueMeetingRgFieldName] = new SPFieldLookupValue(meetingRgId, meetingRgId.ToString());
            newIssueRg[IssueOrderRgFieldName] = new SPFieldLookupValue(orderRg.ID, orderRg.ID.ToString());
            newIssueRg[IssueNumberRgFieldName] = issueRgNumber;

            foreach (string orderRgFieldName in OrderToIssueRgCopiedFieldNames.Keys)
                newIssueRg[OrderToIssueRgCopiedFieldNames[orderRgFieldName]] = orderRg[orderRgFieldName];

            web.AllowUnsafeUpdates = true;
            newIssueRg.Update();
            web.AllowUnsafeUpdates = false;
        }

        public static SPFolder EnsureAttachmentSubFolder(SPWeb web, int meetingRgId)
        {
            SPList meetingRgList = web.GetListByUrl(MeetingRgListName);
            SPListItem meetingRg = meetingRgList.GetItemById(meetingRgId);
            return EnsureAttachmentSubFolder(web, meetingRg);
        }

        public static SPFolder EnsureAttachmentSubFolder(SPWeb web, SPListItem meetingRg)
        {
            SPList attachmentList = web.GetListByUrl(AttachmentListName);

            string folderName = string.Format(AttachmentFolderNameTemplate,
                meetingRg.GetFieldValue(MeetingNumberRgFieldName),
                meetingRg.GetFieldValueDateTime(MeetingDateRgFieldName).ToShortDateString());

            SPFolder folder = null;
            SPSecurity.RunWithElevatedPrivileges(
                () => folder = SPFolderHierarchy.GetSubFolder(attachmentList.RootFolder, folderName, true));

            return folder;
        }

        public static int GetNextMeetingNumber(SPWeb web)
        {
            int? number = web.GetNextMeetingNumberRg();
            if (!number.HasValue || number.Value <= 0)
                number = 1;
            web.SetNextMeetingNumberRg(number.Value + 1);
            return number.Value;
        }
    }
}
