using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAMRT.Model.Definitions.Fields
{
    public static class IssueObjectMVKModel
    {
        public static readonly string GroupName = "САМРТ.РГ.Объекты вопроса заседания";

        public static FieldDefinition IssueObjectIssueMVK = new FieldDefinition
        {
            InternalName = "IssueObjectIssueMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{BEECC27F-669E-4E63-B410-3784D07EDFB3}"),
            Title = "Вопрос"

        };

        public static FieldDefinition IssueObjectIssueIdMVK = new FieldDefinition
        {
            InternalName = "IssueObjectIssueIdMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{6B21D56C-5798-4CE7-BCC3-60396CDD0988}"),
            Title = "Вопрос: Id",
            FieldRefId = IssueObjectIssueMVK.Id
        };

        public static FieldDefinition IssueObjectObjectMVK = new FieldDefinition
        {
            InternalName = "IssueObjectObjectMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{BA3B52C3-64D1-40FA-8AF1-66E0A24A19DC}"),
            Title = "Объект"
        };

        public static FieldDefinition IssueObjectObjectAddressMVK = new FieldDefinition
        {
            InternalName = "IssueObjectObjectAddressMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{D4CCECD5-129D-40C6-A9B8-531344915603}"),
            Title = "Объект: Адрес",
            FieldRefId = IssueObjectObjectMVK.Id
        };

        public static FieldDefinition IssueObjectObjectAreaMVK = new FieldDefinition
        {
            InternalName = "IssueObjectObjectAreaMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{F152E695-A760-4393-B222-66D11B1758DF}"),
            Title = "Объект: Общая площадь ЗУ",
            FieldRefId = IssueObjectObjectMVK.Id
        };

        public static FieldDefinition IssueObjectObjectCadastralIdMVK = new FieldDefinition
        {
            InternalName = "IssueObjectObjectCadastralIdMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{3299E650-06B8-4772-BA04-43E10DBF9A9B}"),
            Title = "Объект: Кадастровый номер ЗУ",
            FieldRefId = IssueObjectObjectMVK.Id
        };

        public static FieldDefinition IssueObjectObjectFirmNameMVK = new FieldDefinition
        {
            InternalName = "IssueObjectObjectFirmNameMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{70C480D0-7C1B-4E5A-899B-7165B5B3B733}"),
            Title = "Объект: Фирменное название",
            FieldRefId = IssueObjectObjectMVK.Id
        };

        public static FieldDefinition IssueObjectObjectFactDateMVK = new FieldDefinition
        {
            InternalName = "IssueObjectObjectFactDateMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{0A94744F-9012-46A4-9C68-5CEA553A56D4}"),
            Title = "Объект: Фактическая дата ввода",
            FieldRefId = IssueObjectObjectMVK.Id
        };

        public static FieldDefinition IssueObjectObjectDateMVK = new FieldDefinition
        {
            InternalName = "IssueObjectObjectDateMVK",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{6DC570A5-51E3-4B13-9AB2-00731B62456F}"),
            Title = "Объект: Дата начала строительства",
            FieldRefId = IssueObjectObjectMVK.Id
        };
    }
}
