using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace SAMRT.Model.Definitions.Fields
{
    public static class MeetingAttachmentMVKModel
    {
        public static readonly string GroupName = "САМРТ.РГ.Вложения заседания";

        public static FieldDefinition MeetingAttachmentMeetingMVK = new FieldDefinition
        {
            InternalName = "MeetingAttachmentMeetingMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("9F034BE1-66BF-4B79-8A80-8AD13A96E475"),
            Title = "Заседание",
            ShowField = "MeetingNumberMVK"
        };

        public static FieldDefinition MeetingAttachmentDocTypeMVK = new FieldDefinition
        {
            InternalName = "MeetingAttachmentDocTypeMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("181C78E7-4770-4683-AAA2-7F490EED7FFE"),
            Title = "Тип документа",
            ShowField = "Title"
        };

        public static FieldDefinition MeetingAttachmentDescriptionMVK = new FieldDefinition
        {
            InternalName = "MeetingAttachmentDescriptionMVK",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("6105AB61-5E4C-4561-A323-D31440C1EF2F"),
            Title = "Описание вложения"
        };

        public static FieldDefinition MeetingAttachmentProtocolCopyMVK = new FieldDefinition
        {
            InternalName = "MeetingAttachmentProtocolCopyMVK",
            Description = "",
            FieldType = FieldType.Boolean.ToString(),
            Group = GroupName,
            Id = new Guid("{154F8D91-1D78-406F-A013-B569B7BBC728}"),
            Title = "Скан-копия протокола"
        };
    }
}
