using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace SAMRT.Model.Definitions.Fields
{
    public static class DocumentTPAttachmentModel
    {
        public static readonly string GroupName = "САМРТ.Вложения документа ТП";

        public static FieldDefinition DocumentTPAttachmentDocumentTP = new FieldDefinition
        {
            InternalName = "DocumentTPAttachmentDocumentTP",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("A1CC85D9-C227-487E-9616-F6331C4CAD92"),
            Title = "Документ ТП",
            ShowField = "Title"
        };
    }
}
