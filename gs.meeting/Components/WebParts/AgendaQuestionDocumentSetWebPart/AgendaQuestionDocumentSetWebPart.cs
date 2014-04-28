// <copyright file="AgendaQuestionDocumentSetWebPart.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-02-18 13:40:16Z</date>
namespace gs.meeting.Components
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Security;
    using Microsoft.SharePoint.Utilities;
    using aspnetwebparts = System.Web.UI.WebControls.WebParts;
    using wsswebparts = Microsoft.SharePoint.WebPartPages;
    using Microsoft.Office.Server.WebControls;
    using Microsoft.Office.Server.Utilities;


    /// <summary>
    /// Веб часть  отображения содержимого набора документов
    /// </summary>
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    public class AgendaQuestionDocumentSetWebPart : DocumentSetContentsWebPart
    {
        private bool error = false;

        /// <summary>
        /// Initializes a new instance of the AgendaQuestionDocumentSetWebPart class
        /// </summary>
        public AgendaQuestionDocumentSetWebPart()
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
                returnValue = erAgendaQuestionItem.doGetQuestionDocumentSet(dsQuestionId, web);
            }

            return returnValue;
        }

        internal static SPListItem getDocumentSetItemById(int Id, SPWeb web)
        {
            SPListItem returnValue = null;
            SPDocumentLibrary targetLib = web.Lists.Cast<SPList>().FirstOrDefault(
                l => l.BaseTemplate.ToString().Equals(Consts.StorageListTemplateId)) as SPDocumentLibrary;

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

        private Control getLinkToDocumentSetControl(SPListItem dsItem)
        {
            string dsItemUrl = SPUrlUtility.CombineUrl(
                                    SPUrlUtility.CombineUrl(
                                        SPContext.Current.Web.Url,
                                        dsItem.ContentType.ResourceFolder.Url),
                                    "docsethomepage.aspx");
            NameValueCollection urlParams = buildDocumentSetUrlParamsCollection(dsItem);
            dsItemUrl = String.Concat(dsItemUrl, "?", buildQueryParamsString(urlParams));

            return new LiteralControl(
                @"<a style='padding: 20px;' href='" + dsItemUrl + @"' target='_blank' mscui:controltype='Button' role='button' unselectable='on'>
	                <div style='display: table;overflow: hidden;'>
		                <div style='display: table-cell;'>	                                
                            <span class=' ms-cui-img-32by32 ms-cui-img-cont-float ms-cui-imageDisabled' unselectable='on'>
			                    <img unselectable='on' src='/_layouts/images/docset_captureversion_32.png?rev=23'>
			                </span>
		                </div>
	                    <div style='display: table-cell;padding-left: 5px;vertical-align: middle;'>
			                <span unselectable='on'>Открыть материалы в отдельном окне</span>
		                </div>
                    </div>
                    </a>"
            );
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

            returnValue.Add("ID",           dsItem.ID.ToString());
            returnValue.Add("FolderCTID",   dsItem.ContentTypeId.ToString());
            returnValue.Add("List",         dsItem.ParentList.ID.ToString());
            returnValue.Add("RootFolder",   dsItem.Folder.Url);
            returnValue.Add("RecSrc",       dsItem.Folder.Url);

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
                    this.Controls.Add(
                        getLinkToDocumentSetControl(dsItem));

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
       
        protected override void OnLoad(EventArgs e)
        {
            if (!this.error)
            {
                try
                {
                    base.OnLoad(e);
                    this.EnsureChildControls();
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
}

