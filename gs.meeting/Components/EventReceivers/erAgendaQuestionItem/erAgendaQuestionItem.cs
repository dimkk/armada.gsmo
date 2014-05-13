// <copyright file="erAgendaQuestionItem.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-02-14 15:53:48Z</date>
namespace gs.meeting.Components
{
    using System;
    using System.Security.Permissions;
    using System.Linq;
    using System.Collections;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.Security;
    using Microsoft.SharePoint.Utilities;
    using Microsoft.Office.Server.Utilities;
    using Microsoft.Office.DocumentManagement.DocumentSets;

    internal struct QuestionTitleData
    {
        public object Number;
    }

    /// <summary>
    /// TODO: Add comment for erAgendaQuestionItem
    /// </summary>
    public class erAgendaQuestionItem : SPItemEventReceiver
    {
        internal static string getTitle(QuestionTitleData data)
        {
            var number = data.Number == null ? "неизвестно" : data.Number.ToString();
            return String.Format("Вопрос №{0}", number);
        }

        internal static SPListItem doGetQuestionDocumentSet(int Id, SPWeb web)
        {
            SPListItem returnValue = null;
            var targetLib = web.Lists.Cast<SPList>().First(
                l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) as SPDocumentLibrary;

            if (targetLib != null)
            {
                var query = new SPQuery()
                {
                    Query = @"<Where>
                                <And>
                                    <Eq>
                                        <FieldRef Name='AgendaQuestionLinkFromDS' LookupId='TRUE' />
                                        <Value Type='Lookup'>" + Id + @"</Value>
                                    </Eq>
                                    <BeginsWith>
			                            <FieldRef Name='ContentTypeId' />
			                            <Value Type='ContentTypeId'>0x0120D520</Value>
		                            </BeginsWith>
                                </And>
                            </Where>"
                };
                query.ViewAttributes = "Scope='RecursiveAll'";

                var items = targetLib.GetItems(query);
                returnValue = (items.Count != 1) ? null : items[0];
            }

            return returnValue;
        }

        public SPListItem getMeeting(SPItemEventProperties properties)
        {
            var meetingId = Helpers.getFieldLookupId(properties.ListItem, "MeetingLink");

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

            var list = web.GetList("/Lists/MeetingList");
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
            using (new SPMonitoredScope("gradsovet - erAgendaQuestionItem Event Receiver - ItemAdded"))
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
                        new object[] {properties.ListTitle, properties.Web.GetList("Lists/MeetingList").Title});

                    return;
                }

                var mTitle = erMeetingItem.getTitle(new MeetingTitleData()
                {
                    Number = meetingItem["MeetingNumber"],
                    Date = meetingItem["MeetingDate"]
                });

                if (properties.Web.Lists.Cast<SPList>().First(
                    l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) == null)
                {
                    dsvc.WriteEvent(0,
                        new SPDiagnosticsCategory(
                            Consts.EventLogCategory,
                            TraceSeverity.Monitorable,
                            EventSeverity.Warning),
                        EventSeverity.Error,
                        "Не существует экземпляра списка по шаблону {0}. Невозможно создать структуру каталогов для хранения вложений",
                        new object[] {Consts.StorageListTemplateId});

                    return;
                }

                var qTitle = getTitle(new QuestionTitleData()
                {
                    Number = properties.ListItem["AgendaQuestionNumber"]
                });

                var props = new Hashtable
                {
                    {"DocumentSetDescription", "Набор документов для вопроса повестки"},
                    {"AgendaQuestionLinkFromDS", properties.ListItem},
                    {"MeetingLinkFromDS", meetingItem}
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
                                    l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) as SPDocumentLibrary;

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
                        new object[] {ex});
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
                l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) as SPDocumentLibrary;

            if (targetLib == null)
            {
                dsvc.WriteEvent(0,
                    new SPDiagnosticsCategory(
                        Consts.EventLogCategory,
                        TraceSeverity.Monitorable,
                        EventSeverity.Warning),
                    EventSeverity.Error,
                    "Не существует экземпляра списка по шаблону {0}. Невозможно изменить структуру каталогов для хранения вложений",
                    new object[] {Consts.StorageListTemplateId});

                return;
            }

            using (new SPMonitoredScope("gradsovet - erAgendaQuestionItem Event Receiver - ItemDeleting"))
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

        /// <summary>
        /// TODO: Add comment for event ItemUpdating in erAgendaQuestionItem 
        /// </summary>
        /// <param name="properties">Contains list event properties</param>   
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;

            try
            {
                var oldTitle = getTitle(new QuestionTitleData()
                {
                    Number = properties.ListItem["AgendaQuestionNumber"]
                });
                var newTitle = getTitle(new QuestionTitleData()
                {
                    Number = properties.AfterProperties["AgendaQuestionNumber"]
                });

                if (oldTitle.Equals(newTitle)) return;

                var dsvc = SPDiagnosticsService.Local;
                var targetLib = properties.Web.Lists.Cast<SPList>().First(
                    l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) as SPDocumentLibrary;

                if (targetLib == null)
                {
                    dsvc.WriteEvent(0,
                        new SPDiagnosticsCategory(
                            Consts.EventLogCategory,
                            TraceSeverity.Monitorable,
                            EventSeverity.Warning),
                        EventSeverity.Error,
                        "Не существует экземпляра списка по шаблону {0}. Невозможно обновить структуру каталогов для хранения вложений",
                        new object[] {Consts.StorageListTemplateId});

                    return;
                }

                using (new SPMonitoredScope("gradsovet - erAgendaQuestionItem Event Receiver - ItemUpdating"))
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
                                    new object[] {properties.ListTitle, properties.ListItemId, targetLib.Title});

                                return;
                            }

                            try
                            {
                                var renameTo = SPUrlUtility.CombineUrl(dsItem.Folder.ParentFolder.Url, newTitle);
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
                                    new object[] {dsItem.ID, ex});
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
}

