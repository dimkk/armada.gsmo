/// <reference path="renderCore.js" />
/// <reference path="../SP.debug.js" />
/// <reference path="../SP.Core.debug.js" />
/// <reference path="../SP.runtime.debug.js" />

(function () {

    var author, editor, created, modified;
    var exceptList = [];
    var renderCore;

    function init() {
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            Templates: {
                Item: renderFields
            },
            OnPostRender: OnPostRender,
            ListTemplateType: 10051,
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
        resultHtml += renderFieldBlock(context, 'Описание', 2, 10, "IssueDescriptionMVK");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Инвестор', 2, 10, "IssueInvestorMVK");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Кадастровый номер', 2, 10, "IssueCadastreIdMVK");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Категория вопроса', 2, 4, "IssueCategoryMVK");
        resultHtml += renderFieldBlock(context, 'Тип решения', 2, 2, "IssueDecisionTypeMVK");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Докладчик', 2, 4, "IssueReporterMVK");
        resultHtml += renderFieldBlock(context, 'Содокладчики', 2, 4, "IssueCoReportersMVK");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Муниципальный район', 2, 4, "IssueMunicipalDistrictMVK");
        resultHtml += renderFieldBlock(context, 'Поселение', 2, 4, "IssueSettlementMVK");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Решение', 2, 10, "IssueDecisionMVK");
        resultHtml += '</div>';

		if (context.ControlMode != SPClientTemplates.ClientControlMode.DisplayForm) {
			resultHtml += '<div class="form-group">';
			resultHtml += renderFieldBlock(context, 'Адрес', 2, 10, "IssueAddressMVK");
			resultHtml += '</div>';

			resultHtml += '<div class="form-group">';
			resultHtml += renderFieldBlock(context, 'Тема', 2, 10, "IssueThemeMVK");
			resultHtml += '</div>';
			
			resultHtml += '<div class="form-group">';
			resultHtml += renderFieldBlock(context, 'Номер', 2, 4, "IssueNumberMVK");
			resultHtml += renderFieldBlock(context, 'Подномер', 2, 4, "IssueSubnumberMVK");
			resultHtml += '</div>';
		}

        resultHtml += '<div class="form-group">';
		if (context.ControlMode != SPClientTemplates.ClientControlMode.DisplayForm) {
			resultHtml += renderFieldBlock(context, 'Заседание', 2, 4, "IssueMeetingMVK");
		}
        resultHtml += renderFieldBlock(context, 'Заявка', 2, 4, "IssueOrderMVK");
        resultHtml += '</div>';
        
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

    function OnPostRender(context) {
        var prefix = context.FormUniqueId + context.FormContext.listAttributes.Id;
        $get(prefix + 'Author').innerHTML   = author;
        $get(prefix + 'Created').innerHTML  = created;
        $get(prefix + 'Editor').innerHTML   = editor;
        $get(prefix + 'Modified').innerHTML = modified;
    }

    function createLabelMarkup(value, span) {
        return '<label class="col-lg-' + span + '">' + value + '</label>';
    }

    function renderFieldBlock(context, label, labelSpan, inputSpan, fieldName) {
        var resultHtml = '';
        resultHtml += createLabelMarkup(label, labelSpan);
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

            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/_layouts/15/SAMRT.Web/Scripts/csr/renderIssueMVK.js"), init);

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