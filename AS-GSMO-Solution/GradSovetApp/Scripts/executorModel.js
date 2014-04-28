var ExecutorPage = (function ($) {
    var root = {};

    root.init = function () {

        function ResultEntity(data) {
            return {
                Id : ko.observable(data.Id),
                FullName: data.ExecutorType == "FIO" ? ko.observable(data.ParticipantFullName) : ko.observable(""),
                Position: data.ExecutorType == "FIO" ? ko.observable(data.ParticipantPositionLink.Title) : data.ExecutorType == "Position" ? ko.observable(data.Title) : ko.observable(""),
                Org: data.ExecutorType == "FIO" ? ko.observable(data.ParticipantOrgLink.OrganizationName) : data.ExecutorType == "Organization" ? ko.observable(data.OrganizationName) : ko.observable(""),
                Role: data.ExecutorType == "FIO" ? ko.observable(data.ParticipantRole) : ko.observable(""),
                ExecutorType: ko.observable(data.ExecutorType)
            };
        };

        var ExecutorModel = function () {
            var self = this;
            self.radioSelectedOptionValue = ko.observable("FIO");
            self.searchResults = ko.observableArray([]);
            self.searchInput = ko.observable("");

            self.search = function() {
                var result = [];
                var select = "";
                var expand = "";
                var filter = "";
                var listTitle = "";

                switch (self.radioSelectedOptionValue()) {
                    case "FIO":
                        if (self.searchInput() != "") filter = "&$filter=substringof('" + self.searchInput() + "', ParticipantFullName)";
                        select = "$select=ID,ParticipantFullName,ParticipantPositionLink/Title,ParticipantOrgLink/OrganizationName,ParticipantRole";
                        expand = "&$expand=ParticipantPositionLink,ParticipantOrgLink";
                        listTitle = "Справочник участников";
                        break;
                    case "Position":
                        if (self.searchInput() != "") filter = "&$filter=substringof('" + self.searchInput() + "', Title)";
                        select = "$select=ID,Title";
                        expand = "";
                        listTitle = "Справочник должностей";
                        break;
                    case "Organization":
                        if (self.searchInput() != "") filter = "&$filter=substringof('" + self.searchInput() + "', OrganizationName)";
                        select = "$select=ID,OrganizationName";
                        expand = "";
                        listTitle = "Справочник организаций";
                        break;
                }

                var fileQuery = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('" + listTitle + "')/items?" + select + expand + filter;
                $.ajax({
                    url: fileQuery,
                    headers: { Accept: "application/json;odata=verbose" },
                    method: "GET",
                    async: false,
                    success: function (data) {
                        if (data.d) {
                            for (i = 0; i < data.d.results.length; i++) {
                                data.d.results[i].ExecutorType = self.radioSelectedOptionValue();
                                result.push(new ResultEntity(data.d.results[i]));
                            }
                            self.searchResults(result);
                        }
                    }
                });
            };

            self.CommitPopUp = function () {
                window.frameElement.commitPopup(ko.toJSON(this));
                return false;
            };
            
            self.ClosePopUp = function () {
                window.frameElement.cancelPopUp();
                return false;
            };
        };
        
        ko.applyBindings(new ExecutorModel());
    };
    return root;
})(jQuery);