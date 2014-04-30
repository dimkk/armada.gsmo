using System;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.IdentityModel.Pages;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;

namespace gs.meeting.Components
{
    public partial class Login : FormsSignInPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ButtonInternalUsers.Click += new EventHandler(ButtonInternalUsers_Click);
        }

        void ButtonInternalUsers_Click(object sender, EventArgs e)
        {
            if (null != SPContext.Current && null != SPContext.Current.Site)
            {
                SPIisSettings iisSettings = SPContext.Current.Site.WebApplication.IisSettings[SPUrlZone.Default];
                if (null != iisSettings && iisSettings.UseWindowsClaimsAuthenticationProvider)
                {
                    SPAuthenticationProvider provider = iisSettings.WindowsClaimsAuthenticationProvider;
                    RedirectToLoginPage(provider);
                }
            }
        }

        //borrowed from Microsoft.SharePoint.IdentityModel.LogonSelector
        private void RedirectToLoginPage(SPAuthenticationProvider provider)
        {
            string components = HttpContext.Current.Request.Url.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped);
            string url = provider.AuthenticationRedirectionUrl.ToString();
            if (provider is SPWindowsAuthenticationProvider)
            {
                components = EnsureUrlSkipsFormsAuthModuleRedirection(components, true);
            }
            SPUtility.Redirect(url, SPRedirectFlags.Default, this.Context, components);
        }

        //borrowed from Microsoft.SharePoint.Utilities.SPUtility
        private string EnsureUrlSkipsFormsAuthModuleRedirection(string url, bool urlIsQueryStringOnly)
        {
            if (!url.Contains("ReturnUrl="))
            {
                if (urlIsQueryStringOnly)
                {
                    url = url + (string.IsNullOrEmpty(url) ? "" : "&");
                }
                else
                {
                    url = url + ((url.IndexOf('?') == -1) ? "?" : "&");
                }
                url = url + "ReturnUrl=";
            }
            return url;
        }

    }
}
