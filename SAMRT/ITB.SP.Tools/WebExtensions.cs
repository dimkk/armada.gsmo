using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace ITB.SP.Tools
{
    public static class WebExtensions
    {
        public static SPList GetListByUrl(this SPWeb web, string url)
        {
            return web.GetList(SPUrlUtility.CombineUrl(web.Url, "Lists/" + url));
        }
    }
}
