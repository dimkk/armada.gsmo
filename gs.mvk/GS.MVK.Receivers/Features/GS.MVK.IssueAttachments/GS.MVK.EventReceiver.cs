using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace GS.MVK.Receivers.Features.GS.MVK.IssueAttachments
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("88f4290c-ac8d-4bc9-8a36-caac52b4c6be")]
    public class IssueAttachmentsEventReceiver : SPFeatureReceiver
    {
        private readonly string storageListTemplateId = "10056";
        private readonly string attachmentIssueFieldName = "IssueAttachmentIssueMVK";
        private readonly string meetingListName = "MeetingMVKList";
        private readonly string meetingDateFieldName = "MeetingDateMVK";
        private readonly string meetingNumberFieldName = "MeetingNumberMVK";
        private readonly string issueListName = "IssueMVKList";
        private readonly string issueMeetingFieldName = "IssueMeetingMVK";
        private readonly string issueNumberFieldName = "IssueNumberTextMVK";

        #region private members

        private SPList m_meetingList;
        private SPList m_questionList;
        private SPWeb m_web;
        private SPDocumentLibrary m_targetLib;

        #endregion

        public SPList meetingList
        {
            get
            {
                if (m_meetingList == null)
                {
                    m_meetingList = this.web != null ? this.web.GetList("Lists/" + meetingListName) : null;
                }

                return m_meetingList;
            }
        }

        public SPList questionList
        {
            get
            {
                if (m_questionList == null)
                {
                    m_questionList = this.web != null ? this.web.GetList("Lists/" + issueListName) : null;
                }

                return m_questionList;
            }
        }

        public SPWeb web
        {
            get
            {
                return m_web;
            }
        }

        public SPDocumentLibrary targetLib
        {
            get
            {
                if (m_targetLib == null)
                {
                    SPList lib = this.web != null ? this.web.Lists.Cast<SPList>().FirstOrDefault(
                        l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) : null;

                    m_targetLib = lib as SPDocumentLibrary;
                }

                return m_targetLib;
            }
        }

        private void init(SPFeatureReceiverProperties properties)
        {
            m_web = properties.Feature.Parent as SPWeb;
            if (m_web == null)
                m_web = ((SPSite)properties.Feature.Parent).RootWeb;
        }

        private SPListItemCollection getRelatedQuestionList(SPListItem meetingItem)
        {
            SPQuery query = new SPQuery()
            {
                Query = string.Format(@"<Where>
                            <Eq>
                                <FieldRef Name='{0}' LookupId='TRUE' />
                                <Value Type='Lookup'>{1}</Value>
                            </Eq>
                        </Where>", issueMeetingFieldName, meetingItem.ID)
            };

            return questionList.GetItems(query);
        }

        private SPFolder createFolderIfNotExists(SPListItem meetingItem)
        {
            string mTitle = MeetingItem.getTitle(new MeetingTitleData()
            {
                Number = meetingItem[meetingNumberFieldName],
                Date = meetingItem[meetingDateFieldName]
            });

            SPFolder meetingFolder = SPFolderHierarchy.GetSubFolder(targetLib.RootFolder, mTitle, true);

            return meetingFolder;
        }

        private void createDocumentSetIfNotExists(SPListItem questionItem, SPListItem meetingItem, SPFolder parentFolder)
        {
            string qTitle = IssueItem.getTitle(new QuestionTitleData()
            {
                Number = questionItem
            });

            SPFolder questionFolder = SPFolderHierarchy.GetSubFolder(parentFolder, qTitle, false);

            if (questionFolder == null || !questionFolder.Exists)
            {
                Hashtable props = new Hashtable();
                props.Add("DocumentSetDescription", "Автоматически созданный набор документов для вопроса");
                props.Add(attachmentIssueFieldName, questionItem);

                DocumentSet.Create(parentFolder, qTitle, targetLib.ContentTypes["Набор документов"].Id, props, true);
            }
        }

        /// <summary>
        /// TODO: Add comment to describe the actions after feature activation
        /// </summary>
        /// <param name="properties">Properties of the feature</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            init(properties);

            SPDiagnosticsService dsvc = SPDiagnosticsService.Local;

            if ((meetingList == null) || (questionList == null))
            {
                dsvc.WriteEvent(0,
                    new SPDiagnosticsCategory(
                        Consts.EventLogCategory,
                        TraceSeverity.Monitorable,
                        EventSeverity.Error),
                    EventSeverity.Error,
                    "Не существует экземпляров списков заседания и вопросов повестки",
                    new object[] { storageListTemplateId });

                return;
            }

            if (targetLib == null)
            {
                dsvc.WriteEvent(0,
                    new SPDiagnosticsCategory(
                        Consts.EventLogCategory,
                        TraceSeverity.Monitorable,
                        EventSeverity.Warning),
                    EventSeverity.Error,
                    "Не существует экземпляра списка по шаблону {0}. Невозможно создать структуру каталогов для хранения вложений",
                    new object[] { storageListTemplateId });

                return;
            }

            try
            {
                using (new SPMonitoredScope("GS.MVK.FeatureActivated"))
                {
                    SPListItemCollection meetingItemList = meetingList.GetItems(new SPQuery());
                    foreach (SPListItem meetingItem in meetingItemList)
                    {
                        SPFolder meetingFolder = createFolderIfNotExists(meetingItem);
                        SPListItemCollection questionItemList = getRelatedQuestionList(meetingItem);

                        foreach (SPListItem questionItem in questionItemList)
                        {
                            createDocumentSetIfNotExists(questionItem, meetingItem, meetingFolder);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dsvc.WriteTrace(0,
                    new SPDiagnosticsCategory(
                        Consts.TraceLogCategory,
                        TraceSeverity.Unexpected,
                        EventSeverity.Error),
                    TraceSeverity.Unexpected,
                    "Произошла ошибка при создании структуры каталогов для хранения вложений вопросов повестки: {0}",
                    new object[] { ex });
                throw;
            }
        }
    }
}
