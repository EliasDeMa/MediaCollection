using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using MediaCollection.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediaCollection.Domain;
using MediaCollection.Services;

namespace MediaCollection
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<MediaCollectionUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<ISongService, SongService>();
            services.AddTransient<IPhotoService, PhotoService>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("https://www.youtube.com/")
                               .AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            CreateRolesAndAssignUsers(serviceProvider);
        }

        private void CreateRolesAndAssignUsers(IServiceProvider serviceProvider)
        {
            CreateRoleIfNotExists(serviceProvider, "Admin");
            CreateRoleIfNotExists(serviceProvider, "User");

            var userManager = serviceProvider.GetRequiredService<UserManager<MediaCollectionUser>>();

            var elias = userManager.FindByEmailAsync("elias.demaertelaere@gmail.com").Result;

            if (elias != null && !userManager.IsInRoleAsync(elias, "Admin").Result)
            {
                userManager.AddToRoleAsync(elias, "Admin").Wait();
            }
        }

        private static void CreateRoleIfNotExists(IServiceProvider serviceProvider, string role)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!roleManager.RoleExistsAsync(role).Result)
            {
                roleManager.CreateAsync(new IdentityRole(role)).Wait();
            }
        }
    }
}
