"use strict";

var gsFields = window.gsFields || {};
gsFields.CSR = gsFields.CSR || {};

(function () {
    // constructor
    gsFields.CSR.LinkedField = function (fieldName, autoSetLinkFromContext, parentListName, dlgUrl, renderFieldValueCallback) {
        this.fn = fieldName;
        this.auto = autoSetLinkFromContext;
        this.guiId = Math.random().toString(36).substr(2);
        this.dlg = dlgUrl + "?rev=" + this.guiId;
        // render field value callback
        this.renderFieldValue = renderFieldValueCallback;
        // selected value from dialog
        this.linkValue = null;
        // value from parent context
        this.autoValue = null;
        // previous field value
        this.prevValue = null;
        
        if (this.auto) {
            var contextId = renderCore && renderCore.getParentIdFromContext(parentListName);
            var sameList = ~document.location.href.indexOf(parentListName + '/DispForm') ||
                ~document.location.href.indexOf(parentListName + '/EditForm');
            
            if (!sameList) {
                this.autoValue = contextId;                
            } else {
                // maybe self master-detail link?
                var currentId = gsUtils.getIDParamFromUrl(document.location.href);
                var sameId = currentId && currentId == contextId;
                if (!sameId) {
                    this.autoValue = contextId;
                }
            }
        }
    };

    gsFields.CSR.LinkedField.prototype.init = function () {
        var fieldTemplate = {
            Templates: {
                Fields: {}
            },
            OnPostRender: onPostRender.bind(this)
        };
        fieldTemplate.Templates.Fields[this.fn] = {
            "NewForm":      renderNewFormField.bind(this),
            "EditForm":     renderEditFormField.bind(this),
            "DisplayForm":  renderDispFormField.bind(this)
        };

        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(fieldTemplate);
    };

    function renderSelectButton(href, targetId, caption) {
        var html = '';

        html +=
            '<div style="float: left;">';
        html += (String).format(
                '<a data-toggle="modal" href="{0}" data-target="#{1}">', href, targetId);
        html += (String).format(
                    '<button type="button" class="btn btn-default">{0}</button>', caption);
        html +=
                '</a>';
        html +=
            '</div>';
        html += (String).format(
            '<div class="modal fade" id="{0}" role="dialog" aria-hidden="true">', targetId);
        html +=
            '</div>';

        return html;
    }

    function renderClearButton(guiId, caption) {
        var html = '';

        html +=
            '<div style="float: left;">';
        html += (String).format(
                '<button id="clearbtn_{0}" type="button" class="btn btn-default">{1}</button>', guiId, caption);
        html +=
            '</div>';

        return html;
    }
    
    function renderPresentationContainer(itemId, text) {
        return (String).format('<div id="{0}" style="float: left; padding: 5px;">{1}</div>', 'view_' + itemId, text ? text : "");
    }

    function renderNewFormField(ctx) {
        // TODO
        var self = this;
        
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
        formCtx.registerGetValueCallback(formCtx.fieldName, function () {
            return self.autoValue ? self.autoValue : (self.linkValue ? self.linkValue.ID : null);
        });

        var html = "";
        if (self.autoValue) {
            html = (String).format("<div id='{0}'></div>", self.fn);
        } else {
            html =  renderSelectButton(self.dlg, self.guiId, "Выбрать");
            html += renderClearButton(self.guiId, "Очистить");
            html += renderPresentationContainer(self.guiId);
        }
        return html;
    };
    
    function renderEditFormField(ctx) {
        // TODO
        var self = this;
        
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
        self.prevValue = formCtx.fieldValue ? SPClientTemplates.Utility.ParseLookupValue(formCtx.fieldValue).LookupId : null;
        formCtx.registerGetValueCallback(formCtx.fieldName, function () {
            return self.linkValue ? self.linkValue.ID : self.prevValue;
        });

        var html = "";
        if (self.autoValue) {
            html = (String).format("<div id='{0}'></div>", self.fn);
        } else {
            html =  renderSelectButton(self.dlg, self.guiId, "Выбрать");
            html += renderClearButton(self.guiId, "Очистить");
            html += renderPresentationContainer(self.guiId, SPFieldLookup_Display(ctx));
        }
        return html;
    }
    
    function renderDispFormField(ctx) {
        return SPFieldLookup_Display(ctx);
    }

    function onClearButtonClick() {
        this.linkValue = this.prevValue = null;
        $("[id=view_" + this.guiId + "]").html("");
    }

    function onPostRender(ctx) {
        // TODO
        var self = this;

        var fieldName = ctx.ListSchema.Field && ctx.ListSchema.Field[0] && ctx.ListSchema.Field[0].Name;
        if (fieldName !== this.fn) return;
        
        if (self.autoValue) {
            var fieldElement = $('[id=' + self.fn + ']');
            fieldElement.html(
                (String).format("Идентификатор родителя {0}", self.autoValue));
            fieldElement.closest('tr').css('display', 'none');
            return;
        }

        var gsModals = window.gsModals = window.gsModals || {};
        var prevDlg = null;

        $('[id=' + this.guiId + ']')
            .appendTo('body')
            .on('shown.bs.modal', function (e) {
                prevDlg = gsModals.active;
                gsModals.active = e.target.id;
            })
            .on('hidden.bs.modal', function (e) {
                var model = ko.dataFor(this);
                if (model && (self.linkValue = model.selectedObject())) {
                    $("[id=view_" + self.guiId + "]").html(self.renderFieldValue(self.linkValue));
                }
                gsModals.active = prevDlg;
                prevDlg = null;
            });

        $('[id=clearbtn_' + self.guiId + ']').on('click', onClearButtonClick.bind(self));
    };

})();
