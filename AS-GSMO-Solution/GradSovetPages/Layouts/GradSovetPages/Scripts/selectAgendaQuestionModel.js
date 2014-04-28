/// <reference path="knockout-2.3.0.debug.js" />
/// <reference path="SP.Core.debug.js" />
/// <reference path="SP.debug.js" />
/// <reference path="SP.RequestExecutor.debug.js" />
/// <reference path="SP.runtime.debug.js" />
/// <reference path="init.debug.js" />
/// <reference path="jquery-1.9.1.js" />

(function () {

    function selectQuestionModel() {
        var self = this;
        self.consts = {
            meetingCT: "STS_ListItem_10002"
        }

        self.meetingSearch = ko.observable();
        self.meetingSearchResult = ko.observableArray([]);
        self.agendaQuestionSearchResult = ko.observableArray([]);
        self.selectedMeeting = ko.observable();
        self.selectedAgendaQuestion = ko.observable();

        self.onSearchStringKeyPress = function (data, event) {
            if (event.keyCode == 13) {
                self.startSearch(self);
            }
            else return true;
        }

        self.startSearch = function (data, event) {
            $("#aqDialogError").html("");
            self.meetingSearchResult.removeAll();
            self.agendaQuestionSearchResult.removeAll();
            self.selectedMeeting('');
            self.selectedAgendaQuestion('');
            doSearch(data.meetingSearch());
        }

        self.onShowRelatedQuestions = function (data, event) {
            self.selectedMeeting(data);
            self.selectedAgendaQuestion('');
            var ctx = SP.ClientContext.get_current();
            var agendaQuestionList = ctx.get_web().get_lists().getByTitle("Вопросы повестки заседания");
            var query = new SP.CamlQuery();
            query.set_viewXml('<View><Query><Where><Eq><FieldRef Name="MeetingLink" LookupId="True" /><Value Type="Integer">' + data.ID + '</Value></Eq></Where></Query></View>');
            var questionListInstance = agendaQuestionList.getItems(query);
            ctx.load(questionListInstance, "Include(ID, AgendaQuestionNumber, AgendaQuestionAddress, AgendaQuestionDescription)");
            ctx.executeQueryAsync(function () {
                var enumerator = questionListInstance.getEnumerator();
                var searchResult = [];
                while (enumerator.moveNext()) {
                    searchResult.push({
                        ID: enumerator.get_current().get_item("ID"),
                        AgendaQuestionNumber: enumerator.get_current().get_item("AgendaQuestionNumber"),
                        AgendaQuestionAddress: enumerator.get_current().get_item("AgendaQuestionAddress"),
                        AgendaQuestionDescription: enumerator.get_current().get_item("AgendaQuestionDescription")
                    });
                }
                self.agendaQuestionSearchResult(searchResult);
            }, function () {
                logMessage("Не удалось запросить данные вопросов");
            });


        }

        self.onSelectAgendaQuestion = function (data, event) {
            self.selectedAgendaQuestion(data);
            $('#btnOK').triggerHandler('click');
        }

        self.onClose = function (data, event) {
            if (!self.selectedAgendaQuestion()) {
                logMessage("Необходимо выбрать вопрос", true);
                return;
            }
            $(self.targetLookupId.replace(/(:|\.|\[|\]|\$)/g, "\\$1")).val(self.selectedAgendaQuestion().ID);
            $('#linkedAgendaQuestionTextPresentation').html(
                (String).format('{0} №{1} п.№{2}',
                    self.selectedMeeting().MeetingDate,
                    self.selectedMeeting().MeetingNumber,
                    self.selectedAgendaQuestion().AgendaQuestionNumber));

            // установим глобальные переменные для страницы
            if ("gsLinkedData" in window) {
                window.gsLinkedData = {
                    AgendaQuestionLink: self.selectedAgendaQuestion().ID,
                    MeetingLink: self.selectedMeeting().ID
                };
            }

            if (window.closeSelectQuestionModal) window.closeSelectQuestionModal();
        }

        function doSearch(text) {
            var ctx = SP.ClientContext.get_current();
            
            // check for text
            if (!text) {
                logMessage("Пожалуйста, введите поисковый запрос", true);
                return;
            };

            // build request string
            var keywordQuery = new Microsoft.SharePoint.Client.Search.Query.KeywordQuery(ctx);
            // only in current site, only in meetings
            keywordQuery.set_queryText((String).format("*{0}* contentclass:{1}", text, self.consts.meetingCT));
            if (!document.location.origin) {
                document.location.origin = document.location.protocol + "//" + document.location.host;
            }
            keywordQuery.set_hiddenConstraints((String).format("site:\"{0}\"", document.location.origin));
            var searchExecutor = new Microsoft.SharePoint.Client.Search.Query.SearchExecutor(ctx);
            var results = searchExecutor.executeQuery(keywordQuery);
            ctx.executeQueryAsync(function () {
                var rows = results.m_value.ResultTables[0].ResultRows;
                var count = results.m_value.ResultTables[0].RowCount;
                var idList = [];

                if (!count) {
                    logMessage("По искомой фразе не найдено ни одного заседания", true);
                }
                else {
                    $.each(rows, function (i, e) {
                        if (e.contentclass !== self.consts.meetingCT) return;
                        if (!~e.Path.indexOf("DispForm.aspx")) return;

                        var params = e.Path.split("?")[1] ? e.Path.split("?")[1].split("&") : null;
                        for (var i = 0; i < params.length; i++) {
                            var values = params[i].split("=");
                            if (values[0] !== "ID") continue;

                            idList.push(values[1]);
                            break;
                        }
                    });

                    if (!idList.length) {
                        logMessage("По искомой фразе не найдено ни одного заседания", true);
                    }
                    else {
                        var meetingList = ctx.get_web().get_lists().getByTitle("Заседания");
                        var query = new SP.CamlQuery();
                        query.set_viewXml(buildInCAMLQuery(idList));
                        var meetingInstance = meetingList.getItems(query);
                        ctx.load(meetingInstance, "Include(ID, MeetingNumber, MeetingDate)");
                        ctx.executeQueryAsync(function () {
                            var enumerator = meetingInstance.getEnumerator();
                            var searchResult = [];
                            while (enumerator.moveNext()) {
                                searchResult.push({
                                    ID: enumerator.get_current().get_item("ID"),
                                    MeetingNumber: enumerator.get_current().get_item("MeetingNumber"),
                                    MeetingDate: formatDate(enumerator.get_current().get_item("MeetingDate"))
                                });
                            }
                            self.meetingSearchResult(searchResult);
                        }, function () {
                            logMessage("Не удалось запросить данные заседаний");
                        });
                    }
                }
            }, function () {
                logMessage("Не удалось выполнить поисковый запрос");
            });
        }
    }

    function formatDate(date) {
        if (date === undefined) return;

        var day =   date.getDate();
        var month = date.getMonth() + 1;
        var year =  date.getFullYear();
        
        return (String).format("{0}/{1}/{2}", day < 10 ? "0" + day : day, month < 10 ? "0" + month : month, year);
    }

    function buildInCAMLQuery(list) {
        if (!list.length) return null;

        var res = "<View><Query><Where><In><FieldRef Name='ID'/><Values>";
        $.each(list, function (i, e) {
            res += "<Value Type='Integer'>" + e + "</Value>";
        });
        res += "</Values></In></Where></Query></View>";

        return res;
    }

    function logMessage(text, isWarn) {
        if ("console" in window) {
            if (isWarn) console.warn(text)
            else
                console.error(text);
        }
        
        var el = $("#aqDialogError");
        if (el) {
            el.removeClass("text-danger text-warning");
            el.addClass(isWarn ? "text-warning" : "text-danger");
            el.html("<small>" + text + "</small>");
        }
        else {
            alert(text);
        }
    }

    function sharepointReady() {
        ko.applyBindings(new selectQuestionModel(), document.getElementById(window.gsModals.selectQuestion));
    }

    $(document).ready(function () {
        SP.SOD.executeFunc("sp.js", "SP.ClientContext", function () {
            SP.SOD.executeFunc("sp.search.js", "Microsoft.SharePoint.Client.Search", sharepointReady);
        });
    });

})();