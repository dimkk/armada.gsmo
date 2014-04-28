/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts"/>
$(document).ready(() => {
    var el = $("#DeltaPlaceHolderMain");
    var parent = $("#DeltaPlaceHolderMain").parent();

    if (el && parent) {
        $.each(el.find("table"), (i: number, el: any) => {
            if (el.id) {
                $(el).addClass("table");
                $("th:nth-child(1)", el).remove();
                $("td:nth-child(1)", el).remove();
                return false;
            }
        });

        parent.removeClass();
    } 
})