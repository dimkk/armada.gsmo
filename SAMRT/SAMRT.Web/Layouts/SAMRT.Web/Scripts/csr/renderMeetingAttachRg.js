$(document).ready(function() {
	var meetingControl = $('[id^="MeetingAttachmentMeetingMVK"]');
	var docTypeControl = $('[id^="MeetingAttachmentDocTypeMVK"]');
	var protocolControl = $('[id^="MeetingAttachmentProtocolCopyMVK"]');
	var titleControl = $('[id^="Title"]');

	var meetingId = renderCore.getParentListItemId(['/Lists/MeetingMVKList/EditForm']);
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
