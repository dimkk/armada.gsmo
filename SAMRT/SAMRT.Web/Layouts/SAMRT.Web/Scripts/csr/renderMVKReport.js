/// <reference path="renderCore.js" />
/// <reference path="../SP.debug.js" />
/// <reference path="../SP.Core.debug.js" />
/// <reference path="../SP.runtime.debug.js" />

var isCancelAddAttach = false;
(function () {

    var author, editor, created, modified, assignmentLinkId, resolutionId;
    var exceptList = [];
    var renderCore;

    function init() {
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            Templates: {
                Item: renderFields
            },
            OnPostRender: OnPostRender,
            ListTemplateType: 10053,
        });
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

    function renderFields(context) {
        renderCore = window.renderCore && window.renderCore.init(context);
        if (!renderCore) {
            console.error('Не удалось инициализировать renderCore');
            return;
        }

        var resultHtml = '';
        resultHtml += renderItemHeader(context);

        resultHtml += '<div class="form-horizontal" role="form">';

        resultHtml += '<div class="form-group">';
        var assignmentLinkHtml = renderFieldBlock('Поручение', 2, 4, "ReportAssignmentMVK");
        var lookupElement = renderCore.getLookupFromRenderedHtml(assignmentLinkHtml)
        assignmentLinkId = $(lookupElement).attr('id');
        // в режиме просмотра текст ссылки нужно будет заменить
        if (context.ControlMode === SPClientTemplates.ClientControlMode.DisplayForm) {
            var htmldoc = $("<div></div>").append(assignmentLinkHtml);
            var a = htmldoc.find('a')[0];
            if (a) {
                $(a).attr("id", "ReportAssignmentMVK");
                assignmentLinkHtml = htmldoc.html();
            }
        }
        resultHtml += assignmentLinkHtml;

        resultHtml += renderFieldBlock('Дата', 2, 4, "ReportDateMVK");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += '<label class="col-lg-2">Текст поручения</label>';
        resultHtml += '<div class="col-lg-10" id="LinkedAssignmentText"></div>';
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock('Текст отчета', 2, 10, "ReportTextMVK");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group" id="decision" style="display:none">';
        var resolutionHtml = renderFieldBlock('Резолюция', 2, 10, "ReportDecisionMVK");
        lookupElement = renderCore.getLookupFromRenderedHtml(resolutionHtml);
        resolutionId = $(lookupElement).attr('id');
        resultHtml += resolutionHtml;
        resultHtml += '</div>';

        resultHtml += '<div class="form-group" id="newdeadline" style="display:none">';
        resultHtml += renderFieldBlockNoFormControl('Новый срок', 2, 10, "ReportNewDeadlineMVK");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group" id="changeRequestComment" style="display:none">';
        resultHtml += renderFieldBlock('Комментарий', 2, 10, "ReportResolutionCommentMVK");
        resultHtml += '</div>';

        resultHtml += '</div>';

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

    function renderFieldBlock(label, labelSpan, inputSpan, fieldName) {
        exceptList.push(fieldName);
        return renderCore.bs.renderFieldBlock(label, labelSpan, inputSpan, fieldName);
    }

    function renderFieldBlockNoFormControl(label, labelSpan, inputSpan, fieldName) {
        exceptList.push(fieldName);
        return renderCore.bs.renderFieldBlock(label, labelSpan, inputSpan, fieldName, false, true);
    }

    function getQuestionData(Id, onsuccess, onfail) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            var ctx = SP.ClientContext.get_current();
            var List = ctx.get_web().get_lists().getByTitle("МВК: Вопросы повестки заседания");
            var query = new SP.CamlQuery();
            query.set_viewXml("<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Text'>" + Id + "</Value></Eq></Where></Query></View>");
            var Instance = List.getItems(query);
            ctx.load(Instance, "Include(IssueMeetingMVK)");
            ctx.executeQueryAsync(function () {
                if (!Instance.get_data()[0]) {
                    console.error('Не удалось запросить данные вопроса повестки');
                    if (onfail) {
                        onfail();
                    }
                    return;
                }

                if (onsuccess) {
                    onsuccess(Instance.get_data()[0]);
                }

            }, function () {
                console.error('Не удалось запросить данные вопроса повестки');
                if (onfail) {
                    onfail();
                }
            });
        }, 'sp.js');
    }

    function getAssignmentData(Id, onsuccess, onfail) {
        // получим данные поручения
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            var assignmentId = Id;
            var ctx = SP.ClientContext.get_current();
            var assignmentList = ctx.get_web().get_lists().getByTitle("МВК: Поручения");
            var query = new SP.CamlQuery();
            query.set_viewXml("<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Text'>" + assignmentId + "</Value></Eq></Where></Query></View>");
            var assignmentInstance = assignmentList.getItems(query);
            ctx.load(assignmentInstance, "Include(AssignmentIssueMVK, AssignmentTextMVK, AssignmentNumberMVK)");
            ctx.executeQueryAsync(function () {
                if (!assignmentInstance.get_data()[0]) {
                    console.error('Не удалось запросить данные поручения');
                    if (onfail) {
                        onfail();
                    }
                    return;
                }

                if (onsuccess) {
                    onsuccess(assignmentInstance.get_data()[0]);
                }
                
            }, function () {
                console.error('Не удалось запросить данные поручения');
                if (onfail) {
                    onfail();
                }
            });
        }, 'sp.js');
    }

    function OnPostRender(context) {
        var deadlineTriggeredValue = "Перенести срок";
        var changeRequestTriggeredValue = "Отправить на доработку";

        // автор, редактор
        var prefix = context.FormUniqueId + context.FormContext.listAttributes.Id;
        renderCore.ifget(prefix + 'Author', function (e) { e.innerHTML = author; });
        renderCore.ifget(prefix + 'Created', function (e) { e.innerHTML = created; });
        renderCore.ifget(prefix + 'Editor', function (e) { e.innerHTML = editor; });
        renderCore.ifget(prefix + 'Modified', function (e) { e.innerHTML = modified; });

		//Если резолюция уже проставлена, то запрещаем добавление материалов
		isCancelAddAttach = context.ListData.Items[0].AssignmentReportResolutionDecision != '';
		
        // текст ссылки на поручение заменяем на форматированный текст
        if (context.ControlMode === SPClientTemplates.ClientControlMode.DisplayForm) {
            // в режиме просмотра резолюция, новый срок и комментарий к резолюции доступны всем
            renderCore.ifget('decision', function (e) {
                $(e).css('display', 'block');

                if (context.ListData.Items[0].ReportDecisionMVK == deadlineTriggeredValue) {
                    renderCore.ifget('newdeadline', function (e) {
                        $(e).css('display', 'block');
                    });
                    renderCore.ifget('changeRequestComment', function (e) {
                        $(e).css('display', 'block');
                    });
                }

                if (context.ListData.Items[0].ReportDecisionMVK == changeRequestTriggeredValue) {
                    renderCore.ifget('changeRequestComment', function (e) {
                        $(e).css('display', 'block');
                    });
                }
            });


            getAssignmentData(context.ListData.Items[0].ReportAssignmentMVK.split(';')[0], function (e) {
                var questionNumber = e.get_item("AssignmentIssueMVK").get_lookupValue();
                var assignmentText = e.get_item("AssignmentTextMVK"); 
                var assignmentNumber = e.get_item("AssignmentNumberMVK");;

                getQuestionData(e.get_item("AssignmentIssueMVK").get_lookupId(), function (e) {
                    var meetingNumber = e.get_item("IssueMeetingMVK").get_lookupValue();
                    // текст ссылки на поручение
                    $('#AssignmentLinkLink').text((String).format(
                        "Заседание №{0} - Вопрос №{1} - Поручение №{2}",
                        meetingNumber,
                        questionNumber,
                        assignmentNumber));
                    // текст поручения
                    if (assignmentText) {
                        $('#LinkedAssignmentText').html(assignmentText);
                    }
                });
            });
            
            return;
        };

        // для резолюции зарегистрируем свой обработчик
        $('#' + resolutionId.replace(/(:|\.|\[|\]|\$)/g, "\\$1")).on("change", function (event) {
            if (this.value == deadlineTriggeredValue) {
                renderCore.ifget('newdeadline', function (e) {
                    $(e).css('display', 'block');
                });
                renderCore.ifget('changeRequestComment', function (e) {
                    $(e).css('display', 'block');
                });
            } else if (this.value == changeRequestTriggeredValue) {
                renderCore.ifget('newdeadline', function (e) {
                    $(e).css('display', 'none');
                });
                renderCore.ifget('changeRequestComment', function (e) {
                    $(e).css('display', 'block');
                });
            } else {
                renderCore.ifget('newdeadline', function (e) {
                    $(e).css('display', 'none');
                });
                renderCore.ifget('changeRequestComment', function (e) {
                    $(e).css('display', 'none');
                });
            }

            return true;

        });

        // для поручения зарегистрируем свой обработчик
        $('#' + assignmentLinkId.replace(/(:|\.|\[|\]|\$)/g, "\\$1")).on("change", function (event) {
            if (this.value) {
                getAssignmentData(this.value, function (e) {
                    $('#LinkedAssignmentText').html(e.get_item("AssignmentTextMVK"));
                });
            }
            else {
                $('#LinkedAssignmentText').html("");
            }

            return true;
        });

        // при создании нового элемента в случае наличия контекста устанавливаем 
        // значение связанного поручения
        if (context.ControlMode === SPClientTemplates.ClientControlMode.NewForm) {
            var hasContext = document.referrer && ~document.referrer.indexOf('Lists/AssignmentMVKList');
            if (hasContext) {
                var params = document.referrer.split('?')[1].split('&');
                for (var i = 0; i < params.length; i++) {
                    var param = params[i].split('=');
                    if (param[0] !== 'ID') continue;
                    $('#' + assignmentLinkId.replace(/(:|\.|\[|\]|\$)/g, "\\$1")).val(param[1]);
                    // заодно инициализируем текст поручения
                    getAssignmentData(param[1], function (e) {
                        $('#LinkedAssignmentText').html(e.get_item("AssignmentTextMVK"));
                    });
                    break;
                }
            }
        }

        // для режима редактирования также проинициализируем текст поручения
        if (context.ControlMode === SPClientTemplates.ClientControlMode.EditForm) {
            if (context.ListData.Items[0].ReportAssignmentMVK) {
                getAssignmentData(context.ListData.Items[0].ReportAssignmentMVK.split(';')[0], function (e) {
                    $('#LinkedAssignmentText').html(e.get_item("AssignmentTextMVK"));
                });
            }
        }

        // поле резолюции доступно только определенной группе пользователей во всех 
        // режимах кроме просмотра
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            var ctx = SP.ClientContext.get_current();
            var groups = ctx.get_web().get_currentUser().get_groups();
            ctx.load(groups);
            ctx.executeQueryAsync(function () {
                var found = false;

                for (var i = 0; i < groups.get_count() ; i++) {
                    if (groups.get_item(i).get_title() !== "ГрадСовет - Редакторы") continue;
                    found = true;
                    break;
                }

                if (found) {
                    renderCore.ifget('decision', function (e) {
                        $(e).css('display', 'block');

                        // также нужно инициализировать поле нового срока
                        if ($('#' + resolutionId.replace(/(:|\.|\[|\]|\$)/g, "\\$1")).val() == deadlineTriggeredValue) {
                            renderCore.ifget('newdeadline', function (e) {
                                $(e).css('display', 'block');
                            });
                            renderCore.ifget('changeRequestComment', function (e) {
                                $(e).css('display', 'block');
                            });
                        }

                        // ... и поле комментария
                        if ($('#' + resolutionId.replace(/(:|\.|\[|\]|\$)/g, "\\$1")).val() == changeRequestTriggeredValue) {
                            renderCore.ifget('changeRequestComment', function (e) {
                                $(e).css('display', 'block');
                            });
                        }
                    });
                }
            }, function () {
                console.error('Не удалось проверить группы доступа пользователя');
            });
        }, 'sp.js');
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        
        init();

        // MDS
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            
            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/_layouts/15/SAMRT.Web/Scripts/csr/renderAssignmentMVK.js"), init);

        }, 'sp.js');

    }, 'clienttemplates.js');
})();

//Настройка интерфейса
$(function () {
	if (isCancelAddAttach) {
		$(".ms-addnew").css("display", "none");
		$("#idHomePageNewItem").css("display", "none");
	}
    //Устанавливаем нужный класс для стандартных кнопок
    $('#buttons input[type="button"]').attr('class', 'form-control');
    //Убираем лишний отступ у кнопок MultipleValueLookup
    $('[type="button"][id$="Button"]').css("margin", 0);
    //Увеличиваем ширину MultipleValueLookup
    $('table[id$="MultiLookup_topTable"]').css("width", "100%").find('select').parent().css("width", "50%").children().css("width", "100%");
});
