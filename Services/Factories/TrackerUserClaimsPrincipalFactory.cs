using System.Security.Claims;
using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BugTracker.Services.Factories
{
    public class TrackerUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<BugTrackerUser, IdentityRole>
    {
        public TrackerUserClaimsPrincipalFactory(
            UserManager<BugTrackerUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {

        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(BugTrackerUser user)
        {
            if (!user.CompanyId.HasValue)
                return await base.GenerateClaimsAsync(user);

            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
            Claim companyClaim = new Claim("Company", user.CompanyId.Value.ToString());
            identity.AddClaim(companyClaim);

            return identity;
        }
    }
}
