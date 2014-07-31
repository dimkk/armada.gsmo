// <copyright file="MarksListWebPartUserControl.ascx.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-03-13 17:24:38Z</date>
namespace gs.meeting.Components
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Add comment for usercontrol MarksListWebPartUserControl 
    /// </summary>
    public partial class MarksListWebPartUserControl : System.Web.UI.UserControl
    {
        private const string _captionStub = "Рассмотрено на ГрадСовете";

        public string CountMarkBackColor { get; set; }

        public string TextMarkBackColor { get; set; }

        // TODO: Add code behind
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            var webPart = this.Parent as MarksListWebPart;
            PanelTitleLabel.Text = webPart == null
                ? _captionStub
                : (String.IsNullOrEmpty(webPart.StyledTitle) ? _captionStub : webPart.StyledTitle);
        }
    }
}

