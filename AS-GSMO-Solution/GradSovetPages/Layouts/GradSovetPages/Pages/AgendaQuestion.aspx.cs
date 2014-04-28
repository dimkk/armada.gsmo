using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace GradSovetPages.Layouts.GradSovetPages
{
    public partial class AgendaQuestion : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void WPAgendaQuestionForm_Init(object sender, EventArgs e)
        {
            /*string listId = Page.Request.QueryString["List"];
            string itemId = Page.Request.QueryString["ID"];*/
            string mode = Page.Request.QueryString["mode"];

            ListFormWebPart wp = (ListFormWebPart)sender;
            
            /*wp.ListName = listId;
            wp.ListItemId = Convert.ToInt32(itemId);*/

            if (mode.Equals("display")) {
                wp.PageType = PAGETYPE.PAGE_DISPLAYFORM;
            }
            else if (mode.Equals("edit"))
            {
                wp.PageType = PAGETYPE.PAGE_EDITFORM;
            }
            else if (mode.Equals("new"))
            {
                wp.PageType = PAGETYPE.PAGE_NEWFORM;
            }
            else
            {
                wp.PageType = PAGETYPE.PAGE_INVALID;
            }
        }

        protected void WPAssignmentList_Init(object sender, EventArgs e)
        {
            SPWeb web = SPContext.Current.Web;
            SPList list = web.GetList("Lists/AssignmentList");
            SPView view = list.Views["Карточка вопроса"];

            XsltListViewWebPart wp = sender as XsltListViewWebPart;
            wp.ViewGuid = view.ID.ToString("B");
        }

        protected void WPAttachList_Init(object sender, EventArgs e)
        {
            SPWeb web = SPContext.Current.Web;
            SPList list = web.GetList("Lists/AgendaAttachmentList");
            SPView view = list.Views["Карточка вопроса"];

            XsltListViewWebPart wp = sender as XsltListViewWebPart;
            wp.ViewGuid = view.ID.ToString("B");
        }
    }
}
