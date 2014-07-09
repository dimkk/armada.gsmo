using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GS.MVK.Utils
{
    public static class Utility
    {
        #region Resources
        private static readonly string resourceFilePathRelative             = @"GS.MVK\Common";
        private static readonly string resListNotFoundFmtErr                = "ListNotFoundFmtErr";
        private static readonly string resSiteColumnAddFmtErr               = "SiteColumnAddFmtErr";
        private static readonly string resContentTypeNotFoundFmtErr         = "ContentTypeNotFoundFmtErr";
        private static readonly string resRemovedSiteColumnNotFoundFmtErr   = "RemovedSiteColumnNotFoundFmtErr";

        public static string GetResString(this SPWeb web, string featureId, string resourceName)
        {
            return SPUtility.GetLocalizedString(string.Format("$Resources:_FeatureId{0},{1}", featureId, resourceName), string.Empty, web.Language);
        }

        public static string GetResString(this SPWeb web, string resourceName)
        {
            return SPUtility.GetLocalizedString("$Resources:" + resourceName, resourceFilePathRelative, web.Language);
        }
        #endregion

        public static SPField AddField(this SPWeb web, string fieldName, string fieldXml)
        {
            SPField field = null;
            try
            {
                web.Fields.AddFieldAsXml(fieldXml, false, SPAddFieldOptions.AddFieldInternalNameHint);
                field = web.Fields.GetFieldByInternalName(fieldName);
                field.Update();
            }
            catch (Exception ex)
            {
                Error(resSiteColumnAddFmtErr, web, fieldName, ex.ToString());
            }
            return field;
        }

        public static void AddExistField(this SPSite site, string contentTypeName, string fieldName)
        {
            using (SPWeb web = site.RootWeb)
            {
                web.AddExistField(contentTypeName, fieldName);
            }
        }

        public static void AddExistField(this SPWeb web, string contentTypeName, string fieldName)
        {
            SPField field = web.Fields.GetFieldByInternalName(fieldName);
            web.AddExistField(contentTypeName, field);
        }

        public static void AddExistField(this SPWeb web, string contentTypeName, SPField field)
        {
            SPContentType contentType = web.GetContentType(contentTypeName);
            SPFieldLink fieldLink = new SPFieldLink(field);
            contentType.FieldLinks.Add(fieldLink);
            contentType.Update(true);
        }

        /// <summary>
        /// Добавляет столбец типа Lookup в коллекцию сайтов и связывает его с требуемым типом содержимого
        /// </summary>
        /// <param name="site">Коллекция сайтов</param>
        /// <param name="fieldContentTypeName">Наименование типа содержимого</param>
        /// <param name="fieldGuid">Идентификатор добавляемого столбца</param>
        /// <param name="fieldName">Внутреннее имя добавляемого столбца</param>
        /// <param name="fieldGroupNameRes">Имя ресурса с наименованием группы столбцов, куда будет добавляться новый столбец</param>
        /// <param name="fieldDisplayNameRes">Имя ресурса с отображаемым именем добавляемого столбца</param>
        /// <param name="fieldDescriptionRes">Имя ресурса с описанием добавляемого столбца</param>
        /// <param name="targetShowFieldName">Наименование столбца из сущности Lookup-листа для отображения</param>
        /// <param name="targetLookupListRelativeUrl">Относительный путь к Lookup-листу</param>
        /// <returns></returns>
        public static void AddLookupField(this SPSite site, string parentFeatureId, string fieldContentTypeName, string fieldGuid, string fieldName, string fieldGroupNameRes, string fieldDisplayNameRes, string fieldDescriptionRes, string targetShowFieldName, string targetLookupListRelativeUrl)
        {
            using (SPWeb web = site.RootWeb)
            {
                SPList lookupList = web.GetListByUrl(targetLookupListRelativeUrl);
                SPField field = web.AddLookupField(parentFeatureId, fieldGuid, fieldName, fieldGroupNameRes, fieldDisplayNameRes, fieldDescriptionRes, targetShowFieldName, lookupList);
                web.AddExistField(fieldContentTypeName, field);
            }
        }

        private static SPField AddLookupField(this SPWeb web, string parentFeatureId, string fieldGuid, string fieldName, string fieldGroupNameRes, string fieldDisplayNameRes, string fieldDescriptionRes, string targetShowFieldName, SPList targetLookupList)
        {
            string groupName = web.GetResString(parentFeatureId, fieldGroupNameRes);
            string displayName = web.GetResString(parentFeatureId, fieldDisplayNameRes);
            string description = web.GetResString(parentFeatureId, fieldDescriptionRes);

            XElement field = new XElement("Field",
                new XAttribute("Group", groupName),
                new XAttribute("DisplayName", displayName),
                new XAttribute("ID", fieldGuid),
                new XAttribute("SourceID", "http://schemas.microsoft.com/sharepoint/v3"),
                new XAttribute("StaticName", fieldName),
                new XAttribute("Name", fieldName),
                new XAttribute("Type", "Lookup"),
                new XAttribute("List", "{" + targetLookupList.ID + "}"),
                new XAttribute("ShowField", targetShowFieldName),
                new XAttribute("Mult", "False"),
                new XAttribute("UnlimitedLengthInDocumentLibrary", "False"),
                new XAttribute("Description", description),
                new XAttribute("Overwrite", "True"),
                new XAttribute("AllowDeletion", "FALSE")
                );

            return web.AddField(fieldName, field.ToString());
        }

        public static SPContentType GetContentType(this SPWeb web, string contentTypeName)
        {
            SPContentType contentType = web.ContentTypes[contentTypeName];
            if (contentType == null)
                Error(resContentTypeNotFoundFmtErr, web, contentTypeName);

            return contentType;
        }

        public static SPList GetListByUrl(this SPWeb web, string url)
        {
            try
            {
                return web.GetList(url);
            }
            catch (FileNotFoundException)
            {
                Error(resListNotFoundFmtErr, web, url);
            }
            return null;
        }

        /// <summary>
        /// Removes the Site Column from the Site Collection
        /// </summary>
        /// <param name="site">Target Site Collection</param>
        /// <param name="fieldName">Name of the site column that needs to be removed.</param>
        /// <returns></returns>
        public static void DeleteField(this SPSite site, string fieldName)
        {
            using (SPWeb web = site.RootWeb)
            {
                foreach (SPContentType contentType in web.ContentTypes.Cast<SPContentType>().Where(s => s.Fields.ContainsField(fieldName) && !s.Sealed))
                {
                    contentType.FieldLinks.Delete(fieldName);
                    contentType.Update();
                }

                if (web.Fields.ContainsField(fieldName))
                {
                    SPField field = web.Fields.GetFieldByInternalName(fieldName);
                    if (field.AllowDeletion.HasValue && !field.AllowDeletion.Value)
                    {
                        field.AllowDeletion = true;
                        field.Update();
                    }
                    field.Delete();
                }
                else
                    Warning(resRemovedSiteColumnNotFoundFmtErr, web, fieldName);
            }
        }

        #region Log
        private static SPDiagnosticsCategory logCategory = SPDiagnosticsService.Local.Areas["SharePoint Foundation"].Categories["Feature Infrastructure"];

        public static void Error(string resFormatMessage, SPWeb web, params string[] values)
        {
            Event(resFormatMessage, web, EventSeverity.Error, values);
            throw new Exception(string.Format(web.GetResString(resFormatMessage), values));
        }
        public static void Warning(string resFormatMessage, SPWeb web, params string[] values)
        {
            Event(resFormatMessage, web, EventSeverity.Warning, values);
        }
        public static void Event(string resFormatMessage, SPWeb web, EventSeverity level, params string[] values)
        {
            SPDiagnosticsService.Local.WriteEvent(1, logCategory, level, web.GetResString(resFormatMessage), values);
        }
        #endregion
    }
}
