using Galal.IDP.Services;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerProvider.Services
{
    public class IDPUserProfileService : IProfileService
    {
        private readonly IIDPUserRepository _iDpUserRepository;

        public IDPUserProfileService(IIDPUserRepository iDPUserRepository)
        {
            _iDpUserRepository = iDPUserRepository;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var claimsForUser = _iDpUserRepository.GetUserClaimsBySubjectId(subjectId);

            context.IssuedClaims = claimsForUser.Select
              (c => new Claim(c.ClaimType, c.ClaimValue)).ToList();

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            context.IsActive = _iDpUserRepository.IsUserActive(subjectId);

            return Task.FromResult(0);
        }
    }
}
