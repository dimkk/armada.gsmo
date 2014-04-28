(function () {

    var consts = {
        fieldName: "_x0420__x043e__x0434__x0438__x04",
        parentListName: "Lists/Object",
        dlgUrl: "/_layouts/15/gradsovetpages/pages/selectObject.html"
    };

    function init() {
        var parentObjectField = new gsFields.CSR.LinkedField(
            consts.fieldName,
            true,
            consts.parentListName,
            consts.dlgUrl,
            renderPresentationContainerValue);

        parentObjectField.init();
    }

    function renderPresentationContainerValue(selectedObject) {
        var html = "";

        html +=
            "<ul>";
        html += (String).format(
                "<li><b>{0}</b>: {1}</li>", "Наименование", selectedObject.objectTitle);
        html += selectedObject.objectAddress ? (String).format(
                "<li><b>{0}</b>: {1}</li>", "Адрес", selectedObject.objectAddress) : "";
        html += selectedObject.objectBuilder ? (String).format(
                "<li><b>{0}</b>: {1}</li>", "Застройщик", selectedObject.objectBuilder.get_lookupValue()) : "";
        html += selectedObject.objectCadastre ? (String).format(
                "<li><b>{0}</b>: {1}</li>", "Кад. номер", selectedObject.objectCadastre) : "";
        html += selectedObject.objectArea ? (String).format(
                "<li><b>{0}</b>: {1}</li>", "Площадь ЗУ", selectedObject.objectArea) : "";
        html +=
            "</ul>";

        return html;
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
        // MDS
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~/_layouts/15/gradsovetpages/Scripts/csr/renderObjectLinkField.js"), init);
        }, "sp.js");
    }, "clienttemplates.js");
})();