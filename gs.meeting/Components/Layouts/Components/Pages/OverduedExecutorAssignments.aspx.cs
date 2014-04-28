// <copyright file="OverduedExecutorAssignments.aspx.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-03-04 14:38:01Z</date>

using System.Globalization;
using System.Linq;

namespace gs.meeting.Components
{
    using System;
    using System.IO;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Security;
    using Microsoft.SharePoint.Utilities;
    using Microsoft.SharePoint.WebControls;

    /// <summary>
    /// TODO: Add comment for OverduedExecutorAssignments
    /// </summary>
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    public partial class OverduedExecutorAssignments : LayoutsPageBase
    {
        private const string _modalPageViewFields = @"<FieldRef Name='LinkTitle' />
                                                    <FieldRef Name='AssignmentNumber' />
                                                    <FieldRef Name='AssignmentPlanDate' />
                                                    <FieldRef Name='AgendaQuestionLink' />
                                                    <FieldRef Name='AssignmentStatus' />";

        private const string _singlePageViewFields = @"<FieldRef Name='LinkTitle' />
                                                    <FieldRef Name='AssignmentNumber' />
                                                    <FieldRef Name='AssignmentPlanDate' />
                                                    <FieldRef Name='AgendaQuestionLink' />
                                                    <FieldRef Name='AssignmentStatus' />
                                                    <FieldRef Name='AssignmentInspectState' />
                                                    <FieldRef Name='AssignmentExecutorOrganizationLink' />";

        /// <summary>
        /// Initializes a new instance of the OverduedExecutorAssignments class
        /// </summary>
        public OverduedExecutorAssignments()
        {
            this.RightsCheckMode = RightsCheckModes.OnPreInit;
        }

        protected string camlQueryAllExecutorsTemplate
        {
            get
            {
                return @"<Where>
	                        <And>
		                        <IsNotNull>
			                        <FieldRef Name='AssignmentPlanDate' />
		                        </IsNotNull>
		                        <And>
			                        <Leq>
				                        <FieldRef Name='AssignmentPlanDate' />
				                        <Value IncludeTimeValue='False' Type='DateTime'>
					                        <Today />
				                        </Value>
			                        </Leq>
			                        <IsNull>
			                            <FieldRef Name='AssignmentFactDate' />
		                            </IsNull>
		                        </And>
	                        </And>
                        </Where>";
            }
        }

        protected string camlQueryExecutorNotNullTemplate
        {
            get
            {
                return @"<Where>
	                        <And>
		                        <IsNotNull>
			                        <FieldRef Name='AssignmentPlanDate' />
		                        </IsNotNull>
		                        <And>
			                        <Leq>
				                        <FieldRef Name='AssignmentPlanDate' />
				                        <Value IncludeTimeValue='False' Type='DateTime'>
					                        <Today />
				                        </Value>
			                        </Leq>
			                        <And>
				                        <IsNull>
			                                <FieldRef Name='AssignmentFactDate' />
		                                </IsNull>
				                        <Eq>
					                        <FieldRef Name='AssignmentExecutorOrganizationLink' LookupId='True' />
					                        <Value Type='Integer'>{0}</Value>
				                        </Eq>
			                        </And>
		                        </And>
	                        </And>
                        </Where>";
            }
        }

        protected string camlQueryExecutorIsNullTemplate
        {
            get
            {
                return @"<Where>
	                        <And>
		                        <IsNotNull>
			                        <FieldRef Name='AssignmentPlanDate' />
		                        </IsNotNull>
		                        <And>
			                        <Leq>
				                        <FieldRef Name='AssignmentPlanDate' />
				                        <Value IncludeTimeValue='False' Type='DateTime'>
					                        <Today />
				                        </Value>
			                        </Leq>
			                        <And>
				                        <IsNull>
			                                <FieldRef Name='AssignmentFactDate' />
		                                </IsNull>
				                        <IsNull>
					                        <FieldRef Name='AssignmentExecutorOrganizationLink' LookupId='True' />
				                        </IsNull>
			                        </And>
		                        </And>
	                        </And>
                        </Where>";
            }
        }

        private SPQuery CreateQuery(SPView view, object executorId)
        {
            string caml = null;
            string viewFields = null;
            uint rowLimit;
            
            if (executorId == null)
            {
                // single page all executors mode
                caml = camlQueryAllExecutorsTemplate;
                viewFields = _singlePageViewFields;
                rowLimit = 20;
            }
            else
            {
                // show particular executor mode
                var idInt = 0;
                int.TryParse(executorId.ToString(), out idInt);
                
                caml = idInt.Equals(0) // assignments without executor
                    ? camlQueryExecutorIsNullTemplate
                    : String.Format(camlQueryExecutorNotNullTemplate, idInt);
                viewFields = _modalPageViewFields;
                rowLimit = 10;
            }

            return new SPQuery(view)
            {
                Query = caml,
                ViewFields = viewFields,
                RowLimit = rowLimit
            };
        }

        /// <summary>
        /// Sets the inital values of controls
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            var siteCollection = this.Site;
            var site = this.Web;

            object exId;
            if (this.Page.IsPostBack)
            {
                // if paging then need to apply some crack - remove view param from querystring
                // source: http://karthikeyanblog.wordpress.com/2011/11/22/sorting-filtering-and-paging-in-listviewbyquery/
                var paged   = this.Context.Request.QueryString["Paged"];
                var view    = this.Context.Request.QueryString["View"];
                if (!string.IsNullOrEmpty(paged) && !string.IsNullOrEmpty(view))
                {
                    var newQueryString =
                        this.Context.Request.QueryString.Keys.Cast<string>()
                            .Where(key => key.ToLower() != "view")
                            .Aggregate(string.Empty,
                                (current, key) =>
                                    current + String.Format("{0}={1}&", key, this.Context.Request.QueryString[key]));
                    exId = this.ViewState["ExecutorId"];
                    if (exId != null)
                    {
                        int idInt = 0;
                        int.TryParse(exId.ToString(), out idInt);
                        newQueryString += "ExecutorId=" + idInt.ToString();
                    }

                    SPUtility.Redirect(this.Context.Request.Url.GetLeftPart(UriPartial.Path), SPRedirectFlags.Default,
                        HttpContext.Current, newQueryString);
                    return;
                }
            }

            MainListView.List = site.Lists.TryGetList("Поручения");
            exId = this.Context.Request.QueryString["ExecutorId"] != null
                ? this.Context.Request.QueryString["ExecutorId"]
                : this.ViewState["ExecutorId"];
            MainListView.Query = CreateQuery(
                MainListView.List.DefaultView, exId);
            MainListView.DisableFilter = true;
            MainListView.DisableSort = true;
            // need to save executor in order to process postback correctly
            if (!String.IsNullOrEmpty(this.Context.Request.QueryString["ExecutorId"]))
                this.ViewState["ExecutorId"] = this.Context.Request.QueryString["ExecutorId"];
        }

        /*
        Use this code to perform own security checks
        protected virtual void CheckCustomRights()
        {

          bool userCheckedForCustomLogic = false;
          //write here a custom logic to check if user has enough rights to access application page
          //if yes, set userCheckedForCustomLogic to true;

          if (!userCheckedForCustomLogic)
          {
            SPUtility.HandleAccessDenied(new UnauthorizedAccessException());
          }
        } 

        protected override void OnLoad(EventArgs e)
        {
          this.CheckCustomRights();   
        }
        */
    }
}

