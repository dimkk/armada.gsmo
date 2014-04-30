// <copyright file="NewsWebPart.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-03-11 14:31:00Z</date>

using System.ComponentModel;

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
    /// TODO: Add comment for webpart NewsWebPart
    /// </summary>
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    public class NewsWebPart : WebPart
    {
        private const string ASCXPATH = @"/_CONTROLTEMPLATES/15/gs_meeting/NewsWebPartUserControl.ascx";
        private const string _newswebpartpropsfieldid = "NewsWebPartPropsFieldID";

        private UserControl userControl;
        
        public NewsWebPart()
        {
        }

        protected override void CreateChildControls()
        {
            userControl = (UserControl)this.Page.LoadControl(ASCXPATH);
            var propsField = new HiddenField() { ID = _newswebpartpropsfieldid };

            Controls.Add(userControl);
            Controls.Add(propsField);

            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            string propsJSON = "{ \"DataSourceListName\": \"" + DataSourceListName + "\", \"LastDayCount\": \"" + LastDayCount + "\" }";
            ((HiddenField) this.FindControl(_newswebpartpropsfieldid)).Value = propsJSON;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.RenderContents(writer);
        }

        [WebBrowsable(true)]
        [WebDisplayName("Имя списка")]
        [WebDescription("Название библиотеки рисунков с новостями. Обязательно должно быть поле Url")]
        [Personalizable(PersonalizationScope.Shared)]
        public string DataSourceListName { get; set; }

        [WebBrowsable(true)]
        [WebDisplayName("Количество дней")]
        [WebDescription("Новости будут отбираться за последние дни, количество которых здесь указано")]
        [Personalizable(PersonalizationScope.Shared)]
        public int LastDayCount { get; set; }
    }
}

