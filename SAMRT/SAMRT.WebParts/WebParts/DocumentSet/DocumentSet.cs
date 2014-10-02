using System.Web;
using ITB.SP.Tools;
using Microsoft.Office.Server.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using wsswebparts = Microsoft.SharePoint.WebPartPages;

namespace SAMRT.WebParts
{
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    public class DocumentSet : DocumentSetContentsWebPart
    {
        private readonly string _ascxPath = @"/_CONTROLTEMPLATES/15/SAMRT/DocumentSetUserControl.ascx";
        private bool _error;

        [Category("Common"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string DocumentLibraryUrl { get; set; }

        [Category("Common"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string QuestionLinkFieldName { get; set; }

        private SPListItem GetCurrentDocumentSetItem(SPWeb web)
        {
            SPListItem returnValue = null;

            if (ViewState["DocSetID"] != null)
            {
                returnValue = GetDocumentSetItemById(
                    int.Parse(ViewState["DocSetID"].ToString()), web);
            }
            else if (Page.Request.QueryString["ID"] != null)
            {
                int dsQuestionId = int.Parse(Page.Request.QueryString["ID"]);
                returnValue = DoGetQuestionDocumentSet(dsQuestionId, web);
            }

            return returnValue;
        }

        private SPListItem GetDocumentSetItemById(int Id, SPWeb web)
        {
            SPListItem returnValue = null;
            var targetLib = web.GetListByUrl(DocumentLibraryUrl) as SPDocumentLibrary;

            if (targetLib != null)
            {
                var query = new SPQuery
                {
                    Query = string.Format(@"<Where>" +
                                                "<Eq>" +
                                                    "<FieldRef Name='ID' />" +
                                                    "<Value Type='Integer'>{0}</Value>" +
                                                "</Eq>" +
                                            "</Where>", Id),
                    ViewAttributes = "Scope='RecursiveAll'"
                };

                SPListItemCollection items = targetLib.GetItems(query);
                returnValue = (items.Count != 1) ? null : items[0];
            }

            return returnValue;
        }

        private SPListItem DoGetQuestionDocumentSet(int Id, SPWeb web)
        {
            SPListItem returnValue = null;
            var targetLib = web.GetListByUrl(DocumentLibraryUrl) as SPDocumentLibrary;

            if (targetLib != null)
            {
                var query = new SPQuery
                {
                    Query = string.Format(@"<Where>
                                <And>
                                    <Eq>
                                        <FieldRef Name='{0}' LookupId='TRUE' />
                                        <Value Type='Lookup'>{1}</Value>
                                    </Eq>
                                    <BeginsWith>
			                            <FieldRef Name='ContentTypeId' />
			                            <Value Type='ContentTypeId'>0x0120D520</Value>
		                            </BeginsWith>
                                </And>
                            </Where>", QuestionLinkFieldName, Id),
                    ViewAttributes = "Scope='RecursiveAll'"
                };

                var items = targetLib.GetItems(query);
                returnValue = (items.Count != 1) ? null : items[0];
            }

            return returnValue;
        }

        private string GetLinkToDocumentSetControl(SPListItem dsItem)
        {
            string dsItemUrl = SPUrlUtility.CombineUrl(SPUrlUtility.CombineUrl(SPContext.Current.Web.Url, dsItem.ContentType.ResourceFolder.Url), "docsethomepage.aspx");
            string urlParams = BuildDocumentSetUrlParamsCollection(dsItem);
            return String.Concat(dsItemUrl, "?", urlParams);
        }

        private static string BuildDocumentSetUrlParamsCollection(SPListItem dsItem)
        {
            NameValueCollection result = HttpUtility.ParseQueryString(string.Empty);
            result.Add("ID", dsItem.ID.ToString());
            result.Add("FolderCTID", dsItem.ContentTypeId.ToString());
            result.Add("List", dsItem.ParentList.ID.ToString());
            result.Add("RootFolder", dsItem.Folder.Url);
            result.Add("RecSrc", dsItem.Folder.Url);
            return result.ToString();
        }

        protected override void CreateChildControls()
        {
            if (!_error)
            {
                try
                {
                    SPListItem dsItem = GetCurrentDocumentSetItem(SPContext.Current.Web);
                    if (dsItem == null)
                        throw new Exception("Для элемента не найден набор документов с вложениями");

                    if (ViewState["DocSetID"] == null)
                        ViewState["DocSetID"] = dsItem.ID;
                    if (ViewState["DocSetListID"] == null)
                        ViewState["DocSetListID"] = dsItem.ParentList.ID;
                    m_docSetList = dsItem.ParentList;
                    m_docSetFolder = dsItem.Folder;

                    base.CreateChildControls();

                    var control = (DocumentSetUserControl)Page.LoadControl(_ascxPath);
                    control.DocumentSetLink = GetLinkToDocumentSetControl(dsItem);
                    Controls.Add(control);

                    var contentWp = base.Controls.Cast<Control>().OfType<wsswebparts.DataFormWebPart>().FirstOrDefault();
                    if (contentWp != null)
                        contentWp.ParameterValues.Set("RootFolder", dsItem.Folder.Url);
                }
                catch (Exception ex)
                {
                    _error = true;
                    Controls.Clear();
                    Controls.Add(new LiteralControl(ex.Message));
                }
            }
        }
    }
}

