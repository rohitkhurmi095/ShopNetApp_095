using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopNetApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNetApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //=========
        //SERVICES
        //=========
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //Connection String
            //------------------
            //Connection string from appsettings.json
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ApplicationDbContext")));

            //________________
            //Session Service (Set manually)
            //----------------
            //You cannot view the session state variable at client side.
            //Session state is stored at server, and Client browser only knows SessionID which is stored in cookie or URL.

            //To access session in non-controller class (via Dependency Injection)
            services.AddHttpContextAccessor();
            //Session options
            services.AddSession(options =>
            {
                //Destroy session after 10min (Default = 20min)
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                //CookieOptions
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            //---------------------

            //Cloudscribe.web.pagination
            //---------------------------
            services.AddCloudscribePagination();
        }


        //============
        //MIDDLEWARES
        //============
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

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //--- Session Middleware -----
            app.UseSession();
         

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
