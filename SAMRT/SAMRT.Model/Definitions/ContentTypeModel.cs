using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAMRT.Model.Definitions
{
    public static class ContentTypeModel
    {
        public static readonly string GroupName = "САМРТ";

        public static ContentTypeDefinition IssueObjectMVK = new ContentTypeDefinition
        {
            Name = "IssueObjectMVK",
            Description = "",
            Group = GroupName,
            Id = new Guid("B3AA4B99-E189-E24D-9B7C-6E884C0CEB84"),
            ParentContentTypeId = BuiltInContentTypeId.Item
        };

        public static ContentTypeDefinition MeetingAttachmentMVK = new ContentTypeDefinition
        {
            Name = "MeetingAttachmentMVK",
            Description = "",
            Group = GroupName,
            Id = new Guid("0B0FC5BE-7238-4B6E-95B4-FF3C32334D41"),
            ParentContentTypeId = BuiltInContentTypeId.Item
        };

        public static ContentTypeDefinition IssueAttachmentMVK = new ContentTypeDefinition
        {
            Name = "IssueAttachmentMVK",
            Description = "",
            Group = GroupName,
            Id = new Guid("0B0FC5BE-7238-4B6E-95B4-FF3C32334D41"),
            ParentContentTypeId = BuiltInSiteContentTypeId.DocumentSet
        };

        public static ContentTypeDefinition Attachment = new ContentTypeDefinition
        {
            Name = "Attachment",
            Description = "",
            Group = GroupName,
            Id = new Guid("C2B862B1-4E4F-4AC8-AB40-87D35B39ACE6"),
            ParentContentTypeId = BuiltInSiteContentTypeId.DocumentSet
        };

        public static ContentTypeDefinition DocumentTPAttachment = new ContentTypeDefinition
        {
            Name = "DocumentTPAttachment",
            Description = "",
            Group = GroupName,
            Id = new Guid("C9D840E6-ED43-49ED-8501-8C38A636CC4B"),
            ParentContentTypeId = BuiltInSiteContentTypeId.DocumentSet
        };

        public static ContentTypeDefinition IgHistory = new ContentTypeDefinition
        {
            Name = "IgHistory",
            Description = "",
            Group = GroupName,
            Id = new Guid("{CD89522D-9D8B-4690-BE2D-465C04603648}"),
            ParentContentTypeId = BuiltInSiteContentTypeId.Item
        };

        public static ContentTypeDefinition IgMessage = new ContentTypeDefinition
        {
            Name = "IgMessage",
            Description = "",
            Group = GroupName,
            Id = new Guid("{81F4E229-2C36-4620-B7C1-D5E8FDC86193}"),
            ParentContentTypeId = BuiltInSiteContentTypeId.Item
        };

        public static ContentTypeDefinition Municipality = new ContentTypeDefinition
        {
            Name = "Municipality",
            Description = "",
            Group = GroupName,
            Id = new Guid("{786A6077-A17C-49E5-9430-98935CBC5EFB}"),
            ParentContentTypeId = BuiltInSiteContentTypeId.Item
        };
    }
}
