using IdentityServer4.Services;
using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Extensions;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace FooDesk.STS.Identity
{
    /// <summary>
    /// Profile service
    /// </summary>
    public class ProfileService : IProfileService
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public ProfileService()
        {
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //sub is your userId.
            var subClaim = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");

            if (!string.IsNullOrEmpty(subClaim?.Value))
            {
                context.IssuedClaims = await this.getClaims(subClaim.Value);
            }

            // return Task.CompletedTask;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            // Find user by context.Subject.GetSubjectId()
            //var user = Users.FindBySubjectId(context.Subject.GetSubjectId());
            //context.IsActive = user?.IsActive == true;

            context.IsActive = true;
            // return Task.CompletedTask;
        }

        private async Task<List<Claim>> getClaims(string userName)
        {
            var claims = new List<Claim>();

            #region Method 1.Add extra const roles
            claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Role, "Admin"),
                    new Claim(JwtClaimTypes.Role, "user")
                };
            #endregion

            //#region Method 2. Add extra roles from redis
            //var cacheKey = this.cacheKeys.GetKeyRoles(userName);
            //(UserRole userRole, bool isOK) = await this.cache.GetCacheAsync<UserRole>(cacheKey);

            //if (isOK)
            //{
            //    claims = userRole.Roles.Split(',').Select(x => new Claim(ClaimTypes.Role, x.Trim())).ToList();
            //}
            //#endregion

            return claims;
        }
    }
}
