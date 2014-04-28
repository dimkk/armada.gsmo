using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace gs.meeting.Components
{
    internal static class EventReceiverExtensions
    {
        static System.Text.RegularExpressions.Regex tagRegex
                    = new System.Text.RegularExpressions.Regex("<[^>]*>", System.Text.RegularExpressions.RegexOptions.Compiled);

        public static bool IsFieldChanged(this SPItemEventProperties properties, string fieldName)
        {
            return properties.IsFieldChanged(properties.List.Fields.GetFieldByInternalName(fieldName));
        }

        public static bool IsFieldChanged(this SPItemEventProperties properties, SPField field)
        {
            var after = (string)properties.AfterProperties[field.InternalName];
            var before = Convert.ToString(properties.ListItem[field.Id]);


            //AfterProperties[fieldname] == null - field not changed
            if (after == null)
            {
                return false;
            }

            //AfterProperties[fieldname] == "" - field set to null
            if (after == "" && string.IsNullOrEmpty(before))
            {
                return false;
            }

            //AfterProperties[fieldname] != "", old value is null or empty - field changed
            if (string.IsNullOrEmpty(before))
            {
                return true;
            }

            var afterValue = field.GetFieldValue(after);
            var beforeValue = field.GetFieldValue(before);

            if (afterValue.Equals(beforeValue))
            {
                return false;
            }

            //Compare SPFieldLookupValue and SPFieldUserValue
            if (afterValue is SPFieldLookupValue)
            {
                return (afterValue as SPFieldLookupValue).LookupId != (beforeValue as SPFieldLookupValue).LookupId;
            }

            //Compare SPFieldMultiChoiceValue
            if (afterValue is SPFieldMultiChoiceValue)
            {
                return !EqualChoices(afterValue as SPFieldMultiChoiceValue, beforeValue as SPFieldMultiChoiceValue);
            }

            //Compare strings
            if (afterValue is string)
            {
                //normalize tags on rich edit fields
                if (field is SPFieldMultiLineText && (field as SPFieldMultiLineText).RichText)
                {
                    var a = tagRegex.Replace(afterValue as string, m => m.Value.ToLower());
                    var b = tagRegex.Replace(beforeValue as string, m => m.Value.ToLower());
                    return !a.Equals(b);
                }
            }

            //Compare SPFieldLookupValueCollection and SPFieldUserValueCollection
            if (field is SPFieldLookup && (field as SPFieldLookup).AllowMultipleValues)
            {
                var hsa = new HashSet<int>((afterValue as SPFieldLookupValueCollection).OfType<SPFieldLookupValue>().Select(l => l.LookupId));
                var hsb = new HashSet<int>((beforeValue as SPFieldLookupValueCollection).OfType<SPFieldLookupValue>().Select(l => l.LookupId));
                return !hsa.SetEquals(hsb);
            }

            return after != before;
        }

        private static bool EqualChoices(SPFieldMultiChoiceValue a, SPFieldMultiChoiceValue b)
        {
            if (a.Count != b.Count) return false;

            var hsa = new HashSet<string>();
            var hsb = new HashSet<string>();
            for (int i = 0; i < a.Count; i++)
            {
                hsa.Add(a[i]);
                hsb.Add(b[i]);
            }

            return hsa.SetEquals(hsb);
        }
    }
}
