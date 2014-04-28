using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace GSWeb.WebParts.AgendaQuestionConsideration
{
    public partial class AgendaQuestionConsiderationUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string Trim(string value, int len)
        {
            return value.Length <= len ? value : value.Substring(0, len - 3) + "...";
        }
    }
}
