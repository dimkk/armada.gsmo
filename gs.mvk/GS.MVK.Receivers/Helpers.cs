using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.MVK.Receivers
{
    internal static class Helpers
    {
        private static int IncorrectId { get { return -1; } }

        internal static string CamlQueryByParent(string parentFieldName, object parentValue)
        {
            return @"<Where>
                        <Eq>
                            <FieldRef Name='" + parentFieldName + @"' LookupId='TRUE' />
                            <Value Type='Lookup'>" + parentValue.ToString() + @"</Value>
                        </Eq>
                    </Where>";
        }

        internal static string CamlQueryById(int id)
        {
            return @"<Where>
                        <Eq>
                            <FieldRef Name='ID' />
                            <Value Type='Integer'>" + id + @"</Value>
                        </Eq>
                    </Where>";
        }

        public static int getFieldLookupId(SPListItem item, string fieldName)
        {
            int res = IncorrectId;

            if ((item != null) && (fieldName != String.Empty))
            {
                SPFieldLookup field = item.Fields.GetField(fieldName) as SPFieldLookup;
                if (field != null)
                {
                    SPFieldLookupValue value = field.GetFieldValue(item[fieldName].ToString()) as SPFieldLookupValue;
                    if (value != null)
                    {
                        res = value.LookupId;
                    }
                }
            }

            return res;
        }

        public static SPListItemCollection getParticipantsByPositionId(int id)
        {
            return getChildItems("ParticipantPositionLink", id, "Lists/ParticipantBookList");
        }

        public static SPListItemCollection getParticipantsByOrgId(int id)
        {
            return getChildItems("ParticipantOrgLink", id, "Lists/ParticipantBookList");
        }

        public static SPListItem getParticipantById(int id)
        {
            return getItemById("Lists/ParticipantBookList", id);
        }

        internal static SPListItemCollection getChildItems(string parentFieldName, object parentId, string childListTitle)
        {
            string caml = CamlQueryByParent(parentFieldName, parentId);

            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists.TryGetList(childListTitle);
            if (list == null)
                throw new Exception(String.Format("Не удалось получить список {0}", childListTitle));

            return list.GetItems(new SPQuery() { Query = caml });
        }

        internal static SPListItem getItemById(string listTitle, int id)
        {
            string caml = CamlQueryById(id);

            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists.TryGetList(listTitle);
            if (list == null)
                throw new Exception(String.Format("Не удалось получить список {0}", listTitle));

            return list.GetItems(new SPQuery() { Query = caml }).Cast<SPListItem>().First();
        }

        internal static List<string> splitMultiChoiceValue(object value)
        {
            List<string> retVal = new List<string>();

            if (value != null)
            {
                SPFieldMultiChoiceValue mcValue = new SPFieldMultiChoiceValue(value.ToString());
                for (int i = 0; i < mcValue.Count; i++)
                {
                    retVal.Add(mcValue[i]);
                }
            }

            return retVal;
        }

        public static List<SPListItem> getAssignmentExecutors(SPListItem assignment)
        {
            List<SPListItem> retVal = new List<SPListItem>();

            if (assignment["AssignmentExecutorFullNameLink"] != null)
            {
                SPListItem item =
                    getItemById("ParticipantBookList", getFieldLookupId(assignment, "AssignmentExecutorFullNameLink"));
                if (item != null)
                    retVal.Add(item);
            }

            if (assignment["AssignmentExecutorPositionLink"] != null)
            {
                SPListItemCollection items =
                    getParticipantsByPositionId(getFieldLookupId(assignment, "AssignmentExecutorPositionLink"));

                foreach (SPListItem item in items)
                {
                    retVal.Add(item);
                }
            }

            if (assignment["AssignmentExecutorOrganizationLink"] != null)
            {
                SPListItemCollection items =
                    getParticipantsByOrgId(getFieldLookupId(assignment, "AssignmentExecutorOrganizationLink"));

                foreach (SPListItem item in items)
                {
                    retVal.Add(item);
                }
            }

            return retVal;
        }
    }
}
