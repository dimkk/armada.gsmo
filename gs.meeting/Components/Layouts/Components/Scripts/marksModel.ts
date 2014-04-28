/// <reference path="../../../Scripts/typings/sharepoint/SharePoint.d.ts"/>
/// <reference path="../../../Scripts/typings/knockout/knockout.d.ts"/>
/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts"/>
/// <reference path="../../../Scripts/typings/camljs/camljs.d.ts"/> 
/// <reference path="gsCore.ts"/>

'use strict';

module MarksModule {

    declare var RegExp;

    export class MarkRecord extends gsCore.Record {
        public Id: number;
        public Title: string;
        public AgendaQuestionCategoryLink: string;
        public CountValue: number;
        public MarkSumValue: number;
        public MarkLink: string;
        public getFormattedTitle(): string {
            // trying to find curly braces
            var re = new RegExp(/\{([0-9]+)\}/g);
            if (re.test(this.Title)) {
                var replaceValue = this.MarkSumValue ? this.MarkSumValue.toString() : "нет значения";
                replaceValue = "<span class='label label-primary font-larger'>" + gsCore.Math.formatThousands(replaceValue, " ") + "</span>";
                return this.Title.replace(/\{([0-9]+)\}/g, replaceValue);
            } else return this.Title;
        }        
    }

    export class Model implements gsCore.IDataLoader {
        
        private context: SP.ClientContext;
        public MarkRecordList: KnockoutObservableArray<MarkRecord>;
        public ErrorMsg: KnockoutObservable<string>;

        constructor(ctx: SP.ClientContext) {
            this.MarkRecordList = ko.observableArray<MarkRecord>([]);
            this.ErrorMsg = ko.observable<string>("");
            this.context = ctx;
        }

        public LoadData(listName: string) {
            this.MarkRecordList.removeAll();

            try {
                var loadParams = {
                    Include: "Include(ID, Title, AgendaQuestionCategoryLink, CountValue, MarkSumValue, MarkLink)",
                    SortField: "SortIndex"
                };
                gsCore.Utils.LoadList(listName, loadParams, this.context,
                    (listInstance: SP.ListItemCollection) => {
                        var enumerator = listInstance.getEnumerator();

                        while (enumerator.moveNext()) {
                            var current = enumerator.get_current();
                            this.MarkRecordList.push(new MarkRecord({
                                Id: current.get_item("ID"),
                                Title: current.get_item("Title"),
                                AgendaQuestionCategoryLink: current.get_item("AgendaQuestionCategoryLink"),
                                CountValue: current.get_item("CountValue"),
                                MarkSumValue: current.get_item("MarkSumValue"),
                                MarkLink: current.get_item("MarkLink")
                            }));
                        }
                    },
                    (errorMsg: string) => {
                        this.ErrorMsg = ko.observable(errorMsg);
                    });
            } catch (e) {
                var err = <Error> e;
                this.ErrorMsg = ko.observable(err.message);
            }

            return this;
        }

    }

}