/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts"/>
$(document).ready(function () {
    var el = $("#DeltaPlaceHolderMain");
    var parent = $("#DeltaPlaceHolderMain").parent();

    if (el && parent) {
        $.each(el.find("table"), function (i, el) {
            if (el.id) {
                $(el).addClass("table");
                $("th:nth-child(1)", el).remove();
                $("td:nth-child(1)", el).remove();
                return false;
            }
        });

        parent.removeClass();
    }
});
