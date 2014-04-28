using System;
using System.Security.Permissions;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace GSWeb.erAssignmentReport
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class erAssignmentReport : SPItemEventReceiver
    {

        private string reporterGroupName = "ГрадСовет - Докладчики";


        private bool isReporter(SPUser user)
        {
            return user.Groups.Cast<SPGroup>().Any(g => g.Name.Equals(reporterGroupName));
        }

        /// <summary>
        /// An item is being added.
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            if (properties.AfterProperties["AssignmentReportResolutionDecision"].ToString() != "Перенести срок")
            {
                properties.AfterProperties["AssignmentReportResolutionNewDate"] = null;
            }

            if (properties.AfterProperties["AssignmentReportResolutionDecision"].ToString() != "Отправить на доработку")
            {
                properties.AfterProperties["AssignmentReportResolutionComment"] = null;
            }

            base.ItemAdding(properties);
        }

        /// <summary>
        /// An item is being updated.
        /// </summary>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            if (properties.ListItem["AssignmentReportResolutionDecision"] != null) 
            {
                if (isReporter(properties.Web.CurrentUser))
                {
                    properties.Status = SPEventReceiverStatus.CancelWithError;
                    properties.ErrorMessage = "Пользователям с ролью \"Докладчик\" запрещено изменять отчеты, по которым указана резолюция";
                    return;
                }
            }

            if (properties.AfterProperties["AssignmentReportResolutionDecision"].ToString() != "Перенести срок")
            {
                properties.AfterProperties["AssignmentReportResolutionNewDate"] = null;
            }

            if (properties.AfterProperties["AssignmentReportResolutionDecision"].ToString() != "Отправить на доработку")
            {
                properties.AfterProperties["AssignmentReportResolutionComment"] = null;
            }

            base.ItemUpdating(properties);
        }

        public override void ItemDeleting(SPItemEventProperties properties)
        {
            if (properties.ListItem["AssignmentReportResolutionDecision"] != null)
            {
                if (isReporter(properties.Web.CurrentUser))
                {
                    properties.Status = SPEventReceiverStatus.CancelWithError;
                    properties.ErrorMessage = "Пользователям с ролью \"Докладчик\" запрещено удалять отчеты, по которым указана резолюция";
                    return;
                }
            }

            base.ItemDeleting(properties);
        }


    }
}