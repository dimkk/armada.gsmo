using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace ITB.SP.Tools
{
    public static class Extensions
    {
        public static string AddUrlParam(this string url, string name, string value)
        {
            string[] parts = url.Split('?');

            string root = parts[0];
            NameValueCollection query = HttpUtility.ParseQueryString(parts.Length > 1 ? parts[1] : string.Empty);
            query.Add(name, value);

            return String.Concat(root, "?", query);
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

        public static string ToString(this AggregateException e, string message)
        {
            var s = new StringBuilder();
            s.AppendFormat("{0}: {1}{3}{2}{3}{3}", e.GetType(), message, e.StackTrace, Environment.NewLine);
            int i = 1;
            foreach (var ie in e.InnerExceptions)
            {
                s.AppendFormat("<--Begin Inner Exception #{0}-->{1}", i, Environment.NewLine);
                s.AppendLine(ie.ToString());
                s.AppendFormat("<--End Inner Exception #{0}-->{1}{1}", i++, Environment.NewLine);
            }
            return s.ToString();
        }

    }
}
