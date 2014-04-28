// <copyright file="Login.aspx.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-04-16 17:16:52Z</date>

using Microsoft.SharePoint.IdentityModel.Pages;

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
    using Microsoft.SharePoint.IdentityModel;

    /// <summary>
    /// TODO: Add comment for Login
    /// </summary>
    public partial class Login : FormsSignInPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}

