using IdentityServer4;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.HttpsPolicy;
//using Microsoft.AspNetCore.Mvc;

//using IdentityServerProvider.Data;
//using IdentityServerProvider.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;
// using Microsoft.IdentityModel.Tokens;

using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
//using System.Security.Claims;
//using Microsoft.Extensions.Logging;
using Serilog.AspNetCore;
using Serilog;
using Galal.IDP.Entities;
using Galal.IDP.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace IdentityServerProvider
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // configures IIS out-of-proc settings (see https://github.com/aspnet/AspNetCore/issues/14882)
            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            //configures IIS in-proc settings
            services.Configure<IISServerOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            //services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<IDPUserContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("IDPUserDBConnectionString")));
            //services.AddScoped<IIDPUserRepository, IDPUserRepository>();
            
            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddDeveloperSigningCredential()
                .AddGalalUserStore()
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryClients(Config.Clients);
            // .AddTestUsers(Config.Users());
            // .AddAspNetIdentity<ApplicationUser>();
            // not recommended for production - you need to store your key material somewhere secure

            ///// builder.AddDeveloperSigningCredential();
            services.AddAuthentication(options=> {
                // options.RequireAuthenticatedSignIn = false;
            }).AddFacebook(facebookOptions =>
            {
                facebookOptions.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            })
            .AddCookie("idsrv.2FA", options => {
                // options.LoginPath = "/Account/login";
                // options.AccessDeniedPath = "/Access/login";
                // options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = (context) =>
                {
                    context.Response.Headers["Location"] = context.RedirectUri;
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
            //services.AddAuthentication();
            //.AddGoogle(options =>
            //{
            //    // register your IdentityServer with Google at https://console.developers.google.com
            //    // enable the Google+ API
            //    // set the redirect URI to http://localhost:5000/signin-google
            //    options.ClientId = "copy client ID from Google here";
            //    options.ClientSecret = "copy client secret from Google here";
            //});
        }

        public void Configure(IApplicationBuilder app, IDPUserContext iDPUserContext)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseDatabaseErrorPage();
            }

            //initiat Users Database context
            iDPUserContext.Database.Migrate();
            iDPUserContext.EnsureSeedDataForContext();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
