(function () {

    var bindTargetId = null;

    function selectObjectViewModel() {
        var self = this;

        self.consts = {
            objectListCT: "STS_ListItem_GenericList"
        };

        self.objectSearch = ko.observable();
        self.objectSearchResult = ko.observableArray([]);
        self.selectedObject = ko.observable();
        self.onSearchStringKeyPress = function (data, event) {
            if (event.keyCode == 13) {
                self.startSearch(self);
            }
            else return true;
        };
        self.startSearch = function (data, event) {
            $('[id=dialogError]').html("");
            self.objectSearchResult.removeAll();
            self.selectedObject('');
            doSearch(data.objectSearch());
        };
        self.onSelectObject = function (data, event) {
            self.selectedObject(data);
            $('[id=btnOK]').triggerHandler('click');
        }
        self.onClose = function (data, event) {
            if (!self.selectedObject()) {
                logMessage("Необходимо выбрать объект", true);
                return;
            }
            if (bindTargetId) {
                $("[id=" + bindTargetId + "]").modal("hide");
            } else {
                var dlg = $(event.target).closest("div.modal");
                $(dlg).modal("hide");
            }
        };

        function doSearch(text) {
            var ctx = SP.ClientContext.get_current();

            // check for text
            if (!text) {
                logMessage("Пожалуйста, введите поисковый запрос", true);
                return;
            };

            // build request string
            var keywordQuery = new Microsoft.SharePoint.Client.Search.Query.KeywordQuery(ctx);
            // only in current site, only in lists (objects list does not have content type)
            keywordQuery.set_queryText((String).format("*{0}* contentclass:{1}", text, self.consts.objectListCT));
            if (!document.location.origin) {
                document.location.origin = document.location.protocol + "//" + document.location.host;
            }
            keywordQuery.set_hiddenConstraints((String).format("site:\"{0}\"", document.location.origin));
            var searchExecutor = new Microsoft.SharePoint.Client.Search.Query.SearchExecutor(ctx);
            var results = searchExecutor.executeQuery(keywordQuery);
            ctx.executeQueryAsync(function () {
                var rows = results.m_value.ResultTables[0].ResultRows;
                var count = results.m_value.ResultTables[0].RowCount;
                var idList = [];

                if (!count) {
                    logMessage("По искомой фразе не найдено ни одного объекта строительства", true);
                }
                else {
                    $.each(rows, function (i, e) {
                        if (e.contentclass !== self.consts.objectListCT) return;
                        if (!~e.Path.indexOf("DispForm.aspx")) return;

                        var params = e.Path.split("?")[1] ? e.Path.split("?")[1].split("&") : null;
                        for (var i = 0; i < params.length; i++) {
                            var values = params[i].split("=");
                            if (values[0] !== "ID") continue;

                            idList.push(values[1]);
                            break;
                        }
                    });

                    if (!idList.length) {
                        logMessage("По искомой фразе не найдено ни одного объекта строительства", true);
                    }
                    else {
                        var objectList = ctx.get_web().get_lists().getByTitle("Объекты");
                        var query = new SP.CamlQuery();
                        query.set_viewXml(buildInCAMLQuery(idList));
                        var objectInstance = objectList.getItems(query);
                        ctx.load(objectInstance, "Include(ID, Title, Adress, Builder, Cadastralnumber, Area)");
                        ctx.executeQueryAsync(function () {
                            var enumerator = objectInstance.getEnumerator();
                            var searchResult = [];
                            while (enumerator.moveNext()) {
                                searchResult.push({
                                    ID: enumerator.get_current().get_item("ID"),
                                    objectTitle: enumerator.get_current().get_item("Title"),
                                    objectAddress: enumerator.get_current().get_item("Adress"),
                                    objectBuilder: enumerator.get_current().get_item("Builder"),
                                    objectCadastre: enumerator.get_current().get_item("Cadastralnumber"),
                                    objectArea: enumerator.get_current().get_item("Area")
                                });
                            }
                            self.objectSearchResult(searchResult);
                        }, function () {
                            logMessage("Не удалось запросить данные списка объектов");
                        });
                    }
                }
            }, function () {
                logMessage("Не удалось выполнить поисковый запрос");
            });
        }
        
    }

    function buildInCAMLQuery(list) {
        if (!list.length) return null;

        var res = "<View><Query><Where><In><FieldRef Name='ID'/><Values>";
        $.each(list, function (i, e) {
            res += "<Value Type='Integer'>" + e + "</Value>";
        });
        res += "</Values></In></Where></Query></View>";

        return res;
    }

    function logMessage(text, isWarn) {
        if ("console" in window) {
            if (isWarn) { console.warn(text) }
            else {
                console.error(text);
            }
        }

        var el = $("#dialogError");
        if (el) {
            el.removeClass("text-danger text-warning");
            el.addClass(isWarn ? "text-warning" : "text-danger");
            el.html("<small>" + text + "</small>");
        }
        else {
            alert(text);
        }
    }

    $(document).ready(function () {
        SP.SOD.executeFunc("sp.js", "SP.ClientContext", function () {
            SP.SOD.executeFunc("sp.search.js", "Microsoft.SharePoint.Client.Search.Query.KeywordQuery", function () {
                bindTargetId = window.gsModals ? window.gsModals.active : null;
                if (bindTargetId) {
                    ko.applyBindings(new selectObjectViewModel(), document.getElementById(bindTargetId));
                } else {
                    ko.applyBindings(new selectObjectViewModel());
                }
            });
        });
    });
})();