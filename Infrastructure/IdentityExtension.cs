using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MovieProject.Data;

namespace MovieProject.Infrastructure
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public MyUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor) {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user) {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("Slug", user.Slug));
            identity.AddClaim(new Claim("CreatedAt", user.CreatedAt.ToString("d MMMM yyyy")));
            identity.AddClaim(new Claim("UpdatedAt", user.UpdatedAt.ToString("d MMMM yyyy")));
            return identity;
        }
    } 
}