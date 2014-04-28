function getSelectedWebPartId() {
    var e = document.getElementById("_wpSelected");
	if (!e) return null;

	var selectedId = e.getAttribute("value");
	if (!selectedId) return null;

	selectedId = selectedId.substr(12);
	e = document.getElementById(selectedId);
	if (!e) return null;

	var wpId;
	if (window._spWebPartComponents &&
		window._spWebPartComponents[selectedId]) {
		wpId = window._spWebPartComponents[selectedId].storageId;
	}
	else {
		wpId = e.getAttribute("WebPartID");
	}

	return wpId;
}