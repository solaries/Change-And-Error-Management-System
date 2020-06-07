using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using sphinxsolaries.Caems.Models;
using sphinxsolaries.Caems.BusinessLogic;
using sphinxsolaries.Caems.Data.Models;
    
namespace sphinxsolaries.Caems
{
    public class AuthorizationProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.OwinContext.Set<string>("userType", context.Parameters["clientid"]);
            context.Validated();
        } 
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType); 
            bool found = false;  
            if (context.OwinContext.Get<string>("userType") == "Admin")
            {
                List<CAEMS_authenticate_Admin> response = centralCalls.get_authenticate_Admin(" where replace(password, '@','#')  = '" + Audit.GetEncodedHash(context.Password, "doing it well").Replace("@", "#") + "' and replace(email, '@','#') = '" + context.UserName.Replace("@", "#") + "' ");
                found = response.Count > 0;
            }

            if (context.OwinContext.Get<string>("userType") == "SuperAdmin")
            {
                List<CAEMS_authenticate_SuperAdmin> response = centralCalls.get_authenticate_SuperAdmin(" where replace(password, '@','#')  = '" + Audit.GetEncodedHash(context.Password, "doing it well").Replace("@", "#") + "' and replace(email, '@','#') = '" + context.UserName.Replace("@", "#") + "' ");
                found = response.Count > 0;
            }

            if (context.OwinContext.Get<string>("userType") == "User")
            {
                List<CAEMS_authenticate_User> response = centralCalls.get_authenticate_User(" where replace(password, '@','#')  = '" + Audit.GetEncodedHash(context.Password, "doing it well").Replace("@", "#") + "' and replace(email, '@','#') = '" + context.UserName.Replace("@", "#") + "' ");
                found = response.Count > 0;
            }

            if (found)
            { 
                identity.AddClaim(new Claim("username", ".")); 
                context.Validated(identity);
            }
            else
            { 
                context.SetError("invalid_grant", "Provided username and password invalid");
                return;
            } 
        }
    }
}
