$(document).ready(function() {
	var meetingControl = $('[id^="MeetingLink"]');
	var docTypeControl = $('[id^="AttachmentDocumentTypeLink"]');
	var protocolControl = $('[id^="AttachmentProtocolScanCopy"]');
	var titleControl = $('[id^="Title"]');

	var meetingId = renderCore.getParentListItemId(['/Lists/MeetingList/EditForm']);
	if (meetingId) {
		meetingControl.val(meetingId);
		meetingControl.attr('disabled', 'disabled');
	}
	protocolControl.attr('disabled', 'disabled');
	titleControl.attr('disabled', 'disabled').val(docTypeControl.find('option:selected').text());
	docTypeControl.change(function() {
		titleControl.val(this.options[this.selectedIndex].innerHTML);
		protocolControl.prop('checked', this.value == '1');
	});
});
