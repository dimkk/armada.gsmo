using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace SAMRT.Model.Definitions.Fields
{
    public static class IssueAttachmentMVKModel
    {
        public static readonly string GroupName = "САМРТ.РГ.Вложения вопроса заседания";

        public static FieldDefinition IssueAttachmentIssueMVK = new FieldDefinition
        {
            InternalName = "IssueAttachmentIssueMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("38C8DE07-2FE7-424D-9EE7-5FB887AEECF1"),
            Title = "Вопрос",
            ShowField = "Title"
        };
    }
}
