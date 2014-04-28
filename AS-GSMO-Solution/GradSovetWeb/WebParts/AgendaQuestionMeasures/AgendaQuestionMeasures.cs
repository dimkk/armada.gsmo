using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace GradSovetWeb.WebParts.AgendaQuestionMeasures
{
    [ToolboxItemAttribute(false)]
    public class AgendaQuestionMeasures : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPathPrefix = @"~/_CONTROLTEMPLATES/15/GradSovetWeb.WebParts/AgendaQuestionMeasures/AgendaQuestionMeasures";

        protected override void CreateChildControls()
        {
            var ascxPath = SPContext.Current.FormContext.FormMode == SPControlMode.Display ? _ascxPathPrefix + "DisplayMode.ascx" : _ascxPathPrefix + "EditMode.ascx";

            Controls.Add(Page.LoadControl(ascxPath));
        }
    }
}
