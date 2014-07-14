using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Specialized;
using System.Web;

namespace GSWeb.WebParts
{
    public static class Extensions
    {
        public static string AddUrlParam(this string url, string name, string value)
        {
            string[] parts = url.Split('?');

            string root = parts[0];
            NameValueCollection query = HttpUtility.ParseQueryString(parts.Length > 1 ? parts[1] : string.Empty);
            query.Add(name, value);

            return root + "?" + query.ToString();
        }

        public static T ErrorHandler<T>(this Func<T> worker, Action<Exception> handler)
        {
            T result;
            try
            {
                result = worker();
            }
            catch (Exception ex)
            {
                result = default(T);
                handler(ex);
            }
            return result;
        }

        public static string GetFieldValue(this SPContext context, string fieldName)
        {
            SPFieldCalculated cf = context.Fields.GetFieldByInternalName(fieldName) as SPFieldCalculated;
            if (cf != null)
                return cf.GetFieldValueAsText(context.Item[fieldName]);
            return Convert.ToString(context.Item[fieldName]) ?? string.Empty;
        }

        public static SPList GetListByUrl(this SPWeb web, string url)
        {
            return web.GetList(SPUrlUtility.CombineUrl(web.Url, "Lists/" + url));
        }
    }
}
