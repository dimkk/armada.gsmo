using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections;
using System.Linq;
using System.Security.Permissions;

namespace GS.MVK.Receivers
{

    /// <summary>
    /// TODO: Add comment for Issue
    /// </summary>
    public class IssueItem : SPItemEventReceiver
    {
        private readonly string storageListTemplateId = "10056";
        private readonly string attachmentIssueFieldName = "IssueAttachmentIssueMVK";
        private readonly string issueMeetingFieldName = "IssueMeetingMVK";
        private static readonly string issueNumberFieldName = "IssueNumberTextMVK";
        private readonly string meetingListName = "MeetingMVKList";
        private readonly string meetingDateFieldName = "MeetingDateMVK";
        private readonly string meetingNumberFieldName = "MeetingNumberMVK";

        internal static string getTitle(QuestionTitleData data)
        {
            SPListItem item = data.Number as SPListItem;
            SPFieldCalculated cf = item != null ? item.Fields.GetFieldByInternalName(issueNumberFieldName) as SPFieldCalculated : null;
            object number = cf != null ? cf.GetFieldValueAsText(item[issueNumberFieldName]) : data.Number;
            string result = number != null && number.ToString() != string.Empty ? number.ToString() : "неизвестно";
            return String.Format("Вопрос №{0}", result);
        }

        internal SPListItem doGetQuestionDocumentSet(int Id, SPWeb web)
        {
            SPListItem returnValue = null;
            var targetLib = web.Lists.Cast<SPList>().First(
                l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) as SPDocumentLibrary;

            if (targetLib != null)
            {
                var query = new SPQuery()
                {
                    Query = string.Format(@"<Where>
                                <And>
                                    <Eq>
                                        <FieldRef Name='{0}' LookupId='TRUE' />
                                        <Value Type='Lookup'>{1}</Value>
                                    </Eq>
                                    <BeginsWith>
			                            <FieldRef Name='ContentTypeId' />
			                            <Value Type='ContentTypeId'>0x0120D520</Value>
		                            </BeginsWith>
                                </And>
                            </Where>", attachmentIssueFieldName, Id)
                };
                query.ViewAttributes = "Scope='RecursiveAll'";

                var items = targetLib.GetItems(query);
                returnValue = (items.Count != 1) ? null : items[0];
            }

            return returnValue;
        }

        public SPListItem getMeeting(SPItemEventProperties properties)
        {
            var meetingId = Helpers.getFieldLookupId(properties.ListItem, issueMeetingFieldName);

            return meetingId > 0 ? doGetMeeting(meetingId, properties.Web) : null;
        }

        private SPListItem doGetMeeting(int Id, SPWeb web)
        {
            var query = new SPQuery()
            {
                Query = @"<Where>
                            <Eq>
                                <FieldRef Name='ID' />
                                <Value Type='Integer'>" + Id + @"</Value>
                            </Eq>
                        </Where>"
            };

            var list = web.GetList("/Lists/" + meetingListName);
            var items = list.GetItems(query);

            return (items.Count != 0) ? items[0] : null;
        }

        /// <summary>
        /// TODO: Add comment for event ItemAdded in erAgendaQuestionItem 
        /// </summary>
        /// <param name="properties">Contains list event properties</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            using (new SPMonitoredScope("GS.MVK.IssueItemAdded"))
            {
                var dsvc = SPDiagnosticsService.Local;
                var meetingItem = getMeeting(properties);

                if (meetingItem == null)
                {
                    dsvc.WriteEvent(0,
                        new SPDiagnosticsCategory(
                            Consts.EventLogCategory,
                            TraceSeverity.Monitorable,
                            EventSeverity.Warning),
                        EventSeverity.Error,
                        "Создан элемент списка {0} без ссылки на элемент списка {1}",
                        new object[] { properties.ListTitle, properties.Web.GetList("Lists/" + meetingListName).Title });

                    return;
                }

                var mTitle = MeetingItem.getTitle(new MeetingTitleData()
                {
                    Number = meetingItem[meetingNumberFieldName],
                    Date = meetingItem[meetingDateFieldName]
                });

                if (properties.Web.Lists.Cast<SPList>().First(
                    l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) == null)
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

                var qTitle = getTitle(new QuestionTitleData()
                {
                    Number = properties.ListItem
                });

                var props = new Hashtable
                {
                    {"DocumentSetDescription", "Набор документов для вопроса повестки"},
                    {attachmentIssueFieldName, properties.ListItem}
                };

                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (var site = new SPSite(properties.SiteId))
                        {
                            using (var web = site.OpenWeb(properties.RelativeWebUrl))
                            {
                                var tLib = web.Lists.Cast<SPList>().First(
                                    l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) as SPDocumentLibrary;

                                if (tLib == null) return;
                                var folder = SPFolderHierarchy.GetSubFolder(tLib.RootFolder, mTitle, true);

                                DocumentSet.Create(folder, qTitle, tLib.ContentTypes["Набор документов"].Id, props,
                                    true);
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    dsvc.WriteTrace(0,
                        new SPDiagnosticsCategory(
                            Consts.TraceLogCategory,
                            TraceSeverity.Unexpected,
                            EventSeverity.Error),
                        TraceSeverity.Unexpected,
                        "Не удалось создать набор документов: {0}",
                        new object[] { ex });
                    throw;
                }
            }
        }

        /// <summary>
        /// TODO: Add comment for event ItemDeleting in erAgendaQuestionItem 
        /// </summary>
        /// <param name="properties">Contains list event properties</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            SPDiagnosticsService dsvc = SPDiagnosticsService.Local;
            var targetLib = properties.Web.Lists.Cast<SPList>().First(
                l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) as SPDocumentLibrary;

            if (targetLib == null)
            {
                dsvc.WriteEvent(0,
                    new SPDiagnosticsCategory(
                        Consts.EventLogCategory,
                        TraceSeverity.Monitorable,
                        EventSeverity.Warning),
                    EventSeverity.Error,
                    "Не существует экземпляра списка по шаблону {0}. Невозможно изменить структуру каталогов для хранения вложений",
                    new object[] { storageListTemplateId });

                return;
            }

            using (new SPMonitoredScope("GS.MVK.IssueItemDeleting"))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (var site = new SPSite(properties.SiteId))
                    using (var web = site.OpenWeb(properties.RelativeWebUrl))
                    {
                        var dsItem = doGetQuestionDocumentSet(properties.ListItemId, web);
                        if (dsItem == null)
                        {
                            dsvc.WriteEvent(0,
                                new SPDiagnosticsCategory(
                                    Consts.EventLogCategory,
                                    TraceSeverity.Monitorable,
                                    EventSeverity.Warning),
                                EventSeverity.Warning,
                                "Не удалось определить набор документов в списке {2}, который бы соответствовал элементу списка {0} с Id {1}",
                                new object[] { properties.ListTitle, properties.ListItemId, targetLib.Title });

                            return;
                        }

                        try
                        {
                            dsItem.Delete();
                        }
                        catch (Exception ex)
                        {
                            dsvc.WriteTrace(0,
                                new SPDiagnosticsCategory(
                                    Consts.TraceLogCategory,
                                    TraceSeverity.Unexpected,
                                    EventSeverity.Error),
                                TraceSeverity.Unexpected,
                                "Не удалось удалить набор документов с Id {0}: {1}",
                                new object[] { dsItem.ID, ex });
                            throw;
                        }
                    }
                });
            }
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;

            try
            {
                var newTitle = getTitle(new QuestionTitleData()
                {
                    Number = properties.ListItem
                });

                var dsvc = SPDiagnosticsService.Local;
                var targetLib = properties.Web.Lists.Cast<SPList>().First(
                    l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) as SPDocumentLibrary;

                if (targetLib == null)
                {
                    dsvc.WriteEvent(0,
                        new SPDiagnosticsCategory(
                            Consts.EventLogCategory,
                            TraceSeverity.Monitorable,
                            EventSeverity.Warning),
                        EventSeverity.Error,
                        "Не существует экземпляра списка по шаблону {0}. Невозможно обновить структуру каталогов для хранения вложений",
                        new object[] { storageListTemplateId });

                    return;
                }

                using (new SPMonitoredScope("GS.MVK.IssueItemUpdated"))
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (var site = new SPSite(properties.SiteId))
                        using (var web = site.OpenWeb(properties.RelativeWebUrl))
                        {
                            var dsItem = doGetQuestionDocumentSet(properties.ListItemId, web);
                            if (dsItem == null)
                            {
                                dsvc.WriteEvent(0,
                                    new SPDiagnosticsCategory(
                                        Consts.EventLogCategory,
                                        TraceSeverity.Monitorable,
                                        EventSeverity.Warning),
                                    EventSeverity.Warning,
                                    "Не удалось определить набор документов в списке {2}, который бы соответствовал элементу списка {0} с Id {1}",
                                    new object[] { properties.ListTitle, properties.ListItemId, targetLib.Title });

                                return;
                            }

                            try
                            {
                                var renameTo = SPUrlUtility.CombineUrl(dsItem.Folder.ParentFolder.Url, newTitle);
                                if (dsItem.Folder.Url.Equals(renameTo, StringComparison.Ordinal))
                                    return;

                                dsItem.Folder.MoveTo(renameTo);
                            }
                            catch (Exception ex)
                            {
                                dsvc.WriteTrace(0,
                                    new SPDiagnosticsCategory(
                                        Consts.TraceLogCategory,
                                        TraceSeverity.Unexpected,
                                        EventSeverity.Error),
                                    TraceSeverity.Unexpected,
                                    "Не удалось переименовать набор документов с Id {0}: {1}",
                                    new object[] { dsItem.ID, ex });
                                throw;
                            }
                        }
                    });
                }
            }
            finally
            {
                EventFiringEnabled = true;
            }
        }
    }

    internal struct QuestionTitleData
    {
        public object Number;
    }
}

