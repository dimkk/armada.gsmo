/// <reference path="renderCore.js" />
/// <reference path="../SP.debug.js" />
/// <reference path="../SP.Core.debug.js" />
/// <reference path="../SP.runtime.debug.js" />

(function () {

    var author, editor, created, modified;
    var exceptList = [];
    var selectQuestionControlId, selectAssignmentControlId;
    var renderCore;
    // модальные диалоги
    window.gsModals = window.gsModals || {};
    // структура для исполнителя (ФИО, должность, организация)
    window.gsExecutor = window.gsExecutor || {};

    function init() {
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            Templates: {
                Fields: {
                    "AssignmentExecutorMVK": { "NewForm": renderExecutorFields, "EditForm": renderExecutorFields },
                    "AssignmentExecutorPositionMVK": { "NewForm": renderExecutorFields, "EditForm": renderExecutorFields },
                    "AssignmentExecutorOrgMVK": { "NewForm": renderExecutorFields, "EditForm": renderExecutorFields }
                },
                Item: renderFields
            },
            OnPostRender: OnPostRender,
            ListTemplateType: 10052,
        });
    }

    function getLinkId() {
        var params = document.referrer.split('?')[1].split('&');
        for (var i = 0; i < params.length; i++) {
            var param = params[i].split('=');
            if (param[0] !== 'ID') continue;

            return param[1];
        }
        return null;
    }

    function renderItemHeader(context) {
        var resultHtml = '';
        resultHtml += '<div class="container" style="margin-top: 25px;">';

        return resultHtml;
    }

    function renderItemFooter(context) {
        var resultHtml = '';
        resultHtml += '</div>';

        return resultHtml;
    }

    function renderExecutorFields(ctx) {
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);

        var f;

        switch (formCtx.fieldName) {
            case "AssignmentExecutorMVK":
                f = function () {
                    return window.gsExecutor.AssignmentExecutorFullNameLink
                };
                break;
            case "AssignmentExecutorPositionMVK":
                f = function () {
                    return window.gsExecutor.AssignmentExecutorPositionLink
                };
                break;
            case "AssignmentExecutorOrgMVK":
                f = function () {
                    return window.gsExecutor.AssignmentExecutorOrganizationLink
                };
                break;
        }

        if (f) {

            formCtx.registerGetValueCallback(formCtx.fieldName, f);
        }
    }

    function renderFields(context) {
        renderCore = window.renderCore && window.renderCore.init(context);
        if (!renderCore) {
            console.error('Не удалось инициализировать renderCore');
            return;
        }

        var resultHtml = '';
        resultHtml += renderItemHeader(context);

        if (context.ControlMode === SPClientTemplates.ClientControlMode.DisplayForm) {
            resultHtml += '<label>Решение</label>';
            resultHtml += '<div id="decision" style="margin-bottom: 25px"></div>';

            resultHtml += '<div class="form-horizontal" role="form">';

            resultHtml += '<div class="form-group">';

            var calcFieldName = renderCore.getInternalFieldName("AssignmentDependentTermTextMVK");
            if (calcFieldName && context.CurrentItem[calcFieldName] && !context.CurrentItem["AssignmentPlanDateMVK"]) {
                resultHtml += renderFieldBlock('Срок исполнения', 2, 4, calcFieldName);
            }
            else {
                resultHtml += renderFieldBlock('Срок исполнения', 2, 4, "AssignmentPlanDateMVK");
            }
            resultHtml += renderFieldBlock('Фактический срок', 2, 4, "AssignmentFactDateMVK");
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += renderFieldBlock('Ответственный', 2, 4, "AssignmentResponsibleExecutorMVK");
            resultHtml += renderFieldBlock('Соисполнители', 2, 4, "AssignmentCoExecutorsMVK");
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += renderFieldBlock('Поручение', 2, 10, "AssignmentTextMVK");
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += '<label class="col-lg-2">Ход исполнения</label>';
            resultHtml += '<div id="execution" class="col-lg-10"></div>';
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += '<label class="col-lg-2">Резолюция</label>';
            resultHtml += '<div id="resolution" class="col-lg-10"></div>';
            resultHtml += '</div>';

            resultHtml += '</div>'; // form-horizontal

        } else {
            resultHtml += '<div class="form-horizontal" role="form">';

            resultHtml += '<div class="form-group">';
            resultHtml += renderFieldBlock('Номер', 2, 4, "AssignmentNumberMVK");
            resultHtml += renderFieldBlock('Фактический срок', 2, 4, "AssignmentFactDateMVK");
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += renderFieldBlock('Тип поручения', 2, 10, "AssignmentTypeMVK");
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += renderFieldBlock('Статус', 2, 4, "AssignmentStatusMVK");
            var agendaQuestionLinkHtml = renderFieldBlock('Вопрос повестки', 2, 2, "AssignmentIssueMVK", true);
            var lookupElement = renderCore.getLookupFromRenderedHtml(agendaQuestionLinkHtml);
            resultHtml += agendaQuestionLinkHtml;
            resultHtml += '<div class="col-lg-2" id="linkedAgendaQuestionTextPresentation"></div>';
            selectQuestionControlId = $(lookupElement).attr('id');
            var modalLink = renderCore.bs.renderModalLink(
                "/_layouts/15/gradsovetpages/pages/selectIssueMVK.html?rev=" + Math.random().toString(36).substr(2),
                "Выбрать",
                2,
                selectQuestionControlId);

            window.gsModals.selectQuestion = modalLink.modalId;
            resultHtml += modalLink.html;
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += renderFieldBlock('Состояние контроля', 2, 10, "AssignmentControlStateMVK");
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += renderFieldBlock('Поручение', 2, 10, "AssignmentTextMVK");
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += '<label class="col-lg-2">Срок исполнения</label>';
            resultHtml += '<div class="col-lg-2"><div class="radio"><label><input type="radio" value="ByDate" name="executionDate" id="executionDateByDateRadio" onclick="$($(\'#AssignmentPlanDate\').find(\'input\')[0]).removeAttr(\'disabled\'); $($(\'#AssignmentDaysForResolve\').find(\'input\')[0]).attr(\'disabled\', \'disabled\'); $($(\'#AssignmentDayType\').find(\'select\')[0]).attr(\'disabled\', \'disabled\'); return true;" />Абсолютный</label></div></div>';
            resultHtml += '<div class="col-lg-4" id="AssignmentPlanDate">' +
                                renderCore.bs.applyCSS(context.RenderFieldByName(context, "AssignmentPlanDateMVK")) +
                          '</div>';
            resultHtml += '</div>';
            resultHtml += '<div class="form-group">';
            resultHtml += '<div class="col-lg-offset-2 col-lg-2"><div class="radio"><label><input type="radio" value="ByAssignment" name="executionDate" id="executionDateByAssignmentRadio" onclick="$($(\'#AssignmentPlanDate\').find(\'input\')[0]).attr(\'disabled\', \'disabled\'); $($(\'#AssignmentDaysForResolve\').find(\'input\')[0]).removeAttr(\'disabled\'); $($(\'#AssignmentDayType\').find(\'select\')[0]).removeAttr(\'disabled\'); $($(\'#AssignmentPlanDate\').find(\'input\')[0]).val(null); return true;" />Относительный</label></div></div>';
            resultHtml += '<div class="col-lg-1" id="AssignmentDaysForResolve">' +
                                renderCore.bs.applyCSS(context.RenderFieldByName(context, "AssignmentDaysForResolveMVK")) +
                          '</div>';
            resultHtml += '<div class="col-lg-3">дней после зависимого поручения</div>';
            resultHtml += '</div>';
            resultHtml += '<div class="form-group">';
            resultHtml += '<div class="col-lg-offset-2 col-lg-1">дни</div>';
            resultHtml += '<div class="col-lg-5" id="AssignmentDayType">' +
                                renderCore.bs.applyCSS(context.RenderFieldByName(context, "AssignmentDayTypeMVK")) +
                          '</div>';
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            var assignmentLinkHtml = renderFieldBlock('Зависимое поручение', 2, 2, "AssignmentDependentAssignmentMVK", true);
            lookupElement = renderCore.getLookupFromRenderedHtml(assignmentLinkHtml);
            resultHtml += assignmentLinkHtml;
            selectAssignmentControlId = $(lookupElement).attr('id');
            modalLink = renderCore.bs.renderModalLink(
                "/_layouts/15/gradsovetpages/pages/selectLinkedAssignmentMVK.html?rev=" + Math.random().toString(36).substr(2),
                "Выбрать",
                1,
                selectAssignmentControlId);

            window.gsModals.selectAssignment = modalLink.modalId;
            resultHtml += modalLink.html;
            resultHtml += '<div class="col-lg-9" id="linkedAssignmentTextPresentation"></div>';
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += '<label class="col-lg-2">Исполнитель</label>';
            context.RenderFieldByName(context, "AssignmentExecutorMVK");
            context.RenderFieldByName(context, "AssignmentExecutorPositionMVK");
            context.RenderFieldByName(context, "AssignmentExecutorOrgMVK");
            window.gsExecutor = {
                AssignmentExecutorFullNameLink: context.CurrentItem["AssignmentExecutorMVK"],
                AssignmentExecutorPositionLink: context.CurrentItem["AssignmentExecutorPositionMVK"],
                AssignmentExecutorOrganizationLink: context.CurrentItem["AssignmentExecutorOrgMVK"]
            };
            modalLink = renderCore.bs.renderModalLink(
                "/_layouts/15/gradsovetpages/pages/selectExecutor.html?rev=" + Math.random().toString(36).substr(2), "Выбрать", 1, "");

            window.gsModals.selectExecutor = modalLink.modalId;
            resultHtml += modalLink.html;
            var responseFieldName = renderCore.getInternalFieldName("Ответственный исполнитель");
            var responseFieldValue = responseFieldName ? context.CurrentItem[responseFieldName] : "";
            resultHtml += '<div class="col-lg-9" id="ExecutorTextPresentation">' + responseFieldValue + '</div>';
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += renderFieldBlock('Соисполнители', 2, 10, "AssignmentCoExecutorsMVK");
            resultHtml += '</div>';

            resultHtml += '<div class="form-group">';
            resultHtml += renderFieldBlock('Примечание', 2, 10, "AssignmentNoteMVK");
            resultHtml += '</div>';

            resultHtml += '</div>'; // form-horizontal
        }

        author = renderCore.renderFieldByName("Author");
        exceptList.push("Author");
        created = renderCore.renderFieldByName("Created");
        exceptList.push("Created");
        editor = renderCore.renderFieldByName("Editor");
        exceptList.push("Editor");
        modified = renderCore.renderFieldByName("Modified");
        exceptList.push("Modified");

        resultHtml += renderItemFooter(context);

        return resultHtml;
    }

    function formatHeader(data) {
        var captionFmt = 'Поручение по решению Межведомственной комиссии №{0}';
        var questionFmt = '{0} №{1} п.№{2}';
        var pageUrlFmt = '/Lists/IssueMVKList/DispForm2.aspx?ID={0}';
        var addressFmt = '<strong>{0} {1}</strong>';
        var categoryFmt = '<strong>{0}</strong>';

        renderCore.ifget('assignmentCaption', function (e) {
            e.innerHTML = (String).format(captionFmt, data.AssignmentNumber);
        });
        renderCore.ifget('questionLink', function (e) {
            e.innerHTML = (String).format(questionFmt, renderCore.formatDate(data.MeetingDate), data.MeetingNumber, data.QuestionNumber);
        });
        renderCore.ifget('linkedAgendaQuestionTextPresentation', function (e) {
            e.innerHTML = (String).format(questionFmt, renderCore.formatDate(data.MeetingDate), data.MeetingNumber, data.QuestionNumber);
        });
        renderCore.ifget('questionLink', function (e) {
            e.href = _spPageContextInfo.webAbsoluteUrl + (String).format(pageUrlFmt, data.Id);
        });
        renderCore.ifget('questionAddress', function (e) {
            e.innerHTML = (String).format(addressFmt, data.MeetingPlace, data.CadastreNumber ? "Кадастровый номер " + data.CadastreNumber : "");
        });
        renderCore.ifget('customText', function (e) {
            e.innerHTML = (String).format(categoryFmt, data.CategoryName);
        });
        renderCore.ifget('timedOut', function (e) { e.innerHTML = data.Status; });
        if (data.Status) {
            var failed =
                data.Status == 'На исполнении с нарушением срока' ||
                data.Status == 'Выполнено с нарушением срока' || 
                data.Status == 'Срок истек';
            var statusClass = failed ? 'btn-danger' : 'btn-success';
            $('#timedOut').addClass(statusClass);
        }
    }

    function OnPostRender(context) {
        // bootstrap диалоги для корректного отображения нужно вынести из контейнеров в конец документа
        // дополнительно инициализируем модель контролом, который отвечает за сохранение выбранного значения
        $('#' + window.gsModals.selectQuestion).appendTo('body').on('shown.bs.modal', function (e) {
            var qModel = ko.dataFor(this);
            qModel.targetLookupId = $(e.relatedTarget).data('lookup');
        });

        $('#' + window.gsModals.selectAssignment).appendTo('body').on('shown.bs.modal', function (e) {
            var aModel = ko.dataFor(this);

            aModel.targetLookupId = $(e.relatedTarget).data('lookup');
            // при открытии диалога сразу отображаем выбор поручений
            aModel.Search();
        });

        $('#' + window.gsModals.selectExecutor).appendTo('body');
        
        window.closeSelectQuestionModal = function () {
            $('#' + window.gsModals.selectQuestion).modal('hide');
        };

        window.closeSelectAssignmentModal = function () {
            $('#' + window.gsModals.selectAssignment).modal('hide');
        };

        window.closeSelectExecutorModal = function () {
            $('#' + window.gsModals.selectExecutor).modal('hide');
        };

        // инициализация радио кнопок выбора срока исполнения
        if (context.ControlMode === SPClientTemplates.ClientControlMode.NewForm) {
            $("#executionDateByDateRadio").attr("checked", "checked");
            $("#executionDateByDateRadio").triggerHandler("click");
        } else if (context.ControlMode === SPClientTemplates.ClientControlMode.EditForm) {
            if (context.ListData.Items[0].AssignmentPlanDate) {
                $("#executionDateByDateRadio").attr("checked", "checked");
                $("#executionDateByDateRadio").triggerHandler("click");
            } else {
                $("#executionDateByAssignmentRadio").attr("checked", "checked");
                $("#executionDateByAssignmentRadio").triggerHandler("click");
            }
        }

        var hasContext = document.referrer &&
                (~document.referrer.indexOf('Lists/IssueMVKList/DispForm') ||
                ~document.referrer.indexOf('Lists/IssueMVKList/EditForm'));

        if (context.ControlMode === SPClientTemplates.ClientControlMode.NewForm && hasContext) {
            // в режиме создания нового поручения при наличии контекста автоматически установим ссылку на вопрос
            renderCore.ifget(selectQuestionControlId, function (e) {
                $(e).val(getLinkId());
            });
        };

        var prefix = context.FormUniqueId + context.FormContext.listAttributes.Id;

        renderCore.ifget(prefix + 'Author', function (e) { e.innerHTML = author; });
        renderCore.ifget(prefix + 'Created', function (e) { e.innerHTML = created; });
        renderCore.ifget(prefix + 'Editor', function (e) { e.innerHTML = editor; });
        renderCore.ifget(prefix + 'Modified', function (e) { e.innerHTML = modified; });

        window.gsLinkedData = window.gsLinkedData || {};

        // достанем данные вопроса и заседания, чтобы отобразить шапку документа
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            var aqLink = context.ListData.Items[0].AssignmentIssueMVK;
            var asLink = context.ListData.Items[0].AssignmentDependentAssignmentMVK;
            var questionId = aqLink ? aqLink.split(';')[0] : (hasContext ? getLinkId() : "");
            var assignmentId = asLink ? asLink.split(';')[0] : "";

            // установим глобальные переменные для страницы
            window.gsLinkedData.AgendaQuestionLink = questionId;

            var ctx = SP.ClientContext.get_current();

            // связанное поручение
            if (assignmentId) {
                var assignmentList = ctx.get_web().get_lists().getByTitle("МВК: Поручения");
                var aQuery = new SP.CamlQuery();
                aQuery.set_viewXml("<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Text'>" + assignmentId + "</Value></Eq></Where></Query></View>");
                var assignmentInst = assignmentList.getItems(aQuery);
                ctx.load(assignmentInst, "Include(AssignmentNumberMVK, AssignmentTextMVK)");
            }
            
            var agendaQuestionList = ctx.get_web().get_lists().getByTitle("МВК: Вопросы повестки заседания");
            var query = new SP.CamlQuery();
            query.set_viewXml("<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Text'>" + questionId + "</Value></Eq></Where></Query></View>");
            var questionInstance = agendaQuestionList.getItems(query);
            ctx.load(questionInstance);
            ctx.executeQueryAsync(function () {
                if (!assignmentInst || !assignmentInst.get_data()[0]) {
                    console.warn('Не удалось запросить данные связанного поручения');
                }
                else {
                    // форматированный текст для отображения связанного поручения
                    var aText = assignmentInst.get_data()[0].get_item("AssignmentText");
                    $('#linkedAssignmentTextPresentation').html(
                        (String).format("№{0} - {1}",
                            assignmentInst.get_data()[0].get_item("AssignmentNumber"),
                            aText ? aText.substring(0, 255) + "..." : "Текст поручения не указан")
                    );
                }

                if (!questionInstance.get_data()[0]) {
                    console.warn('Не удалось запросить данные вопроса повестки');
                    return;
                }

                // достанем данные заседания
                var meetingList = ctx.get_web().get_lists().getByTitle("МВК: Заседания");
                var mId = questionInstance.get_data()[0].get_item("IssueMeetingMVK").get_lookupId();

                // установим глобальные переменные для страницы
                window.gsLinkedData.MeetingLink = mId;

                query.set_viewXml("<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Text'>" + mId + "</Value></Eq></Where></Query></View>");
                var meetingListInst = meetingList.getItems(query);
                ctx.load(meetingListInst);
                ctx.executeQueryAsync(function () {
                    if (!meetingListInst.get_data()[0]) {
                        console.error('Не удалось запросить данные заседания');
                        return;
                    }
                    var categoryFieldValue = questionInstance.get_data()[0].get_item("IssueCategoryMVK");

                    // шапка не отображается в режиме создания нового элемента
                    if (context.ControlMode === SPClientTemplates.ClientControlMode.NewForm && hasContext) {
                        $('#linkedAgendaQuestionTextPresentation').html(
                            (String).format('{0} №{1} п.№{2}',
                                renderCore.formatDate(meetingListInst.get_data()[0].get_item('MeetingDateMVK')),
                                meetingListInst.get_data()[0].get_item('MeetingNumberMVK'),
                                questionInstance.get_data()[0].get_item("IssueNumberTextMVK")));

                        return;
                    }

                    formatHeader({
                        Id: questionInstance.get_data()[0].get_item("ID"),
                        MeetingDate: meetingListInst.get_data()[0].get_item('MeetingDateMVK'),
                        MeetingPlace: questionInstance.get_data()[0].get_item("IssueAddressMVK"),
                        MeetingNumber: meetingListInst.get_data()[0].get_item('MeetingNumberMVK'),
                        QuestionNumber: questionInstance.get_data()[0].get_item("IssueNumberTextMVK"),
                        CategoryName: categoryFieldValue ? categoryFieldValue.get_lookupValue() : "",
                        Status: context.ListData.Items[0].AssignmentStatusMVK,
                        AssignmentNumber: context.ListData.Items[0].AssignmentNumberMVK,
                        CadastreNumber: questionInstance.get_data()[0].get_item("IssueCadastreIdMVK")

                    });

                    renderCore.ifget('decision', function (e) {
                        e.innerHTML = questionInstance.get_data()[0].get_item("IssueDecisionMVK");
                    });

                    // достанем данные последнего отчета
                    var lastReportFieldName = renderCore.getInternalFieldName('Последний отчет');
                    if (context.ListData.Items[0][lastReportFieldName]) { 
                        var reportList = ctx.get_web().get_lists().getByTitle("МВК: Отчеты по поручению");
                        var rId = context.ListData.Items[0][lastReportFieldName].split(';')[0];
                        query.set_viewXml("<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Text'>" + rId + "</Value></Eq></Where></Query></View>");
                        var reportListInst = reportList.getItems(query);
                        ctx.load(reportListInst);
                        ctx.executeQueryAsync( function() {
                            if (!reportListInst.get_data()[0]) {
                                console.error('Не удалось запросить данные последнего отчета');
                                return;}

                            renderCore.ifget('execution', function (e) {
                                e.innerHTML = (String).format("<p>{0}</p> <p>{1}</p>",
                                    context.ListData.Items[0].AssignmentStatusMVK,
                                    reportListInst.get_data()[0].get_item("ReportTextMVK"));
                            });
                            renderCore.ifget('resolution', function (e) {
                                e.innerHTML = (String).format("<p>{0}</p>",
                                    reportListInst.get_data()[0].get_item("ReportDecisionMVK"));
                            });

                        }, function() {
                            console.error("Не удалось запросить данные последнего отчета");
                        });
                    }

                }, function () {
                    console.error("Не удалось запросить данные заседания");
                });

            }, function () {
                console.error("Не удалось запросить данные вопроса повестки");
            });
        }, 'sp.js');
    }

    function renderFieldBlock(label, labelSpan, inputSpan, fieldName, controlInvisible) {
        exceptList.push(fieldName);
        return renderCore.bs.renderFieldBlock(label, labelSpan, inputSpan, fieldName, controlInvisible);
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {

        init();

        // MDS
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {

            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/_layouts/15/gradsovetpages/Scripts/csr/renderAssignmentMVK.js"), init);

        }, 'sp.js');

    }, 'clienttemplates.js');
})();

//Настройка интерфейса
$(function () {
    //Устанавливаем нужный класс для стандартных кнопок
    $('#buttons input[type="button"]').attr('class', 'form-control');
    //Убираем лишний отступ у кнопок MultipleValueLookup
    $('[type="button"][id$="Button"]').css("margin", 0);
    //Увеличиваем ширину MultipleValueLookup
    $('table[id$="MultiLookup_topTable"]').css("width", "100%").find('select').parent().css("width", "50%").children().css("width", "100%");
});
