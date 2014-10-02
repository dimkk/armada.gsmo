/// <reference path="SP.Core.debug.js" />
/// <reference path="SP.debug.js" />
/// <reference path="SP.RequestExecutor.debug.js" />
/// <reference path="SP.runtime.debug.js" />
/// <reference path="init.debug.js" />
/// <reference path="jquery-1.9.1.js" />

// we have to hide body in order to prepare DOM
$(document.body).css("display", "none");

(function () {

    function sharepointReady() {
        var ctx = SP.ClientContext.get_current();
        var list = ctx.get_web().get_lists().getByTitle("MainMenuDenyList");
        var listInst = list.getItems(new SP.CamlQuery());
        ctx.load(listInst, "Include(Title, DenyGroups)");
        ctx.executeQueryAsync(function () {
            // if we don't have any restrictions ...
            if (!listInst.get_count()) {
                $(document.body).css("display", "block");
                return;
            }

            // getting user groups
            var userGroups = ctx.get_web().get_currentUser().get_groups();
            ctx.load(userGroups);
            ctx.executeQueryAsync(function () {
                var userGroupIdList = [];
                
                for (var i = 0; i < userGroups.get_count(); i++) {
                    userGroupIdList.push(userGroups.get_item(i).get_id());
                }

                // loop for restrictions
                for (i = 0; i < listInst.get_count() ; i++) {
                    var elId = listInst.get_item(i).get_item("Title");
                    var denyList = listInst.get_item(i).get_item("DenyGroups");

                    // loop for items in DenyGroups column
                    $.each(denyList, function (i, val) {
                        // val instanceof SP_FieldUserValue == true
                        if (~userGroupIdList.indexOf(val.get_lookupId())) {
                            var el = $("#" + elId);
                            if (el) {
                                el.closest("td").css("display", "none");
                            }
                            return false;
                        }
                    });
                }
				
                $(document.body).css("display", "block");

            }, function () {
                console.error("Не удалось проверить группы доступа пользователя");
                $(document.body).css("display", "block");
            });

        },
        function () {
            console.error("Не удалось запросить данные списка MainMenuDenyList");
            $(document.body).css("display", "block");
        });

    }

    $(document).ready(function () {
        SP.SOD.executeFunc("sp.js", "SP.ClientContext", sharepointReady);
    });
})()
