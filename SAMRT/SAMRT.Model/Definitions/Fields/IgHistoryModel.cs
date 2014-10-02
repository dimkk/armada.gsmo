using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace SAMRT.Model.Definitions.Fields
{
    public static class IgHistoryModel
    {
        public static readonly string GroupName = "САМРТ.Интеграция.История";

        public static FieldDefinition IgHistoryIssueRg = new FieldDefinition
        {
            InternalName = "IgHistoryIssueRg",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{7B7F2BB9-0BDE-4007-8ADD-268B5DB4ADC5}"),
            Title = "Вопрос РГ"

        };

        public static FieldDefinition IgHistoryIssueAk = new FieldDefinition
        {
            InternalName = "IgHistoryIssueAk",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{8748BFE5-F522-4D77-A3D3-2A8C92E4C4B5}"),
            Title = "Вопрос АК"
        };

        public static FieldDefinition IgHistoryDocumentTp = new FieldDefinition
        {
            InternalName = "IgHistoryDocumentTp",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{977DECC3-ED23-4E45-B961-A20BAD0D9C47}"),
            Title = "Документ ТП"
        };

        public static FieldDefinition IgHistoryDictionary = new FieldDefinition
        {
            InternalName = "IgHistoryDictionary",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{AE152DD6-B51A-4D1E-A231-5210C4B491C3}"),
            Title = "Справочник"
        };

        public static FieldDefinition IgHistoryDirection = new FieldDefinition
        {
            InternalName = "IgHistoryDirection",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{7BB19D05-8339-43F8-A08F-CD7C939DF7E5}"),
            Title = "Направление"
        };

        public static FieldDefinition IgHistorySenderSystem = new FieldDefinition
        {
            InternalName = "IgHistorySenderSystem",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{988C8455-FA44-4266-8F7C-71DB7480C829}"),
            Title = "Система-отправитель"
        };

        public static FieldDefinition IgHistoryReceiverSystem = new FieldDefinition
        {
            InternalName = "IgHistoryReceiverSystem",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{7E19DD4D-7C8F-4521-853D-19846F333129}"),
            Title = "Система-получатель"
        };

        public static FieldDefinition IgHistoryStatus = new FieldDefinition
        {
            InternalName = "IgHistoryStatus",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{327FCA25-25B5-432B-AE97-5946EEE2C5BC}"),
            Title = "Статус"
        };

        public static FieldDefinition IgHistoryError = new FieldDefinition
        {
            InternalName = "IgHistoryError",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{D9FD69AB-9F42-43FE-A6B6-B5E8445F1A73}"),
            Title = "Описание ошибки"
        };

        public static FieldDefinition IgHistorySendTryCount = new FieldDefinition
        {
            InternalName = "IgHistorySendTryCount",
            Description = "",
            FieldType = FieldType.Integer.ToString(),
            Group = GroupName,
            Id = new Guid("{E8D34977-EC68-48AC-A3D5-41E0558D61C3}"),
            Title = "Количество попыток отправки"
        };
    }
}
