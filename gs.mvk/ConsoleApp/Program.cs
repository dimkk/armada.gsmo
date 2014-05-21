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
        static void Main(string[] args)
        {
            using (SPSite site = new SPSite("http://sp2013dev"))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPContentType ct = web.ContentTypes[new SPContentTypeId("0x0100C274D4A47E4742EEABF2D5FDF35DFBC9")];
                    IEnumerable<SPContentTypeUsage> u = SPContentTypeUsage.GetUsages(ct);

                    var a = u.Count<SPContentTypeUsage>();
                    Console.WriteLine(String.Format("Content Type usages count: {0}", a));
                    Console.ReadKey();
                }
            }
        }
    }
}
