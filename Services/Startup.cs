using FEALVES.AspNetMVCCore.Boilerpate.Data;
using FEALVES.AspNetMVCCore.Boilerpate.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace FEALVES.AspNetMVCCore.Boilerpate.Services
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Configure DbContext with SQLite
            services.AddSingleton<ApplicationDbContextFactory>();
            services.AddScoped(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<ApplicationDbContextFactory>();
                return factory.CreateDbContext();
            });

            // Configure Identity services
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure cookie-based authentication
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            // Configure MVC and Razor Pages
            services.AddControllersWithViews();

            // Configure authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
                //options.AddPolicy("Over18", policy => policy.RequireClaim("Age", "18"));
            });

            // Configure logging
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
            });
        }


        public static void ConfigureMiddleware(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage(); // Detailed error pages for development
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }


        public static async Task InitializeDatabase(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                // Apply pending migrations and ensure the database is created
                await context.Database.MigrateAsync();

                // Create roles if the database is available
                await Startup.CreateRoles(services);
            }
        }

        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Check if the database can connect (is created)
            if (!await context.Database.CanConnectAsync())
            {
                return; // Database is not created or not accessible
            }

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUser = new ApplicationUser { UserName = "admin@fealves.com", Email = "admin@fealves.com" };
            string adminPassword = "Admin@1234";
            var _user = await userManager.FindByEmailAsync(adminUser.Email);

            if (_user == null)
            {
                var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdminUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}