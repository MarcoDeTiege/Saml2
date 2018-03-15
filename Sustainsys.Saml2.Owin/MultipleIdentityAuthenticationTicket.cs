using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Sustainsys.Saml2.Owin
{
    class MultipleIdentityAuthenticationTicket : AuthenticationTicket
    {
        public MultipleIdentityAuthenticationTicket(IEnumerable<ClaimsIdentity> identities,
            AuthenticationProperties properties) :
            base(identities.FirstOrDefault(), properties)
        {
            this.identities = identities;
        }

        private IEnumerable<ClaimsIdentity> identities;

        public IEnumerable<ClaimsIdentity> Identities
        {
            get
            {
                return identities;
            }
        }
    }
}
