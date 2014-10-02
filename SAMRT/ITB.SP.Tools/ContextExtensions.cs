using Microsoft.SharePoint;

namespace ITB.SP.Tools
{
    public static class ContextExtensions
    {
        public static string GetFieldValue(this SPContext context, string fieldName)
        {
            return context.ListItem.GetFieldValue(fieldName);
        }
    }
}
