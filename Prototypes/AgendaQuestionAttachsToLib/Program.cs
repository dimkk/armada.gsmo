using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Security.Principal;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.Office.DocumentManagement.DocumentSets;
using NLog;
using CommandLine;
using gs.meeting.Components;

namespace AgendaQuestionAttachsToLib
{
    class Options
    {
        [Option('s', "site", Required = true, HelpText = "Укажите адрес сайта")]
        public string siteUrl { get; set; }
    }

    class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger("ExceptionLogger");
        private static readonly Logger cLogger = LogManager.GetLogger("ConsoleLogger");

        private static void Work(Options options)
        {
            using (SPSite site = new SPSite(options.siteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList attachList = web.GetList("Lists/AgendaAttachmentList");
                    SPQuery query = new SPQuery();
                    SPListItemCollection items = attachList.GetItems(query);

                    cLogger.Info("Общее количество элементов в списке вложений: {0}", items.Count);
                    int doneCounter = 0;
                    int skippedCounter = 0;

                    foreach (SPListItem item in items)
                    {
                        if (item.Attachments.Count == 0)
                        {
                            skippedCounter++;
                            continue;
                        }

                        IEnumerable<string> itemAttachList =
                            from string fileName in item.Attachments
                            select SPUrlUtility.CombineUrl(item.Attachments.UrlPrefix, fileName);

                        if (item["AgendaQuestionLink"] == null)
                        {
                            skippedCounter++;
                            Logger.Info("Элемент списка {0} с id={1} не имеет ссылки на связанный вопрос повестки. Элемент будет пропущен.", 
                                "Lists/AgendaAttachmentList", item.ID);
                            continue;
                        }

                        SPListItem dsItem = null;
                        int questionId = new SPFieldLookupValue(item["AgendaQuestionLink"].ToString()).LookupId;
                        try
                        {
                            dsItem = erAgendaQuestionItem.doGetQuestionDocumentSet(questionId, web);
                        }
                        catch (Exception ex)
                        {
                            string message = String.Format("Ошибка при получении набора документов для вопроса с id={0} для элемента списка вложений с id={1}: {2}",
                                questionId, item.ID, ex.Message);
                            cLogger.Info("Ошибка при обработке элемента с id={0}. Подробности в логе", item.ID);
                            Logger.Info(message);
                        }

                        if (dsItem == null)
                        {
                            skippedCounter++;
                            continue;
                        }
                        
                        foreach (string attachUrl in itemAttachList)
                        {
                            try
                            {
                                SPFile attachFile = web.GetFile(attachUrl);
                                attachFile.CopyTo(SPUrlUtility.CombineUrl(dsItem.Folder.Url, attachFile.Name), true);
                                web.Update();
                                doneCounter++;
                                Logger.Info(
                                    "Успешно скопировано вложение размером {3} байт по адресу {0} в набор документов по адресу {1} c id={2}",
                                    attachUrl, dsItem.Folder.Url, dsItem.ID, attachFile.TotalLength);
                                cLogger.Info("Скопировано {0}", doneCounter);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(String.Format(
                                    "Ошибка при копировании вложения по адресу {0} в набор документов c id={1} : {2}", 
                                    attachUrl, dsItem.ID, ex.Message));
                            }
                        }
                    }

                    cLogger.Info("Скопировано элементов: {0}", doneCounter);
                    cLogger.Info("Пропущено элементов: {0}", skippedCounter);
                }
            }
        }

        private static bool sureAdminRights()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void Main(string[] args)
        {
            Options options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, options);

            if (options.siteUrl == null)
            {
                cLogger.Info("Необходимо указать адрес сайта, параметр командной строки -s");
                Console.ReadKey();
                return;
            }

            try
            {
                if (!sureAdminRights())
                    throw new Exception("Необходимы права администратора");
                Work(options);
            }
            catch (Exception ex)
            {
                cLogger.Info("В процессе работы возникла ошибка, см. лог");
                Logger.Error(ex.Message);
            }

            cLogger.Info("Работа завершена, нажмите любую клавишу");
            Console.ReadKey();
        }
    }
}
