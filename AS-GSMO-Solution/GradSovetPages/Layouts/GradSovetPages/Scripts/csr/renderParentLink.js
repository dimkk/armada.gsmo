/// <reference path="renderCore.js" />
/// <reference path="../SP.debug.js" />
/// <reference path="../SP.Core.debug.js" />
/// <reference path="../SP.runtime.debug.js" />
/// <reference path="../clienttemplates.debug.js" />

(function () {

    function init() {
        if (!gsRenderParams) return;
        var hasContext = document.referrer &&
            (~document.referrer.indexOf(gsRenderParams.parentListName + '/DispForm') ||
            ~document.referrer.indexOf(gsRenderParams.parentListName  + '/EditForm'));
        if (!hasContext) return;

        // регистрируем шаблон только в случае открытия новой формы из контекста формы родителя
        var renderFields = {};
        renderFields[gsRenderParams.parentLinkFieldName] = { 'NewForm': renderLink };

        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            Templates: {
                Fields: renderFields,
                OnPostRender: OnPostRender
            }
        });
    }
    
    function renderLink(ctx) {
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
        formCtx.registerGetValueCallback(formCtx.fieldName, getLinkId);

        return (String).format("<div id='{0}'></div>", gsRenderParams.parentLinkFieldName);
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

    function OnPostRender(ctx) {
        var parentId = getLinkId();
        if (parentId) {
            $('#' + gsRenderParams.parentLinkFieldName).html(
                (String).format("Идентификатор родителя {0}", parentId));
        }

        $('#' + gsRenderParams.parentLinkFieldName).closest('tr').css('display', 'none');
    }


    SP.SOD.executeOrDelayUntilScriptLoaded(function () {

        init();

        // MDS
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {

            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/_layouts/15/gradsovetpages/Scripts/csr/renderParentLink.js"), init);

        }, 'sp.js');

    }, 'clienttemplates.js');
})();


// usage
/*

1. Click "Edit Page" on target page in browser 
2. Add script editor webpart on this page
3. Click "Edit fragment"
4. Insert the code below

<script type="text/javascript">
   var gsRenderParams = { parentLinkFieldName: "AgendaQuestionLink", parentListName: "AgendaQuestionList" };
</script>
<script type="text/javascript" src="/_layouts/15/gradsovetpages/Scripts/csr/renderParentLink.js?rev=25122013"></script>

5. parentLinkFieldName & parentListName must have according values

*/