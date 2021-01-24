using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using www.doctormembrain.com.Data;
using www.doctormembrain.com.SharedClasses;

namespace www.doctormembrain.com
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
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();
            //services.AddControllersWithViews();
            //services.AddControllersWithViews().AddRazorRuntimeCompilation();
            //services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddDistributedMemoryCache();//To Store session in Memory, This is default implementation of IDistributedCache    
            services.AddSession();

            //to access in non-controller class
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<Session>();

            services.AddMvc();

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Default SignIn settings.
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            //services.AddDefaultIdentity<IdentityUser>()
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            
            // Add framework services.
            //services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("ApplicationDbContextConnection")));
            //services.AddIdentity<IdentityUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    //.AddEntityFrameworkStores<ApplicationDbContext, long>()
            //    .AddDefaultTokenProviders()
            //    .AddDefaultUI();

            if (!Statics.IsDebug)
            {
                services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromMinutes(60 * 24 * 30);
                    options.ExcludedHosts.Add("doctormembrain.com");
                    options.ExcludedHosts.Add("www.doctormembrain.com");
                });
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 443;
                });
            }

            //services.AddRazorPages(options =>
            //{
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "konto/login");
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/Register", "konto/registrer");
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/ConfirmEmail", "konto/bekræft-email");
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/Logout", "konto/logout");
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/ForgotPassword", "konto/glemt-kodeord");
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/ResetPassword", "konto/nulstil-kodeord");
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/ResetPasswordConfirmation", "konto/bekræft-nulstilling");
            //});

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);//time logged in

                options.LoginPath = "/Identity/Account/Login"; // Set here your login path.
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // set here your access denied path.
                options.SlidingExpiration = true;
                //options.Events = new CookieAuthenticationEvents
                //{
                //    OnRedirectToLogin = ctx => {
                //        var requestPath = ctx.Request.Path;
                //        ctx.Response.Redirect("/Identity/Account/Login?ReturnUrl=" + requestPath);
                //      return Task.CompletedTask;
                //    }
                //};
            });
            services.AddMvc(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(Statics.IsDebug)
            //if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseCookiePolicy();
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();

                /*endpoints.MapAreaControllerRoute(
                    name: "login",
                    pattern: "login",
                    areaName: "Identity",
                    defaults: new { controller = "Account", action = "Login", AreaAttribute = "Identity" }
                    //constraints: new { msg = "login" }
                );*/

                endpoints.MapControllerRoute(
                    name: "welcome",
                    pattern: "welcome",
                    defaults: new { controller = "Home", action = "Index" });
                endpoints.MapControllerRoute(
                    name: "admin_control_panel",
                    pattern: "admin_control_panel",
                    defaults: new { controller = "Doctor", action = "AdminControlPanel" });
                endpoints.MapControllerRoute(
                    name: "wiki",
                    pattern: "wiki/{wiki_id?}",
                    defaults: new { controller = "Doctor", action = "Wiki" });
                endpoints.MapControllerRoute(
                    name: "create_wiki",
                    pattern: "create_wiki",
                    defaults: new { controller = "Doctor", action = "CreateWiki" });
                endpoints.MapControllerRoute(
                    name: "create_wiki_post",
                    pattern: "create_wiki_post",
                    defaults: new { controller = "Doctor", action = "CreateWikiPost" });
                endpoints.MapControllerRoute(
                    name: "add_chapter_post",
                    pattern: "add_chapter_post",
                    defaults: new { controller = "Doctor", action = "AddChapterPost" });
                endpoints.MapControllerRoute(
                    name: "remove_chapter",
                    pattern: "remove_chapter",
                    defaults: new { controller = "Doctor", action = "RemoveChapterPost" });
                endpoints.MapControllerRoute(
                    name: "edit_chapter_post",
                    pattern: "edit_chapter_post",
                    defaults: new { controller = "Doctor", action = "EditChapterPost" });
                endpoints.MapControllerRoute(
                    name: "move_chapter",
                    pattern: "move_chapter",
                    defaults: new { controller = "Doctor", action = "MoveChapter" });
                endpoints.MapControllerRoute(
                    name: "userprofile",
                    pattern: "userprofile",
                    defaults: new { controller = "Doctor", action = "UserProfile" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "any",
                    pattern: "{*any}",
                    defaults: new { controller = "Doctor", action = "CatchAll" });
            });
        }
    }
}
