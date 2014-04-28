/// <reference path="../../../Scripts/typings/sharepoint/SharePoint.d.ts"/>
/// <reference path="../../../Scripts/typings/knockout/knockout.d.ts"/>
/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts"/>
/// <reference path="../../../Scripts/typings/camljs/camljs.d.ts"/> 
/// <reference path="gsCore.ts"/>

'use strict';

module ObjectGalleryModule {

    export class GalleryRecord extends gsCore.Record {
        public Id: number;
        public Title: string;
        public Url: string;
    }

    export class Model implements gsCore.IDataLoader {

        private context: SP.ClientContext;
        public GalleryRecordList: KnockoutObservableArray<GalleryRecord>;
        public ErrorMsg: KnockoutObservable<string>;

        constructor(ctx: SP.ClientContext) {
            this.GalleryRecordList = ko.observableArray<GalleryRecord>([]);
            this.ErrorMsg = ko.observable<string>("");
            this.context = ctx;
        }

        public LoadData(params: any, callback: (success: boolean) => void) {
            this.GalleryRecordList.removeAll();

            try {
                var camlText = new CamlBuilder().Where().LookupField(params.filterField).Id().EqualTo(params.filterValue).ToString();

                var loadParams = {
                    Include: "Include(ID, Title, EncodedAbsWebImgUrl)",
                    caml: camlText
                };
                gsCore.Utils.LoadList(params.listName, loadParams, this.context,
                    (listInstance: SP.ListItemCollection) => {
                        var enumerator = listInstance.getEnumerator();

                        while (enumerator.moveNext()) {
                            var current = enumerator.get_current();
                            this.GalleryRecordList.push(new GalleryRecord({
                                Id:     current.get_item("ID"),
                                Title:  current.get_item("Title"),
                                Url:    current.get_item("EncodedAbsWebImgUrl")
                            }));
                        }

                        callback && callback(true);
                    },
                    (errorMsg: string) => {
                        this.ErrorMsg = ko.observable(errorMsg);
                        callback && callback(false);
                    });
            } catch (e) {
                var err = <Error> e;
                this.ErrorMsg = ko.observable(err.message);
            }

            return this;
        }

    }

}