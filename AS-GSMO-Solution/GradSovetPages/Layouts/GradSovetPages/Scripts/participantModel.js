var ParticipantPage = (function ($) {
    var root = {};

    root.init = function (isMulti) {

        function Participant(data) {
            return {
                Id : ko.observable(data.Id),
                ParticipantFullName : ko.observable(data.ParticipantFullName),
                ParticipantPosition : ko.observable(data.ParticipantPositionLink.Title),
                ParticipantOrg : ko.observable(data.ParticipantOrgLink.OrganizationName),
                ParticipantRole : ko.observable(data.ParticipantRole),
            };
        };

        var ParticipantModel = function () {
            var self = this;
            self.selectedParticipants = ko.observableArray([]);
            self.searchResults = ko.observableArray([]);
            self.searchFIO = ko.observable("");
            self.searchPosition = ko.observable("");
            self.searchOrganization = ko.observable("");
            self.isMulti = ko.observable(isMulti == "1" ? true : false);

            self.search = function() {
                var result = [];
                var select = "$select=ID,ParticipantFullName,ParticipantPositionLink/Title,ParticipantOrgLink/OrganizationName,ParticipantRole";
                var expand = "&$expand=ParticipantPositionLink,ParticipantOrgLink";
                var filter = "&$filter=";
                if (self.searchFIO() == "" && self.searchPosition() == "" && self.searchOrganization() == "") filter = "";
                else {
                    if (self.searchFIO() != "") {
                        filter += "substringof('" + self.searchFIO() + "', ParticipantFullName)";
                        if (self.searchPosition() != "") filter += " and substringof('" + self.searchPosition() + "', ParticipantPositionLink/Title)";
                        if (self.searchOrganization() != "") filter += " and substringof('" + self.searchOrganization() + "', ParticipantOrgLink/OrganizationName)";
                    }
                    else {
                        if (self.searchPosition() != "") {
                            filter += "substringof('" + self.searchPosition() + "', ParticipantPositionLink/Title)";
                            if (self.searchOrganization() != "") filter += " and substringof('" + self.searchOrganization() + "', ParticipantOrgLink/OrganizationName)";
                        }
                        else filter += "substringof('" + self.searchOrganization() + "', ParticipantOrgLink/OrganizationName)";
                    }
                }
                var fileQuery = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('Справочник участников')/items?" + select + expand + filter;
                $.ajax({
                    url: encodeURI(fileQuery),
                    headers: { Accept: "application/json;odata=verbose" },
                    method: "GET",
                    async: false,
                    success: function (data) {
                        if (data.d) {
                            for (i = 0; i < data.d.results.length; i++) {
                                result.push(new Participant(data.d.results[i]));
                            }
                            self.searchResults(result);
                        }
                    }
                });
            };

            self.select = function () {
                if (self.selectedParticipants.indexOf(this) < 0) self.selectedParticipants.push(this);
                if (!self.isMulti()) self.CommitPopUp();
            };

            self.remove = function () {
                self.selectedParticipants.remove(this);
            };
            
            self.CommitPopUp = function () {
                window.frameElement.commitPopup(ko.toJSON(self.selectedParticipants()));
                return false;
            };
            
            self.ClosePopUp = function () {
                window.frameElement.cancelPopUp();
                return false;
            };
        };
        
        ko.applyBindings(new ParticipantModel());
    };
    return root;
})(jQuery);