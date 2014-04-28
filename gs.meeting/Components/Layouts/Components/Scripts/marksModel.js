/// <reference path="../../../Scripts/typings/sharepoint/SharePoint.d.ts"/>
/// <reference path="../../../Scripts/typings/knockout/knockout.d.ts"/>
/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts"/>
/// <reference path="../../../Scripts/typings/camljs/camljs.d.ts"/>
/// <reference path="gsCore.ts"/>
'use strict';
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var MarksModule;
(function (MarksModule) {
    var MarkRecord = (function (_super) {
        __extends(MarkRecord, _super);
        function MarkRecord() {
            _super.apply(this, arguments);
        }
        MarkRecord.prototype.getFormattedTitle = function () {
            // trying to find curly braces
            var re = new RegExp(/\{([0-9]+)\}/g);
            if (re.test(this.Title)) {
                var replaceValue = this.MarkSumValue ? this.MarkSumValue.toString() : "нет значения";
                replaceValue = "<span class='label label-primary font-larger'>" + gsCore.Math.formatThousands(replaceValue, " ") + "</span>";
                return this.Title.replace(/\{([0-9]+)\}/g, replaceValue);
            } else
                return this.Title;
        };
        return MarkRecord;
    })(gsCore.Record);
    MarksModule.MarkRecord = MarkRecord;

    var Model = (function () {
        function Model(ctx) {
            this.MarkRecordList = ko.observableArray([]);
            this.ErrorMsg = ko.observable("");
            this.context = ctx;
        }
        Model.prototype.LoadData = function (listName) {
            var _this = this;
            this.MarkRecordList.removeAll();

            try  {
                var loadParams = {
                    Include: "Include(ID, Title, AgendaQuestionCategoryLink, CountValue, MarkSumValue, MarkLink)",
                    SortField: "SortIndex"
                };
                gsCore.Utils.LoadList(listName, loadParams, this.context, function (listInstance) {
                    var enumerator = listInstance.getEnumerator();

                    while (enumerator.moveNext()) {
                        var current = enumerator.get_current();
                        _this.MarkRecordList.push(new MarkRecord({
                            Id: current.get_item("ID"),
                            Title: current.get_item("Title"),
                            AgendaQuestionCategoryLink: current.get_item("AgendaQuestionCategoryLink"),
                            CountValue: current.get_item("CountValue"),
                            MarkSumValue: current.get_item("MarkSumValue"),
                            MarkLink: current.get_item("MarkLink")
                        }));
                    }
                }, function (errorMsg) {
                    _this.ErrorMsg = ko.observable(errorMsg);
                });
            } catch (e) {
                var err = e;
                this.ErrorMsg = ko.observable(err.message);
            }

            return this;
        };
        return Model;
    })();
    MarksModule.Model = Model;
})(MarksModule || (MarksModule = {}));
