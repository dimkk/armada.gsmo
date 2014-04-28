using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace GSJobs.Jobs
{
    class AssignmentStatusTimerJob : SPJobDefinition
    {
        // TODO: В CAML есть ограничение по количеству условий In в запросе,
        // в связи с этим обработку поручений нужно производить максимально 
        // допустимыми частями
        const int limitQueryConditions = 200;

        public AssignmentStatusTimerJob() : base() { }

        public AssignmentStatusTimerJob(string jobName, SPService service)
            : base(jobName, service, null, SPJobLockType.None)
        {
            this.Title = "Assignments Status Timer Job";
        }

        public AssignmentStatusTimerJob(string jobName, SPWebApplication webapp)
            : base(jobName, webapp, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "Assignments Status Timer Job";
        }

        /// <summary>
        /// Обновление статусов поручений
        /// </summary>
        /// <param name="list">Список поручений</param>
        private void updateStatuses(SPList list)
        {
            SPListItemCollection assignments = list.GetItems(new SPQuery()
            {
                Query = @"<Where>
	                        <And>
		                        <And>
			                        <IsNotNull>
				                        <FieldRef Name='AssignmentPlanDate' />
			                        </IsNotNull>
			                        <Lt>
				                        <FieldRef Name='AssignmentPlanDate' />
				                        <Value Type='DateTime'>
					                        <Today />
				                        </Value>
			                        </Lt>
		                        </And>
		                        <And>
			                        <IsNull>
				                        <FieldRef Name='AssignmentFactDate' />
			                        </IsNull>
			                        <Eq>
				                        <FieldRef Name='AssignmentStatus' />
				                        <Value Type='Choice'>На исполнении</Value>
			                        </Eq>
		                        </And>
	                        </And>
                        </Where>"
            });

            foreach (SPListItem item in assignments)
            {
                item["AssignmentStatus"] = "Срок истек";
                item.Update();
            }
        }

        public override void Execute(Guid targetInstanceId)
        {
            SPWebApplication webApp = this.Parent as SPWebApplication;
            foreach (SPSite siteCollection in webApp.Sites)
            {
                SPList assignmentList = siteCollection.RootWeb.Lists.TryGetList("Поручения");

                if (assignmentList == null) continue;

                try
                {
                    updateStatuses(assignmentList);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }
    }
}
