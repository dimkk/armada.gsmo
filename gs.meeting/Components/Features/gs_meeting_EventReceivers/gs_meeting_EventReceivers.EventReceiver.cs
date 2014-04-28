// <copyright file="gs_meeting_EventReceivers.EventReceiver.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-02-13 15:50:11Z</date>
namespace gs.meeting.Components
{
    using System;
    using System.Collections.Generic;
    using System.Security.Permissions;
    using System.Text;
    using System.Collections;
    using System.Linq;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.Utilities;
    using Microsoft.SharePoint.Security;
    using Microsoft.Office.Server.Utilities;
    using Microsoft.Office.DocumentManagement.DocumentSets;

    /// <summary>
    /// TODO: Add comment to EventReceiversEventReceiver
    /// </summary>
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    public class EventReceiversEventReceiver : SPFeatureReceiver
    {
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
                    m_meetingList = this.web != null ? this.web.GetList("Lists/MeetingList") : null;
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
                    m_questionList = this.web != null ? this.web.GetList("Lists/AgendaQuestionList") : null;
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
                        l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) : null;

                    m_targetLib = lib as SPDocumentLibrary;
                }

                return m_targetLib;
            }
        }

        private void init(SPFeatureReceiverProperties properties)
        {
            m_web = properties.Feature.Parent as SPWeb;
        }

        private SPListItemCollection getRelatedQuestionList(SPListItem meetingItem)
        {
            SPQuery query = new SPQuery()
            {
                Query = @"<Where>
                            <Eq>
                                <FieldRef Name='MeetingLink' LookupId='TRUE' />
                                <Value Type='Lookup'>" + meetingItem.ID + @"</Value>
                            </Eq>
                        </Where>"
            };

            return questionList.GetItems(query);
        }

        private SPFolder createFolderIfNotExists(SPListItem meetingItem)
        {
            string mTitle = erMeetingItem.getTitle(new MeetingTitleData()
            {
                Number = meetingItem["MeetingNumber"],
                Date = meetingItem["MeetingDate"]
            });

            SPFolder meetingFolder = SPFolderHierarchy.GetSubFolder(targetLib.RootFolder, mTitle, true);

            return meetingFolder;
        }

        private void createDocumentSetIfNotExists(SPListItem questionItem, SPListItem meetingItem, SPFolder parentFolder)
        {
            string qTitle = erAgendaQuestionItem.getTitle(new QuestionTitleData()
            {
                Number = questionItem["AgendaQuestionNumber"]
            });

            SPFolder questionFolder = SPFolderHierarchy.GetSubFolder(parentFolder, qTitle, false);

            if (questionFolder == null || !questionFolder.Exists)
            {
                Hashtable props = new Hashtable();
                props.Add("DocumentSetDescription", "Автоматически созданный набор документов для вопроса");
                props.Add("AgendaQuestionLinkFromDS", questionItem);
                props.Add("MeetingLinkFromDS", meetingItem);

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
                    new object[] { Consts.StorageListTemplateId });

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
                    new object[] { Consts.StorageListTemplateId });

                return;
            }

            try
            {
                using (new SPMonitoredScope("gradsovet - gs_meeting_EventReceivers Feature Receiver - FeatureActivated"))
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

        /// <summary>
        /// TODO: Add comment to describe the actions during feature deactivation
        /// </summary>
        /// <param name="properties">Properties of the feature</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            ////TODO: place receiver code here or remove method
        }

        /// <summary>
        /// TODO: Add comment to describe the actions after feature installation
        /// </summary>
        /// <param name="properties">Properties of the feature</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
            ////TODO: place receiver code here or remove method
        }

        /// <summary>
        /// TODO: Add comment to describe the actions during feature uninstalling
        /// </summary>
        /// <param name="properties">Properties of the feature</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            ////TODO: place receiver code here or remove method
        }

        /// <summary>
        /// TODO: Add comment to describe the actions during feature upgrading
        /// </summary>
        /// <param name="properties">Properties of the feature</param>
        /// <param name="upgradeActionName">The name of the custom upgrade action to execute. The value can be null if the override of this method implements only one upgrade action.</param>
        /// <param name="parameters">Parameter names and values for the custom action</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        {
            ////TODO: place receiver code here or remove method
        }
    }
}

