using Microsoft.SharePoint;
using System;

namespace ITB.SP.Tools
{
    public static class ItemExtensions
    {
        public static string GetFieldValue(this SPListItem item, string fieldName)
        {
            SPField field = item.Fields.GetFieldByInternalName(fieldName);
            object value = item[fieldName];
            var cf = field as SPFieldCalculated;
            if (cf != null)
                return cf.GetFieldValueAsText(value);

            return Convert.ToString(value);
        }

        public static DateTime GetFieldValueDateTime(this SPListItem item, string fieldName)
        {
            return DateTime.Parse(GetFieldValue(item, fieldName));
        }

        public static int? GetFieldValueInt(this SPListItem item, string fieldName)
        {
            int result = 0;
            return int.TryParse(GetFieldValue(item, fieldName), out result) ? result : (int?) null;
        }

        public static SPFieldLookupValue GetFieldLookup(this SPListItem item, string fieldName)
        {
            return new SPFieldLookupValue(GetFieldValue(item, fieldName));
        }
    }
}
