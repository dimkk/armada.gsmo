using Microsoft.Office.Server.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using wsswebparts = Microsoft.SharePoint.WebPartPages;

// <copyright file="DocumentSetWebPart.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>SPDEV\achernikov</author>
// <date>2014-07-09 16:24:28Z</date>
namespace GS.MVK.Web
{
    /// <summary>
    /// TODO: Add comment for webpart DocumentSetWebPart
    /// </summary>
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    public class DocumentSet : DocumentSetContentsWebPart
    {
        private const string _ascxPath = @"/_CONTROLTEMPLATES/15/GS.MVK/DocumentSetUserControl.ascx";
        private bool error = false;

        [Category("Common"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string DocumentLibraryUrl { get; set; }

        [Category("Common"), WebBrowsable, Personalizable(PersonalizationScope.Shared)]
        public string QuestionLinkFieldName { get; set; }

        public DocumentSet()
        {
        }

        private SPListItem getCurrentDocumentSetItem(SPWeb web)
        {
            SPListItem returnValue = null;

            if (this.ViewState["DocSetID"] != null)
            {
                returnValue = getDocumentSetItemById(
                    int.Parse(this.ViewState["DocSetID"].ToString()), web);
            }
            else if (this.Page.Request.QueryString["ID"] != null)
            {
                int dsQuestionId = int.Parse(this.Page.Request.QueryString["ID"].ToString());
                returnValue = doGetQuestionDocumentSet(dsQuestionId, web);
            }

            return returnValue;
        }

        private SPListItem getDocumentSetItemById(int Id, SPWeb web)
        {
            SPListItem returnValue = null;
            SPDocumentLibrary targetLib = web.GetListByUrl(DocumentLibraryUrl) as SPDocumentLibrary;

            if (targetLib != null)
            {
                SPQuery query = new SPQuery()
                {
                    Query = @"<Where>
                                <Eq>
                                    <FieldRef Name='ID' />
                                    <Value Type='Integer'>" + Id + @"</Value>
                                </Eq>
                            </Where>"
                };
                query.ViewAttributes = "Scope='RecursiveAll'";

                SPListItemCollection items = targetLib.GetItems(query);
                returnValue = (items.Count != 1) ? null : items[0];
            }

            return returnValue;
        }

        private SPListItem doGetQuestionDocumentSet(int Id, SPWeb web)
        {
            SPListItem returnValue = null;
            var targetLib = web.GetListByUrl(DocumentLibraryUrl) as SPDocumentLibrary;

            if (targetLib != null)
            {
                var query = new SPQuery()
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
                            </Where>", QuestionLinkFieldName, Id)
                };
                query.ViewAttributes = "Scope='RecursiveAll'";

                var items = targetLib.GetItems(query);
                returnValue = (items.Count != 1) ? null : items[0];
            }

            return returnValue;
        }

        private string getLinkToDocumentSetControl(SPListItem dsItem)
        {
            string dsItemUrl = SPUrlUtility.CombineUrl(
                                    SPUrlUtility.CombineUrl(
                                        SPContext.Current.Web.Url,
                                        dsItem.ContentType.ResourceFolder.Url),
                                    "docsethomepage.aspx");
            NameValueCollection urlParams = buildDocumentSetUrlParamsCollection(dsItem);
            return String.Concat(dsItemUrl, "?", buildQueryParamsString(urlParams));
        }

        private static string buildQueryParamsString(NameValueCollection parameters)
        {
            List<string> items = new List<string>();
            foreach (string paramName in parameters)
                items.Add(String.Concat(paramName, "=", HttpUtility.UrlEncode(parameters[paramName])));

            return String.Join("&", items);
        }

        private static NameValueCollection buildDocumentSetUrlParamsCollection(SPListItem dsItem)
        {
            NameValueCollection returnValue = new NameValueCollection();

            returnValue.Add("ID", dsItem.ID.ToString());
            returnValue.Add("FolderCTID", dsItem.ContentTypeId.ToString());
            returnValue.Add("List", dsItem.ParentList.ID.ToString());
            returnValue.Add("RootFolder", dsItem.Folder.Url);
            returnValue.Add("RecSrc", dsItem.Folder.Url);

            return returnValue;
        }

        protected override void CreateChildControls()
        {
            if (!this.error)
            {
                try
                {
                    SPListItem dsItem = getCurrentDocumentSetItem(SPContext.Current.Web);
                    if (dsItem == null)
                        throw new Exception("Для элемента не найден набор документов с вложениями");

                    if (this.ViewState["DocSetID"] == null)
                        this.ViewState["DocSetID"] = dsItem.ID;
                    if (this.ViewState["DocSetListID"] == null)
                        this.ViewState["DocSetListID"] = dsItem.ParentList.ID;
                    base.m_docSetList = dsItem.ParentList;
                    base.m_docSetFolder = dsItem.Folder;

                    base.CreateChildControls();

                    DocumentSetUserControl control = Page.LoadControl(_ascxPath) as DocumentSetUserControl;
                    control.DocumentSetLink = getLinkToDocumentSetControl(dsItem);
                    Controls.Add(control);

                    wsswebparts.DataFormWebPart contentWP = base.Controls.
                        Cast<System.Web.UI.Control>().
                        OfType<wsswebparts.DataFormWebPart>().
                        FirstOrDefault();
                    if (contentWP != null)
                        contentWP.ParameterValues.Set("RootFolder", dsItem.Folder.Url);
                }
                catch (Exception ex)
                {
                    this.error = true;
                    this.Controls.Clear();
                    this.Controls.Add(new LiteralControl(ex.Message));
                }
            }
        }
    }

    public static class Extensions
    {
        public static SPList GetListByUrl(this SPWeb web, string url)
        {
            return web.GetList(SPUrlUtility.CombineUrl(web.Url, "Lists/" + url));
        }
    }
}

