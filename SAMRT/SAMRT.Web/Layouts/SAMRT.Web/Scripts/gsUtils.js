var gsUtils;

(function (gsUtils) {

    gsUtils.getSelectedWebPartId = function () {
        var e = document.getElementById("_wpSelected");
        if (!e) return null;

        var selectedId = e.getAttribute("value");
        if (!selectedId) return null;

        selectedId = selectedId.substr(12);
        e = document.getElementById(selectedId);
        if (!e) return null;

        var wpId;
        if (window._spWebPartComponents &&
            window._spWebPartComponents[selectedId]) {
            wpId = window._spWebPartComponents[selectedId].storageId;
        }
        else {
            wpId = e.getAttribute("WebPartID");
        }

        return wpId;
    };

    gsUtils.getURLParam = function (url, paramName) {
        var splitted = url.split('?');
        var params = splitted[1] ? splitted[1].split('&') : null;
        if (!params) return null;
        
        for (var i = 0; i < params.length; i++) {
            var p = params[i].split('=');
            if (p[0] !== paramName) continue;

            return p[1];
        }

        return null;
    };

    gsUtils.getIDParamFromUrl = function(url) {
        return gsUtils.getURLParam(url, "ID");
    };

})(gsUtils || (gsUtils = {}));