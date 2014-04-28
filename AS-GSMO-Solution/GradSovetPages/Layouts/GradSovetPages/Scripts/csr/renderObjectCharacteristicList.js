(function () {

    var fieldNames = {
        deviation:      "_x0425__x0430__x0440__x0430__x04",
        value:          "_x0417__x043d__x0430__x0447__x04",
        standardValue:  "_x0417__x043d__x0430__x0447__x040"
    };

    var deviationValues = {
        theBiggerTheBetter: "Чем больше тем ЛУЧШЕ",
        theBiggerTheWorse:  "Чем больше тем ХУЖЕ",
        noEstimate:         "Без оценки"
    };

    var deviationColors = {        
        noEstimate: "rgb(255, 255, 255)",
        better:     "rgb(176, 255, 200)",
        worse:      "rgb(255, 186, 193)"
    };
    
    function init() {
        var viewRenderTemplate = {
            Templates: {
            },
            OnPostRender: highLightRows,
            ListTemplateType: 100, // !!!
            BaseViewID: 1
        };

        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(viewRenderTemplate);
    }

    function calculateColor(row) {
        var bColor = null;

        var deviation = row[fieldNames.deviation];
        if (!deviation || deviation == deviationValues.noEstimate) return null;

        var value = row[fieldNames.value] || 0;
        var stValue = row[fieldNames.standardValue] || 0;

        switch (deviation) {
            case deviationValues.theBiggerTheBetter:
                {
                    bColor = value >= stValue ? deviationColors.better : deviationColors.worse;
                    break;
                }
            case deviationValues.theBiggerTheWorse:
                {
                    bColor = value > stValue ? deviationColors.worse : deviationColors.better;
                    break;
                }
            default:
                {
                    bColor = deviationColors.noEstimate;
                    break;
                }
        }
        return bColor;
    }        

    function highLightRow(ctx, row) {
        var iid = GenerateIIDForListItem(ctx, row);
        var el = document.getElementById(iid);
        if (el) {
            var bColor = calculateColor(row);
            if (bColor) {
                el.style.backgroundColor = bColor;
            }
        }
    }
    
    function highLightRows(ctx) {
        for (var i = 0; i < ctx.ListData.Row.length; i++) {
            var row = ctx.ListData.Row[i];
            highLightRow(ctx, row);
        }
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
    }, "clienttemplates.js");
})();