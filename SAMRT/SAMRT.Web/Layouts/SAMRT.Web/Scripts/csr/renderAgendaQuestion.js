/// <reference path="renderCore.js" />
/// <reference path="../SP.debug.js" />
/// <reference path="../SP.Core.debug.js" />
/// <reference path="../SP.runtime.debug.js" />

(function () {

    var author, editor, created, modified;
    var exceptList = [];
    var renderCore;

	function getMeetingControl() {
		return renderCore.getControlByFieldName('MeetingLink');
	}
	
	function getNumberControl() {
		return renderCore.getControlByFieldName('AgendaQuestionNumber');
	}
	
    function init() {
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            Templates: {
                Item: renderFields
            },
            OnPostRender: OnPostRender,
            ListTemplateType: 10000,
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
        resultHtml += renderFieldBlock(context, 2, 10, "AgendaQuestionAddress");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 10, "AgendaQuestionDescription");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "AgendaQuestionInvestor");
        resultHtml += renderFieldBlock(context, 2, 4, "IssueProjectOrgAk");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "CadastreNumber");
        resultHtml += renderFieldBlock(context, 2, 4, "QuestionCategoryLink");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "IssueReporterAk");
        //resultHtml += renderFieldBlock(context, 2, 5, "AgendaQuestionSoreporterFullNameLink");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "_x041c__x0443__x043d__x0438__x04");
        resultHtml += renderFieldBlock(context, 2, 4, "_x041f__x043e__x0441__x0435__x04");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 10, "AgendaQuestionProtocolDecision");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
		resultHtml += renderFieldBlock(context, 2, 4, "AgendaQuestionNumber");
		resultHtml += renderFieldBlock(context, 2, 4, "MeetingLink");
        resultHtml += '</div>';

		if (context.ControlMode === SPClientTemplates.ClientControlMode.DisplayForm) {
			resultHtml += '<div class="form-group">';
			resultHtml += renderFieldBlock(context, 2, 10, "AgendaQuestionIssue");
			resultHtml += '</div>';
		}
		
        author = context.RenderFieldByName(context, "Author");
        exceptList.push("Author");
        created = context.RenderFieldByName(context, "Created");
        exceptList.push("Created");
        editor = context.RenderFieldByName(context, "Editor");
        exceptList.push("Editor");
        modified = context.RenderFieldByName(context, "Modified");
        exceptList.push("Modified");

        resultHtml += '</div>'; //form-horizontal
        resultHtml += renderItemFooter(context);

        return resultHtml;
    }

	function fillNumber() {
		renderCore.loadLists(function() {
			console.log('Lists initialized');
			var issueList = renderCore.getListByUrl('AgendaQuestionList');
			var meetingId = getMeetingControl().val();
			var numberControl = getNumberControl();
			numberControl.val('');
			if (!meetingId || meetingId == 0)
				return;
			
			var query = new SP.CamlQuery();
			query.set_viewXml(String.format("<View><Query><Where><Eq><FieldRef Name='{0}' LookupId='TRUE'/><Value Type='Lookup'>{1}</Value></Eq></Where><OrderBy><FieldRef Name='{2}' Ascending='FALSE'/></OrderBy></Query><RowLimit>1</RowLimit></View>", 'MeetingLink', meetingId, 'AgendaQuestionNumber'));
			var issueMaxNumber = issueList.getItems(query);
			
			renderCore.spctx.load(issueMaxNumber, 'Include(AgendaQuestionNumber)');
			renderCore.spctx.executeQueryAsync(function() {
					console.log('issueMaxNumber loaded');
					var issueNumber = issueMaxNumber.get_count() > 0 ? issueMaxNumber.getItemAtIndex(0).get_item('AgendaQuestionNumber') + 1 : 1;
					console.log(issueNumber);
					numberControl.val(issueNumber);
				}
				,function (sender, args) {
                    alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
				}
			)
		});
	}
	
    function OnPostRender(context) {
		var meetingId = renderCore.getParentListItemId(['/Lists/MeetingList/EditForm', '/Pages/MeetingAk.aspx']);
		var meetingControl = getMeetingControl();
		meetingControl.change(function() {
			fillNumber();
		});
		var numberControl = getNumberControl();
		numberControl.attr('disabled', 'disabled');
		if (context.ControlMode === SPClientTemplates.ClientControlMode.NewForm && meetingId) {
			meetingControl.val(meetingId);
			meetingControl.attr('disabled', 'disabled');
			console.log("Установлено поле заседания");
		}
	
        var prefix = context.FormUniqueId + context.FormContext.listAttributes.Id;
        $get(prefix + 'Author').innerHTML   = author;
        $get(prefix + 'Created').innerHTML  = created;
        $get(prefix + 'Editor').innerHTML   = editor;
        $get(prefix + 'Modified').innerHTML = modified;
    }

    function createLabelMarkup(value, span) {
        return '<label class="col-lg-' + span + '">' + value + '</label>';
    }

    function renderFieldBlock(context, labelSpan, inputSpan, fieldName) {
        var resultHtml = '';
        resultHtml += createLabelMarkup(renderCore.getFieldTitle(fieldName), labelSpan);
        resultHtml += '<div class="col-lg-' + inputSpan + '">';
        resultHtml += renderField(context, fieldName);
        resultHtml += '</div>';
        exceptList.push(fieldName);

        return resultHtml;		
    }

    function renderField(context, fieldname) {
        var html = context.RenderFieldByName(context, fieldname);
        var controlMode = context.FieldControlModes[fieldname];

        if (controlMode == SPClientTemplates.ClientControlMode.DisplayForm) {
            return html;
        }

        var container = document.createElement("div");
        container.innerHTML = html;
        $.each(container.getElementsByTagName("textarea"), function () {
            this.className = 'form-control';
        });
        $.each(container.querySelectorAll('input:not([type="checkbox"])'), function () {
            this.className = 'form-control';
        });
        $.each(container.getElementsByTagName("select"), function () {
            this.className = 'form-control';
        });

        return container.innerHTML;
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
        // MDS
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/_layouts/15/SAMRT.Web/Scripts/csr/renderAgendaQuestion.js"), init);
			if (!getNumberControl().val()) {
				fillNumber();
			}
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
