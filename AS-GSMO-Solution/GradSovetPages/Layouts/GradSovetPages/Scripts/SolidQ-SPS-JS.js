var debug = false;

function SolidQLookupField(listGuid, columnName, columnStaticName) {
    //NOTE:
    //In SharePoint, when you put a Single Lookup Column in a List, then this column may be represented or rendered
    //in two ways:
    //Type D:
    //	- If lookup list have less than 20 items
    //	- HTML generated is <select> that stores list of options with "Name" and "ID" of LookupField
    //
    //Type I:
    //	- If lookup list have 20 or more items
    //	- HTML generated is:
    //		- <input> that stores: choices (list of all pairs "Name", "ID"), optHid (id of hidden input that stores Selected Lookup ID), value (stores Selected Lookup Name)
    //		- Hidden <input>: stores selected id and is referenced by optHid
    //		- <img>: represents dropdown arrow
    //		- Dinamically generated <select>

    //GUID of the lookup list for this column
    this.listGuid = listGuid;
    this.getListGuid = function () {
        return this.listGuid;
    };

    //Name of the column in Form 
    //Remarks: Notice that may be in multiLanguage, make sure that pass
    //this parameter in correct language
    this.columnName = columnName;
    this.columnStaticName = columnStaticName;
    this.getColumnName = function () {
        return this.columnName;
    };

    //Instance of <select> field when fieldMode is D
    this.selectField = null;

    //Instance of <input> field when fieldMode is I
    this.inputField = null;
    this.getInputFieldValue = function () {
        if (this.inputField != null) {
            return this.inputField.value;
        }
        else {
            if (debug)
                LogError("SolidQLookupField.getInputFieldValue", "columnName: inputField", "inputField is null.");
            return null;
        }
    };

    //Instance of hidden <input> (ID) when fieldMode is I
    this.inputIDField = null;
    this.getInputIDFieldValue = function () {
        if (this.inputIDField != null) {
            return this.inputIDField.value;
        }
        else if (this.inputField != null) {
            this.inputIDField = document.getElementById(this.inputField.optHid);
            if (this.inputIDField != null) {
                return this.inputIDField.value;
            }
            else {
                if (debug)
                    LogError("SolidQLookupField.getInputIDFieldValue", "columnName: inputIDField", "inputIDField is null.");
                return 0;
            }
        }
        else {
            if (debug)
                LogError("SolidQLookupField.getInputIDFieldValue", "columnName: inputField", "inputField is null.");
            return 0;
        }
    };
    //Variable to track Input changes values in onpropertychange method when fieldMode = I
    this.oldInputFieldValue = '';

    //Variable to track Lock in onpropertychange event, when fieldMode = I
    this.inputFieldLock = false;

    //fieldMode have three posible values: 
    //'' --> Undefined
    //D  --> DropDown (When have less than 20 items)
    //I  --> Input (When have 20 or more items
    this.fieldMode = '';
    this.getFieldMode = function () {
        return this.fieldMode;
    };


    //Variable that stores 0 if there aren't field selected on load (None is considered as 0), 
    //otherwise, stores the ID from selected item
    this.selectedID = 0;
    this.generateSelectedID = function () {
        if (this.fieldMode == 'D') {
            this.selectedID = this.selectField.val();
            return this.selectedID;
        }
        else if (this.fieldMode == 'I') {
            //Proyecto tiene mas de 20 items --> Input / Select
            this.selectedID = this.getInputIDFieldValue();
            return this.selectedID;
        }
    }


    //SElected value
    this.selectedValue = '';
    this.generateSelectedValue = function () {
        if (this.fieldMode == 'D') {
            this.selectedValue = this.selectField.text();
            return this.selectedValue;
        }
        else if (this.fieldMode == 'I') {
            //Proyecto tiene mas de 20 items --> Input / Select
            this.selectedValue = this.inputField.value;
            return this.selectedValue;
        }
    }


    this.Init();
};


SolidQLookupField.prototype.Init = function () {
    this.selectField = $("select[title='" + this.columnName + "']");
    if (this.selectField != null) {
        this.fieldMode = 'D';
    }
    else {
        this.inputField = $("input[title='" + this.columnName + "']");
        if (this.inputField != null) {
            this.fieldMode = 'I';
            this.getInputIDFieldValue();

            //track value status
            this.oldInputFieldValue = this.inputField.value;

        }
        else {
            this.fieldMode = '';
            LogError("SolidQLookupField.Init", "columnName: " + this.columnName, "Error getting field object, fieldMode is Undefined.");
            return;
        }
    }

    this.generateSelectedID();
};

SolidQLookupField.prototype.AttachChangeEvent = function (cascadeObject) {
    var eventString = '';
    var thisclass = this;
    var func = null;

    if (this.fieldMode == 'D') {
        func = function () { thisclass.OnInputFieldChangeDropDown($(this), thisclass, cascadeObject); };
        this.selectField.change(func);
    }
    else if (this.fieldMode == 'I') {
        func = function () { thisclass.OnInputFieldChangeInput($(this), thisclass, cascadeObject); };
        this.inputField.change(func);
    }
    else {
        if (debug)
            LogError("SolidQLookupField.AttachChangeEvent", "columnName: " + this.columnName, "Error setting events, fieldMode is Undefined.");
    }
};

SolidQLookupField.prototype.RemoveValues = function (typeRemove) {
    //typeRemove
    //1: Selected Value is none --> in type I don't change input value
    //2: No eliminar el None
    if (this.fieldMode == 'D') {
        var options = this.selectField.prop('options');

        if (typeRemove == 3) {
            for (i = options.length - 1; i >= 0; i--) {
                if ((options[i].value != 0) && (!options[i].selected)) {
                    options.remove(i);
                }
            }
        }
        else {
            this.selectField.empty();
            this.selectField.append($('<option>', { value: 0, text: "" }));

        }
    }
    else if (this.fieldMode == 'I') {
        //value is None
        if (typeRemove == 1) {
            this.inputField.choices = '|0';
        }
        else if (typeRemove == 3) {
            this.inputField.choices = '|0';
            if (this.inputIDField.value != 0) {
                this.inputField.choices += '|' + this.inputField.value + '|' + this.inputIDField.value;
            }
        }
        else {
            this.inputIDField.value = 0;
            this.inputField.choices = '|0';
            this.inputField.value = '';
            this.generateSelectedID();
        }
    }
    else {
        if (debug)
            LogError("SolidQLookupField.RemoveValues", "columnName: " + this.columnName, "Error removing values, fieldMode is Undefined.");
        return;
    }

};

SolidQLookupField.prototype.AddItemCheckingSelected = function (id, title, selected) {
    if (this.fieldMode == 'D') {
        this.selectField.append($('<option>', { value: id, text: title, selected: selected }));
        if (selected)
            this.generateSelectedID();
    }
    else if (this.fieldMode == 'I') {
        var newTitle = title;
        if (title.indexOf("|") != -1) {
            newTitle = '';
            var mySplitResult = title.split('|');
            for (i = 0; i < mySplitResult.length; i++) {
                if (mySplitResult[i] != '')
                    newTitle += '||' + mySplitResult[i];
            }
        }
        //alert(newTitle);
        /*if (debug)
			alert(	'Antes \n' +
					'Choices: ' + this.inputField.choices + '\n' + 
					'Title: ' + newTitle + '\n' +
					'Id: ' + id + '\n' +
					'Selected: ' + selected + '\n');
		*/
        var cad = '|' + newTitle + '|' + id;
        this.inputField.choices = this.inputField.choices + cad;
        if (selected) {
            this.inputField.value = newTitle;
            this.inputIDField.value = id;
            this.generateSelectedID();
        }
        /*if (debug)
			alert(	'Despues \n' +
					'Choices: ' + this.inputField.choices + '\n' + 
					'Value: ' + this.inputField.value + '\n' +
					'Id: ' + this.inputIDField.value + '\n');
		*/
    }
    else {
        if (debug)
            LogError("SolidQLookupField.RemoveValues", "columnName: " + this.columnName, "Error removing values, fieldMode is Undefined.");
        return;
    }
};

SolidQLookupField.prototype.GetAllIDs = function () {
    //alert('Ento a GetAllIDs');
    var values = [];
    if (this.fieldMode == 'D') {
        for (i = 0; i < this.selectField.length; i++) {
            values.push(this.selectField.options[i].value);
        }
    }
    else if (this.fieldMode == 'I') {
        var vals = this.inputField.choices.split("|");
        for (i = 0; i < vals.length; i = i + 2) {
            values.push(vals[i + 1]);
        }
    }
    else {
        if (debug)
            LogError("SolidQLookupField.RemoveValues", "columnName: " + this.columnName, "Error removing values, fieldMode is Undefined.");
        return;
    }
    //alert('salgo de GetAllIDs');
    return values;
};

SolidQLookupField.prototype.DeleteItems = function (ids) {
    if (this.fieldMode == 'D') {
        //Delete all ids passed

        for (i = this.selectField.length - 1; i >= 0; i--) {
            var elem = this.selectField.options[i].value;
            for (b = 0; b < ids.length; b++) {
                if (elem == ids[b]) {
                    //alert('removing: ' + elem + ', at position: ' + i);
                    this.selectField.remove(i);
                }
            }
        }
    }
    else if (this.fieldMode == 'I') {
        var choices = this.inputField.choices.split('|');
        for (i = choices.length - 1; i >= 0; i--) {
            var elem = choices[i];
            for (b = 0; b < ids.length; b++) {
                if (elem == ids[b]) {
                    choices.splice(i - 1, 2);
                }
            }
        }
        var cadChoices = choices[0];
        for (b = 1; b < choices.length; b++) {
            cadChoices += '|' + choices[b];
        }
        this.inputField.choices = cadChoices;

        //if (selected != -1)
        //{
        //	this.inputIDField.value = id;
        //	this.inputField.value = title;
        //}

    }
    else {
        if (debug)
            LogError("SolidQLookupField.RemoveValues", "columnName: " + this.columnName, "Error removing values, fieldMode is Undefined.");
        return;
    }
};




/**************
	Event Methods
	Remarks: In event methods we can't invoke this.* properties, because it is running in anohter execution context. 
			Instead we can use event.srcElement and we can pass through parameter current object too
****************/
SolidQLookupField.prototype.OnInputFieldChangeDropDown = function (element, lookupObj, cascadeObject) {
    var newValue = element.val();
    var oldValue = lookupObj.oldInputFieldValue;

    if (oldValue != newValue) {
        if (lookupObj.inputFieldLock == false) {
            lookupObj.inputFieldLock = true;

            if (lookupObj.generateSelectedID() != 0) {
                // typeRemove = 0, in order to change selected value
                cascadeObject.FilterListByFieldID(0);
            }
            else {
                // removes all options except None (value = 0)
                cascadeObject.childField.RemoveValues(2);
            }


            //Assign new oldValue
            lookupObj.oldInputFieldValue = newValue;

            //No se desbloquea despues del aynch response del OnClientChangeGeneric porque si esa respuesta
            //no llega nunca el sistema queda inconsistente.
            lookupObj.inputFieldLock = false;
        }
    }
};

SolidQLookupField.prototype.OnInputFieldChangeInput = function (lookupObj, cascadeObject) {
    if (event.propertyName == 'value') {
        var newValue = event.srcElement.value;
        var oldValue = lookupObj.oldInputFieldValue;
        //if (debug)
        //	alert('Old Client Value: ' + oldValue + ', New Client Value: ' + event.srcElement.value);
        if (oldValue != newValue) {
            var clientChoices = event.srcElement.choices.split("|");
            var clientChoiceEncontrado = false;
            for (i = 0; i < clientChoices.length; i = i + 2) {
                //alert('newValue: ' + newValue + 'ClientChoice: ' + clientChoices[i] );
                if (newValue == clientChoices[i]) {
                    clientChoiceEncontrado = true;
                    break;
                }
            }
            if (clientChoiceEncontrado) {
                if (lookupObj.inputFieldLock == false) {
                    //Locking client access, not forgive to unlock in OnClientChange async response
                    lookupObj.inputFieldLock = true;

                    //if (debug)
                    //		alert('Entro en Filter del proertychanged con childSelected: ' + lookupObj.selectedID);

                    if (lookupObj.generateSelectedID() != 0) {
                        //typeRemove = 0, in order to change selected value
                        cascadeObject.FilterListByFieldID(0);
                    }
                    else {
                        cascadeObject.childField.RemoveValues(2);
                    }


                    //Assign new oldValue
                    lookupObj.oldInputFieldValue = newValue;

                    //No se desbloquea despues del aynch response del OnClientChangeGeneric porque si esa respuesta
                    //no llega nunca el sistema queda inconsistente.
                    lookupObj.inputFieldLock = false;
                }
            }
        }
    }
};


/***********************************
	SolidQ Cascade Relationship Class
 ***********************************/
function SolidQCascadeRelationShip(parentField, childField, nameFieldInChildList, staticNameFieldInChildList) {
    this.parentField = parentField;
    this.childField = childField;
    this.nameFieldInChildList = nameFieldInChildList;
    this.staticNameFieldInChildList = staticNameFieldInChildList;

    this.Init();
}

SolidQCascadeRelationShip.prototype.Init = function () {
    this.parentField.AttachChangeEvent(this);

    var selectedParent = this.parentField.selectedID;
    var selectedChild = this.childField.selectedID;

    if ((selectedParent == 0) && (selectedChild == 0)) {
        this.childField.RemoveValues(1);
    }
    else if ((selectedParent != 0) && (selectedChild != 0)) {
        this.FilterListByFieldID(3);
    }
    else if ((selectedParent != 0) && (selectedChild == 0)) {
        this.FilterListByFieldID(1);
    }
}

SolidQCascadeRelationShip.prototype.FilterListByFieldID = function (typeRemove) {
    var listGuid = this.childField.getListGuid();
    var parentLookupID = this.parentField.generateSelectedID();

    var staticNameFieldInChildListAux = this.staticNameFieldInChildList;

    if ((listGuid != null) && (parentLookupID != null) && (staticNameFieldInChildListAux != null)) {
        try {
            var clientContext = new SP.ClientContext.get_current();
            if (clientContext != undefined && clientContext != null) {
                var web = clientContext.get_web();
                var list = web.get_lists().getById(listGuid);
                var query = '<View Scope=\'RecursiveAll\'>' +
								'<Query>' +
									'<Where>' +
										'<Eq>' +
											'<FieldRef Name=\'' + staticNameFieldInChildListAux + '\' LookupId=\'TRUE\'/>' +
											'<Value Type=\'Lookup\'>' + parentLookupID + '</Value>' +
										'</Eq>' +
									'</Where>' +
								'</Query>' +
							'</View>';

                var camlQuery = new SP.CamlQuery();
                camlQuery.set_viewXml(query);
                this.productcollection = list.getItems(camlQuery);
                clientContext.load(this.productcollection);

                var thisclass = this;
                var func = function () { thisclass.processFilter(typeRemove); };
                clientContext.executeQueryAsync(Function.createDelegate(this, func), Function.createDelegate(this, this.processError));
            }
            else {
                LogError("SolidQCascadeRelationShip.FilterListByFieldID", "columnName: " + this.childField.getColumnName(), 'No se ha podido conectar a SharePoint para obtener los proyectos.');
            }
        }
        catch (e) {
            LogError("SolidQCascadeRelationShip.FilterListByFieldID", "columnName: " + this.childField.getColumnName(), 'Excepci�n. ' + e.message + '\n' + e.stackTrace);
        }
    }
}

SolidQCascadeRelationShip.prototype.processFilter = function (typeRemove) {
    try {
        var selectedID = this.childField.selectedID;

        this.childField.RemoveValues(typeRemove);

        var filteredValues = [];

        var listItemEnumerator = this.productcollection.getEnumerator();
        while (listItemEnumerator.moveNext()) {
            var oListItem = listItemEnumerator.get_current();

            var id = oListItem.get_item('ID');
            var title = oListItem.get_item('Title');

            if (id == selectedID) {
                if (typeRemove != 3) {
                    this.childField.AddItemCheckingSelected(id, title, true);
                }
            }
            else {
                this.childField.AddItemCheckingSelected(id, title, false);
            }
            filteredValues.push(id);
        }
    }
    catch (e) {
        LogError("SolidQCascadeRelationShip.processFilter", "columnName: " + this.childField.getColumnName(), 'Excepci�n. ' + e.message + '\n' + e.stackTrace);
    }
}

SolidQCascadeRelationShip.prototype.processError = function (sender, args) {
    LogError("SolidQCascadeRelationShip.processError", "columnName: " + this.childField.getColumnName(), 'Excepci�n. ' + args.get_message() + '\n' + args.get_stackTrace());
}

function getFirstTagFromIdentifier(tagName, identifier) {
    var len = identifier.length;
    var tags = document.getElementsByTagName(tagName);
    for (var i = 0; i < tags.length; i++) {
        var tempString = tags[i].id;
        if (tempString.indexOf(identifier) == tempString.length - len) {
            return tags[i];
        }
    }
    return null;
}

function getQueryStringParameter(ji) {
    hu = window.location.search.substring(1);
    gy = hu.split("&");
    for (i = 0; i < gy.length; i++) {
        ft = gy[i].split("=");
        if (ft[0] == ji) {
            return ft[1];
        }
    }
}

function LogError(func, param, msg) {
    var message = "Error in function " + func + "\n" +
		"Parameter: " + param + "\n" +
		"Message: " + msg + "\n";

    alert(message);
}
