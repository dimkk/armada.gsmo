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

        static void Main(string[] args)
        {
            using (SPSite site = new SPSite("http://sp2013dev"))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    deleteList(web, "MVK issue");
                    deleteList(web, "MVK meeting");
                    deleteList(web, "MVK assignment");


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
