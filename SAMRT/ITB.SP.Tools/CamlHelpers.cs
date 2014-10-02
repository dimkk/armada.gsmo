using System.Collections.Generic;
using System.Xml.Linq;

namespace ITB.SP.Tools
{
    public static class CamlHelpers
    {
        public static string ToCamlLookupIdNotIn(this IEnumerable<int> items, string lookupFieldName)
        {
            IEnumerator<int> enumerator = items.GetEnumerator();
            if (!enumerator.MoveNext())
                return string.Empty;

            return ToCamlLookupIdNotIn(enumerator, lookupFieldName).ToString();
        }

        const string NeqLookupTemplate = "<Neq><FieldRef Name='{0}' LookupId='TRUE'/><Value Type='Lookup'>{1}</Value></Neq>";
        private static XElement ToCamlLookupIdNotIn(IEnumerator<int> enumerator, string lookupFieldName)
        {
            XElement current = XElement.Parse(string.Format(NeqLookupTemplate, lookupFieldName, enumerator.Current));
            if (enumerator.MoveNext())
            {
                var and = new XElement("And");
                and.Add(current);
                and.Add(ToCamlLookupIdNotIn(enumerator, lookupFieldName));
                return and;
            }
            else
                return current;
        }
    }
}
