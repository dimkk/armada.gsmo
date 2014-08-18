(function () {

    var author, editor, created, modified;

    function init() {
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            Templates: {
                Item: renderFields
            },
            OnPostRender: onPostRender,
            ListTemplateType: 100,
            BaseViewID: "DisplayForm"
        });
    }

    function renderItemHeader(context) {
        var resultHtml = '';
        resultHtml += '<div>';
        resultHtml += '<table class="ms-formtable" border="0" cellspacing="0" cellpadding="0">';

        return resultHtml;
    }
    
    function renderFieldRow(context, field) {
        var resultHtml = '<tr>';
        resultHtml += '<td width="113" class="ms-formlabel" nowrap="true" valign="top"><h3 class="ms-standardheader"><nobr>';
        resultHtml += field.Title;
        if (field.Required && context.FieldControlModes[field.Name] != SPClientTemplates.ClientControlMode.DisplayForm) {
            resultHtml += (String).format('<span title="{0}" class="ms-accentText"> *</span>', Strings.STS.L_RequiredField_Tooltip);
        }
        resultHtml += '</nobr></h3></td>';
        resultHtml += (String).format('<td width="350" class="ms-formbody" valign="top">{0}</td>', context.RenderFieldByName(context, field.Name));
        resultHtml += '</tr>';
        return resultHtml;
    }

    function renderItemFooter(context) {
        var resultHtml = '';
        resultHtml += '</table>';
        resultHtml += '</div>';

        return resultHtml;
    }

    function renderFields(context) {

        var resultHtml = '';
        resultHtml += renderItemHeader(context);

        var fields = context.ListSchema.Field;
        for (var i = 0; i < fields.length; i++) {
            if (fields[i].Name != "Modified" &&
                fields[i].Name != "Editor" &&
                fields[i].Name != "Created" &&
                fields[i].Name != "Author" &&
                context.CurrentItem[fields[i].Name]) {
                resultHtml += renderFieldRow(context, fields[i]);
            }
        }

        author = context.RenderFieldByName(context, "Author");
        created = context.RenderFieldByName(context, "Created");
        editor = context.RenderFieldByName(context, "Editor");
        modified = context.RenderFieldByName(context, "Modified");

        resultHtml += renderItemFooter(context);

        return resultHtml;
    }

    function onPostRender(context) {
        var prefix = context.FormUniqueId + context.FormContext.listAttributes.Id;
        $get(prefix + 'Author').innerHTML = author;
        $get(prefix + 'Created').innerHTML = created;
        $get(prefix + 'Editor').innerHTML = editor;
        $get(prefix + 'Modified').innerHTML = modified;
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
        // MDS
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/_layouts/15/SAMRT.Web/Scripts/csr/renderObjectForm.js"), init);
        }, 'sp.js');
    }, 'clienttemplates.js');
})();