using System.Linq;
using System.Security.Claims;

namespace Sustainsys.Saml2.TestHelpers
{
    public class ClaimsAuthenticationManagerStub : ClaimsAuthenticationManager
    {
        public bool ClearNameIdentifier { get; set; } = false;

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            var newPrincipal = new ClaimsPrincipal(incomingPrincipal);

            if(ClearNameIdentifier)
            {
                newPrincipal.Identities.First().RemoveClaim(
                    newPrincipal.FindFirst(ClaimTypes.NameIdentifier));
            }
            
            var id = new ClaimsIdentity("ClaimsAuthenticationManager");
            id.AddClaim(new Claim(ClaimTypes.Role, "RoleFromClaimsAuthManager", null, "ClaimsAuthenticationManagerStub"));
            newPrincipal.AddIdentity(id);

            return newPrincipal;
        }
    }
}
