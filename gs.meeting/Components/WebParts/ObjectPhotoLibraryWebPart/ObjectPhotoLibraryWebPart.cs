// <copyright file="ObjectPhotoLibraryWebPart.cs" company="Armada">
// Copyright Armada. All rights reserved.
// </copyright>
// <author>RKIS\developer</author>
// <date>2014-04-15 13:56:57Z</date>

using System.ComponentModel;
using System.Reflection;
using System.Web.UI.WebControls;

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
    /// TODO: Add comment for webpart ObjectPhotoLibraryWebPart
    /// </summary>
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    public class ObjectPhotoLibraryWebPart : WebPart
    {
        private const string ASCXPATH = @"/_CONTROLTEMPLATES/15/gs_meeting/ObjectPhotoLibraryWebPartUserControl.ascx";
        private const string _webpartpropertieshiddenfieldid = "WebPartPropertiesHiddenFieldID";
        private const string _connectionFailed =
            "���������� ���������� ���������� � ����������� ������ �� ������� �������������";
        private const string _connectionNoData =
            "��� ����� �� �������� ������ �� ���������� ���������� �� ������� �������������";
        private const string _paramNoList =
            "������� � ���������� ��� ����� �������� ������-��������� ������";
        private const string _paramNoGConf =
            "������� � ���������� ��� ����� ��������� ������������ ��� ����������� �������";
        private const string _paramNoGTheme =
            "������� � ���������� ��� ����� ����� �������, ����������� ���� ��� ����������� �������";
        private const string _paramNoLinkField =
            "������� � ���������� ��� ����� �������� ����-������ �� ������ � ������-��������� ������";
        private const string jsonParamFmt = "\"{0}\": \"{1}\"";
        private const string jsonNestedParamFmt = "\"{0}\": {1}";

        private UserControl _userControl;

        private IWebPartField _provider;
        private object _fieldValue;
        private System.Web.UI.WebControls.Label _message; 

        private void GetFieldValue(object value)
        {
            _fieldValue = value;
        }

        public bool ConnectionPointEnabled
        {
            get
            {
                object o = ViewState["ConnectionPointEnabled"];
                return (o == null) || (bool)o;
            }
            set
            {
                ViewState["ConnectionPointEnabled"] = value;
            }
        }

        [ConnectionConsumer("������ ������������� (�������������)", "CONNPOINT1", 
            typeof(PhotoLibraryConsumerConnectionPoint), AllowsMultipleConnections = true)]
        public void SetConnectionInterface(IWebPartField provider)
        {
            this._provider = provider;
        }

        public ObjectPhotoLibraryWebPart()
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (this._provider != null)
            {
                _provider.GetFieldValue(new FieldCallback(GetFieldValue));
            }

            base.OnPreRender(e);
        }

        protected override void CreateChildControls()
        {
            _userControl = (UserControl)this.Page.LoadControl(ASCXPATH);
            _message = (System.Web.UI.WebControls.Label)_userControl.FindControl("labelMessage");
            var props = new HiddenField() {ID = _webpartpropertieshiddenfieldid};

            Controls.Add(_userControl);
            Controls.Add(props);

            base.CreateChildControls();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            string objectId = String.Empty;

            // check for web part params
            if (String.IsNullOrEmpty(DataSourceListName))
            {
                _message.Text = _paramNoList;
            }
            else if (String.IsNullOrEmpty(LinkFieldName))
            {
                _message.Text = _paramNoLinkField;
            }
            else if (String.IsNullOrEmpty(GalleriaConf))
            {
                _message.Text = _paramNoGConf;
            }
            else if (String.IsNullOrEmpty(GalleriaTheme))
            {
                _message.Text = _paramNoGTheme;
            }
            // check for connection
            else if (_provider == null)
            {
                _message.Text = _connectionFailed;
            }
            else
            {
                PropertyDescriptor prop = _provider.Schema;
                if ((prop != null) && (_fieldValue != null))
                {
                    objectId = _fieldValue.ToString();
                    _message.Text = String.Empty;
                }
                else
                {
                    _message.Text = _connectionNoData;
                }
            }
            
            var paramsJSON = "{" +
                                String.Format(jsonParamFmt, "DataSourceListName", DataSourceListName) + "," +
                                String.Format(jsonParamFmt, "ObjectID", objectId) + "," +
                                String.Format(jsonParamFmt, "LinkFieldName", LinkFieldName) + "," +
                                String.Format(jsonParamFmt, "GalleriaTheme", GalleriaTheme) + "," +
                                String.Format(jsonNestedParamFmt, "GalleriaConf", GalleriaConf) +
                             "}";
            ((HiddenField)this.FindControl(_webpartpropertieshiddenfieldid)).Value = paramsJSON;

            this.RenderContents(writer);
        }

        public class PhotoLibraryConsumerConnectionPoint : ConsumerConnectionPoint
        {
            public PhotoLibraryConsumerConnectionPoint(MethodInfo callbackMethod, Type interfaceType,
                Type controlType, string name, string id, bool allowsMultipleConnections)
                : base(callbackMethod, interfaceType, controlType,
                    name, id, allowsMultipleConnections)
            {
                
            }

            public override bool GetEnabled(Control control)
            {
 	             return ((ObjectPhotoLibraryWebPart)control).ConnectionPointEnabled;
            }
        }

        [WebBrowsable(true)]
        [WebDisplayName("�������� ������-��������� ������")]
        [WebDescription("������� �������� ������, ������� ����� ������� ���������� ������ ��� ��� �����. ������ �� ������ ������� ���������� ���������� ������� �������������")]
        [Personalizable(PersonalizationScope.Shared)]
        public string DataSourceListName { get; set; }

        [WebBrowsable(true)]
        [WebDisplayName("�������� ����-������ �� ������ ������������� � ������-��������� ������ (InternalName)")]
        [WebDescription("������� ���������� ������������ ����, ������� �������� ������ �� ��������� ������ �������������")]
        [Personalizable(PersonalizationScope.Shared)]
        public string LinkFieldName { get; set; }

        [WebBrowsable(true)]
        [WebDisplayName("���� ��� ������� (������������� ����� �������, ������: classic/galleria.classic.min.js)")]
        [WebDescription("������� ������������� ����� �������, ����������� ���� �������")]
        [Personalizable(PersonalizationScope.Shared)]
        public string GalleriaTheme { get; set; }

        [WebBrowsable(true)]
        [WebDisplayName("��������� ������������ ��� ������� (http://galleria.io/docs/options/)")]
        [WebDescription("������� ��������� ���������������� ������� ����������")]
        [Personalizable(PersonalizationScope.Shared)]
        public string GalleriaConf { get; set; }
    }
}

