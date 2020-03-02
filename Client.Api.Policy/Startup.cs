using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IdentityServer4.AccessTokenValidation;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Client.Api.Policy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
               .AddIdentityServerAuthentication(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.Authority = "https://localhost:44316/";

                    options.ApiName = "policybasedapi";
                    options.ApiSecret = "Sico007_";
                });
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
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        // ensure generic 500 status code on fault.
                        context.Response.StatusCode = 500;
                        await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("An unexpected fault happened. Try again later."));
                    });
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();


            app.UseRouting();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
