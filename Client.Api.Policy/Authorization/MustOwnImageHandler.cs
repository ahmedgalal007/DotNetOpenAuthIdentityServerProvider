using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Api.Policy.Authorization
{
    public class MustOwnImageHandler : AuthorizationHandler<MustOwnImageRequirement>
    {
        // private ApplicationDbContext _db;
        public MustOwnImageHandler()
        {

        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustOwnImageRequirement requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;
            if (filterContext == null)
            {
                context.Fail();
                return Task.FromResult(0);
            }

            var ImageId = filterContext.RouteData.Values["id"].ToString();
            Guid ImageIdAsGuid;
            if (!Guid.TryParse(ImageId, out ImageIdAsGuid))
            {
                context.Fail();
                return Task.FromResult(0);
            }
            var ownerId = context.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            // check the repository data here

            context.Succeed(requirement);
            return Task.FromResult(0);
        }
    }
}
