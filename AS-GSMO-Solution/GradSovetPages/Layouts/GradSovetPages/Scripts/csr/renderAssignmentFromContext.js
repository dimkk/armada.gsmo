/// <reference path="renderCore.js" />
/// <reference path="../SP.debug.js" />
/// <reference path="../SP.Core.debug.js" />
/// <reference path="../SP.runtime.debug.js" />
/// <reference path="../clienttemplates.debug.js" />

(function () {

    function init() {
        var hasContext = document.referrer && (~document.referrer.indexOf('Lists/List/DispForm') ||
            ~document.referrer.indexOf('Lists/List/EditForm'));
        if (!hasContext) return;

        // регистрируем шаблон только в случае открытия новой формы из контекста формы заявки
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            Templates: {
                Fields: { 'OrderLink': { 'NewForm': renderOrderLink } },
                OnPostRender: OnPostRender
            }
        });
    }

    function renderOrderLink(ctx) {
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
        formCtx.registerGetValueCallback(formCtx.fieldName, getOrderLinkId);

        return (String).format("<div id='{0}'></div>", formCtx.fieldName);
    }

    function getOrderLinkId() {
        var params = document.referrer.split('?')[1].split('&');
        for (var i = 0; i < params.length; i++) {
            var param = params[i].split('=');
            if (param[0] !== 'ID') continue;

            return param[1];
        }
        return null;
    }

    function OnPostRender(ctx) {
        var parentId = getOrderLinkId();
        if (parentId) {
            $('#OrderLink').html(
                (String).format("Идентификатор заявки {0}", parentId));
        }

        $('#OrderLink').closest('tr').css('display', 'none');
    }


    SP.SOD.executeOrDelayUntilScriptLoaded(function () {

        init();

        // MDS
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {

            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/SiteAssets/renderAssignmentFromContext.js"), init);

        }, 'sp.js');

    }, 'clienttemplates.js');
})();