// <copyright file="NewsWebPartUserControl.ascx.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-03-11 14:31:04Z</date>
namespace gs.meeting.Components
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Add comment for usercontrol NewsWebPartUserControl 
    /// </summary>
    public partial class NewsWebPartUserControl : System.Web.UI.UserControl
    {
        // TODO: Add code behind
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            PanelTitleLabel.Text = "Новости";
        }
    }

}

