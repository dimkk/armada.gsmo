using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using System;
using System.Linq;
using System.Security.Permissions;

namespace GS.MVK.Receivers
{
    /// <summary>
    /// TODO: Add comment for MeetingItem
    /// </summary>
    public class MeetingItem : SPItemEventReceiver
    {
        private readonly string storageListTemplateId = "10056";
        private readonly string dateFieldName = "MeetingDateMVK";
        private readonly string numberFieldName = "MeetingNumberMVK";

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
                    dateStr = ((DateTime)data.Date).Date.ToShortDateString();
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
            var title = getTitle(new MeetingTitleData()
            {
                Number = properties.ListItem[numberFieldName],
                Date = properties.ListItem[dateFieldName]
            });
            var dsvc = SPDiagnosticsService.Local;

            if (properties.Web.Lists.Cast<SPList>().First(
                l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) == null)
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

            using (new SPMonitoredScope("GS.MVK.MeetingItemAdded"))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (var site = new SPSite(properties.SiteId))
                    using (var web = site.OpenWeb(properties.RelativeWebUrl))
                    {
                        var tLib = web.Lists.Cast<SPList>().First(
                            l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) as SPDocumentLibrary;

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
                Number = properties.ListItem[numberFieldName],
                Date = properties.ListItem[dateFieldName]
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

            using (new SPMonitoredScope("GS.MVK.MeetingItemDeleting"))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (var site = new SPSite(properties.SiteId))
                    using (var web = site.OpenWeb(properties.RelativeWebUrl))
                    {
                        var tLib = web.Lists.Cast<SPList>().First(
                            l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) as SPDocumentLibrary;

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
                if (!properties.IsFieldChanged(numberFieldName) && !properties.IsFieldChanged(dateFieldName)) return;

                var dsvc = SPDiagnosticsService.Local;
                if (properties.Web.Lists.Cast<SPList>().First(
                    l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) == null)
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

                var oldTitle = getTitle(new MeetingTitleData()
                {
                    Number = properties.ListItem[numberFieldName],
                    Date = properties.ListItem[dateFieldName]
                });
                var newTitle = getTitle(new MeetingTitleData()
                {
                    Number = properties.AfterProperties[numberFieldName],
                    Date = properties.AfterProperties[dateFieldName]
                });

                using (new SPMonitoredScope("GS.MVK.MeetingItemUpdating"))
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (var site = new SPSite(properties.SiteId))
                        using (var web = site.OpenWeb(properties.RelativeWebUrl))
                        {
                            var tLib = web.Lists.Cast<SPList>().First(
                                l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) as SPDocumentLibrary;

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

    internal struct MeetingTitleData
    {
        public object Number;
        public object Date;
    }

    internal static class Consts
    {
        public static string EventLogCategory { get { return "GS.MVK Event Receivers (document sets)"; } }
        public static string TraceLogCategory { get { return "GS.MVK Event Receivers (document sets)"; } }
    }
}

