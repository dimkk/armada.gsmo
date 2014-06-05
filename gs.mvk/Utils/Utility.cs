using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace gs.mvk.BusinessLogic.Utils
{
    public static class Utility
    {
        #region Resources
        private static readonly string resourceFilePathRelative             = @"gs.mvk\AssignmentListsStrings";
        private static readonly string resListNotFoundFmtErr                = "$Resources:ListNotFoundFmtErr";
        private static readonly string resSiteColumnAddFmtErr               = "$Resources:SiteColumnAddFmtErr";
        private static readonly string resContentTypeNotFoundFmtErr         = "$Resources:ContentTypeNotFoundFmtErr";
        private static readonly string resRemovedSiteColumnNotFoundFmtErr   = "$Resources:RemovedSiteColumnNotFoundFmtErr";

        public static string GetResString(this SPWeb web, string resourceName, string resourceFilePathRelative)
        {
            return SPUtility.GetLocalizedString(resourceName, resourceFilePathRelative, web.Language);
        }

        public static string GetResString(this SPWeb web, string resourceName)
        {
            return GetResString(web, resourceName, resourceFilePathRelative);
        }
        #endregion

        /// <summary>
        /// Ensures that the site column exists.
        /// </summary>
        /// <param name="web">Rootweb container of site columns</param>
        /// <param name="fieldname">Name of the site column</param>
        /// <param name="groupname">Group in wich the site column need to be created</param>
        /// <param name="fieldtype">Type of the sitecolumn</param>
        /// <param name="showInNewAndEditForm">Boolean determines whether to show the field in new and edit forms.</param>
        /// <returns></returns>
        public static SPField EnsureSiteColumn(this SPWeb web, string fieldname, string groupname, SPFieldType fieldtype, bool showInNewAndEditForm)
        {
            string fieldToAddName = "";
            SPField fieldToAdd;

            try
            {
                web.AllowUnsafeUpdates = true;
                //If the field doesn't exist yet add it.
                SPField fieldExists = null;
                if (web.Fields.ContainsField(fieldname))
                    fieldExists = web.Fields[fieldname];

                if (fieldExists == null)
                {
                    //Make the field on sitecollection level
                    fieldToAddName = web.Fields.Add(fieldname, fieldtype, false);
                    fieldToAdd = web.Fields.GetFieldByInternalName(fieldToAddName);
                    fieldToAdd.Group = groupname;
                    fieldToAdd.ShowInEditForm = showInNewAndEditForm;
                    fieldToAdd.ShowInNewForm = showInNewAndEditForm;
                    fieldToAdd.Update();
                }
                else
                {
                    fieldToAdd = fieldExists;
                }
            }
            finally
            {
                web.AllowUnsafeUpdates = false;
            }
            return fieldToAdd;
        }

        public static SPField EnsureSiteColumnXml(this SPWeb web, string fieldName, string fieldXml)
        {
            SPField addField = null;
            SPField existField = null;

            if (web.Fields.ContainsField(fieldName))
                existField = web.Fields[fieldName];

            if (existField == null)
            {
                string fName = web.Fields.AddFieldAsXml(fieldXml, false, SPAddFieldOptions.AddFieldInternalNameHint);
                addField = web.Fields.GetFieldByInternalName(fName);
                addField.Update();
            }
            else
            {
                addField = existField;
            }

            return addField;
        }

        /// <summary>
        /// Creates a link between the sitecolumn and the content type
        /// </summary>
        /// <param name="web">SPWeb rootweb.</param>
        /// <param name="contentType">Contenttype that needs to get a reference to the field</param>
        /// <param name="fieldToAdd">SPField that need to get linked to the contenttype</param>
        public static void LinkFieldToContentType(this SPWeb web, SPContentType contentType, SPField fieldToAdd)
        {
            Guid rootwebId = web.Site.RootWeb.ID;
            SPContentTypeId contentTypeId = contentType.Id;
            using (SPWeb rootweb = web.Site.OpenWeb(rootwebId))
            {
                rootweb.AllowUnsafeUpdates = true;
                try
                {
                    SPContentType currentContent = rootweb.ContentTypes[contentTypeId];
                    bool linkExists = false;
                    foreach (SPFieldLink link in currentContent.FieldLinks)
                    {
                        if (link.DisplayName == fieldToAdd.Title)
                            linkExists = true;
                    }

                    if (!linkExists)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Content Type Link between {0} and {1} was not found, adding..", contentType.Name, fieldToAdd.Title));
                        //If the field is not yet linked to Content Type create a new fieldlink and add it to the content type
                        SPFieldLink fieldLink = new SPFieldLink(fieldToAdd);
                        fieldLink.DisplayName = fieldToAdd.Title;
                        currentContent.FieldLinks.Add(fieldLink);
                        currentContent.Update(true);
                    }
                }

                finally
                {
                    rootweb.AllowUnsafeUpdates = false;
                }

            }
        }

        /// <summary>
        /// Retrieves a site column returns null if no site column is found.
        /// </summary>
        /// <param name="web">SPWeb object pointing to one of the subwebs in the site collection holding the site column</param>
        /// <param name="fieldname">String name of the site column to retrieve</param>
        /// <returns>SPField or Null</returns>
        public static SPField GetSiteColumn(this SPWeb web, string fieldname)
        {
            SPField resultField = null;
            Guid rootWebId = web.Site.RootWeb.ID;
            using (SPWeb rootweb = web.Site.OpenWeb(rootWebId))
            {
                if (web.Fields.ContainsField(fieldname))
                    resultField = web.Fields[fieldname];
            }
            return resultField;
        }

        /// <summary>
        /// Removes the Site Column from the sitecollection
        /// It will do so even if the site column is marked as hidden/readonly/no deletion allowed and used in lists content types etc.
        /// </summary>
        /// <param name="web">Rootweb containing the sitecollumns</param>
        /// <param name="fieldname">Name of the site column that needs to be removed.</param>
        /// <returns></returns>
        public static bool RemoveSiteColumn(this SPSite site, string fieldNameRes)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            if (site.RootWeb == null)
                throw new Exception("Null root web in site: " + site.ToString());

            bool isRemoved = false;

            using (SPWeb web = site.RootWeb)
            {
                try
                {
                    string fieldName = web.GetResString(fieldNameRes);

                    SPField field = web.GetSiteColumn(fieldName);

                    if (field == null)
                        Debug(resRemovedSiteColumnNotFoundFmtErr, web, fieldName);

                    if (field != null)
                    {
                        web.AllowUnsafeUpdates = true;
                        if (field.ReadOnlyField)
                        { field.ReadOnlyField = false; field.Update(true); }

                        if (field.Hidden)
                        { field.Hidden = false; field.Update(true); }

                        if (field.AllowDeletion == null || !field.AllowDeletion.Value)
                        { field.AllowDeletion = true; field.Update(true); }

                        web.DeleteColumn(field);
                        isRemoved = true;
                    }
                }
                finally
                {
                    web.AllowUnsafeUpdates = false;
                }
            }
            return isRemoved;
        }

        private static void DeleteColumn(this SPWeb web, SPField field)
        {
            Guid webId = web.ID;
            Guid siteId = web.Site.ID;

            using (SPSite _site = new SPSite(siteId))
            {
                using (SPWeb _web = _site.OpenWeb(webId))
                {
                    ICollection<SPFieldTemplateUsage> collection = field.ListsFieldUsedIn();
                    foreach (SPFieldTemplateUsage usage in collection)
                    {
                        SPWeb webSite = _site.AllWebs[usage.WebID];
                        SPList list = webSite.Lists[usage.ListID];

                        if (list.ContentTypesEnabled)
                        {
                            SPContentTypeCollection listContentTypes = list.ContentTypes;
                            foreach (SPContentType contentType in listContentTypes)
                            {
                                if (contentType.Fields.ContainsField(field.InternalName))
                                {
                                    contentType.FieldLinks.Delete(field.InternalName);
                                    contentType.Update();
                                }
                            }
                        }
                        if (list.Fields.ContainsField(field.InternalName))
                        {
                            SPField fieldInstance = list.Fields.GetFieldByInternalName(field.InternalName);
                            if (fieldInstance.AllowDeletion == null || !fieldInstance.AllowDeletion.Value)
                            {
                                fieldInstance.AllowDeletion = true;
                                fieldInstance.Update();
                            }
                            list.Fields.Delete(field.InternalName);
                        }
                        webSite.Dispose();
                    }


                    foreach (SPContentType contentType in _web.ContentTypes)
                    {
                        if ((contentType.Fields.ContainsField(field.InternalName)) && !contentType.Sealed)
                        {
                            contentType.FieldLinks.Delete(field.InternalName);
                            contentType.Update();
                        }
                    }
                    field.Delete();
                }
            }
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
        public static void AddLookupField(this SPSite site, string fieldContentTypeName, string fieldGuid, string fieldName, string fieldGroupNameRes, string fieldDisplayNameRes, string fieldDescriptionRes, string targetShowFieldName, string targetLookupListRelativeUrl)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            if (site.RootWeb == null)
                throw new Exception("Null root web in site: " + site.ToString());

            using (SPWeb web = site.RootWeb)
            {
                SPList lookupList = web.GetLookupList(targetLookupListRelativeUrl);
                if (lookupList == null)
                    return;

                SPField field = web.AddLookupField(fieldGuid, fieldName, fieldGroupNameRes, fieldDisplayNameRes, fieldDescriptionRes, targetShowFieldName, lookupList);
                if (field == null)
                    return;

                SPContentType ct = web.ContentTypes[fieldContentTypeName];
                if (ct != null)
                    web.LinkFieldToContentType(ct, field);
                else
                    Debug(resContentTypeNotFoundFmtErr, web, fieldContentTypeName);
            }
        }

        private static SPList GetLookupList(this SPWeb web, string listRelativeUrl)
        {
            SPList retVal = null;

            try
            {
                retVal = web.GetList(listRelativeUrl);
            }
            catch (FileNotFoundException)
            {
                Debug(resListNotFoundFmtErr, web, listRelativeUrl, web.Title);
            }

            return retVal;
        }

        private static SPField AddLookupField(this SPWeb web, string fieldGuid, string fieldName, string fieldGroupNameRes, string fieldDisplayNameRes, string fieldDescriptionRes, string targetShowFieldName, SPList targetLookupList)
        {
            string groupName = web.GetResString(fieldGroupNameRes);
            string displayName = web.GetResString(fieldDisplayNameRes);
            string description = web.GetResString(fieldDescriptionRes);

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
                new XAttribute("AllowDeletion", "False")
                );

            SPField retVal = web.EnsureSiteColumnXml(displayName, field.ToString());
            if (retVal == null)
                Debug(resSiteColumnAddFmtErr, web, fieldName);

            return retVal;
        }

        #region Debug
        public static void Debug(string resFormatMessage, SPWeb web, params string[] values)
        {
            System.Diagnostics.Debug.WriteLine(web.GetResString(resFormatMessage), values);
        }
        #endregion
    }
}
