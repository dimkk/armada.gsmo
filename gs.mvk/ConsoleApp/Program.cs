using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

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

        static void Main(string[] args)
        {
            using (SPSite site = new SPSite("http://win-3efppqrbp9t"))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    deleteListsFromContentType(web, "0x0100C274D4A47E4742EEABF2D5FDF35DFBC9");
                    deleteListsFromContentType(web, "0x01009435E8C0C3E94107BD6F56DE758815D6");
                    deleteListsFromContentType(web, "0x01009E8BDC3C1146476781B2724822295453");
                    
                    /*SPContentType ct = web.ContentTypes[new SPContentTypeId("0x0100C274D4A47E4742EEABF2D5FDF35DFBC9")];
                    IEnumerable<SPContentTypeUsage> u = SPContentTypeUsage.GetUsages(ct);

                    var a = u.Count<SPContentTypeUsage>();
                    Console.WriteLine(String.Format("Content Type usages count: {0}", a));
                    Console.ReadKey();*/
                }
            }
        }
    }
}
