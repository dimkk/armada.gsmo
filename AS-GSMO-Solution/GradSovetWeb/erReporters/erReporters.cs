using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.Administration;

namespace GradSovetWeb.erReporters
{

    public struct UserInfo
    {
        public int Id;
        public int PositionId;
        public int OrgId;
    }

    /// <summary>
    /// List Item Events
    /// </summary>
    public class erReporters : SPItemEventReceiver
    {
        // messages
        private string erAssignmentAccessMsg = "Отчеты по поручениям могут формировать/удалять только лица, установленные в качестве исполнителей по поручению";
        private string erAgendaQuestionAccessMsg = "Изменения могут вносить только докладчики по вопросу";
        private string erUserNotFoundMsg = "Права доступа текущего пользователя не определены для внесения изменений в список. Пользователь отсутствует в справочнике. Пожалуйста, обратитесь к администратору";

        private string appProductId = "4da4f9c5-037c-4103-acd3-610f480cc047";

        private string reporterGroupName = "ГрадСовет - Докладчики";

        private string[] reporterLists = { 
                                             "Вложения вопроса повестки", 
                                             "Вложения отчета по поручению",
                                             "Вопросы повестки заседания",      // done
                                             "Записи журналов поручений",
                                             "Отчеты по поручениям"             // done
                                         };

        /// <summary>
        /// Идентификатор lookup
        /// </summary>
        /// 
        private int getLookupId(SPListItem item, string lookupName)
        {
            int id = 0;

            SPFieldLookup field = item.Fields[lookupName] as SPFieldLookup;
            if (field != null && item[lookupName] != null)
            {
                SPFieldLookupValue value = field.GetFieldValue(item[lookupName].ToString()) as SPFieldLookupValue;
                if (value != null)
                {
                    id = value.LookupId;
                }
            }

            return id;
        }

        /// <summary>
        /// Проверка принадлежности к роли докладчика ГрадСовета
        /// </summary>
        private bool isReporter(SPUser user)
        { 
            foreach (SPGroup groupItem in user.Groups)
            {
                if (groupItem.Name.Equals(reporterGroupName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Проверка списка, в котором права докладчика должны контролироваться дополнительно
        /// </summary>
        private bool isReporterList(SPList list)
        {
            return Array.IndexOf(reporterLists, list.Title) > -1;
            
        }

        /// <summary>
        /// Проверка всех условий, при которых требуется дополнительный контроль обновления элемента
        /// </summary>
        private bool needsHandle(SPItemEventProperties props)
        {
            return 
                (props.Web.AllProperties["ProductId"] != null) ? props.Web.AllProperties["ProductId"].Equals(appProductId) : true &&
                isReporterList(props.List) &&
                isReporter(props.Web.CurrentUser);
        }

        /// <summary>
        /// Получение информации о поручении, к которому относится отчет исполнителя
        /// </summary>
        private SPListItem getAssignment(int Id, SPWeb web)
        {
            SPQuery select = new SPQuery();
            select.Query = string.Concat(
                        "<Where><Eq>",
                            "<FieldRef Name='ID' />",
                            "<Value Type='Integer'>" + Id + "</Value>",
                        "</Eq></Where>"
                );
            select.ViewFields = string.Concat(
                        "<FieldRef Name='AssignmentExecutorFullNameLink' />",
                        "<FieldRef Name='AssignmentExecutorPositionLink' />",
                        "<FieldRef Name='AssignmentExecutorOrganizationLink' />",
                        "<FieldRef Name='AssignmentSoexecutors' />"
                );
            select.ViewFieldsOnly = true;

            string listUrl = web.ServerRelativeUrl + "/Lists/AssignmentList";
            SPList list = web.GetList(listUrl);
            SPListItemCollection listItems = list.GetItems(select);

            return (listItems.Count != 0) ? listItems[0] : null;
        }
        

        /// <summary>
        /// Получение информации о текущем пользователе из справочника
        /// </summary>
        private UserInfo getUserFromBook(SPWeb web)
        {
            SPQuery select = new SPQuery();
            select.ViewFields = string.Concat(
                        "<FieldRef Name='ID' />",
                        "<FieldRef Name='ParticipantPositionLink' />",
                        "<FieldRef Name='ParticipantOrgLink' />",
                        "<FieldRef Name='UserProfile' />"
                );
            select.ViewFieldsOnly = true;

            string bookUrl = web.ServerRelativeUrl + "/Lists/ParticipantBookList";
            SPList bookList = web.GetList(bookUrl);
            SPListItemCollection bookItems = bookList.GetItems(select);

            UserInfo bookUser = new UserInfo() { Id = 0, PositionId = 0, OrgId = 0 };

            foreach (SPListItem item in bookItems)
            {
                SPFieldUser field = item.Fields["Профиль"] as SPFieldUser;
                if (item["Профиль"] == null) continue;

                SPFieldUserValue userValue = field.GetFieldValue(item["Профиль"].ToString()) as SPFieldUserValue;
                if (userValue.User.UserId.NameId.Equals(web.CurrentUser.UserId.NameId))
                {
                    bookUser.Id         = (int)item["ID"];
                    bookUser.PositionId = getLookupId(item, "Должность");
                    bookUser.OrgId      = getLookupId(item, "Организация");

                    break;
                }
            }

            return bookUser;
        }

        /// <summary>
        /// Запись в лог Sharepoint
        /// </summary>
        private void WriteToLog(string message, params object[] data)
        {
            SPDiagnosticsCategory category = new SPDiagnosticsCategory(
                            "ГрадСовет",
                            TraceSeverity.Unexpected,
                            EventSeverity.Error);
            SPDiagnosticsService.Local.WriteTrace(0, category, TraceSeverity.Unexpected, message, data);
        }

        /// <summary>
        /// Правила модификации отчетов по поручениям
        /// </summary>
        private bool checkAssignmentReport(SPListItem assignment, UserInfo user)
        {
            if (assignment == null) return true;

            List<int> positionList = new List<int>();
            List<int> orgList = new List<int>();
            List<int> personList = new List<int>();

            int lookupId = 0;

            lookupId = getLookupId(assignment, "Исполнитель");
            if (lookupId != 0)
            {
                personList.Add(lookupId);
                lookupId = 0;
            }

            lookupId = getLookupId(assignment, "Должность исполнителя");
            if (lookupId != 0)
            {
                positionList.Add(lookupId);
                lookupId = 0;
            }

            lookupId = getLookupId(assignment, "Организация исполнителя");
            if (lookupId != 0)
            {
                orgList.Add(lookupId);
                lookupId = 0;
            }

            SPFieldLookupValueCollection coExecutorValues = assignment["Соисполнители"] as SPFieldLookupValueCollection;
            if ((coExecutorValues != null) && (coExecutorValues.Count > 0))
            {
                foreach (SPFieldLookupValue value in coExecutorValues)
                {
                    personList.Add(value.LookupId);
                }
            }

            return  (personList.IndexOf(user.Id) >= 0) || 
                    (positionList.IndexOf(user.PositionId) >= 0) || 
                    (orgList.IndexOf(user.OrgId) >= 0);
        }

        /// <summary>
        /// Правила модификации вопросов повестки заседания
        /// </summary>
        private bool checkQuestion(SPListItem question, int userId)
        {
            List<int> reporters = new List<int>();
            if (question["Докладчик"] != null)
            {
                SPFieldLookupValue reporterValue = question.Fields["Докладчик"].GetFieldValue(
                    question["Докладчик"].ToString()) as SPFieldLookupValue;
                if (reporterValue != null)
                {
                    reporters.Add(reporterValue.LookupId);
                }
            }

            SPFieldLookupValueCollection coReporterValues = question["Содокладчики"] as SPFieldLookupValueCollection;

            if ((coReporterValues != null) && (coReporterValues.Count > 0))
            {
                foreach (SPFieldLookupValue value in coReporterValues)
                {
                    reporters.Add(value.LookupId);
                }
            }

            return (reporters.IndexOf(userId) >= 0);
        }

        /// <summary>
        /// Проверка всех условий
        /// </summary>
        /// 
        private string check(SPItemEventProperties properties, UserInfo user)
        {
            // Вопросы повестки заседания
            if (properties.List.Title.Equals(reporterLists[2]))
            {
                if (!checkQuestion(properties.ListItem, user.Id))
                {
                    return erAgendaQuestionAccessMsg;
                }
            }
            // Отчеты по поручениям
            else if (properties.List.Title.Equals(reporterLists[4]))
            {
                SPListItem assignment = getAssignment(
                    getLookupId(properties.ListItem, "Поручение"),
                    properties.Web);

                if (!checkAssignmentReport(assignment, user))
                {
                    return erAssignmentAccessMsg;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Журналирование ошибки при отсутствии пользователя в справочнике
        /// </summary>
        /// 
        private void userNotFoundError(SPItemEventProperties properties)
        {
            WriteToLog(
                "Текущий пользователь (логин {0}) не определен в справочнике участников", 
                properties.Web.CurrentUser.LoginName);

            properties.Status = SPEventReceiverStatus.CancelWithError;
            properties.ErrorMessage = erUserNotFoundMsg;
        }

        /// <summary>
        /// An item is being updated.
        /// </summary>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            if (needsHandle(properties)) 
            {
                UserInfo currentUser = getUserFromBook(properties.Web);
                if (currentUser.Id == 0)
                {
                    userNotFoundError(properties);
                    return;
                }

                string errorMessage = check(properties, currentUser);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    properties.Status = SPEventReceiverStatus.CancelWithError;
                    properties.ErrorMessage = errorMessage;
                    return;
                }
                                
            }
            
            base.ItemUpdating(properties);
        }

        /// <summary>
        /// An item is being deleted.
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            if (needsHandle(properties))
            {
                UserInfo currentUser = getUserFromBook(properties.Web);
                if (currentUser.Id == 0)
                {
                    userNotFoundError(properties);
                    return;
                }

                string errorMessage = check(properties, currentUser);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    properties.Status = SPEventReceiverStatus.CancelWithError;
                    properties.ErrorMessage = errorMessage;
                    return;
                }
            }

            base.ItemDeleting(properties);
        }

        /// <summary>
        /// An item is being added
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            if (needsHandle(properties))
            {
                UserInfo currentUser = getUserFromBook(properties.Web);
                if (currentUser.Id == 0)
                {
                    userNotFoundError(properties);
                    return;
                }

                string errorMessage = string.Empty;
                if (properties.List.Title.Equals(reporterLists[4]))
                {
                    SPListItem assignment = getAssignment(
                        int.Parse(properties.AfterProperties["AssignmentLink"].ToString()),
                        properties.Web);

                    if (!checkAssignmentReport(assignment, currentUser))
                    {
                        errorMessage = erAssignmentAccessMsg;
                    }
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    properties.Status = SPEventReceiverStatus.CancelWithError;
                    properties.ErrorMessage = errorMessage;
                    return;
                }
            }

            base.ItemAdding(properties);
        }

    }
}