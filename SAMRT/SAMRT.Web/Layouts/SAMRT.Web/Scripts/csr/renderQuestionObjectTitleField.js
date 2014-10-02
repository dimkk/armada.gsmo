(function () {

    var guiId = Math.random().toString(36).substr(2);
    
    function init() {

        var fieldTemplate = {            
          Templates: {
              Fields: {
                  "Title": {
                      "NewForm": renderField,
                      "EditForm": renderField,
                      "DisplayForm": renderField
                  }
              }
          },
          OnPostRender: onPostRender
        };

        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(fieldTemplate);
    }
    
    function renderField(ctx) {
        return (String).format("<div id='{0}'></div>", "Title_" + guiId);
    }
    
    function onPostRender(ctx) {
        var fieldName = ctx.ListSchema.Field && ctx.ListSchema.Field[0] && ctx.ListSchema.Field[0].Name;
        if (fieldName !== "Title") return;
        
        $('[id=Title_' + guiId + ']').closest('tr').css('display', 'none');
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
        // MDS
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~/_layouts/15/SAMRT.Web/Scripts/csr/renderQuestionObjectTitleField.js"), init);
        }, "sp.js");
    }, "clienttemplates.js");
})();
