/// <reference path="../../../Scripts/typings/sharepoint/SharePoint.d.ts"/>
/// <reference path="../../../Scripts/typings/knockout/knockout.d.ts"/>
/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts"/>
/// <reference path="../../../Scripts/typings/camljs/camljs.d.ts"/>
/// <reference path="../../../Scripts/typings/spin/spin.d.ts"/>
'use strict';
var OverduedExecutor;
(function (OverduedExecutor) {
    var spinOptions = {
        lines: 13,
        length: 20,
        width: 10,
        radius: 30,
        corners: 1,
        rotate: 0,
        direction: 1,
        color: '#000',
        speed: 1.7,
        trail: 60,
        shadow: false,
        hwaccel: false,
        className: 'spinner',
        zIndex: 2e9,
        top: 'auto',
        left: 'auto'
    };

    function showExecutorOverduedAssignmentsModal(executorId, executorName) {
        var options = {
            title: "Просроченные поручения - " + executorName,
            url: _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/components/Pages/OverduedExecutorAssignments.aspx?ExecutorId=" + executorId,
            width: 800
        };
        SP.UI.ModalDialog.showModalDialog(options);
    }

    var Executor = (function () {
        function Executor(data) {
            this.Id = data.Id;
            this.Name = data.Name;
            this.OverCount = data.OverCount;
        }
        Executor.prototype.showOverduedAssignments = function () {
            showExecutorOverduedAssignmentsModal(this.Id, this.Name);
        };
        return Executor;
    })();
    OverduedExecutor.Executor = Executor;

    var Model = (function () {
        function Model(ctx) {
            this.Executors = ko.observableArray([]);
            this.context = ctx;
            this.MoreMsg = ko.observable("");
            this.ErrorMsg = ko.observable("");
            this.spinner = new Spinner(spinOptions);
        }
        Model.prototype.getCamlQuery = function () {
            return new CamlBuilder().Where().DateField("AssignmentPlanDate").IsNotNull().And().DateField("AssignmentPlanDate").LessThanOrEqualTo(CamlBuilder.CamlValues.Today).And().DateField("AssignmentFactDate").IsNull().ToString();
        };

        Model.prototype.spin = function (show) {
            var target = document.getElementById("PanelContent");
            if (!target)
                return;

            if (show) {
                $(target.children).each(function (index, elem) {
                    $(elem).hide();
                });
                $(target).css("min-height", "150px");
                this.spinner.spin(target);
            } else {
                this.spinner.stop();
                $(target.children).each(function (index, elem) {
                    $(elem).show();
                });
                $(target).css("min-height", "0px");
            }
        };

        Model.prototype.doLoadData = function (fromList, callback) {
            var _this = this;
            if (!fromList)
                throw new Error("Не указан список-источник данных");

            var query = new SP.CamlQuery();
            query.set_viewXml("<View><Query>" + this.getCamlQuery() + "</Query></View>");
            var itemCollection = fromList.getItems(query);
            this.context.load(itemCollection, "Include(ID, AssignmentExecutorOrganizationLink)");
            this.context.executeQueryAsync(function () {
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
                        var lookupField = executorValue;
                        var id = lookupField.get_lookupId();
                        var name = lookupField.get_lookupValue();

                        if (countArray[id]) {
                            var e = countArray[id];
                            e.OverCount++;
                        } else {
                            countArray[id] = new Executor({ Id: id, Name: name, OverCount: 1 });
                        }
                    }
                }

                var counter = 0;
                for (var prop in countArray) {
                    var e = countArray[prop];
                    _this.Executors.push(e);
                    counter++;
                }
                _this.Executors.push(new Executor({ Id: 0, Name: "Без назначения исполнителя", OverCount: nonameCount }));
                callback(++counter);
            }, function () {
                _this.ErrorMsg = ko.observable("При загрузке данных списка произошла ошибка");
                callback(0);
            });
        };

        Model.prototype.loadData = function (topCount) {
            var _this = this;
            this.Executors.removeAll();

            this.spin(true);

            try  {
                var list = this.context.get_web().get_lists().getByTitle("Поручения");
                this.doLoadData(list, function (count) {
                    if (!_this.ErrorMsg()) {
                        _this.Executors.sort(function (left, right) {
                            return (right.OverCount - left.OverCount);
                        });

                        if (topCount < count && topCount != 0) {
                            _this.Executors.splice(topCount, count - topCount);
                            _this.MoreMsg("Посмотреть всех (еще " + (count - topCount) + ")");
                        }
                    }
                    _this.spin(false);
                });
            } catch (e) {
                var err = e;
                this.ErrorMsg = ko.observable(err.message);
                this.spin(false);
            }

            return this;
        };
        return Model;
    })();
    OverduedExecutor.Model = Model;
})(OverduedExecutor || (OverduedExecutor = {}));
