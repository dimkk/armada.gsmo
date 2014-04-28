/// <reference path="../../../Scripts/typings/sharepoint/SharePoint.d.ts"/>
/// <reference path="../../../Scripts/typings/knockout/knockout.d.ts"/>
/// <reference path="../../../Scripts/typings/camljs/camljs.d.ts"/>
/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts"/>

'use strict';

module gsCore {

    export module Utils {

        export function LoadList(listName: string, loadParams: any, ctx: SP.ClientContext,
            success: (items: SP.ListItemCollection) => void,
            fail: (errorMsg: string) => void)
        {
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
            ctx.executeQueryAsync(() => {
                success(listInst);
            }, () => {
                fail("There was an error while loading list data. List name: " + listName);
            });
        }      

        export function SureSPContext(callback: (context: SP.ClientContext) => void) {
            SP.SOD.executeFunc("sp.js", "SP.ClientContext", () => {
                SP.SOD.executeOrDelayUntilScriptLoaded(() => {
                    var ctx = SP.ClientContext.get_current();
                    callback(ctx);
                }, "sp.js");
            });
        }

        export function getWebPartCustomProperties(webPartId: string) : any {
            var webpart = $("[id=" + webPartId + "]");
            var field = webpart && $(webpart).find("[id*='WebPartPropertiesHiddenFieldID']")[0];
            return field && $.parseJSON($(field).val());
        }

    }

    export module Math {
        export function formatThousands(value: any, separator: string): string {
            return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, separator);
        }
    }

    export class Record {

        constructor(data: any) {
            for (var prop in data) {
                this[prop] = data[prop];
            }
        }

    }

    export interface IDataLoader {
        LoadData(params: any, callback: (success: boolean) => void) : void;
    }

}