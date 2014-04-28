var SearchQuestionPage = (function ($) {
    var root = {};

    root.init = function () {

        function Question(data) {
            return {
                Id: ko.observable(data.get_item("ID")),
                AgendaQuestionNumber: ko.observable(data.get_item("AgendaQuestionNumber")),
                AgendaQuestionTheme: ko.observable(data.get_item("AgendaQuestionTheme"))
            };
        };

        var SearchQuestionModel = function () {
            var self = this;
            self.searchResults = ko.observableArray([]);
            self.searchTheme = ko.observable("");

            self.search = function () {
                var appWebContext = SP.ClientContext.get_current();
                var agendaQuestionList = appWebContext.get_web().get_lists().getByTitle("Вопросы повестки заседания");
                var query = new SP.CamlQuery();
                query.set_viewXml("<View><Query><Where><Contains><FieldRef Name='AgendaQuestionTheme'/><Value Type='Note'>" + self.searchTheme() + "</Value></Contains></Where></Query></View>");
                var aqInstance = agendaQuestionList.getItems(query);
                appWebContext.load(aqInstance);
                appWebContext.executeQueryAsync(function () {
                    var result = [];
                    var enumerator = aqInstance.getEnumerator();
                    while (enumerator.moveNext()) {
                        result.push(new Question(enumerator.get_current()));
                    }
                    self.searchResults(result);
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
        
        ko.applyBindings(new SearchQuestionModel());
    };
    return root;
})(jQuery);