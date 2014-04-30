/// <reference path="../../../Scripts/typings/sharepoint/SharePoint.d.ts"/>
/// <reference path="../../../Scripts/typings/knockout/knockout.d.ts"/>
/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts"/>
/// <reference path="../../../Scripts/typings/camljs/camljs.d.ts"/>
/// <reference path="../../../Scripts/typings/spin/spin.d.ts"/>

'use strict';

module OverduedExecutor{
    var spinOptions = {
        lines: 13, // The number of lines to draw
        length: 20, // The length of each line
        width: 10, // The line thickness
        radius: 30, // The radius of the inner circle
        corners: 1, // Corner roundness (0..1)
        rotate: 0, // The rotation offset
        direction: 1, // 1: clockwise, -1: counterclockwise
        color: '#000', // #rgb or #rrggbb or array of colors
        speed: 1.7, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: false, // Whether to render a shadow
        hwaccel: false, // Whether to use hardware acceleration
        className: 'spinner', // The CSS class to assign to the spinner
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        top: 'auto', // Top position relative to parent in px
        left: 'auto' // Left position relative to parent in px
    };

    function showExecutorOverduedAssignmentsModal(executorId: number, executorName: string) {
        var options = {
            title: "Просроченные поручения - " + executorName,
            url: _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/components/Pages/OverduedExecutorAssignments.aspx?ExecutorId=" + executorId,
            width: 800
        };
        SP.UI.ModalDialog.showModalDialog(options);
    }

    export class Executor {

        public Id: number;
        public Name: string;
        public OverCount: number;

        public showOverduedAssignments() {
            showExecutorOverduedAssignmentsModal(this.Id, this.Name);
        }

        constructor(data) {
            this.Id = data.Id;
            this.Name = data.Name;
            this.OverCount = data.OverCount;
        }
    }

    export class Model {

        private spinner: Spinner;

        private context: SP.ClientContext;

        private getCamlQuery(): string {
            return new CamlBuilder()
                .Where()
                .DateField("AssignmentPlanDate").IsNotNull()
                .And()
                .DateField("AssignmentPlanDate").LessThanOrEqualTo(CamlBuilder.CamlValues.Today)
                .And()
                .DateField("AssignmentFactDate").IsNull()
                .ToString();
        }

        public Executors: KnockoutObservableArray<Executor>;

        public ErrorMsg: KnockoutObservable<string>;

        public MoreMsg: KnockoutObservable<string>;

        private spin(show: boolean) {
            var target = document.getElementById("PanelContent");
            if (!target) return;

            if (show) {
                $(target.children).each((index: number, elem: Element) => {
                    $(elem).hide();
                });
                $(target).css("min-height", "150px");
                this.spinner.spin(target);
            }
            else {
                this.spinner.stop();
                $(target.children).each((index: number, elem: Element) => {
                    $(elem).show();
                });
                $(target).css("min-height", "0px");
            }
        }

        private doLoadData(fromList: SP.List, callback: (count: number) => void) {
            if (!fromList) throw new Error("Не указан список-источник данных");

            var query = new SP.CamlQuery();
            query.set_viewXml("<View><Query>" + this.getCamlQuery() + "</Query></View>");
            var itemCollection = fromList.getItems(query);
            this.context.load(itemCollection, "Include(ID, AssignmentExecutorOrganizationLink)");
            this.context.executeQueryAsync(() => {
                var en = itemCollection.getEnumerator();
                var countArray = {};
                var nonameCount = 0;

                while (en.moveNext()) {
                    var currentItem = en.get_current();
                    var executorValue = currentItem.get_item("AssignmentExecutorOrganizationLink");

                    if (!executorValue) {
                        nonameCount++;
                        continue;
                    }

                    if (executorValue instanceof SP.FieldLookupValue) {
                        var lookupField = <SP.FieldLookupValue> executorValue;
                        var id = lookupField.get_lookupId();
                        var name = lookupField.get_lookupValue();

                        if (countArray[id]) {
                            var e = <Executor> countArray[id];
                            e.OverCount++;
                        }
                        else {
                            countArray[id] = new Executor({ Id: id, Name: name, OverCount: 1 });
                        }
                    }
                }

                var counter = 0;
                for (var prop in countArray) {
                    var e = <Executor> countArray[prop];
                    this.Executors.push(e);
                    counter++;
                }
                this.Executors.push(new Executor({ Id: 0, Name: "Без назначения исполнителя", OverCount: nonameCount }));
                callback(++counter);
            }, () => {
                this.ErrorMsg = ko.observable("При загрузке данных списка произошла ошибка");
                callback(0);
            });
        }

        public loadData(topCount: number): Model {
            this.Executors.removeAll();

            this.spin(true);

            try {
                var list = this.context.get_web().get_lists().getByTitle("Поручения");
                this.doLoadData(list, (count) => {
                    if (!this.ErrorMsg()) {
                        this.Executors.sort((left: Executor, right: Executor) => {
                            return (right.OverCount - left.OverCount);
                        });

                        if (topCount < count && topCount != 0) {
                            this.Executors.splice(topCount, count - topCount);
                            this.MoreMsg("Посмотреть всех (еще " + (count - topCount) + ")");
                        }
                    }
                    this.spin(false);
                });
            } catch (e) {
                var err = <Error> e;
                this.ErrorMsg = ko.observable(err.message);
                this.spin(false);
            }
            
            return this;
        }

        constructor(ctx: SP.ClientContext) {
            this.Executors = ko.observableArray<Executor>([]);
            this.context = ctx;
            this.MoreMsg = ko.observable<string>("");
            this.ErrorMsg = ko.observable<string>("");
            this.spinner = new Spinner(spinOptions);
        }
    }
}