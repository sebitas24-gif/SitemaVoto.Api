using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using VotoMVC_Login.Data;
using VotoMVC_Login.Service;

namespace VotoMVC_Login;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // =======================
        // DB Postgres (Identity)
        // =======================
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // =======================
        // Identity + Roles
        // =======================
        builder.Services
                .AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();
        builder.Services.AddTransient<EmailService>();
        builder.Services.AddRazorPages();

        // =======================
        // HttpClient hacia tu API
        // =======================
        builder.Services.AddHttpClient("Api", c =>
        {
            c.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!);
            c.Timeout = TimeSpan.FromSeconds(60);
        });

        // Tu ApiService local (el que usa IHttpClientFactory y CreateClient("Api"))
       
        builder.Services.AddControllersWithViews();

        // =======================
        // Session (para guardar cedula mientras OTP)
        // =======================
        builder.Services.AddSession(o =>
        {
            o.IdleTimeout = TimeSpan.FromMinutes(20);
            o.Cookie.HttpOnly = true;
            o.Cookie.IsEssential = true;
        });
        

        builder.Services.AddScoped<ApiService>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();           // ✅ CRÍTICO
        app.UseAuthentication();    // ✅
        app.UseAuthorization();     // ✅

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        SeedAdminAsync(app).GetAwaiter().GetResult();


        app.Run();
    }
    private static async Task SeedAdminAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        const string adminRole = "Admin";
        const string adminEmail = "sebastianalbacura24@gmail.com";
        const string adminPass = "Admin123*";

        // 1) Rol
        if (!await roleManager.RoleExistsAsync(adminRole))
            await roleManager.CreateAsync(new IdentityRole(adminRole));

        // 2) Usuario
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createUser = await userManager.CreateAsync(adminUser, adminPass);
            if (!createUser.Succeeded)
            {
                var errors = string.Join(" | ", createUser.Errors.Select(e => e.Description));
                throw new Exception("No se pudo crear el usuario admin: " + errors);
            }
        }

        // 3) Asignar rol
        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            await userManager.AddToRoleAsync(adminUser, adminRole);
    }

}
