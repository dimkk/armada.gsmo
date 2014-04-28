using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace GradSovetWeb.Jobs
{
    public class AssignmentTimerJob : SPJobDefinition
    {
        public AssignmentTimerJob() : base() { }

        public AssignmentTimerJob(string jobName, SPService service)
            : base(jobName, service, null, SPJobLockType.None)
        {
            this.Title = "Assignments Timer Job";
        }

        public AssignmentTimerJob(string jobName, SPWebApplication webapp)
            : base(jobName, webapp, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "Assignments Timer Job";
        }


        private string buildQuery(Dictionary<string, int> dict) 
        {
            if (dict.Count == 0) return String.Empty;

            /// building the query
            string res = @"<Where><In><FieldRef Name='ID'/><Values>";
            foreach (KeyValuePair<string, int> item in dict)
	        {
                res += @"<Value Type='Integer'>" + item.Key + @"</Value>";
	        }
            res += @"</Values></In></Where>";

            return res;
        }

        private string getInternalFieldName(SPList list, string fieldDisplayName)
        {
            foreach (SPField field in list.Fields)
            {
                if (field.Title.Equals(fieldDisplayName)) return field.InternalName;
            }

            return String.Empty;
        }

        public override void Execute(Guid targetInstanceId)
        {
            SPWebApplication webApp = this.Parent as SPWebApplication;
            foreach (SPSite siteCollection in webApp.Sites)
            {
                SPList log = siteCollection.RootWeb.Lists.TryGetList("Log");
                SPList assignmentList = siteCollection.RootWeb.Lists.TryGetList("Поручения");
                SPList reportList = siteCollection.RootWeb.Lists.TryGetList("Отчеты по поручениям");

                if (reportList != null)
                {
                    SPListItemCollection items = reportList.GetItems(new SPQuery()
                        {
                            Query = @"<Where>
                                        <Eq>
                                          <FieldRef Name='AssignmentReportResolutionDecision'/>
                                          <Value Type='Text'>Перенести срок</Value>
                                        </Eq>
                                      </Where>"
                        });

                    var res = new Dictionary<string, int>();

                    foreach (SPListItem item in items)
                    {
                        string key = new SPFieldLookupValue(item["AssignmentLink"].ToString()).LookupId.ToString();
                        if (!res.ContainsKey(key))
                        {
                            res[key] = 0;
                        }

                        res[key] += 1;
                    }


                    if (assignmentList != null)
                    {
                        SPListItemCollection assignments = assignmentList.GetItems(new SPQuery()
                        {
                            Query = buildQuery(res)
                        });

                        foreach (SPListItem item in assignments)
                        {
                            if (Convert.ToInt32(item["Количество продлений"]) == res[item["ID"].ToString()]) continue;
                            
                            item["Количество продлений"] = res[item["ID"].ToString()];
                            item.Update();
                        }

                        
                        assignments = assignmentList.GetItems(new SPQuery()
                        {
                            Query = @"<Where>
                                        <And>
                                            <Or>
                                                <Neq>
                                                  <FieldRef Name='" + getInternalFieldName(assignmentList, "Количество продлений") + @"'/>
                                                  <Value Type='Integer'>0</Value>
                                                </Neq>
                                                <IsNull>
                                                  <FieldRef Name='" + getInternalFieldName(assignmentList, "Количество продлений") + @"'/>
                                                </IsNull>
                                            </Or>
                                            <IsNull>
                                              <FieldRef Name='AssignmentFactDate'/>
                                            </IsNull>
                                        </And>
                                      </Where>"
                        });

                        foreach (SPListItem item in assignments)
                        {
                            if (!res.ContainsKey(item["ID"].ToString()))
                            {
                                item["Количество продлений"] = 0;
                                item.Update();
                            }
                        }

                    }
                    
                }
            }
        }
    }
}
