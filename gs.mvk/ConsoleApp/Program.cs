using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using System.Configuration;

namespace ConsoleApp
{
    class Program
    {
        static void deleteList(SPWeb web, string listTitle)
        {
            var list = web.Lists.TryGetList(listTitle);
            if (list != null)
            {
                web.Lists.Delete(list.ID);
            }
        }

        static void deleteListsFromContentType(SPWeb web, string CTGuid)
        {
            SPContentType ct = web.AvailableContentTypes[new SPContentTypeId(CTGuid)];
            if (ct != null)
            {
                IList<SPContentTypeUsage> usages = SPContentTypeUsage.GetUsages(ct);
                foreach (SPContentTypeUsage item in usages)
                {
                    if (item.IsUrlToList)
                    {
                        SPList list = web.GetList(item.Url);
                        if (list != null)
                        {
                            web.Lists.Delete(list.ID);
                        }
                    }
                }
            }
        }

        private static string WebUrl
        {
            get
            {
                string url;
                try
                {
                    url = ConfigurationManager.AppSettings["WebUrl"] ?? "Not Found";
                }
                catch (Exception)
                {
                    Console.WriteLine("Error reading application settings file");
                    throw;
                }

                if (url.Equals("Not Found"))
                {
                    throw new Exception("Application setting [WebSiteUrl] could not be found");
                }

                return url;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                using (SPSite site = new SPSite(WebUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        deleteListsFromContentType(web, "0x0100C274D4A47E4742EEABF2D5FDF35DFBC9");
                        deleteListsFromContentType(web, "0x01009435E8C0C3E94107BD6F56DE758815D6");
                        deleteListsFromContentType(web, "0x01009E8BDC3C1146476781B2724822295453");
                        deleteListsFromContentType(web, "0x0100A3A87A1CEE124BB98A5563B3F2D5486C");
                        deleteListsFromContentType(web, "0x0100ED5F7CB6A1DD4E8D9D5163520BDDD6D0");
                        deleteListsFromContentType(web, "0x01000B0FC5BE72384B6E95B4FF3C32334D41");          //MeetingAttachmentMVK
                        deleteListsFromContentType(web, "0x0120D520002E09C03AA458425D969069CD2946780D");    //IssueAttachmentMVK
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception: " + ex.ToString());
            }
            Console.WriteLine("Press Enter...");
            Console.ReadLine();
        }
    }
}
