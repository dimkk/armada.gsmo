using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace GSJobs.Jobs
{
    public class AssignmentTimerJob : SPJobDefinition
    {
        // TODO: В CAML есть ограничение по количеству условий In в запросе,
        // в связи с этим обработку поручений нужно производить максимально 
        // допустимыми частями
        const int limitQueryConditions = 200;

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

        /// <summary>
        /// Динамический запрос для выборки всех поручений, входящих в список
        /// </summary>
        /// <param name="dict">
        /// Список поручений
        /// </param>
        /// <returns>
        /// Строка CAML запроса
        /// </returns>
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

        /// <summary>
        /// Внутреннее имя столбца по DisplayName
        /// </summary>
        /// <param name="list">Список, в котором содержится столбец</param>
        /// <param name="fieldDisplayName">DisplayName столбца</param>
        /// <returns></returns>
        private string getInternalFieldName(SPList list, string fieldDisplayName)
        {
            foreach (SPField field in list.Fields)
            {
                if (field.Title.Equals(fieldDisplayName)) return field.InternalName;
            }

            return String.Empty;
        }

        /// <summary>
        /// Получение таблицы, содержащей количество отчетов по заданному критерию для каждого поручения.
        /// В таблицу включены только те поручения, у которых количество отчетов не нулевое (см. updateZeroCounters).
        /// </summary>
        /// <param name="list">Список отчетов</param>
        /// <returns>Словарь, в котором ключом является идентификатор поручения, а значением - количество отчетов</returns>
        private Dictionary<string, int> getCurrentReportsState(SPList list)
        {
            SPListItemCollection items = list.GetItems(new SPQuery()
            {
                Query = @"<Where>
                            <And>
                                <Eq>
                                    <FieldRef Name='AssignmentReportResolutionDecision'/>
                                    <Value Type='Choice'>Перенести срок</Value>
                                </Eq>
                                <IsNotNull>
                                    <FieldRef Name='AssignmentLink'/>
                                </IsNotNull>
                            </And>
                          </Where>"
            });

            var state = new Dictionary<string, int>();
            foreach (SPListItem item in items)
            {
                string key = new SPFieldLookupValue(item["AssignmentLink"].ToString()).LookupId.ToString();
                if (!state.ContainsKey(key))
                {
                    state[key] = 0;
                }

                state[key] += 1;
            }

            return state;
        }

        /// <summary>
        /// Обновление счетчика отчетов в поручениях в соответствии со значениями словаря
        /// </summary>
        /// <param name="state">Словарь</param>
        /// <param name="list">Список поручений</param>
        private void updateAssignmentList(Dictionary<string, int> state, SPList list)
        {
            // если нет отчетов обновлять нечего
            if (state.Count == 0) return;

            SPListItemCollection assignments = list.GetItems(new SPQuery()
            {
                Query = buildQuery(state)
            });

            foreach (SPListItem item in assignments)
            {
                if (Convert.ToInt32(item["Количество продлений"]) == state[item["ID"].ToString()]) continue;

                item["Количество продлений"] = state[item["ID"].ToString()];
                item.Update();
            }
        }

        /// <summary>
        /// Обновление счетчиков поручений до нуля. Алгоритм предполагает, что все поручения, которые имели 
        /// счетчик отчетов, отличный от нуля, не попали в словарь только в том случае, если отчетов больше нет.
        /// Алгоритм не обновляет поручения, жизненный цикл которых завершен, т.е. установлено значение
        /// поля "Фактическая дата выполнения"
        /// </summary>
        /// <param name="state">Словарь</param>
        /// <param name="list">Список поручений</param>
        private void updateZeroCounters(Dictionary<string, int> state, SPList list)
        {
            SPListItemCollection assignments = list.GetItems(new SPQuery()
            {
                Query = @"<Where>
                            <And>
                                <Or>
                                    <Neq>
                                        <FieldRef Name='" + getInternalFieldName(list, "Количество продлений") + @"'/>
                                        <Value Type='Integer'>0</Value>
                                    </Neq>
                                    <IsNull>
                                        <FieldRef Name='" + getInternalFieldName(list, "Количество продлений") + @"'/>
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
                if (!state.ContainsKey(item["ID"].ToString()))
                {
                    item["Количество продлений"] = 0;
                    item.Update();
                }
            }
        }

        private void WriteTrace(string message, Exception ex = null)
        {
            SPDiagnosticsService.Local.WriteTrace(0,
                new SPDiagnosticsCategory("GSJobs category", TraceSeverity.Unexpected, EventSeverity.Error),
                TraceSeverity.Unexpected, message, ex != null ? ex.StackTrace : null);
        }

        public override void Execute(Guid targetInstanceId)
        {
            SPWebApplication webApp = this.Parent as SPWebApplication;
            foreach (SPSite siteCollection in webApp.Sites)
            {
                SPList assignmentList = siteCollection.RootWeb.Lists.TryGetList("Поручения");
                SPList reportList = siteCollection.RootWeb.Lists.TryGetList("Отчеты по поручениям");

                if (reportList == null || assignmentList == null) continue;

                try
                {
                    WriteTrace("AssignmentTimerJob Execute:");
                    var res = getCurrentReportsState(reportList);
                    updateAssignmentList(res, assignmentList);
                    updateZeroCounters(res, assignmentList);
                    WriteTrace("AssignmentTimerJob Execute[end]");
                }
                catch (Exception ex)
                {
                    WriteTrace(String.Format("Got AssignmentTimerJob Execute Exception\nException: {0}", ex), ex);
                    throw ex;
                }

            }
        }
    }
}
