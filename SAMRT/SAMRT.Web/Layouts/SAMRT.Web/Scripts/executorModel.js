var ExecutorPage = (function ($) {
    var root = {};

    root.init = function () {

        function ResultEntity(data) {
            return {
                Id : ko.observable(data.Id),
                FullName: data.ExecutorType == "FIO" ? ko.observable(data.ParticipantFullName) : ko.observable(""),
                PositionId: data.ExecutorType == "FIO" ? ko.observable(data.ParticipantPositionLink.Id) : ko.observable(""),
                Position: data.ExecutorType == "FIO" ? ko.observable(data.ParticipantPositionLink.Title) : data.ExecutorType == "Position" ? ko.observable(data.Title) : ko.observable(""),
                OrgId: data.ExecutorType == "FIO" ? ko.observable(data.ParticipantOrgLink.Id) : ko.observable(""),
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

            self.onSearchClick = function () {
                self.search();
                // do not prevent default browser action
                return true;
            }

            self.search = function() {
                var result = [];
                var select = "";
                var expand = "";
                var filter = "";
                var listTitle = "";

                switch (self.radioSelectedOptionValue()) {
                    case "FIO":
                        if (self.searchInput() != "") filter = "&$filter=substringof('" + self.searchInput() + "', ParticipantFullName) or substringof('" + self.searchInput() + "', ParticipantPositionLink/Title) or substringof('" + self.searchInput() + "', ParticipantOrgLink/OrganizationName)";
                        select = "$select=ID,ParticipantFullName,ParticipantPositionLink/Id,ParticipantPositionLink/Title,ParticipantOrgLink/OrganizationName,ParticipantOrgLink/Id,ParticipantRole";
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
                    url: encodeURI(fileQuery),
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

            self.onSearchStringKeyPress = function (data, event) {
                if (event.keyCode == 13) {
                    self.search();
                }
                else return true;
            }

            self.CommitPopUp = function (data, event) {
                switch (data.ExecutorType()) {
                    case "FIO":
                        window.gsExecutor = {
                            AssignmentExecutorFullNameLink: data.Id(),
                            AssignmentExecutorPositionLink: data.PositionId(),
                            AssignmentExecutorOrganizationLink: data.OrgId()
                        };
                        break;
                    case "Position":
                        window.gsExecutor = {
                            AssignmentExecutorFullNameLink: "",
                            AssignmentExecutorPositionLink: data.Id(),
                            AssignmentExecutorOrganizationLink: ""
                        };
                        break;
                    case "Organization":
                        window.gsExecutor = {
                            AssignmentExecutorFullNameLink: "",
                            AssignmentExecutorPositionLink: "",
                            AssignmentExecutorOrganizationLink: data.Id()
                        };
                }

                $('#ExecutorTextPresentation').html(
                    (data.FullName() ? data.FullName() + "</br>" : "") +
                    (data.Position() ? data.Position() + "</br>" : "") +
                    (data.Org() ? data.Org() : ""));
                if (window.closeSelectExecutorModal) window.closeSelectExecutorModal();
            };
        };
        
        ko.applyBindings(new ExecutorModel(), document.getElementById(window.gsModals.selectExecutor));
    };
    return root;
})(jQuery);
