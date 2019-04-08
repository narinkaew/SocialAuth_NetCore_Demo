using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialAuth_NetCore.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace SocialAuth_NetCore
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication()
                    .AddGoogle(o =>
                    {
                        o.ClientId = Configuration["Authentication:Google:ClientId"];
                        o.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                        o.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                        o.ClaimActions.Clear();
                        o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                        o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                        o.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                        o.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                        o.ClaimActions.MapJsonKey("urn:google:profile", "link");
                        o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    })
                    .AddFacebook(o =>
                    {
                        o.ClientId = Configuration["Authentication:Facebook:AppId"];
                        o.ClientSecret = Configuration["Authentication:Facebook:AppSecret"];
                    });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
