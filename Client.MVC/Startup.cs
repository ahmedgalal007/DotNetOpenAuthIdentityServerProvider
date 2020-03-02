using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Client.MVC.Services;
using Client.MVC.Services.Contracts;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Client.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // Clear automatic Claims Maping manually
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            // JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            // services.AddControllers();

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy("PolicyApi", policyBuilder =>
                {
                    policyBuilder.AddAuthenticationSchemes(new string[] { "Cookies", "oidc" });
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireClaim("country", "nl");
                    policyBuilder.RequireClaim("subscriptionlevel", "FreeUser");
                    //policyBuilder.Build();
                    // policyBuilder.RequireRole("PayingUser"); //we could use role also to Authorize
                    //policyBuilder.RequireAssertion(ctx =>
                    //{
                    //    return ctx.User.HasClaim("subscriptionlevel", "PayingUser");// ||
                    //         //  ctx.User.HasClaim("level", "senior");
                    //});

                });
                //authorizationOptions.AddPolicy("CustomHandlerPolicy", policyBuilder => {
                //    policyBuilder.AddRequirements(new MustOwnImageRequirement());
                //});
            });

            // services.AddSingleton<IAuthorizationHandler, MustOwnImageHandler>();
            services.AddScoped<IApplicationHttpClient, ApplicationHttpClient>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";

            }).AddCookie("Cookies",
                options => {
                    options.AccessDeniedPath = "/Authorization/AccessDenied/";
                }
            )
            .AddOpenIdConnect("oidc", options => {
                options.SignInScheme = "Cookies";
                options.Authority = "https://localhost:44316/";
                options.ClientId = "ClientMVC";
                options.ResponseType = "code id_token";
                // options.CallbackPath = new PathString("...");
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("address"); 
                options.Scope.Add("roles");
                options.Scope.Add("imagegalaryapi"); 
                options.Scope.Add("policybasedapi");
                options.Scope.Add("country");
                options.Scope.Add("subscriptionlevel");
                options.Scope.Add("offline_access"); // to use refresh token feature

                options.SaveTokens = true;
                options.ClientSecret = "Sico007_";
                options.GetClaimsFromUserInfoEndpoint = true;

                options.ClaimActions.Remove("amr");
                // To use "DeleteClaim" Extention Add namespace Microsoft.AspNetCore.Authentication;
                options.ClaimActions.DeleteClaim("sid");
                options.ClaimActions.DeleteClaim("idp");
                // options.ClaimActions.DeleteClaim("address");

                options.ClaimActions.MapUniqueJsonKey("role","role");
                options.ClaimActions.MapUniqueJsonKey("country", "country");
                options.ClaimActions.MapUniqueJsonKey("subscriptionlevel", "subscriptionlevel");
                //options.ClaimActions.MapUniqueJsonKey("policybasedapi", "policybasedapi");

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    NameClaimType = JwtClaimTypes.GivenName,
                    RoleClaimType = JwtClaimTypes.Role,
                };
                //options.Events = new OpenIdConnectEvents()
                //{
                //    OnTokenValidated = tokenValidatedContext =>
                //    {
                //        var identity = tokenValidatedContext.Principal;

                //        var subjectClaim = identity.Claims.FirstOrDefault(z => z.Type == "sub");

                //        var newClaimsIdentity = new ClaimsIdentity(
                //          tokenValidatedContext.Scheme.Name,
                //          "given_name",
                //          "role");

                //        newClaimsIdentity.AddClaim(subjectClaim);

                //        tokenValidatedContext.Principal = new ClaimsPrincipal(newClaimsIdentity);
                //        tokenValidatedContext.Success();
                //        return Task.FromResult(0);
                //    },

                //    OnUserInformationReceived = userInformationReceivedContext =>
                //    {
                //        //userInformationReceivedContext.User.Remove("address");

                //        //foreach (var property in userInformationReceivedContext.Properties.Items)
                //        //{
                //        //    if (!((ClaimsIdentity)userInformationReceivedContext.Principal.Identity).Claims.Any(c => c.Type.Equals(property.Name)))
                //        //    {
                //        //        ((ClaimsIdentity)userInformationReceivedContext.Principal.Identity).AddClaim(new Claim(property.Name, property.Value.ToString()));
                //        //    }
                //        //}
                //        return Task.CompletedTask;

                //    }
                //};

            });
            services.AddMvc(mvcOptions => {
                mvcOptions.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
