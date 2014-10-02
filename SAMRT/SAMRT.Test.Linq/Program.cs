using System;
using System.IO;
using System.Linq;
using Microsoft.SharePoint;
using ITB.SP.Tools;
using System.Collections.Generic;
using SAMRT.Common.BL;

namespace SAMRT.Test.Linq
{
    class Program
    {
        protected static readonly string OrderRgListName = "OrderMVKList";
        protected static readonly string OrderRgIssueRgCountFieldName = "OrderIssueRgCount";

        static void Main(string[] args)
        {
            SivgkService.InformationLinkPortTypeClient client = new SivgkService.InformationLinkPortTypeClient();
            //client.get()

            SPSite site = new SPSite("http://gs.msk.mosreg.ru/sites/gca/");
            SPWeb web = site.RootWeb;

            SPList issueList = web.GetListByUrl("IssueMVKList");
            SPQuery query = new SPQuery();
            query.Query =
            string.Format(
                "<Where><Eq><FieldRef Name='{0}' LookupId='TRUE'/><Value Type='Lookup'>{1}</Value></Eq></Where><OrderBy><FieldRef Name='IssueNumberMVK' Ascending='FALSE'/></OrderBy>",
                "IssueMeetingMVK", 51);
            query.RowLimit = 5;

            IEnumerable<SPListItem> issues = issueList.GetItems(query).Cast<SPListItem>();

            string[] orderRgIdList = {"1","2"};
            MeetingRg.CreateIssuesRgFromOrdersRg(web, 50, orderRgIdList.Select(s => Convert.ToInt32(s)));




            using (StreamWriter sw = new StreamWriter("linq.log", false))
            using (SAMRTDataContext dc = new SAMRTDataContext("http://gs.msk.mosreg.ru/sites/gca"))
            {
                dc.Log = sw;
                var max = dc.АКВопросыЗаседания.Where(i => i.Заседание.Id.Value == 81).Max(m => m.НомерВопроса.Value);
                Console.WriteLine("Press Enter...");
                Console.ReadLine();
            }
        }
    }
}
