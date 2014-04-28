using System.ComponentModel;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using WebPart = System.Web.UI.WebControls.WebParts.WebPart;

namespace GSWeb.WebParts.AgendaQuestionConsideration
{
    [ToolboxItemAttribute(false)]
    public class AgendaQuestionConsideration : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/GSWeb.WebParts/AgendaQuestionConsideration/AgendaQuestionConsiderationUserControl.ascx";

        protected override void CreateChildControls()
        {
            var control = Page.LoadControl(_ascxPath);
            Controls.Add(control);

            control.Controls.OfType<SPDataSource>().First().SelectCommand =
                "<Query><Where><And>" +
                "<Or><Contains><FieldRef Name='AgendaQuestionAddress'/><Value Type='Text'>" + SPContext.Current.Item["AgendaQuestionAddress"] + "</Value></Contains>" +
                "<Contains><FieldRef Name='CadastreNumber'/><Value Type='Text'>" + SPContext.Current.Item["CadastreNumber"] + "</Value></Contains></Or>" +
                "<Neq><FieldRef Name='ID'/><Value Type='Text'>" + SPContext.Current.ItemId + "</Value></Neq>" +
                "</And></Where></Query>";
        }
    }
}
