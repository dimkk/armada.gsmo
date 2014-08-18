/// <reference path="knockout-2.3.0.debug.js" />
/// <reference path="camljs.js" />
/// <reference path="SP.Core.debug.js" />
/// <reference path="SP.debug.js" />
/// <reference path="SP.RequestExecutor.debug.js" />
/// <reference path="SP.runtime.debug.js" />
/// <reference path="init.debug.js" />
/// <reference path="jquery-1.9.1.js" />


(function () {

    function selectAssignmentModel() {
        var self = this;
        self.assignmentSearchResult = ko.observableArray([]);
        self.selectedAssignment = ko.observable();
        self.isWarn = ko.observable();

        self.Search = function () {
            if (!window.gsLinkedData) return;

            self.isWarn(!window.gsLinkedData.AgendaQuestionLink || !window.gsLinkedData.MeetingLink);
            if (self.isWarn()) return;
            
            var ctx = SP.ClientContext.get_current();
            var agendaQuestionList = ctx.get_web().get_lists().getByTitle("МВК: Вопросы повестки заседания");
            var camlQuery = new CamlBuilder().Where().LookupField("IssueMeetingMVK").Id().EqualTo(window.gsLinkedData.MeetingLink);
            var spQuery = new SP.CamlQuery();
            spQuery.set_viewXml("<View><Query>" + camlQuery.ToString() + "</Query></View>");
            var agendaQuestionListInst = agendaQuestionList.getItems(spQuery);
            ctx.load(agendaQuestionListInst, "Include(ID)");
            ctx.executeQueryAsync(function () {
                var enumerator = agendaQuestionListInst.getEnumerator();
                var searchResult = [];
                while (enumerator.moveNext()) {
                    searchResult.push(
                        enumerator.get_current().get_item("ID")
                    );
                }
                
                var assignmentList = ctx.get_web().get_lists().getByTitle("МВК: Поручения");
                camlQuery = new CamlBuilder().Where().LookupField("AssignmentIssueMVK").Id().In(searchResult);
                spQuery = new SP.CamlQuery();
                spQuery.set_viewXml("<View><Query>" + camlQuery.ToString() + "</Query></View>");
                var assignmentListInst = assignmentList.getItems(spQuery);
                ctx.load(assignmentListInst, "Include(ID, AssignmentNumberMVK, AssignmentTextMVK)");
                ctx.executeQueryAsync(function () {
                    var aResult = [];
                    var aEnumerator = assignmentListInst.getEnumerator();
                    while (aEnumerator.moveNext()) {
                        var aText = aEnumerator.get_current().get_item("AssignmentTextMVK");
                        aResult.push({
                            ID: aEnumerator.get_current().get_item("ID"),
                            AssignmentNumber: aEnumerator.get_current().get_item("AssignmentNumberMVK"),
                            AssignmentText: aEnumerator.get_current().get_item("AssignmentTextMVK"),
                            AssignmentContent: (String).format("№{0} - {1}",
                                aEnumerator.get_current().get_item("AssignmentNumberMVK"),
                                aText ? aText.substring(0, 255) : "Текст поручения не указан")
                        });
                    }

                    self.assignmentSearchResult(aResult);

                }, function () {
                    console.error("Не удалось запросить связанных поручений");
                });

            }, function () {
                console.error("Не удалось запросить данные вопросов");
            });

        }

        self.onSelectAssignment = function (data, event) {
            self.selectedAssignment(data);
            $('#btnLinkedAssignmentOK').triggerHandler('click');
        }

        self.onClose = function (data, event) {
            if (!self.selectedAssignment()) {
                alert("Необходимо выбрать поручение");
                return;
            }
            $(self.targetLookupId.replace(/(:|\.|\[|\]|\$)/g, "\\$1")).val(self.selectedAssignment().ID);
            $('#linkedAssignmentTextPresentation').html(self.selectedAssignment().AssignmentContent);
            if (window.closeSelectAssignmentModal) window.closeSelectAssignmentModal();
        }

    }

    function sharepointReady() {
        ko.applyBindings(new selectAssignmentModel(), document.getElementById(window.gsModals.selectAssignment));
    }

    $(document).ready(function () {
        SP.SOD.executeFunc("sp.js", "SP.ClientContext", function () {
            SP.SOD.executeFunc("sp.search.js", "Microsoft.SharePoint.Client.Search", sharepointReady);
        });
    });

    })();