(function () {
    
    // список является атрибутивным как для списка вопросов так и для списка объектов, 
    // соответственно, карточка сущности может быть открыта в разных контекстах
    var ModeEnum = {
        Undef:      0, 
        Question:   1, // конекст вопроса
        Object:     2  // контекст объекта
    };
    
    var mode = ModeEnum.Undef;
    
    var fields = {
        question: "_x0412__x043e__x043f__x0440__x04",
        object: "_x041e__x0431__x044a__x0435__x04"
    };

    function init() {
        var renderFields = {
            "Title": {
                "NewForm":      renderTitleField,
                "EditForm":     renderTitleField,
                "DisplayForm":  renderTitleField
            }
        };

        // при наличии родительского контекста рендеринг полей за нами
        mode = window.renderCore && window.renderCore.hasParentContext("Lists/AgendaQuestionList") ? ModeEnum.Question :
            (window.renderCore && window.renderCore.hasParentContext("Lists/Object") ? ModeEnum.Object : ModeEnum.Undef);
        
        if (mode != ModeEnum.Undef) {
            renderFields[fields.question] = {
                "NewForm":  renderQuestionLinkField,
                "EditForm": renderQuestionLinkField
            };
            renderFields[fields.object] = {
                "NewForm":  renderObjectLinkField,
                "EditForm": renderObjectLinkField 
            };
        }

        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            Templates: {
                Fields: renderFields
            },
            OnPostRender: OnPostRender,
            ListTemplateType: 100, //!!!
        });
    }

    function renderTitleField(ctx) {
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
        formCtx.registerGetValueCallback(formCtx.fieldName, getTitleValue);

        return (String).format("<div id='{0}'></div>", formCtx.fieldName);
    }

    function getTitleValue() {
        return "Просмотреть";
    }

    function renderQuestionLinkField(ctx) {
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
        if (mode == ModeEnum.Question) {
            formCtx.registerGetValueCallback(formCtx.fieldName, getLinkId);
        }

        return (String).format("<div id='{0}'></div>", formCtx.fieldName);
    }

    function getLinkId() {
        return window.renderCore ? window.renderCore.getParentIdFromContext("Lists/AgendaQuestionList") : null;
    }
    
    function renderObjectLinkField(ctx) {
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
        if (mode == ModeEnum.Object) {
            formCtx.registerGetValueCallback(formCtx.fieldName, getObjectLinkId);
        }

        return (String).format("<div id = '{0}'></div>", formCtx.fieldName);
    }
    
    function getObjectLinkId() {
        return window.renderCore ? window.renderCore.getParentIdFromContext("Lists/Object") : null;
    }

    function OnPostRender(context) {
        var hideFunc = function(e) {
            $(e).closest("tr").css("display", "none");
        };
        
        if (window.renderCore) {
            window.renderCore.ifget("Title", hideFunc);

            if (context.ControlMode !== SPClientTemplates.ClientControlMode.DisplayForm) {
                window.renderCore.ifget(fields.question, hideFunc);
                window.renderCore.ifget(fields.object, hideFunc);
            }
        }
    }
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
        // MDS
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/_layouts/15/gradsovetpages/Scripts/csr/renderQuestionSense.js"), init);
        }, 'sp.js');
    }, 'clienttemplates.js');
})();