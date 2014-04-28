/// <reference path="../../../Scripts/typings/sharepoint/SharePoint.d.ts"/>
/// <reference path="../../../Scripts/typings/knockout/knockout.d.ts"/>
/// <reference path="../../../Scripts/typings/camljs/camljs.d.ts"/>
/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts"/>
'use strict';
var gsCore;
(function (gsCore) {
    (function (Utils) {
        function LoadList(listName, loadParams, ctx, success, fail) {
            var list = ctx && ctx.get_web().get_lists().getByTitle(listName);
            if (!list) {
                fail("There isn't list with name: " + listName);
                return;
            }
            var query = new SP.CamlQuery();
            if (loadParams && loadParams.caml) {
                query.set_viewXml("<View><Query>" + loadParams.caml + "</Query></View>");
            } else {
                if (loadParams && loadParams.SortField) {
                    var caml = new CamlBuilder().Where().Any().OrderBy(loadParams.SortField).ToString();
                    query.set_viewXml("<View><Query>" + caml + "</Query></View>");
                }
            }
            var listInst = list.getItems(query);
            ctx.load(listInst, loadParams && loadParams.Include);
            ctx.executeQueryAsync(function () {
                success(listInst);
            }, function () {
                fail("There was an error while loading list data. List name: " + listName);
            });
        }
        Utils.LoadList = LoadList;

        function SureSPContext(callback) {
            SP.SOD.executeFunc("sp.js", "SP.ClientContext", function () {
                SP.SOD.executeOrDelayUntilScriptLoaded(function () {
                    var ctx = SP.ClientContext.get_current();
                    callback(ctx);
                }, "sp.js");
            });
        }
        Utils.SureSPContext = SureSPContext;

        function getWebPartCustomProperties(webPartId) {
            var webpart = $("[id=" + webPartId + "]");
            var field = webpart && $(webpart).find("[id*='WebPartPropertiesHiddenFieldID']")[0];
            return field && $.parseJSON($(field).val());
        }
        Utils.getWebPartCustomProperties = getWebPartCustomProperties;
    })(gsCore.Utils || (gsCore.Utils = {}));
    var Utils = gsCore.Utils;

    (function (Math) {
        function formatThousands(value, separator) {
            return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, separator);
        }
        Math.formatThousands = formatThousands;
    })(gsCore.Math || (gsCore.Math = {}));
    var Math = gsCore.Math;

    var Record = (function () {
        function Record(data) {
            for (var prop in data) {
                this[prop] = data[prop];
            }
        }
        return Record;
    })();
    gsCore.Record = Record;
})(gsCore || (gsCore = {}));
