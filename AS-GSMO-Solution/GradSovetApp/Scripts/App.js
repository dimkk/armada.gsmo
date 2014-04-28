'use strict';

jQuery.extend({
    getUrlVars: function () {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    },
    getUrlVar: function (name) {
        return jQuery.getUrlVars()[name];
    }
});

$(document).ready(function () {
    $.hostweburl = decodeURIComponent($.getUrlVar("SPHostUrl"));
    $.pageMode = decodeURIComponent($.getUrlVar("mode"));
    $.backUrl = decodeURIComponent($.getUrlVar("Source"));
    $.listItemId = decodeURIComponent($.getUrlVar("ID"));
    $.appWebContext = SP.ClientContext.get_current();
    // in case if we don't have SPHostUrl param
    if ($.hostweburl != "undefined") {
        $.hostWebContext = new SP.AppContextSite($.appWebContext, $.hostweburl);
    }
});

$(function () {
    /*
    $("#tabs").tabs();
    $("#assignmentFactDate, #assignmentPlanDate").datepicker({
        numberOfMonths: 3,
        showButtonPanel: true
    });
    var icons = {
        header: "ui-icon-circle-arrow-e",
        activeHeader: "ui-icon-circle-arrow-s"
    };
    $("#accordionAssignments, #accordionProtocol").accordion({
        icons: icons,
        collapsible: true,
        heightStyle: "content"
    });

    $("input[type=submit], button")
        .button()
        .click(function (event) {
            event.preventDefault();
        });

    $(function () {
        $("#dialog-remove-agendaquestion-message").dialog({
            modal: true,
            buttons: {
                Ok: function () {
                    $(this).dialog("close");
                }
            }
        });
    });*/
    
    // локализация datepicker
    moment.lang('ru');

    // локализация jquery datepicker
    $.datepicker.regional['ru'] = {
        closeText: 'Закрыть',
        prevText: '&#x3c;Пред',
        nextText: 'След&#x3e;',
        currentText: 'Сегодня',
        monthNames: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
        monthNamesShort: ['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн', 'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'],
        dayNames: ['воскресенье', 'понедельник', 'вторник', 'среда', 'четверг', 'пятница', 'суббота'],
        dayNamesShort: ['вск', 'пнд', 'втр', 'срд', 'чтв', 'птн', 'сбт'],
        dayNamesMin: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
        weekHeader: 'Нед',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['ru']);

    $.assguid = 0;
})