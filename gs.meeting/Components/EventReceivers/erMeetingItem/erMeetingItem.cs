// <copyright file="erMeetingItem.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-02-13 15:50:14Z</date>
namespace gs.meeting.Components
{
    using System;
    using System.Security.Permissions;
    using System.Linq;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.Security;
    using Microsoft.SharePoint.Utilities;
    using Microsoft.Office.Server.Utilities;


    internal struct MeetingTitleData
    {
        public object Number;
        public object Date;
    }

    /// <summary>
    /// TODO: Add comment for erMeetingItem
    /// </summary>
    public class erMeetingItem : SPItemEventReceiver
    {
        internal static string getTitle(MeetingTitleData data)
        {
            var number = data.Number == null ? "неизвестно" : data.Number.ToString();
            string dateStr;

            if (data.Date == null)
            {
                dateStr = "неизвестно";
            }
            else
            {
                if (data.Date is DateTime)
                {
                    dateStr = ((DateTime) data.Date).Date.ToShortDateString();
                }
                else
                {
                    dateStr = data.Date.ToString();
                    DateTime parsedDate;

                    dateStr = DateTime.TryParse(dateStr, out parsedDate)
                        ? parsedDate.Date.ToShortDateString()
                        : "неизвестно";
                }
            }

            return String.Format("Заседание №{0} от {1}", number, dateStr);
        }

        /// <summary>
        /// TODO: Add comment for event ItemAdded in erMeetingItem 
        /// </summary>
        /// <param name="properties">Contains list event properties</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            var title = getTitle(new MeetingTitleData() { 
                Number = properties.ListItem["MeetingNumber"], 
                Date = properties.ListItem["MeetingDate"] 
            });
            var dsvc = SPDiagnosticsService.Local;

            if (properties.Web.Lists.Cast<SPList>().First(
                l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) == null)
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

            using (new SPMonitoredScope("gradsovet - erMeetingItem Event Receiver - ItemAdded"))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (var site = new SPSite(properties.SiteId))
                    using (var web = site.OpenWeb(properties.RelativeWebUrl))
                    {
                        var tLib = web.Lists.Cast<SPList>().First(
                            l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) as SPDocumentLibrary;

                        if (tLib == null) return;
                        SPFolderHierarchy.GetSubFolder(tLib.RootFolder, title, true);
                    }
                });
            }
        }

        /// <summary>
        /// TODO: Add comment for event ItemDeleting in ItemEventReceiver1 
        /// </summary>
        /// <param name="properties">Contains list event properties</param>
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            var title = getTitle(new MeetingTitleData()
            {
                Number = properties.ListItem["MeetingNumber"],
                Date = properties.ListItem["MeetingDate"]
            });
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

            using (new SPMonitoredScope("gradsovet - erMeetingItem Event Receiver - ItemDeleting"))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (var site = new SPSite(properties.SiteId))
                    using (var web = site.OpenWeb(properties.RelativeWebUrl))
                    {
                        var tLib = web.Lists.Cast<SPList>().First(
                            l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) as SPDocumentLibrary;

                        if (tLib == null) return;

                        var folder = SPFolderHierarchy.GetSubFolder(tLib.RootFolder, title, false);
                        if (folder != null && folder.Exists)
                        {
                            try
                            {
                                var parentFolder = folder.ParentFolder;
                                folder.Delete();
                                parentFolder.Update();
                            }
                            catch (Exception ex)
                            {
                                dsvc.WriteTrace(0,
                                    new SPDiagnosticsCategory(
                                        Consts.TraceLogCategory,
                                        TraceSeverity.Unexpected,
                                        EventSeverity.Error),
                                    TraceSeverity.Unexpected,
                                    "Не удалось удалить папку документов с Id {0}: {1}",
                                    new object[] { folder.Item.ID, ex });
                                throw;
                            }
                        }
                        else
                        {
                            dsvc.WriteEvent(0,
                                new SPDiagnosticsCategory(
                                    Consts.EventLogCategory,
                                    TraceSeverity.Monitorable,
                                    EventSeverity.Warning),
                                EventSeverity.Warning,
                                "Не удалось определить папку документов в списке {2}, которая бы соответствовала элементу списка {0} с Id {1}",
                                new object[] { properties.ListTitle, properties.ListItemId, targetLib.Title });
                        }
                    }
                });
            }
        }

        /// <summary>
        /// TODO: Add comment for event ItemUpdating in ItemEventReceiver1 
        /// </summary>
        /// <param name="properties">Contains list event properties</param>   
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;

            try
            {
                if (!properties.IsFieldChanged("MeetingNumber") && !properties.IsFieldChanged("MeetingDate")) return;

                var dsvc = SPDiagnosticsService.Local;
                if (properties.Web.Lists.Cast<SPList>().First(
                    l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) == null)
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

                var oldTitle = getTitle(new MeetingTitleData()
                {
                    Number = properties.ListItem["MeetingNumber"],
                    Date = properties.ListItem["MeetingDate"]
                });
                var newTitle = getTitle(new MeetingTitleData()
                {
                    Number = properties.AfterProperties["MeetingNumber"],
                    Date = properties.AfterProperties["MeetingDate"]
                });

                using (new SPMonitoredScope("gradsovet - erMeetingItem Event Receiver - ItemUpdating"))
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (var site = new SPSite(properties.SiteId))
                        using (var web = site.OpenWeb(properties.RelativeWebUrl))
                        {
                            var tLib = web.Lists.Cast<SPList>().First(
                                l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) as SPDocumentLibrary;

                            if (tLib == null) return;

                            var folder = SPFolderHierarchy.GetSubFolder(tLib.RootFolder, oldTitle, false);
                            if (folder != null && folder.Exists)
                            {
                                try
                                {
                                    var renameTo = SPUrlUtility.CombineUrl(tLib.RootFolder.Url, newTitle);
                                    folder.MoveTo(renameTo);
                                }
                                catch (Exception ex)
                                {
                                    dsvc.WriteTrace(0,
                                        new SPDiagnosticsCategory(
                                            Consts.TraceLogCategory,
                                            TraceSeverity.Unexpected,
                                            EventSeverity.Error),
                                        TraceSeverity.Unexpected,
                                        "Не удалось переименовать папку документов с Id {0}: {1}",
                                        new object[] { folder.Item.ID, ex });
                                    throw;
                                }
                            }
                            else
                            {
                                SPFolderHierarchy.GetSubFolder(tLib.RootFolder, newTitle, true);
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

