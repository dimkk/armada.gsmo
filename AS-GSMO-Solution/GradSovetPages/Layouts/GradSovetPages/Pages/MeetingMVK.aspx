<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeetingMVK.aspx.cs" Inherits="GradSovetPages.Layouts.GradSovetPages.Pages.MeetingMvk" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/typeahead.min.js"></script>
    <script type="text/javascript" src="../Scripts/knockout-2.3.0.js"></script>
    <script type="text/javascript" src="../Scripts/moment-with-langs.min.js"></script>
    <script type="text/javascript" src="../Scripts/moment-datepicker.min.js"></script>
    <script type="text/javascript" src="../Scripts/moment-datepicker-ko.js"></script>
    <script type="text/javascript" src="/_layouts/15/SP.RequestExecutor.js"></script>

    <!-- Добавьте свои стили CSS в следующий файл -->
    <link rel="stylesheet" type="text/css" href="<%= SPUtility.MakeBrowserCacheSafeLayoutsUrl("gradsovetpages/Content/typeahead.js-bootstrap.css", false) %>" />
    <link rel="stylesheet" type="text/css" href="<%= SPUtility.MakeBrowserCacheSafeLayoutsUrl("gradsovetpages/Content/moment-datepicker/datepicker.css", false) %>" />
    <link rel="Stylesheet" type="text/css" href="<%= SPUtility.MakeBrowserCacheSafeLayoutsUrl("gradsovetpages/Content/App.css", false) %>" />

    <!-- Добавьте свой код JavaScript в следующий файл -->
    <script type="text/javascript" src="<%= SPUtility.MakeBrowserCacheSafeLayoutsUrl("gradsovetpages/Scripts/spin.min.js", false) %>"></script>
    <script type="text/javascript" src="<%= SPUtility.MakeBrowserCacheSafeLayoutsUrl("gradsovetpages/Scripts/camljs.js", false) %>"></script>
    <script type="text/javascript" src="<%= SPUtility.MakeBrowserCacheSafeLayoutsUrl("gradsovetpages/Scripts/Meeting/viewmodel.js", false) %>"></script>
    <script type="text/javascript">

        var modelMetaData = {
            meeting:            { listName: "MeetingMVKList", fields: [] },
            meetingAttachment:  { listName: "MeetingAttachmentMVKList", fields: [] },
            agendaQuestion:     { listName: "IssueMVKList", fields: [] }
        };

        $(function () {
            // move dialogs to the end of body due to unknown position of parents
            $('#questionModal').appendTo("body");
            $('#assignment-dialog-form').appendTo("body");

            // settings for agenda question projecttype typeahead
            $('#questionModal').on('shown.bs.modal', function () {
                var model = ko.dataFor(this);
                var dataset = model.allProjectsTypes();
                var elem = $('#inputProjectType');

                if (elem) {
                    elem.on('typeahead:selected', model.selectedAgendaQuestion().selectAgendaQuestionProjectType);

                    elem.typeahead({
                        name: 'projectType',                        local: dataset,
                        limit: 4
                    });
                }
            });
        });

        function selectRequestsTab(id) {
            $('.nav-tabs a[href="#' + id + '"]').tab('show');
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
            <!--<li class="">
                <a href="#tab-assignment" data-toggle="tab">Поручения</a>
            </li>-->
            <li class=""><a href="#tab-additional" data-toggle="tab">Дополнительная информация</a></li>
        </ul>

        <%-- Содержимое вкладок --%>
        <div id="tabContent" class="tab-content">
            <div id="tab-common" class="tab-pane fade active in">
                <div class="form-horizontal" role="form">
                    <div class="form-group topspace">
                        <div class="col-lg-2">
                            <label style="font-weight: normal">Дата проведения:</label>
                            <label data-bind="text: meeting().MeetingDateMVK"></label>
                        </div>
                        <div class="col-lg-5">
                            <label style="font-weight: normal">Место проведения:</label>
                            <label data-bind="text: meeting().MeetingPlaceMVK"></label>
                        </div>
                    </div>
                    <div class="form-group">
                    </div>
<%--                    <div class="form-group" data-bind="style: { display: scanAttach().FileUrl() == '' ? 'block' : 'none' }">
                        <label for="meetingScanAttach" class="col-lg-2 control-label">Электронная версия протокола</label>
                        <div class="col-lg-10">
                            <input data-bind="text: scanAttach().FilePath, event: { change: function (data, event) { scanAttach().selectedFile($element, data, event) } }, enable: editEnabled" id="meetingScanAttach" class="form-control" type='file' placeholder="Укажите файл протокола" />
                        </div>
                    </div>--%>
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
                                    <th>Номер</th>
                                    <th>Адрес</th>
                                    <th>Инвестор</th>
                                    <th>Описание</th>
                                    <th>Докладчики</th>
                                    <th></th>
                                    <th></th>
                                    <!--<th data-bind="style: { display: !editEnabled() ? 'none' : '' }"></th>-->
                                </tr>
                            </thead>
                            <tbody data-bind="foreach: agendaQuestions">
                                <tr>
                                    <td data-bind="text: IssueNumberTextMVK"></td>
                                    <td data-bind="text: IssueAddressMVK"></td>
                                    <td data-bind="text: IssueInvestorMVK"></td>
                                    <td data-bind="text: IssueDescriptionMVK"></td>
                                    <td data-bind="text: calcReporters"></td>
                                    <td>
                                        <button type="button" class="btn btn-default" data-bind="click: showAttachments"><span class="glyphicon glyphicon-paperclip"></span></button>
                                    </td>
                                    <td>
                                        <!--<button type="button" class="btn btn-default" data-bind="click: $parent.editAgendaQuestion"><span class="glyphicon glyphicon-edit"></span></button>-->
                                        <a data-bind="attr: { href: httpLink }" class="btn btn-default" target="_blank" role="button">
	                                        <span class="glyphicon glyphicon-edit"></span>
                                        </a>
                                    </td>
                                    <!--
                                    <td data-bind="style: { display: !$parent.editEnabled() ? 'none' : '' }">
                                        <button type="button" class="btn btn-default" data-bind="click: $parent.removeAgendaQuestion, enable: $parent.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                    </td>
                                    -->
                                </tr>
                            </tbody>
                        </table>
                        <button type="button" class="btn btn-primary" data-bind="click: addAgendaQuestion, enable: editEnabled" id="AddAgendaQuestionButton" name="AddAgendaQuestionButton">Добавить</button>
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
                        <button class="btn btn-primary" data-bind="click: addAttach, enable: editEnabled">Добавить</button>
                    </div>
                </div>
            </div>
            <!--<div id="tab-assignment" class="tab-pane fade">
                <div class="panel-group" id="aq-accordion" data-bind="foreach: agendaQuestions" style="margin-top: 25px; margin-bottom: 25px;">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <div class="row">
                                <div class="col-lg-1">
                                    <label data-bind="text: $root.filteredAssignments(editAgendaQuestionNumber()).length, attr: { class: $root.filteredAssignments(editAgendaQuestionNumber()).length == 0 ? 'label label-danger' : 'label label-default' }"></label>
                                </div>
                                <div class="col-lg-11">
                                    <h4 class="panel-title">
                                        <a class="accordion-toggle" data-toggle="collapse" data-parent="#aq-accordion" data-bind="text: editAgendaQuestionNumber() + '. ' + editAgendaQuestionTheme(), attr: { href: '#aq-panel-body-' + editAgendaQuestionNumber() }"></a>
                                    </h4>
                                    <label class="q-detail-info" data-bind="text: calcContent"></label>
                                </div>
                            </div>
                        </div>
                        <div data-bind="attr: { id: 'aq-panel-body-' + editAgendaQuestionNumber() }" class="panel-collapse collapse">
                            <div class="panel-body">
                                <table id="assignmentsTable" class="table">
                                    <thead>
                                        <tr>
                                            <th>Номер</th>
                                            <th>Срок исполнения</th>
                                            <th>Фактическая дата</th>
                                            <th>Контроль</th>
                                            <th>Ответственный</th>
                                            <th>Статус</th>
                                            <th>Текст поручения</th>
                                            <th></th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody data-bind="foreach: $root.filteredAssignments(editAgendaQuestionNumber())">
                                        <tr>
                                            <td data-bind="text: AssignmentNumber"></td>
                                            <td data-bind="text: AssignmentPlanDate"></td>
                                            <td data-bind="text: AssignmentFactDate"></td>
                                            <td data-bind="text: AssignmentInspectState"></td>
                                            <td data-bind="text: AssignmentExecutorFullNameLinkValue"></td>
                                            <td data-bind="text: AssignmentStatus"></td>
                                            <td data-bind="text: AssignmentText"></td>
                                            <td>
                                                <button type="button" class="btn btn-default" data-bind="click: $root.editAssignment"><span class="glyphicon glyphicon-edit"></span></button>
                                            </td>
                                            <td>
                                                <button type="button" class="btn btn-default" data-bind="click: $root.removeAssignment, enable: $root.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
                <button type="button" class="btn btn-primary" data-bind="click: addAssignment, enable: $root.editEnabled" id="addAssignmentButton" name="addAssignmentButton">Добавить</button>
            </div>-->
            <div id="tab-additional" class="tab-pane fade">
                <div class="form-horizontal" role="form">
                    <div class="form-group topspace">
                        <label for="inputNumber" class="col-lg-2 control-label">Номер</label>
                        <div class="col-lg-3">
                            <input data-bind="value: meeting().MeetingNumberMVK, enable: editEnabled" type="text" class="form-control" id="inputNumber" placeholder="Номер заседания">
                        </div>
                        <label for="inputStatus" class="col-lg-2 control-label">Статус</label>
                        <div class="col-lg-3">
                            <select id="inputStatus" class="form-control" placeholder="Выберите значение" data-bind="
    options: $root.availableMeetingStatuses,
    optionsText: 'name',
    optionsValue: 'name',
    value: meeting().MeetingStatusMVK,
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
            <div class="col-lg-offset-10 col-lg-1">
                <button class="btn btn-default" data-bind="click: closeForm" id="oCancelButton" name="oCancelButton">Закрыть</button>
            </div>
            <div class="col-lg-1">
                <button class="btn btn-success" data-bind="click: save, enable: $root.editEnabled" id="oSubmitButton" name="oSubmitButton">Сохранить</button>
            </div>
        </div>
    </div>
                        <%--Модальный диалог добавления вопроса повестки--%>
                        <div data-bind="showModal: selectedAgendaQuestion, with: selectedAgendaQuestion" class="modal fade" id="questionModal" tabindex="-1" role="dialog" aria-hidden="true">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                        <h4 class="modal-title" data-bind="text: New ? 'Новый вопрос повестки' : 'Вопрос #' + editAgendaQuestionNumber()"></h4>
                                    </div>
                                    <div class="modal-body">
                                        <div class="container">
                                            <div class="panel-group" id="addQuestionDlgAccordion">
                                                <div class="panel panel-default" data-bind="style: { display: mode() == 'onlyFiles' ? 'none' : '' }">
                                                    <div class="panel-heading">
                                                        <h4 class="panel-title">
                                                            <a class="accordion-toggle" data-toggle="collapse" data-parent="#addQuestionDlgAccordion" href="#addQcommon">Общие сведения</a>
                                                        </h4>
                                                    </div>
                                <div id="addQcommon" data-bind="attr: { class: mode() == 'onlyFiles' ? 'panel-collapse collapse' : 'panel-collapse collapse in' }">
                                                        <div class="panel-body">
                                                            <div class="container">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Номер</span>
                                                                    <input data-bind="value: editAgendaQuestionNumber, enable: $parent.editEnabled" type="text" id="inputNumberDlg" class="form-control" />
                                                                </div>
                                                                <br />
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Тема</span>
                                                                    <textarea data-bind="value: editAgendaQuestionTheme, enable: $parent.editEnabled" id="inputThemeDlg" class="form-control" rows="2"></textarea>
                                                                </div>
                                                                <br />
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Описание</span>
                                                                    <textarea data-bind="value: editAgendaQuestionDescription, enable: $parent.editEnabled" id="Textarea1" class="form-control" rows="2"></textarea>

                                                                </div>
                                                                <br />
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Связ. вопрос</span>
                                                                    <input data-bind="value: editAgendaLinkedQuestionLinkValue, enable: $parent.editEnabled" disabled="disabled" type="text" class="form-control" />
                                                                </div>
                                                                <br />
                                                                <div class="input-group">
                                                                    <button type="button" class="btn btn-default" data-bind="click: selectLinkedQuestion, enable: $parent.editEnabled" id="selectLinkedQuestionButton" name="selectLinkedQuestionButton">Выбрать</button>
                                                                </div>
                                                                <div style="display: none;">
                                                                    <input id="hiddenLinkedQuestion" data-bind="value: onLinkedQuestionChange" disabled="disabled" type="text" />
                                                                </div>
                                                                <br />
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Источники</span>
                                                                    <textarea data-bind="value: editAgendaQuestionExtResources, enable: $parent.editEnabled" id="inputAddResDlg" class="form-control" rows="2" placeholder="Укажите текстовый перечень внешних источников информации"></textarea>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="panel panel-default" data-bind="style: { display: mode() == 'onlyFiles' ? 'none' : '' }">
                                                    <div class="panel-heading">
                                                        <h4 class="panel-title">
                                                            <a class="accordion-toggle" data-toggle="collapse" data-parent="#addQuestionDlgAccordion" href="#addQcategory">Категория вопроса</a>
                                                        </h4>
                                                    </div>
                                                    <div id="addQcategory" class="panel-collapse collapse">
                                                        <div class="panel-body">
                                                            <div class="container">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Тип вопроса</span>
                                                                    <select class="form-control" data-bind="options: $root.agendaQuestionTypes, optionsText: 'Name', optionsValue: 'Id', optionsCaption: 'Выберите значение', value: editQuestionCategoryId, enable: $root.editEnabled"></select>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="panel panel-default" data-bind="style: { display: mode() == 'onlyFiles' ? 'none' : '' }">
                                                    <div class="panel-heading">
                                                        <h4 class="panel-title">
                                                            <a class="accordion-toggle" data-toggle="collapse" data-parent="#addQuestionDlgAccordion" href="#addQadditional">Дополнительная информация</a>
                                                        </h4>
                                                    </div>
                                                    <div id="addQadditional" class="panel-collapse collapse">
                                                        <div class="panel-body">
                                                            <div class="container">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Объект</span>
                                                                    <input data-bind="value: editAgendaQuestionSiteName, enable: $parent.editEnabled" type="text" id="inputObjectNameDlg" class="form-control" placeholder="Укажите полное наименование объекта" />
                                                                </div>
                                                                <br />
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Основание</span>
                                                                    <input data-bind="value: editAgendaQuestionReason, enable: $parent.editEnabled" type="text" id="inputCauseDlg" class="form-control" />
                                                                </div>
                                                                <br />
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Дата</span>
                                                                    <input data-bind="datepicker: editAgendaQuestionIncomingDate, datepickerOptions: { dataType: 'format', format: 'DD/MM/YYYY', autoHide: true, weekStart: 1 }, enable: $parent.editEnabled" type='text' class="form-control" placeholder="Дата поступления" id="inputArrivalDateDlg" />
                                                                </div>
                                                                <br />
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Тип проекта</span>
                                                                    <input id="inputProjectType" data-bind="value: editAgendaQuestionProjectType, enable: $parent.editEnabled" type="text" class="twitter-typeahead" placeholder="Выберите значение" />
                                                                </div>
                                                                <br />
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Адрес</span>
                                                                    <input data-bind="value: editAgendaQuestionAddress, enable: $parent.editEnabled" type="text" id="inputAddressDlg" class="form-control" />
                                                                </div>
                                                                <br />
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">Инвестор</span>
                                                                    <input data-bind="value: editAgendaQuestionInvestor, enable: $parent.editEnabled" type="text" id="inputInvestorDlg" class="form-control" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel panel-default" data-bind="style: { display: mode() == 'onlyFiles' ? 'none' : '' }">
                                                    <div class="panel-heading">
                                                        <h4 class="panel-title">
                                                            <a class="accordion-toggle" data-toggle="collapse" data-parent="#addQuestionDlgAccordion" href="#addQReporters">Докладчики</a>
                                                        </h4>
                                                    </div>
                                                    <div id="addQReporters" class="panel-collapse collapse">
                                                        <div class="panel-body">
                                                            <div class="well">
                                                                <label data-bind="style: { display: changedReporterIsNull() ? 'inline-block' : 'none' }">Выберите основного докладчика</label>
                                                                <label data-bind="text: editAgendaQuestionReporterFIO"></label>
                                                                <br />
                                                                <label data-bind="text: editAgendaQuestionReporterPosition"></label>
                                                                <br />
                                                                <label data-bind="text: editAgendaQuestionReporterOrganizationName"></label>
                                                                <div style="float: right;">
                                                                    <button class="btn btn-default" data-bind="click: selectReporter, enable: $parent.editEnabled" name="oSubmitButton">Выбрать</button>
                                                                </div>
                                                                <input style="display: none;" id="hiddenReporter" data-bind="value: changedReporter, valueUpdate: 'afterkeydown'" />
                                                            </div>
                                                            <table class="table">
                                                                <thead>
                                                                    <tr>
                                                                        <th>ФИО</th>
                                                                        <th>Должность</th>
                                                                        <th>Организация</th>
                                                                        <th></th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody data-bind="foreach: editAgendaQuestionSoreporters">
                                                                    <tr>
                                                                        <td data-bind="text: ParticipantFullName"></td>
                                                                        <td data-bind="text: ParticipantPosition"></td>
                                                                        <td data-bind="text: ParticipantOrg"></td>
                                                                        <td>
                                                                            <button type="button" class="btn btn-default" data-bind="click: $parent.removeSoreporter, enable: $root.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                            <div style="float: right;">
                                                                <button class="btn btn-default" data-bind="click: selectSoreporters, enable: $parent.editEnabled" id="addSoreporters" name="addSoreportersButton">Добавить</button>
                                                            </div>
                                                            <input style="display: none;" id="hiddenSoreporters" data-bind="value: changedSoreporters, valueUpdate: 'afterkeydown'"></input>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel panel-default">
                                                    <div class="panel-heading">
                                                        <h4 class="panel-title">
                                                            <a class="accordion-toggle" data-toggle="collapse" data-parent="#addQuestionDlgAccordion" href="#addQOutcome" data-bind="text: mode() == 'onlyFiles' ? 'Материалы' : 'Решение и вложения'"></a>
                                                        </h4>
                                                    </div>
                                                    <div id="addQOutcome" data-bind="attr: { class: mode() == 'onlyFiles' ? 'panel-collapse collapse in' : 'panel-collapse collapse' }">
                                                        <div class="panel-body">
                                        <textarea data-bind="value: editAgendaQuestionProtocolDecision, enable: $parent.editEnabled, style: { display: mode() == 'onlyFiles' ? 'none' : '' }" class="form-control" rows="4"></textarea>
                                                            <table id="agendaQuestionAttachmentsTable" class="table">
                                                                <thead>
                                                                    <tr>
                                                                        <th>Файл</th>
                                                                        <th>Тип документа</th>
                                                                        <th></th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody data-bind="foreach: editAgendaQuestionAttachments">
                                                                    <tr>
                                                                        <td>
                                                                            <a data-bind="attr: { href: FileUrl, target: '_blank' }, text: FileName, visible: FileUrl() != ''"></a>
                                                                            <input class="form-control" data-bind="text: FilePath, visible: FileUrl() == '', event: { change: function (data, event) { selectedFile($element, data, event) } }, enable: $root.editEnabled" type='file' />
                                                                        </td>
                                                                        <td>
                                                                            <select class="form-control" data-bind="options: $root.availableAttachDocTypes, optionsText: 'name', optionsValue: 'id', value: DType, enable: $root.editEnabled"></select>
                                                                        </td>
                                                                        <td>
                                                                            <button type="button" class="btn btn-default" data-bind="click: $parent.removeQuestionAttachment, enable: $root.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <textarea class="form-control" data-bind="value: Descr, enable: $root.editEnabled" rows="2" placeholder="Введите описание вложения"></textarea>
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                            <div style="float: right;">
                                                                <button type="button" class="btn btn-default" data-bind="click: addQuestionAttachment, enable: $parent.editEnabled" id="addQuestionAttachmentButton" name="addQuestionAttachmentButton">Добавить вложение</button>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-default" data-dismiss="modal" data-bind="click: $root.cancelAgendaQuestion">Закрыть</button>
                                        <button type="button" class="btn btn-primary" data-bind="click: $root.acceptAgendaQuestion, enable: $root.editEnabled">Сохранить</button>
                                    </div>
                                </div>
                                <!-- /.modal-content -->
                            </div>
                            <!-- /.modal-dialog -->
                        </div>
                        <!-- /.modal -->
                        <%-- //Модальный диалог добавления вопроса повестки--%>
                <%--Модальный диалог поручения--%>
                <div data-bind="showModal: selectedAssignment, with: selectedAssignment" class="modal fade" id="assignment-dialog-form" tabindex="-1" role="dialog" aria-hidden="true">

                    <%--Модальный диалог формы запроса отчета--%>
                    <div data-bind="showModal: selectedRequest, with: selectedRequest" class="modal fade" id="request-dialog-form" tabindex="-1" role="dialog" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                    <h4 class="modal-title">Запрос-отчет</h4>
                                </div>
                                <div class="modal-body">
                                    <%-- Вкладки --%>
                                    <ul id="tab-request" class="nav nav-tabs">
                                        <li data-bind="visible: !$root.isReporter()" class="active"><a href="#tab-request-request" data-toggle="tab">Запрос</a></li>
                                        <li class=""><a href="#tab-request-report" data-toggle="tab">Отчет</a></li>
                                    </ul>
                                    <%-- Содержимое вкладок --%>
                                    <div id="tab-request-content" class="tab-content">
                                        <div data-bind="visible: !$root.isReporter()" id="tab-request-request" class="tab-pane fade active in">
                                          <div class="container">
                                            <div class="input-group" style="margin-top: 25px">
                                                <span class="input-group-addon">Дата запроса</span>
                                                <input data-bind="datepicker: editAssignmentReportRequestDate, datepickerOptions: { dataType: 'format', format: 'DD/MM/YYYY', autoHide: true, weekStart: 1 }, enable: $root.editEnabled" type='text' class="form-control" />
                                            </div>
                                            <br />
                                            <div class="input-group">
                                                <span class="input-group-addon">План. ответ</span>
                                                <input data-bind="datepicker: editAssignmentReportRequestPlanAnswerDate, datepickerOptions: { dataType: 'format', format: 'DD/MM/YYYY', autoHide: true, weekStart: 1 }, enable: $root.editEnabled" type='text' class="form-control" />
                                            </div>
                                            <br />
                                            <div class="input-group">
                                                <span class="input-group-addon">Текст</span>
                                                <textarea data-bind="value: editAssignmentReportRequestText, enable: $root.editEnabled" rows="3" class="form-control" placeholder="Укажите текст запроса"></textarea>
                                            </div>
                                            <br />
                                            <div class="input-group">
                                                <span class="input-group-addon">Резолюция</span>
                                                <select data-bind="options: $root.assignmentReportResolutions, optionsText: 'name', optionsValue: 'name', value: editAssignmentReportResolutionDecision, enable: $root.editEnabled" class="form-control"></select>
                                            </div>
                                            <br />
                                            <div class="input-group" data-bind="style: { display: editAssignmentReportResolutionDecision() == 'Перенести срок' ? 'table' : 'none' }">
                                                <span class="input-group-addon">Новый срок</span>
                                                <input data-bind="datepicker: editAssignmentReportResolutionNewDate, datepickerOptions: { dataType: 'format', format: 'DD/MM/YYYY', autoHide: true, weekStart: 1 }, enable: $root.editEnabled" type='text' class="form-control" />
                                            </div>
                                            <br />
                                            <div class="input-group">
                                                <span class="input-group-addon">Комментарий</span>
                                                <input data-bind="value: editAssignmentReportResolutionNewDate, enable: $root.editEnabled" type="text" class="form-control" placeholder="Укажите комментарий к резолюции" />
                                            </div>
                                            <table id="request-attachments-table" class="table">
                                                <thead>
                                                    <tr>
                                                        <th>Файл</th>
                                                        <th>Тип документа</th>
                                                        <th></th>
                                                    </tr>
                                                </thead>
                                                <tbody data-bind="foreach: editRequestAttachmentsFiltered()">
                                                    <tr>
                                                        <td>
                                                            <a data-bind="attr: { href: FileUrl, target: '_blank' }, text: FileName, visible: FileUrl() != ''"></a>
                                                            <input data-bind="text: FilePath, visible: FileUrl() == '', event: { change: function (data, event) { selectedFile($element, data, event) } }, enable: $root.editEnabled" type='file' class="form-control" />
                                                        </td>
                                                        <td>
                                                            <select data-bind="options: $root.availableAttachDocTypes, optionsText: 'name', optionsValue: 'id', value: DType, enable: $root.editEnabled" class="form-control"></select>
                                                        </td>
                                                        <td>
                                                            <button type="button" class="btn btn-default" data-bind="click: $parent.removeReportAttach, enable: $root.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <textarea data-bind="value: Descr, enable: $root.editEnabled" class="form-control"></textarea>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <button type="button" class="btn btn-default" data-bind="click: addRequestAttach, enable: $root.editEnabled" id="addRequestAttachmentButton" name="addRequestAttachmentButton">Добавить вложение</button>
                                        </div>
                            </div>
                                        <div id="tab-request-report" class="tab-pane fade">
                                <div class="container">
                                            <div class="input-group" style="margin-top: 25px">
                                                <span class="input-group-addon">Дата</span>
                                                <input data-bind="datepicker: editAssignmentReportFactAnswerDate, datepickerOptions: { dataType: 'format', format: 'DD/MM/YYYY', autoHide: true, weekStart: 1 }, enable: $root.editEnabled" type='text' class="form-control" placeholder="Укажите дату предоставления отчета" />
                                            </div>
                                            <br />
                                            <div class="input-group">
                                                <span class="input-group-addon">Текст</span>
                                                <textarea data-bind="value: editAssignmentReportText, enable: $root.editEnabled" rows="5" class="form-control"></textarea>
                                            </div>
                                            <table id="report-attachments-table" class="table">
                                                <thead>
                                                    <tr>
                                                        <th>Файл</th>
                                                        <th>Тип документа</th>
                                                        <th></th>
                                                    </tr>
                                                </thead>
                                                <tbody data-bind="foreach: editReportAttachmentsFiltered()">
                                                    <tr>
                                                        <td>
                                                            <a data-bind="attr: { href: FileUrl, target: '_blank' }, text: FileName, visible: FileUrl() != ''"></a>
                                                            <input data-bind="text: FilePath, visible: FileUrl() == '', event: { change: function (data, event) { selectedFile($element, data, event) } }, enable: $root.editEnabled" type='file' class="form-control" />
                                                        </td>
                                                        <td>
                                                            <select data-bind="options: $root.availableAttachDocTypes, optionsText: 'name', optionsValue: 'id', value: DType, enable: $root.editEnabled" class="form-control"></select>
                                                        </td>
                                                        <td>
                                                            <button type="button" class="btn btn-default" data-bind="click: $parent.removeReportAttach, enable: $root.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <textarea data-bind="value: Descr, enable: $root.editEnabled" class="form-control"></textarea>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <button type="button" class="btn btn-default" data-bind="click: addReportAttach, enable: $root.editEnabled" id="addReportAttachmentButton" name="addReportAttachmentButton">Добавить вложение</button>
                                        </div>
                                    </div>
                                </div>
                    </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-default" data-dismiss="modal" data-bind="click: $parent.cancelRequest">Закрыть</button>
                                    <button type="button" class="btn btn-primary" data-bind="click: $parent.acceptRequest, enable: $root.editEnabled">Сохранить</button>
                                </div>
                            </div>
                            <!-- /.modal-content -->
                        </div>
                        <!-- /.modal-dialog -->
                    </div>
                    <!-- /.modal -->
                    <%-- //Модальный диалог формы запроса отчета--%>

                    <div class="modal-dialog modal-assignment">
                        <div class="modal-content">
                            <div class="modal-header">
                                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                <h4 class="modal-title" data-bind="text: New ? 'Новое поручение' : 'Поручение #' + editAssignmentNumber()"></h4>
                            </div>
                            <div class="modal-body">
                                <div class="well">
                                    <label>По вопросу</label>
                                    <select class="form-control" data-bind="options: $root.agendaQuestions, optionsText: 'ListTheme', optionsValue: 'AgendaQuestionNumber', value: editAgendaQuestionLinkValue, enable: $root.editEnabled"></select>
                                </div>
                                <%-- Вкладки поручения--%>
                                <ul id="assigntabs" class="nav nav-tabs">
                                    <li class="active"><a href="#assigntab-common" data-toggle="tab">Общая информация</a></li>
                                    <li class=""><a href="#assigntab-request" data-toggle="tab">Запросы и отчеты</a></li>
                                    <li class=""><a href="#assigntab-journal" data-toggle="tab">Журнал</a></li>
                                </ul>
                                <%-- Содержимое вкладок поручения--%>
                                <div id="assigntabs-content" class="tab-content">
                                    <div id="assigntab-common" class="tab-pane fade active in">
                                        <div class="panel-group" id="addAssignDlgAccordion" style="margin-top: 25px">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <h4 class="panel-title">
                                                        <a class="accordion-toggle" data-toggle="collapse" data-parent="#addAssignDlgAccordion" href="#addAcommon">Общие сведения</a>
                                                    </h4>
                                                </div>
                                                <div id="addAcommon" class="panel-collapse collapse in">
                                                    <div class="panel-body">
                                                        <div class="container">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Номер</span>
                                                                <input data-bind="value: editAssignmentNumber, enable: $root.editEnabled" type="text" class="form-control" />
                                                            </div>
                                                            <br />
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Тип</span>
                                                                <select class="form-control" data-bind="options: $root.assignmentTypes, optionsText: 'AssignmentTypeName', optionsValue: 'Id', value: assignmentTypeComputed, enable: $root.editEnabled"></select>
                                                            </div>
                                                            <br />
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Текст</span>
                                                                <textarea class="form-control" data-bind="value: editAssignmentText, enable: $root.editEnabled" rows="5"></textarea>
                                                            </div>
                                                            <br />
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Примечание</span>
                                                                <textarea class="form-control" data-bind="value: editAssignmentNote, enable: $root.editEnabled" rows="3"></textarea>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <h4 class="panel-title">
                                                        <a class="accordion-toggle" data-toggle="collapse" data-parent="#addAssignDlgAccordion" href="#addAdeadline">Сроки и контроль</a>
                                                    </h4>
                                                </div>
                                                <div id="addAdeadline" class="panel-collapse collapse">
                                                    <div class="panel-body">
                                                        <div class="container">
                                                            <div class="panel panel-default">
                                                                <div class="panel-heading">
                                                                    <h3 class="panel-title">Срок исполнения</h3>
                                                                </div>
                                                                <div class="panel-body">
                                                                    <div class="form-horizontal" role="form">
                                                                        <div class="form-group">
                                                                            <div class=" col-lg-4">
                                                                                <div class="radio">
                                                                                    <label>
                                                                                        <input type="radio" value="ByDate" data-bind="checked: editAssignmentPlanIsDependent, click: function () { editAssignmentDaysForResolve(''); editAssignmentDayType(''); editAssignmentPredecessorLinkValue(''); return true;}">
                                                                                        Абсолютный
                                                                                    </label>
                                                                                </div>
                                                                            </div>
                                                                            <div class="col-lg-8">
                                                                                <input data-bind="datepicker: editAssignmentPlanDate, attr: { 'disabled': editAssignmentPlanIsDependent() == 'ByDate' && $root.editEnabled() ? undefined : 'disabled' }, datepickerOptions: { dataType: 'format', format: 'DD/MM/YYYY', autoHide: true, weekStart: 1 }" type='text' class="form-control" />
                                                                            </div>
                                                                        </div>
                                                                        <div class="form-group">
                                                                            <div class=" col-lg-4">
                                                                                <div class="radio">
                                                                                    <label>
                                                                                        <input type="radio" value="ByAssignment" data-bind="checked: editAssignmentPlanIsDependent, click: function () { editAssignmentPlanDate(null); return true;}">
                                                                                        Относительный
                                                                                    </label>
                                                                                </div>
                                                                            </div>
                                                                            <div class=" col-lg-8">
                                                                                <input data-bind="value: editAssignmentDaysForResolve, attr: { 'disabled': editAssignmentPlanIsDependent() == 'ByAssignment' && $root.editEnabled() ? undefined : 'disabled' }" type="text" class="form-control" />
                                                                            </div>
                                                                        </div>
                                                                        <div class="form-group">
                                                                            <div class="col-lg-5">
                                                                                <select class="form-control" data-bind="options: $root.assignmentDayTypes, optionsText: 'name', optionsValue: 'name', value: editAssignmentDayType, attr: { 'disabled': editAssignmentPlanIsDependent() == 'ByAssignment' && $root.editEnabled() ? undefined : 'disabled' }"></select>
                                                                            </div>
                                                                            <div class="col-lg-2">
                                                                                дней после
                                                                            </div>
                                                                            <div class="col-lg-5">
                                                                                <select class="form-control" data-bind="options: $root.assignments, optionsText: 'AssignmentText', optionsValue: 'AssignmentText', value: editAssignmentPredecessorLinkValue, attr: { 'disabled': editAssignmentPlanIsDependent() == 'ByAssignment' && $root.editEnabled() ? undefined : 'disabled' }"></select>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <br />
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Факт</span>
                                                                <input data-bind="datepicker: editAssignmentFactDate, datepickerOptions: { dataType: 'format', format: 'DD/MM/YYYY', autoHide: true, weekStart: 1 }, enable: $root.editEnabled" type='text' class="form-control" placeholder="Фактическая дата выполнения" />
                                                            </div>
                                                            <br />
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Контроль</span>
                                                                <select class="form-control" data-bind="options: $root.assignmentInspectStates, optionsText: 'name', optionsValue: 'name', value: editAssignmentInspectState, enable: $root.editEnabled"></select>
                                                            </div>
                                                            <br />
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Статус</span>
                                                                <select class="form-control" data-bind="options: $root.assignmentStatuses, optionsText: 'name', optionsValue: 'name', value: editAssignmentStatus, enable: $root.editEnabled"></select>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <h4 class="panel-title">
                                                        <a class="accordion-toggle" data-toggle="collapse" data-parent="#addAssignDlgAccordion" href="#addAExecutors">Исполнители</a>
                                                    </h4>
                                                </div>
                                                <div id="addAExecutors" class="panel-collapse collapse">
                                                    <div class="panel-body">
                                                        <div class="well">
                                                            <label data-bind="style: { display: changedExecutorIsNull() ? 'inline-block' : 'none' }">Выберите ответственного исполнителя</label>
                                                            <label data-bind="text: editAssignmentExecutorFullNameLinkValue"></label>
                                                            <br />
                                                            <label data-bind="text: editAssignmentExecutorPositionLinkValue"></label>
                                                            <br />
                                                            <label data-bind="text: editAssignmentExecutorOrganizationLinkValue"></label>
                                                            <div style="float: right;">
                                                                <button class="btn btn-default" data-bind="click: selectExecutor, enable: $root.editEnabled" id="selectExecutorButton" name="oSubmitButton">Выбрать</button>
                                                            </div>
                                                            <input style="display: none;" id="hiddenExecutor" data-bind="value: changedExecutor, valueUpdate: 'afterkeydown'" />
                                                        </div>
                                                        <table class="table">
                                                            <thead>
                                                                <tr>
                                                                    <th>ФИО</th>
                                                                    <th>Должность</th>
                                                                    <th>Организация</th>
                                                                    <th></th>
                                                                </tr>
                                                            </thead>
                                                            <tbody data-bind="foreach: changedAssignmentSoexecutors">
                                                                <tr>
                                                                    <td data-bind="text: ParticipantFullName"></td>
                                                                    <td data-bind="text: ParticipantPosition"></td>
                                                                    <td data-bind="text: ParticipantOrg"></td>
                                                                    <td>
                                                                        <button class="btn btn-default" data-bind="click: $parent.removeAssignSoexecutor, enable: $root.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                        <div>
                                                            <div style="float: right;">
                                                                <button class="btn btn-default" data-bind="click: selectSoexecutors, enable: $root.editEnabled" id="addSoexecutorsButton" name="addSoexecutorsButton">Добавить</button>
                                                            </div>
                                                        </div>
                                                        <input style="display: none;" id="hiddenSoexecutors" data-bind="value: changedSoexecutors, valueUpdate: 'afterkeydown'" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="assigntab-request" class="tab-pane fade">
                                        <table id="assignRequestsTable" class="table">
                                            <thead>
                                                <tr>
                                                    <th>Дата запроса</th>
                                                    <th>Планируемая дата ответа</th>
                                                    <th>Фактическая дата ответа</th>
                                                    <th>Текст запроса</th>
                                                    <th>Текст ответа</th>
                                                    <th>Резолюция</th>
                                                    <th></th>
                                                    <th></th>
                                                </tr>
                                            </thead>
                                            <tbody data-bind="foreach: editAssignmentReports">
                                                <tr>
                                                    <td data-bind="text: editAssignmentReportRequestDate"></td>
                                                    <td data-bind="text: editAssignmentReportRequestPlanAnswerDate"></td>
                                                    <td data-bind="text: editAssignmentReportFactAnswerDate"></td>
                                                    <td data-bind="text: editAssignmentReportRequestText"></td>
                                                    <td data-bind="text: editAssignmentReportText"></td>
                                                    <td data-bind="text: editAssignmentReportResolutionDecision"></td>
                                                    <td>
                                                        <button type="button" class="btn btn-default" data-bind="click: $parent.editRequest"><span class="glyphicon glyphicon-edit"></span></button>
                                                    </td>
                                                    <td>
                                                        <button type="button" class="btn btn-default" data-bind="click: $parent.removeRequest, enable: $root.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                        <button type="button" class="btn btn-default" id="AddRequestButton" name="AddRequestButton" data-bind="click: addRequest, visible: !$root.isReporter()">Добавить запрос</button>
                                        <button type="button" class="btn btn-default" id="AddReportButton" name="AddReportButton" data-bind="click: addReport, enable: $root.editEnabled">Добавить отчет</button>
                                    </div>
                                    <div id="assigntab-journal" class="tab-pane fade">
                                        <table id="assignJournalTable" class="table">
                                            <thead>
                                                <tr>
                                                    <th>Дата и время</th>
                                                    <th>Пользователь</th>
                                                    <th>Описание</th>
                                                </tr>
                                            </thead>
                                            <tbody data-bind="foreach: assignmentJournal">
                                                <tr>
                                                    <td data-bind="text: CreatedDate"></td>
                                                    <td data-bind="text: Author"></td>
                                                    <td data-bind="text: AssignmentJournalEntryDescr"></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-default" data-dismiss="modal" data-bind="click: $root.cancelAssignment">Закрыть</button>
                                <button type="button" class="btn btn-primary" data-bind="click: $root.acceptAssignment, enable: $root.editEnabled">Сохранить</button>
                            </div>
                        </div>
                        <!-- /.modal-content -->
                    </div>
                    <!-- /.modal-dialog -->
                </div>
                <!-- /.modal -->
                <%-- //Модальный диалог поручения--%>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Страница заседания
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
</asp:Content>
