using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using CamlexNET;
using Microsoft.SharePoint;
using WebPart = System.Web.UI.WebControls.WebParts.WebPart;

namespace GSWeb.WebParts.FilterProvider
{
    [ToolboxItemAttribute(false)]
    public sealed class FilterProvider : WebPart, IWebPartRow
    {
        public FilterProvider()
        {
            _data.Columns.Add("ListItemId", typeof(int));
            _data.Columns.Add("Organization", typeof(string));

            var web = SPContext.Current.Web;

            //var participantBookList = web.Lists["Справочник участников"];
            //var query = Camlex.Query()
            //    .Where(x => x["Профиль"] == (DataTypes.UserId)web.CurrentUser.ID.ToString())
            //    .ToSPQuery();

            //var organization = participantBookList.GetItems(query).OfType<SPListItem>().Select(item => item["Профиль"]).FirstOrDefault();

            var organization = from item in web.Lists["Справочник участников"].Items.OfType<SPListItem>()
                               let user = new SPFieldLookupValue(item["Профиль"] as string)
                               where user.LookupId == web.CurrentUser.ID
                               select new SPFieldLookupValue(Convert.ToString(item["Организация"])).LookupId;

            _data.Rows.Clear();
            _data.Rows.Add(SPContext.Current.ItemId, organization.FirstOrDefault());
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Hidden = WebPartManager.DisplayMode == WebPartManager.BrowseDisplayMode;

            Controls.Add(new Label { Text = "list item id: " + _data.DefaultView[0]["ListItemId"] });
            Controls.Add(new Label { Text = "organization: " + _data.DefaultView[0]["Organization"] });
        }

        [ConnectionProvider("Row")]
        public IWebPartRow ConnectionInterface()
        {
            return this;
        }

        public PropertyDescriptorCollection Schema
        {
            get { return TypeDescriptor.GetProperties(_data.DefaultView[0]); }
        }

        public void GetRowData(RowCallback callback)
        {
            callback(_data.DefaultView[0]);
        }

        private readonly DataTable _data = new DataTable();
    }
}
