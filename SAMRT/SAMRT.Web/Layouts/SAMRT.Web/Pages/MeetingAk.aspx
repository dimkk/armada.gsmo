<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeetingAk.aspx.cs" Inherits="SAMRT.Web.MeetingAk" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/typeahead.min.js"></script>
    <script type="text/javascript" src="../Scripts/knockout-2.3.0.js"></script>
    <script type="text/javascript" src="../Scripts/moment-with-langs.min.js"></script>
    <script type="text/javascript" src="../Scripts/moment-datepicker.min.js"></script>
    <script type="text/javascript" src="../Scripts/moment-datepicker-ko.js"></script>
    <script type="text/javascript" src="/_layouts/15/SP.RequestExecutor.js"></script>

    <!-- Добавьте свои стили CSS в следующий файл -->
    <link rel="stylesheet" type="text/css" href="../Content/typeahead.js-bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="../Content/moment-datepicker/datepicker.css" />
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <!-- Добавьте свой код JavaScript в следующий файл -->
    <script type="text/javascript" src="../Scripts/spin.min.js"></script>
    <script type="text/javascript" src="../Scripts/camljs.js"></script>
    <script type="text/javascript" src="../Scripts/Model/MeetingAk.js"></script>
    <script type="text/javascript">
        var modelMetaData = {
            meeting:              { listName: "MeetingList", fields: [] },
            meetingAttachment:    { listName: "MeetingAttachmentList", fields: [] },
            agendaQuestion:       { listName: "AgendaQuestionList", fields: [] },
            agendaQuestionObject: { listName: "AgendaQuestionObject", fields: [] }
        };
    </script>
    <style>
        input:disabled, select:disabled {
            color: #000;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <%-- Контейнер для всех элементов формы --%>
    <div class="container topspace">
        <%-- Вкладки --%>
        <ul id="tabs" class="nav nav-tabs">
            <li class="active"><a href="#tab-common" data-toggle="tab">Общая информация</a></li>
            <li class=""><a href="#tab-additional" data-toggle="tab">Дополнительная информация</a></li>
        </ul>
        <%-- Содержимое вкладок --%>
        <div id="tabContent" class="tab-content">
            <div id="tab-common" class="tab-pane fade active in">
                <div class="form-horizontal" role="form">
                    <div class="form-group topspace">
                        <div class="col-lg-2">
                            <label style="font-weight: normal">Дата проведения:</label>
                            <label data-bind="text: meeting().MeetingDate"></label>
                        </div>
                        <div class="col-lg-5">
                            <label style="font-weight: normal">Место проведения:</label>
                            <label data-bind="text: meeting().MeetingPlace"></label>
                        </div>
                    </div>
                    <div class="form-group">
                    </div>
                    <div class="form-group" data-bind="style: { display: scanAttach().FileUrl() != '' ? 'block' : 'none' }">
                        <label class="col-lg-2 control-label">Электронная версия протокола</label>
                        <div class="col-lg-10">
                            <a data-bind="attr: { href: scanAttach().FileUrl, target: '_blank' }, text: scanAttach().FileName"></a>
                            <button type="button" class="btn btn-default" data-bind="click: deleteScanAttach, enable: editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                        </div>
                    </div>
                </div>
                <%-- Таблица вопросов повестки --%>
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Вопросы</h3>
                    </div>
                    <div class="panel-body" id="AgendaQuestionTableDiv">
                        <table class="table evenodd" id="AgendaQuestionTableTable">
                            <thead>
                                <tr>
									<th></th>
                                    <th>Номер</th>
                                    <th>Адрес</th>
                                    <th>Инвестор</th>
                                    <th>Описание вопроса</th>
                                    <th>Докладчики</th>
                                    <% if (IsQuestionCommentEnabled)
                                       { %>
                                    <%--<th>Комментарий</th>--%>
                                    <% } %>
									<th></th>
                                    <th></th>
                                    <th></th>
                                    <% if (IsQuestionCommentEnabled)
                                       { %>
                                    <%--<th></th>--%>
                                    <% } %>
                                </tr>
                            </thead>
                            <tbody data-bind="foreach: agendaQuestions">
                                <tr>
									<td style="white-space: nowrap">
										<button type="button" class="btn btn-default" data-bind="click: $parent.moveUpAgendaQuestion, enable: $parent.canMoveUpAgendaQuestion($data)" title="Передвинуть вверх" style="margin:0"><span class="glyphicon glyphicon-arrow-up"/></button>
										<button type="button" class="btn btn-default" data-bind="click: $parent.moveDownAgendaQuestion, enable: $parent.canMoveDownAgendaQuestion($data)" title="Передвинуть вниз" style="margin:0"><span class="glyphicon glyphicon-arrow-down"/></button>
									</td>
                                    <td data-bind="text: AgendaQuestionNumber"></td>
                                    <td data-bind="text: AgendaQuestionAddress"></td>
                                    <td data-bind="text: AgendaQuestionInvestor"></td>
                                    <td data-bind="text: AgendaQuestionDescription"></td>
                                    <td data-bind="text: calcReporters"></td>
                                    <% if (IsQuestionCommentEnabled) { %>
                                    <%--<td data-bind="text: AgendaQuestionComment, css: GetDecisionFieldClass($data)"></td>--%>
                                    <% } %>
                                    <td>
                                        <button type="button" class="btn btn-default" data-bind="click: showObjects, style: { display: agendaQuestionObjects().length == 0 ? 'none' : '' }" title="Посмотреть объекты вопроса" style="margin:0"><span class="glyphicon glyphicon-home"></span></button>
                                    </td>
                                    <td>
                                        <button type="button" class="btn btn-default" data-bind="click: showAttachments" title="Посмотреть вложения вопроса" style="margin:0"><span class="glyphicon glyphicon-paperclip"></span></button>
                                    </td>
                                    <td>
										<button type="button" class="btn btn-default" data-bind="click: gotoEditQuestion" title="Посмотреть вопрос" style="margin:0"><span class="glyphicon glyphicon-edit"/></button>
                                    </td>
                                    <% if (IsQuestionCommentEnabled)
                                       { %>
                                    <%--<td>
                                        <button type="button" class="btn btn-default" data-bind="click: editQuestionComment" style="margin:0"><span class="glyphicon glyphicon-pencil"/></button>
                                    </td>--%>
                                    <% } %>
									<td>
										<button type="button" class="btn btn-default" data-bind="click: $parent.removeAgendaQuestion" title="Удалить вопрос" style="margin:0"><span class="glyphicon glyphicon-remove"/></button>
									</td>
                                </tr>
                            </tbody>
                        </table>
                        <%--<button type="button" class="btn btn-primary" data-bind="click: addAgendaQuestion, enable: editEnabled" id="AddAgendaQuestionButton" name="AddAgendaQuestionButton">Добавить</button>--%>
						<button type="button" class="btn btn-default" data-bind="click: addIssue">Добавить из вопроса Рабочей группы</button>
						<button type="button" class="btn btn-default" data-bind="click: createIssue">Создать</button>
                    </div>
                </div>
                <%-- Таблица вложений заседания --%>
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Вложения</h3>
                    </div>
                    <div class="panel-body">
                        <table class="table">
                            <thead>
                                <tr>
                                    <th class="col-md-3">Файл</th>
                                    <th class="col-md-2">Тип документа</th>
                                    <th class="col-md-6">Описание</th>
                                    <th class="col-md-1"></th>
                                </tr>
                            </thead>
                            <tbody data-bind="foreach: attachments, afterRender: $root.initFileReader()">
                                <tr>
                                    <td>
                                        <a data-bind="attr: { href: FileUrl, target: '_blank' }, text: FileName, visible: FileUrl() != ''"></a>
                                        <input class="form-control" data-bind="text: FilePath, visible: FileUrl() == '', event: { change: function (data, event) { selectedFile($element, data, event) } }, enable: $parent.editEnabled" type='file' />
                                    </td>
                                    <td>
                                        <select class="form-control" placeholder="Выберите значение" data-bind="
    options: $root.availableAttachDocTypes,
    optionsText: 'name',
    optionsValue: 'id',
    value: DType,
    enable: $parent.editEnabled">
                                        </select>
                                    </td>
                                    <td>
                                        <textarea class="form-control" data-bind="value: Descr, enable: $parent.editEnabled"></textarea>
                                    </td>
                                    <td>
                                        <button type="button" class="btn btn-default" data-bind="click: $parent.removeAttach, enable: $parent.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <%--<button class="btn btn-primary" data-bind="click: addAttach, enable: editEnabled">Добавить</button>--%>
                    </div>
                </div>
            </div>
            <div id="tab-additional" class="tab-pane fade">
                <div class="form-horizontal" role="form">
                    <div class="form-group topspace">
                        <label for="inputNumber" class="col-lg-2 control-label">Номер</label>
                        <div class="col-lg-3">
                            <input data-bind="value: meeting().MeetingNumber, enable: editEnabled" type="text" class="form-control" id="inputNumber" placeholder="Номер заседания">
                        </div>
                        <label for="inputStatus" class="col-lg-2 control-label">Статус</label>
                        <div class="col-lg-3">
                            <select id="inputStatus" class="form-control" placeholder="Выберите значение" data-bind="
    options: $root.availableMeetingStatuses,
    optionsText: 'name',
    optionsValue: 'name',
    value: meeting().MeetingStatus,
    enable: editEnabled">
                            </select>
                        </div>
                    </div>
                    <div class="form-group topspace">
                        <label for="ChairManFullNameLinkValue" class="col-lg-2 control-label">Председатель</label>
                        <div class="col-lg-8">
                            <input data-bind="value: meeting().ChairManFullNameLinkValue, enable: $root.editEnabled" id="ChairManFullNameLinkValue" type="text" class="form-control" />
                        </div>
                        <div class="col-lg-2">
                            <button type="button" class="btn btn-default" id="selectMainPartic" name="selectMainPartic" data-bind="click: meeting().selectChairMan, enable: $root.editEnabled">Выбрать</button>
                        </div>
                        <input style="display: none" id="hidden-meeting-chair-man" data-bind="value: meeting().ChairManFullNameLinkId" type="text" />
                    </div>
                    <div class="form-group">
                        <label for="ProtocolResponsibleSecretaryLinkValue" class="col-lg-2 control-label">Ответственный секретарь</label>
                        <div class="col-lg-8">
                            <input data-bind="value: meeting().ProtocolResponsibleSecretaryLinkValue, enable: $root.editEnabled" id="ProtocolResponsibleSecretaryLinkValue" type="text" class="form-control" />
                        </div>
                        <div class="col-lg-2">
                            <button type="button" class="btn btn-default" id="selectResponceSecr" name="selectResponceSecr" data-bind="click: meeting().selectResponsibleSecretary, enable: $root.editEnabled">Выбрать</button>
                        </div>
                        <input style="display: none" id="hidden-responsible-secretary" data-bind="value: meeting().ProtocolResponsibleSecretaryLinkId" type="text" />
                    </div>
                    <div class="form-group">
                        <label for="SecretaryFullNameLinkValue" class="col-lg-2 control-label">Секретарь</label>
                        <div class="col-lg-8">
                            <input data-bind="text: meeting().SecretaryFullNameLinkValue, enable: $root.editEnabled" id="SecretaryFullNameLinkValue" type="text" class="form-control" />
                        </div>
                        <div class="col-lg-2">
                            <button type="button" class="btn btn-default" id="selectSecretary" name="selectSecretary" data-bind="click: meeting().selectSecretary, enable: $root.editEnabled">Выбрать</button>
                        </div>
                        <input style="display: none" id="hidden-secretary" data-bind="value: meeting().SecretaryFullNameLinkId" type="text" />
                    </div>
                </div>
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Члены градостроительного совета и приглашённые участники</h3>
                    </div>
                    <div class="panel-body">
                        <table id="other-participant-table" class="table">
                            <thead>
                                <tr>
                                    <th>ЧГС</th>
                                    <th>ФИО</th>
                                    <th>Должность</th>
                                    <th>Организация</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody data-bind="foreach: meeting().additionalParticipants">
                                <tr>
                                    <td data-bind="text: ParticipantRole"></td>
                                    <td data-bind="text: ParticipantFullName"></td>
                                    <td data-bind="text: ParticipantPosition"></td>
                                    <td data-bind="text: ParticipantOrg"></td>
                                    <td>
                                        <button type="button" class="btn btn-default" data-bind="click: $root.meeting().removeAdditionalPartcipant, enable: $root.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div>
                            <button type="button" class="btn btn-primary" data-bind="click: meeting().selectAdditionalParticipants, enable: $root.editEnabled" id="addAdditionalParticipants" name="addAdditionalParticipants">Добавить</button>
                            <input id="hidden-additioanl-participants" style="display: none;" data-bind="value: meeting().AdditionalParticipantsInput" type="text" />
                        </div>
                    </div>
                </div>
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Список остальных участников градостроительного совета</h3>
                    </div>
                    <div class="panel-body">
                        <textarea data-bind="value: meeting().OtherParticipantsList, enable: $root.editEnabled" rows="4" class="form-control" placeholder="Укажите текстовый перечень участников Градостроительного Совета"></textarea>
                    </div>
                </div>
            </div>
        </div>
        <div class="container">
			<button class="btn btn-default" data-bind="click: closeForm" id="oCancelButton" name="oCancelButton" style="float:right">Закрыть</button>
        </div>
    </div>

    <% if (IsQuestionCommentEnabled)
       { %>
    <div class="modal fade" id="mComment" tabindex="-1" role="dialog" aria-hidden="true">
        <style type="text/css">
            .btn-success-gs, .btn-primary-gs, .btn-danger-gs {
                color: #000;
                border-color: #ccc;
                background-color: #fff;
            }
        </style>
        <div class="modal-dialog" style="width: 800px">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h3 class="modal-title">Добавление комментария</h3>
                </div>
                <div class="modal-body">
                    <div class="container">
                        <div class="form-group">
                            <label class="col-lg-2">Тип решения</label>
                            <div class="btn-group col-lg-10" data-toggle="buttons" id="rbDecisionType">
                                <label class="btn btn-success col-lg-4 btn-success-gs" id="rbDecisionType2">
                                    <input type="radio" name="options" />Утвердить</label>
                                <label class="btn btn-primary col-lg-4 btn-primary-gs" id="rbDecisionType0">
                                    <input type="radio" name="options" />Нет решения</label>
                                <label class="btn btn-danger col-lg-4 btn-danger-gs" id="rbDecisionType1">
                                    <input type="radio" name="options" />Отклонить</label>
                            </div>
                        </div>
                        <div class="form-group">
                            <br />
                        </div>
                        <div class="form-group">
                            <label class="col-lg-2">Комментарий</label>
                            <div class="col-lg-10">
                                <textarea type="text" id="tComment" title="Комментарий" rows="5" class="form-control"></textarea>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" id="bSaveComment">Сохранить</button>
                        <button type="button" class="btn btn-default" data-dismiss="modal">Закрыть</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        var IsRequestSended = false;
        $('#mComment').appendTo("body");
        function GetDecisionFieldClass(data) {
            var decisionId = GetDecisionId(data);
            switch (decisionId) {
                case "1": return "btn-danger";
                case "2": return "btn-success";
                default: return "";
            }
        }
        function GetDecisionId(data) {
            var decisionId = data.AgendaQuestionDecisionType();
            return !!decisionId ? decisionId.get_lookupId().toString() : null;
        }
        function SetDecisionId(data, decisionTypeId) {
            var lookup = new SP.FieldLookupValue();
            lookup.set_lookupId(decisionTypeId);
            data.AgendaQuestionDecisionType(lookup);
        }
        function ShowCommentWindow(data) {
            if (IsRequestSended)
                return;
            $('#bSaveComment').unbind("click").click(function () {
                IsRequestSended = true;
                $("#bSaveComment").attr("disabled", "disabled");
                SaveComment(data, $("#rbDecisionType label.active").attr("id").substr(14), $('#tComment').val());
                $('#mComment').modal("hide")
            });
            var decisionId = GetDecisionId(data);
            var labelId = decisionId == "1" | decisionId == "2" ? decisionId : "0";
            $("#rbDecisionType label").removeClass("active").filter("#rbDecisionType" + labelId).addClass("active");
            $('#tComment').val(data.AgendaQuestionComment());
            $('#mComment').modal("show");
        }
        function SaveComment(data, decisionTypeId, comment) {
            var queryUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('Вопросы повестки заседания')/items(" + data.Id() + ")";
            var newData = { __metadata: { 'type': 'SP.Data.AgendaQuestionListListItem' }, AgendaQuestionDecisionTypeId: decisionTypeId, AgendaQuestionComment: comment };
            SetDecisionId(data, decisionTypeId);
            data.AgendaQuestionComment(comment);
            $.ajax({
                url: encodeURI(queryUrl),
                data: JSON.stringify(newData),
                headers: {
                    "accept": "application/json;odata=verbose",
                    "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                    "X-HTTP-Method": "MERGE",
                    "content-type": "application/json;odata=verbose",
                    "If-Match": "*"
                },
                type: "POST",
                success: function () {
                    IsRequestSended = false;
                    $("#bSaveComment").removeAttr("disabled");
                },
                error: function () {
                    IsRequestSended = false;
                    $("#bSaveComment").removeAttr("disabled");
                    alert("RequestError!");
                }
            });
        }
    </script>
    <% } %>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Заседание Архитектурной комиссии
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Заседание Архитектурной комиссии
</asp:Content>
