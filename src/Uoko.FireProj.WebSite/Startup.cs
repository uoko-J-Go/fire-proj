using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Security.Claims;
using Microsoft.Owin.Security.Cookies;
using Microsoft.IdentityModel.Protocols;
using Thinktecture.IdentityModel.Client;
using System.Linq;
using System.Configuration;

[assembly: OwinStartup(typeof(Uoko.FireProj.WebSite.Startup))]

namespace Uoko.FireProj.WebSite
{
    public class Startup
    {
        private readonly string _ssoUrl = ConfigurationManager.AppSettings["sso.url"];
        private readonly string _domainUrl = ConfigurationManager.AppSettings["domain.url"];
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType //AuthenticationType必须保持一致
            });
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = _ssoUrl.TrimEnd('/') + "/identity",//SSO服务地址
                ClientId = "fire",//必须跟服务端配置的ClientId一致
                Scope = "openid profile roles",
                ResponseType = "id_token token",
                RedirectUri = _domainUrl, //登录成功跳转地址=>接入网站地址
                PostLogoutRedirectUri = _domainUrl, //登出跳转地址=>接入网站地址

                SignInAsAuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType,//AuthenticationType必须保持一致
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        var nid = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType, "name", "role");

                        // get userinfo data
                        var userInfoClient = new UserInfoClient(new Uri(n.Options.Authority + "/connect/userinfo"), n.ProtocolMessage.AccessToken);

                        var userInfo = await userInfoClient.GetAsync();
                        userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui.Item1, ui.Item2)));

                        // keep the id_token for logout
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        // add access token for sample API
                        nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));
                        // keep track of access token expiration
                        nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

                        nid.AddClaim(new Claim("app_nonce", n.ProtocolMessage.Nonce));

                        n.AuthenticationTicket = new AuthenticationTicket(nid, n.AuthenticationTicket.Properties);
                    },

                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
