using System.Security.Claims;
using Microsoft.Owin.Security;

namespace Uoko.FireProj.WebSite
{
    internal class AuthenticationTicket : Microsoft.Owin.Security.AuthenticationTicket
    {
        public AuthenticationTicket(ClaimsIdentity identity, AuthenticationProperties properties) : base(identity, properties)
        {
        }
    }
}