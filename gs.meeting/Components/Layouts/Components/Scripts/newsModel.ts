/// <reference path="../../../Scripts/typings/sharepoint/SharePoint.d.ts"/>
/// <reference path="../../../Scripts/typings/knockout/knockout.d.ts"/>
/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts"/>
/// <reference path="../../../Scripts/typings/camljs/camljs.d.ts"/> 

'use strict';

module NewsModule {

    export class News {
        public ImgUrl: string;
        public Caption: string;
        public ContentUrl: string;

        constructor(data) {
            this.ImgUrl = data.ImgUrl;
            this.Caption = data.Caption;
            this.ContentUrl = data.ContentUrl;
        }
    }

    export class Model {

        public News: KnockoutObservableArray<News>;
        public ErrorMsg: KnockoutObservable<string>;
        public DetailsMsg: KnockoutObservable<string>;
        private context: SP.ClientContext;

        constructor(ctx: SP.ClientContext) {
            this.context = ctx;
            this.News = ko.observableArray<News>([]);
            this.ErrorMsg = ko.observable<string>("");
            this.DetailsMsg = ko.observable<string>("Подробнее на сайте...");
        }

        public LoadData(listName: string, lastDayCount: number) {
            this.News.removeAll();

            try {
                var list = this.context.get_web().get_lists().getByTitle(listName);
                var whereDate = new Date();
                whereDate.setDate(whereDate.getDate() - lastDayCount);
                var caml = new CamlBuilder().Where().DateTimeField("Modified").GreaterThan(whereDate).ToString();
                var query = new SP.CamlQuery();
                query.set_viewXml("<View><Query>" + caml + "</Query></View>");
                var listInst = list.getItems(query);
                this.context.load(listInst, "Include(ID, Title, URL, FileRef)");
                this.context.executeQueryAsync(() => {
                    var enumerator = listInst.getEnumerator();

                    while (enumerator.moveNext()) {
                        var current = enumerator.get_current();
                        this.News.push(new News({
                            ContentUrl: current.get_item("URL").get_url(),
                            ImgUrl: current.get_item("FileRef"),
                            Caption: current.get_item("Title")
                        }));
                    }
                }, () => {
                    this.ErrorMsg = ko.observable("При загрузке данных списка произошла ошибка");
                });

            } catch(e) {
                var err = <Error> e;
                this.ErrorMsg = ko.observable(err.message);
            } 

            return this;
        }
    }

}