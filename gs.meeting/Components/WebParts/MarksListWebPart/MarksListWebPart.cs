// <copyright file="MarksListWebPart.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-03-13 17:24:33Z</date>
namespace gs.meeting.Components
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Security;

    /// <summary>
    /// TODO: Add comment for webpart MarksListWebPart
    /// </summary>
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    public class MarksListWebPart : WebPart
    {
        private const string ASCXPATH = @"/_CONTROLTEMPLATES/15/gs_meeting/MarksListWebPartUserControl.ascx";
        private const string _webpartpropertieshiddenfieldid = "WebPartPropertiesHiddenFieldID";

        private UserControl userControl;
       
        public MarksListWebPart()
        {
        }

        protected override void CreateChildControls()
        {
            userControl = (UserControl)this.Page.LoadControl(ASCXPATH);
            var props = new HiddenField() {ID = _webpartpropertieshiddenfieldid};

            Controls.Add(userControl);
            Controls.Add(props);

            base.CreateChildControls();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.RenderContents(writer);
        }

        protected override void OnPreRender(EventArgs e)
        {
 	        base.OnPreRender(e);

            var paramsJSON = "{ \"DataSourceListName\": \"" + DataSourceListName + "\" }";
            ((HiddenField) this.FindControl(_webpartpropertieshiddenfieldid)).Value = paramsJSON;
        }

        [WebBrowsable(true)]
        [WebDisplayName("Название списка-источника данных")]
        [WebDescription("Укажите название списка, который будет служить источником данных для веб части. Список на основе шаблона Отчет по показателям работы министерства")]
        [Personalizable(PersonalizationScope.Shared)]
        public string DataSourceListName { get; set; }

        [WebBrowsable(true)]
        [WebDisplayName("Стилизованный заголовок")]
        [WebDescription("Заголовок веб части, отображающийся в стилизованной шапке")]
        [Personalizable(PersonalizationScope.Shared)]
        public string StyledTitle { get; set; }

    }
}

