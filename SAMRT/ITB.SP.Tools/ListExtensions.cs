using Microsoft.SharePoint;
using System.Collections.Generic;
using System.Linq;

namespace ITB.SP.Tools
{
    public static class ListExtensions
    {
        public static IEnumerable<SPListItem> GetItemsByLookupId(this SPList list, string lookupFieldName, int lookupId)
        {
            var query = new SPQuery()
            {
                Query = string.Format(@"<Where>" +
                                            "<Eq>" +
                                                "<FieldRef Name='{0}' LookupId='TRUE' />" +
                                                "<Value Type='Lookup'>{1}</Value>" +
                                            "</Eq>" +
                                        "</Where>", lookupFieldName, lookupId)
            };
            return list.GetItems(query).Cast<SPListItem>();
        }
    }
}
