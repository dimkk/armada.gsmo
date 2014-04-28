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
var ObjectGalleryModule;
(function (ObjectGalleryModule) {
    var GalleryRecord = (function (_super) {
        __extends(GalleryRecord, _super);
        function GalleryRecord() {
            _super.apply(this, arguments);
        }
        return GalleryRecord;
    })(gsCore.Record);
    ObjectGalleryModule.GalleryRecord = GalleryRecord;

    var Model = (function () {
        function Model(ctx) {
            this.GalleryRecordList = ko.observableArray([]);
            this.ErrorMsg = ko.observable("");
            this.context = ctx;
        }
        Model.prototype.LoadData = function (params, callback) {
            var _this = this;
            this.GalleryRecordList.removeAll();

            try  {
                var camlText = new CamlBuilder().Where().LookupField(params.filterField).Id().EqualTo(params.filterValue).ToString();

                var loadParams = {
                    Include: "Include(ID, Title, EncodedAbsWebImgUrl)",
                    caml: camlText
                };
                gsCore.Utils.LoadList(params.listName, loadParams, this.context, function (listInstance) {
                    var enumerator = listInstance.getEnumerator();

                    while (enumerator.moveNext()) {
                        var current = enumerator.get_current();
                        _this.GalleryRecordList.push(new GalleryRecord({
                            Id: current.get_item("ID"),
                            Title: current.get_item("Title"),
                            Url: current.get_item("EncodedAbsWebImgUrl")
                        }));
                    }

                    callback && callback(true);
                }, function (errorMsg) {
                    _this.ErrorMsg = ko.observable(errorMsg);
                    callback && callback(false);
                });
            } catch (e) {
                var err = e;
                this.ErrorMsg = ko.observable(err.message);
            }

            return this;
        };
        return Model;
    })();
    ObjectGalleryModule.Model = Model;
})(ObjectGalleryModule || (ObjectGalleryModule = {}));
