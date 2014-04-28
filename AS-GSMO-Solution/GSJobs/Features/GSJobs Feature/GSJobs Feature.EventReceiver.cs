using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using GSJobs.Jobs;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace GSJobs.Features.GSJobs_Feature
{
    /// <summary>
    /// Этот класс обрабатывает события, возникающие в ходе активации, деактивации, установки, удаления и обновления компонентов.
    /// </summary>
    /// <remarks>
    /// GUID, присоединенный к этому классу, может использоваться при создании пакета и не должен изменяться.
    /// </remarks>

    [Guid("6f7d55d5-f524-48b7-81a0-8de7fe3d5a76")]
    public class GSJobs_FeatureEventReceiver : SPFeatureReceiver
    {
        const string JobName = "Calculate Assignment Prolongation";
        const string JobExpStatuses = "Calculate Assignment Expired Statuses";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;
                    SPSite site = properties.Feature.Parent as SPSite;
                    DeleteExistingJob(JobName, parentWebApp);
                    DeleteExistingJob(JobExpStatuses, parentWebApp);
                    CreateJobs(parentWebApp);
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private bool CreateJobs(SPWebApplication site)
        {
            bool jobCreated = false;
            try
            {
                AssignmentTimerJob job = new AssignmentTimerJob(JobName, site);
                SPMinuteSchedule schedule = new SPMinuteSchedule();
                schedule.BeginSecond = 0;
                schedule.EndSecond = 59;
                schedule.Interval = 1;
                job.Schedule = schedule;
                job.Update();

                AssignmentStatusTimerJob jobExpStatuses = new AssignmentStatusTimerJob(JobExpStatuses, site);
                SPDailySchedule dailyschedule = new SPDailySchedule();
                dailyschedule.BeginHour = 6;
                dailyschedule.BeginMinute = 0;
                dailyschedule.BeginSecond = 0;
                dailyschedule.EndSecond = 15;
                dailyschedule.EndMinute = 0;
                dailyschedule.EndHour = 6;
                jobExpStatuses.Schedule = dailyschedule;
                jobExpStatuses.Update();

                jobCreated = true;
            }
            catch (Exception)
            {
                return jobCreated;
            }
            return jobCreated;
        }

        public bool DeleteExistingJob(string jobName, SPWebApplication site)
        {
            bool jobDeleted = false;
            try
            {
                foreach (SPJobDefinition job in site.JobDefinitions)
                {
                    if (job.Name == JobName)
                    {
                        job.Delete();
                        jobDeleted = true;
                    }
                }
            }
            catch (Exception)
            {
                return jobDeleted;
            }
            return jobDeleted;
        }

        // Раскомментируйте ниже метод для обработки события, возникающего перед деактивацией компонента.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            lock (this)
            {
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;
                        DeleteExistingJob(JobName, parentWebApp);
                        DeleteExistingJob(JobExpStatuses, parentWebApp);
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        // Раскомментируйте ниже метод для обработки события, возникающего после установки компонента.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Раскомментируйте ниже метод для обработки события, возникающего перед удалением компонента.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Раскомментируйте ниже метод для обработки события, возникающего при обновлении компонента.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
