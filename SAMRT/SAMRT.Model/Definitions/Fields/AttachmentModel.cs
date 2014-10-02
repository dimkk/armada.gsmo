using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace SAMRT.Model.Definitions.Fields
{
    public static class AttachmentModel
    {
        public static readonly string GroupName = "САМРТ.Вложения";

        public static FieldDefinition AttachmentOrder = new FieldDefinition
        {
            InternalName = "AttachmentOrder",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("96F0F6BE-9D71-4B75-A275-40D350E20670"),
            Title = "Заявка",
            ShowField = "Title"
        };

        public static FieldDefinition AttachmentIssueRg = new FieldDefinition
        {
            InternalName = "AttachmentIssueRg",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("42BE0175-05EC-4BC9-82B0-0DF9FEA1DC42"),
            Title = "Вопрос РГ",
            ShowField = "Title"
        };

        public static FieldDefinition AttachmentIssueAk = new FieldDefinition
        {
            InternalName = "AttachmentIssueAk",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("EE614418-D090-4148-AC5F-55BB21BFCC18"),
            Title = "Вопрос АК",
            ShowField = "Title"
        };
    }
}
