using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace SAMRT.Model.Definitions.Fields
{
    public static class IgMessageModel
    {
        public static readonly string GroupName = "САМРТ.Интеграция.Сообщения";

        public static FieldDefinition IgMessageIgHistory = new FieldDefinition
        {
            InternalName = "IgMessageIgHistory",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{94C89DF9-F2CA-497E-B6CC-593D8AF6C1C5}"),
            Title = "История интеграции"

        };

        public static FieldDefinition IgMessageParentIgMessage = new FieldDefinition
        {
            InternalName = "IgMessageParentIgMessage",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{E87D03C9-443A-4EB7-9379-AF0E36466070}"),
            Title = "Родительское сообщение"
        };

        public static FieldDefinition IgMessageHandlerVersion = new FieldDefinition
        {
            InternalName = "IgMessageHandlerVersion",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{69A1A7A3-2E0F-46D8-9D3B-E519989DE17B}"),
            Title = "Версия обработчика сообщений"
        };

        public static FieldDefinition IgMessageDirection = new FieldDefinition
        {
            InternalName = "IgMessageDirection",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{76DC032B-C231-4A22-ABBF-EA1BE95E5353}"),
            Title = "Направление"
        };

        public static FieldDefinition IgMessageSenderSystem = new FieldDefinition
        {
            InternalName = "IgMessageSenderSystem",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{6A4F542E-A371-487D-947C-B67F65E28922}"),
            Title = "Система-отправитель"
        };

        public static FieldDefinition IgMessageReceiverSystem = new FieldDefinition
        {
            InternalName = "IgMessageReceiverSystem",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{1E493869-24C0-48BF-93FF-5248B80D9B6C}"),
            Title = "Система-получатель"
        };

        public static FieldDefinition IgMessageDataVersion = new FieldDefinition
        {
            InternalName = "IgMessageDataVersion",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{0D399778-6528-453E-BE98-1590ED8BFB12}"),
            Title = "Версия данных"
        };

        public static FieldDefinition IgMessageType = new FieldDefinition
        {
            InternalName = "IgMessageType",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{B804C2DA-3AA6-4BE5-A46A-42B30BFC5E8B}"),
            Title = "Тип сообщения"
        };

        public static FieldDefinition IgMessageContent = new FieldDefinition
        {
            InternalName = "IgMessageContent",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{E25E9C66-F749-47A7-952E-212997F2BDED}"),
            Title = "Содержимое сообщения"
        };

        public static FieldDefinition IgMessageIsSuccess = new FieldDefinition
        {
            InternalName = "IgMessageIsSuccess",
            Description = "",
            FieldType = FieldType.Boolean.ToString(),
            Group = GroupName,
            Id = new Guid("{9102F7EE-9733-46D4-B100-DD66859E2A60}"),
            Title = "Флаг успешной доставки"
        };

        public static FieldDefinition IgMessageError = new FieldDefinition
        {
            InternalName = "IgMessageError",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{01AD8699-2DF9-4E9A-876A-272D185DD767}"),
            Title = "Описание ошибки"
        };
    }
}
