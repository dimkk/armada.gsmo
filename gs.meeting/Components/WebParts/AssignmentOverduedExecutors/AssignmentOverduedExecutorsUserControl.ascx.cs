// <copyright file="AssignmentOverduedExecutorsUserControl.ascx.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-03-01 01:54:13Z</date>
namespace gs.meeting.Components
{
    using System.Web.UI.WebControls;

    /// <summary>
    /// TODO: Add comment for usercontrol AssignmentOverduedExecutorsUserControl 
    /// </summary>
    public partial class AssignmentOverduedExecutorsUserControl : System.Web.UI.UserControl
    {
        // TODO: Add code behind
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            OrgLabel.Text = "Организация";
            AmountLabel.Text = "Кол-во";
            PanelTitleLabel.Text = "Просроченные поручения";
        }
    }
}

