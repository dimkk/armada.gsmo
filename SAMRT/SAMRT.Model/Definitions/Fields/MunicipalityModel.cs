using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace SAMRT.Model.Definitions.Fields
{
    public static class MunicipalityModel
    {
        public static readonly string GroupName = "САМРТ.Справочник муниципальных образований";

        public static FieldDefinition MunicipalityParentMunicipality = new FieldDefinition
        {
            InternalName = "MunicipalityParentMunicipality",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{6FACC932-0588-4A36-B1E9-1F7312877EDF}"),
            Title = "Родитель"
        };

        public static FieldDefinition MunicipalityType = new FieldDefinition
        {
            InternalName = "MunicipalityType",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{64E834C1-FAFF-4CF9-96A3-9AD68FB44090}"),
            Title = "Тип"

        };

        public static FieldDefinition MunicipalityOkato = new FieldDefinition
        {
            InternalName = "MunicipalityOkato",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{95800601-FA78-436A-87B5-DDF48F668E18}"),
            Title = "ОКАТО"
        };
    }
}
