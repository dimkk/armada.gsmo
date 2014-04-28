using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace GSWeb.erTitle
{
    public class erTitle: SPItemEventReceiver
    {
        public override void ItemAdding(SPItemEventProperties properties)
        {
            if (properties.Context != null)
            {
                try
                {
                    string defaultValue = String.Empty;
                    SPField titleField = properties.List.Fields.GetField("Title");
                    if ((titleField != null) && (titleField.DefaultValue != null))
                    {
                        defaultValue = titleField.DefaultValue;
                    }
                    
                    string userValue = String.Empty;
                    if (properties.AfterProperties["Title"] != null)
                    {
                        userValue = properties.AfterProperties["Title"].ToString();
                    }

                    if ((userValue == String.Empty) && (properties.AfterProperties["Title"] != null))
                    {
                        if (defaultValue != String.Empty)
                        {
                            properties.AfterProperties["Title"] = defaultValue;
                        }
                        else
                        {
                            properties.AfterProperties["Title"] = "Просмотреть";
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                    throw ex;
                }
            }

            base.ItemAdding(properties);
        }
    }
}
