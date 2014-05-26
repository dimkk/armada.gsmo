using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace DeleteColumnApp
{
    class Program
    {
        public static SPField GetSiteColumn(SPWeb web, string fieldname)
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

        private static void DeleteColumn(SPField field, SPWeb web)
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
                            list.Fields.Delete(field.InternalName);
                        }
                        webSite.Dispose();
                    }


                    /*foreach (SPContentType contentType in _web.ContentTypes)
                    {
                        if (contentType.Fields.ContainsField(field.InternalName))
                        {
                            contentType.FieldLinks.Delete(field.InternalName);
                            contentType.Update();
                        }
                    }*/
                    field.Delete();
                }
            }
        }

        public static bool RemoveSiteColum(SPWeb web, string fieldname)
        {
            bool isRemoved = false;
            try
            {
                SPField field;
                field = GetSiteColumn(web, fieldname);

                if (field == null)
                    System.Diagnostics.Debug.WriteLine(String.Format("SiteColumn field with name:{0} not found and thus cannot be removed", fieldname));

                if (field != null)
                {
                    web.AllowUnsafeUpdates = true;
                    if (field.ReadOnlyField)
                    { field.ReadOnlyField = false; field.Update(); }

                    if (field.Hidden)
                    { field.Hidden = false; field.Update(); }

                    if (field.AllowDeletion == null || !field.AllowDeletion.Value)
                    { field.AllowDeletion = true; field.Update(); }

                    DeleteColumn(field, web);
                    isRemoved = true;
                }
            }
            finally
            {
                web.AllowUnsafeUpdates = false;
            }
            return isRemoved;
        }

        static void Main(string[] args)
        {
            using (SPSite site = new SPSite("http://win-3efppqrbp9t"))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    //RemoveSiteColum(web, "AssignmentDependentAssignment");
                    RemoveSiteColum(web, "$Resources:AssignmentDependentAssignmentDispName;");
                }
            }
        }
    }
}
