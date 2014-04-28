using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace GSWeb.erLnkedAssignments
{

    /// <summary>
    /// List Item Events
    /// </summary>
    public class erLnkedAssignments : SPItemEventReceiver
    {
        /// <summary>
        /// Проверка изменения значения в поле
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="fieldName">Internal name поля</param>
        /// <returns></returns>
        private bool valueChanged(SPItemEventProperties properties, string fieldName)
        {
            var beforeValue = properties.BeforeProperties[fieldName];
            var afterValue = properties.AfterProperties[fieldName];

            return (beforeValue != afterValue);
        }

        /// <summary>
        /// Получение коллекции связанных поручений
        /// </summary>
        /// <param name="parentItem">Родительское поручение</param>
        /// <returns>Список связанных с parentItem поручений</returns>
        private SPListItemCollection getLinkedAssignments(SPListItem parentItem)
        {
            SPListItemCollection items = null;
            try
            {
                items = parentItem.ParentList.GetItems(new SPQuery()
                {
                    Query = @"<Where>
                            <Eq>
                                <FieldRef Name='AssignmentLink' LookupId='True'/>
                                <Value Type='Integer'>" + parentItem.ID + @"</Value>
                            </Eq>
                          </Where>"
                });
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

            return items;
        }

        private void updateAssignmentTerm(SPListItem assignment, SPListItem parentAssignment)
        {
            DateTime factDate = (DateTime)parentAssignment["AssignmentFactDate"];
            int term = Convert.ToInt32(assignment["AssignmentDaysForResolve"]);

            try
            {
                // only calendar days
                assignment["AssignmentPlanDate"] = factDate.AddDays(term);
                assignment.Update();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            
        }

        /// <summary>
        /// An item was updated.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            if (!valueChanged(properties, "AssignmentFactDate")) 
            { 
                base.ItemUpdated(properties);
                return;
            }

            SPListItemCollection linkedAssignmentList = getLinkedAssignments(properties.ListItem);
            
            foreach (SPListItem item in linkedAssignmentList)
            {
                updateAssignmentTerm(item, properties.ListItem);
            }
            base.ItemUpdated(properties);
        }


    }
}