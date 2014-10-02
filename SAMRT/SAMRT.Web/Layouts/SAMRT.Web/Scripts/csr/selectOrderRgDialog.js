$(document).ready(function() {
	if (window.location.href.indexOf("IsDlg=1") >= 0) {
		var dlgCss = document.createElement('style');
		dlgCss.type = 'text/css';
		dlgCss.innerHTML = '#s4-ribbonrow, #Hero-WPQ2 { display: none !important; }';
		document.getElementsByTagName('head')[0].appendChild(dlgCss);
		
		var okbutton = $('<button id="okbutton" type="button" class="btn btn-default">Выбрать</button>');
		okbutton.click(function() {
			SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.OK, JSON.stringify(SP.ListOperation.Selection.getSelectedItems()));
		});
		$("#content").append(okbutton);
	}
});
