using Galal.IDP.Services;
using IdentityServerProvider.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerProvider
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddGalalUserStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddScoped<IIDPUserRepository, IDPUserRepository>();
            builder.AddProfileService<IDPUserProfileService>();
            return builder;
        }
    }
}
