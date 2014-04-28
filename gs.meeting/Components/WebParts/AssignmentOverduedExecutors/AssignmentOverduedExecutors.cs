// <copyright file="AssignmentOverduedExecutors.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-03-01 01:54:05Z</date>
namespace gs.meeting.Components
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Security;

    /// <summary>
    /// TODO: Add comment for webpart AssignmentOverduedExecutors
    /// </summary>
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    public class AssignmentOverduedExecutors : WebPart
    {
        private const string ASCXPATH = @"/_CONTROLTEMPLATES/15/gs_meeting/AssignmentOverduedExecutorsUserControl.ascx";

        private UserControl userControl;

        public AssignmentOverduedExecutors()
        {
        }

        protected override void CreateChildControls()
        {
            userControl = (UserControl)this.Page.LoadControl(ASCXPATH);

            Controls.Add(userControl);

            base.CreateChildControls();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.RenderContents(writer);
        }
    }
}

