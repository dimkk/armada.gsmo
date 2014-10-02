using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAMRT.Model.Definitions
{
    public static class ListModel
    {
        public static ListDefinition IssueObjectMVKList = new ListDefinition
        {
            Title = "РГ - Объекты вопроса повестки заседания",
            Url = "IssueObjectMVKList",
            TemplateType = (int)ListTemplateType.GenericList, //10057,
            Description = "",
            ContentTypesEnabled = true
        };

        public static ListDefinition MeetingAttachmentMVKList = new ListDefinition
        {
            Title = "РГ - Вложения заседания",
            Url = "MeetingAttachmentMVKList",
            TemplateType = (int)ListTemplateType.GenericList, //10058,
            Description = "",
            ContentTypesEnabled = true
        };

        public static ListDefinition IssueAttachmentMVKList = new ListDefinition
        {
            Title = "РГ - Вложения вопроса повестки заседания",
            Url = "IssueAttachmentMVKList",
            TemplateType = (int)ListTemplateType.DocumentLibrary, //10059,
            Description = "",
            ContentTypesEnabled = true
        };

        public static ListDefinition AttachmentList = new ListDefinition
        {
            Title = "Вложения",
            Url = "AttachmentList",
            TemplateType = (int)ListTemplateType.DocumentLibrary,
            Description = "",
            ContentTypesEnabled = true
        };

        public static ListDefinition DocumentTPAttachmentList = new ListDefinition
        {
            Title = "Вложения документа ТП",
            Url = "DocumentTPAttachmentList",
            TemplateType = (int)ListTemplateType.DocumentLibrary,
            Description = "",
            ContentTypesEnabled = true
        };

        public static ListDefinition IgHistory = new ListDefinition
        {
            Title = "Интеграция - История",
            Url = "IgHistory",
            TemplateType = (int)ListTemplateType.GenericList,
            Description = "",
            ContentTypesEnabled = true
        };

        public static ListDefinition IgMessage = new ListDefinition
        {
            Title = "Интеграция - Сообщения",
            Url = "IgMessage",
            TemplateType = (int)ListTemplateType.GenericList,
            Description = "",
            ContentTypesEnabled = true
        };

        public static ListDefinition Municipality = new ListDefinition
        {
            Title = "Муниципальные образования",
            Url = "Municipality",
            TemplateType = (int)ListTemplateType.GenericList,
            Description = "",
            ContentTypesEnabled = true
        };
    }
}
