﻿/// <reference path="../../../Scripts/typings/sharepoint/SharePoint.d.ts"/>
/// <reference path="../../../Scripts/typings/knockout/knockout.d.ts"/>
/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts"/>
/// <reference path="../../../Scripts/typings/camljs/camljs.d.ts"/>
'use strict';
var NewsModule;
(function (NewsModule) {
    var News = (function () {
        function News(data) {
            this.ImgUrl = data.ImgUrl;
            this.Caption = data.Caption;
            this.ContentUrl = data.ContentUrl;
        }
        return News;
    })();
    NewsModule.News = News;

    var Model = (function () {
        function Model(ctx) {
            this.context = ctx;
            this.News = ko.observableArray([]);
            this.ErrorMsg = ko.observable("");
            this.DetailsMsg = ko.observable("Подробнее на сайте...");
        }
        Model.prototype.LoadData = function (listName, lastDayCount) {
            var _this = this;
            this.News.removeAll();

            try  {
                var list = this.context.get_web().get_lists().getByTitle(listName);
                var whereDate = new Date();
                whereDate.setDate(whereDate.getDate() - lastDayCount);
                var caml = new CamlBuilder().Where().DateTimeField("Modified").GreaterThan(whereDate).ToString();
                var query = new SP.CamlQuery();
                query.set_viewXml("<View><Query>" + caml + "</Query></View>");
                var listInst = list.getItems(query);
                this.context.load(listInst, "Include(ID, Title, URL, FileRef)");
                this.context.executeQueryAsync(function () {
                    var enumerator = listInst.getEnumerator();

                    while (enumerator.moveNext()) {
                        var current = enumerator.get_current();
                        _this.News.push(new News({
                            ContentUrl: current.get_item("URL").get_url(),
                            ImgUrl: current.get_item("FileRef"),
                            Caption: current.get_item("Title")
                        }));
                    }
                }, function () {
                    _this.ErrorMsg = ko.observable("При загрузке данных списка произошла ошибка");
                });
            } catch (e) {
                var err = e;
                this.ErrorMsg = ko.observable(err.message);
            }

            return this;
        };
        return Model;
    })();
    NewsModule.Model = Model;
})(NewsModule || (NewsModule = {}));
