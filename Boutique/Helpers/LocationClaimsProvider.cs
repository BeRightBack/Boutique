using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Boutique.Helpers;

public class LocationClaimsProvider : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal != null && !principal.HasClaim(c => c.Type == ClaimTypes.PostalCode))
        {
            if (principal.Identity is ClaimsIdentity identity && identity.IsAuthenticated && identity.Name != null)
            {
                if (identity.Name.ToLower() == "alice")
                {
                    identity.AddClaims(new Claim[] {
                        CreateClaim(ClaimTypes.PostalCode, "DC 20500"),
                        CreateClaim(ClaimTypes.StateOrProvince, "DC")
                    });
                }
                else
                {
                    identity.AddClaims(new Claim[] {
                        CreateClaim(ClaimTypes.PostalCode, "NY 10036"),
                        CreateClaim(ClaimTypes.StateOrProvince, "NY")
                    });
                }
            }
        }
       return Task.FromResult(principal ?? throw new ArgumentNullException(nameof(principal)));

    }
    private static Claim CreateClaim(string type, string value) =>
        new(type, value, ClaimValueTypes.String, "RemoteClaims");

}