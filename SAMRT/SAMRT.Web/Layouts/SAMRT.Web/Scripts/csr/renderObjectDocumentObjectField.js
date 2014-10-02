(function () {
    var consts = {
        fieldName:      "ObjectLink",
        parentListName: "Object",
        dlgUrl:         "/_layouts/15/SAMRT.Web/pages/selectObject.html"
    };

    function init() {
        var objectField = new gsFields.CSR.LinkedField(consts.fieldName, true, consts.parentListName, consts.dlgUrl, function (value) {
            var html = "";

            html +=
                "<ul>";
            html += (String).format(
                    "<li><b>{0}</b>: {1}</li>", "Наименование", value.objectTitle);
            html += value.objectAddress ? (String).format(
                    "<li><b>{0}</b>: {1}</li>", "Адрес", value.objectAddress) : "";
            html += value.objectBuilder ? (String).format(
                    "<li><b>{0}</b>: {1}</li>", "Застройщик", value.objectBuilder.get_lookupValue()) : "";
            html += value.objectCadastre ? (String).format(
                    "<li><b>{0}</b>: {1}</li>", "Кад. номер", value.objectCadastre) : "";
            html += value.objectArea ? (String).format(
                    "<li><b>{0}</b>: {1}</li>", "Площадь ЗУ", value.objectArea) : "";
            html +=
                "</ul>";

            return html;
        });

        objectField.init();
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
        // MDS
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~/_layouts/15/SAMRT.Web/Scripts/csr/renderObjectDocumentObjectField.js"), init);
        }, "sp.js");
    }, "clienttemplates.js");
})();
